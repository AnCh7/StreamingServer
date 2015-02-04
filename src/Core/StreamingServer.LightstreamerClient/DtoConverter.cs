#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Lightstreamer.DotNet.Client;

using StreamingServer.Common.JsonSerializer;
using StreamingServer.Common.Logging;
using StreamingServer.LightstreamerClient.Interfaces;

#endregion

namespace StreamingServer.LightstreamerClient
{
	public class DtoConverter<TDto> : IDtoConverter<TDto> where TDto : new()
	{
		private readonly IJsonSerializer _serializer;
		private readonly ILogger _logger;
		private List<string> _dtoPropertyNames;

		private List<string> DtoPropertyNames
		{
			get
			{
				if (_dtoPropertyNames == null)
				{
					_dtoPropertyNames = (from propertyInfo in typeof (TDto).GetProperties()
										 select propertyInfo.Name).ToList<string>();
					_dtoPropertyNames.Sort();
				}

				return _dtoPropertyNames;
			}
		}

		public DtoConverter(IJsonSerializer serializer, ILogger logger)
		{
			_serializer = serializer;
			_logger = logger;
		}

		public virtual TDto Convert(object data)
		{
			var updateInfo = (IUpdateInfo)data;
			var tDto = (default(TDto) == null) ? Activator.CreateInstance<TDto>() : default(TDto);
			var properties = typeof (TDto).GetProperties();

			foreach (var propertyInfo in properties)
			{
				var fieldIndex = GetFieldIndex(propertyInfo);
				var currentValue = GetCurrentValue(updateInfo, fieldIndex);
				PopulateProperty(tDto, propertyInfo.Name, currentValue);
			}

			return tDto;
		}

		public string GetFieldList()
		{
			return string.Join(" ", DtoPropertyNames.ToArray());
		}

		/// <exception cref="ArgumentException">Condition. </exception>
		private int GetFieldIndex(PropertyInfo fieldPropertyInfo)
		{
			for (var i = 0; i < DtoPropertyNames.Count; i++)
			{
				if (DtoPropertyNames[i] == fieldPropertyInfo.Name)
				{
					return i + 1;
				}
			}

			throw new ArgumentException(string.Format("Not able to find a property with name {0}", fieldPropertyInfo.Name));
		}

		private static string GetCurrentValue(IUpdateInfo updateInfo, int pos)
		{
			string result;
			if (updateInfo.IsValueChanged(pos))
			{
				result = updateInfo.GetNewValue(pos);
			}
			else
			{
				result = updateInfo.GetOldValue(pos);
			}

			return result;
		}

		private static bool IsTypeNullable(Type type, out Type underlyingType)
		{
			underlyingType = type;
			var underlyingType2 = Nullable.GetUnderlyingType(type);
			if (underlyingType2 != null)
			{
				underlyingType = underlyingType2;
				return true;
			}

			return false;
		}

		private object ConvertPropertyValue(Type pType, string propertyName, string value)
		{
			Type type;
			var flag = IsTypeNullable(pType, out type);

			if (flag && string.IsNullOrEmpty(value))
			{
				return null;
			}

			object result;
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					if (string.IsNullOrEmpty(value))
					{
						result = Activator.CreateInstance(type);
						return result;
					}
					try
					{
						result = System.Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
						return result;
					}
					catch (FormatException ex)
					{
						_logger.Error(ex);
						result = Activator.CreateInstance(type);
						return result;
					}

				case TypeCode.DateTime:
					if (string.IsNullOrEmpty(value))
					{
						result = DateTimeOffset.MinValue.DateTime;
						return result;
					}
					result = _serializer.DeserializeObject<DateTimeOffset>("\"" + value + "\"").DateTime;
					return result;

				case TypeCode.String:
					result = value;
					return result;
			}

			throw new NotImplementedException(string.Format("Cannot populate fields of type {0} such as {1} on type {2}",
															type.FullName,
															propertyName,
															typeof (TDto).FullName));
		}

		private void PopulateProperty(TDto dto, string propertyName, string value)
		{
			try
			{
				var property = typeof (TDto).GetProperty(propertyName);
				var value2 = ConvertPropertyValue(property.PropertyType, property.Name, value);
				property.SetValue(dto, value2, null);
			}
			catch (Exception innerException)
			{
				var ex = new Exception(string.Format("Error populating property {0} with {1}", propertyName, value), innerException);
				throw ex;
			}
		}
	}
}
