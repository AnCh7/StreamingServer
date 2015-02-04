#region Usings

using System;
using System.Collections.Generic;
using NUnit.Framework;
using StreamingServer.SocketServer.Models;

#endregion

namespace StreamingServer.Tests.SocketServer
{
	[TestFixture]
	public class RequestTests
	{
		[Test]
		public void Test_Equal()
		{
			//Arrange
			var collection = new Dictionary<Request, string>();

			var r = new Request
					{
						UserName = "UserName1", 
						SessionToken = "SessionToken1", 
						RequestType = "RequestType1", 
						Parameters = "Parameters1"
					};

			var r2 = new Request
					 {
						 UserName = "UserName1", 
						 SessionToken = "SessionToken1", 
						 RequestType = "RequestType1", 
						 Parameters = "Parameters1"
					 };

			// Act
			collection.Add(r, "Value1");

			// Assert
			Assert.Throws<ArgumentException>(delegate { collection.Add(r2, "Value2"); });
		}

		[Test]
		public void Test_Not_Equal()
		{
			//Arrange
			var collection = new Dictionary<Request, string>();

			var r = new Request
					{
						UserName = "UserName1", 
						SessionToken = "SessionToken1",
						RequestType = "RequestType1",
						Parameters = "Parameters1"
					};

			var r2 = new Request
					 {
						 UserName = "UserName2", 
						 SessionToken = "SessionToken2", 
						 RequestType = "RequestType2", 
						 Parameters = "Parameters2"
					 };

			// Act
			collection.Add(r, "Value1");
			collection.Add(r2, "Value2");

			// Assert
			Assert.AreEqual(2, collection.Count);
		}
	}
}