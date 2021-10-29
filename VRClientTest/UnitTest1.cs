using CommunicationObjects;
using CommunicationObjects.util;
using DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vr_Project.RemoteHealthcare;

namespace VRClientTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestPriorityQueue ()
		{
			PriorityQueue<PriortyQueueMessage> queue = new PriorityQueue<PriortyQueueMessage>(new MessageComparer());
			List<string> resultList = new List<string>();
			queue.Add(new PriortyQueueMessage(new Action(() => SayOne()), Priority.HIGH));
			queue.Add(new PriortyQueueMessage(new Action(() => SayTwo()), Priority.LOW));
			queue.Add(new PriortyQueueMessage(new Action(() => SayThree()), Priority.HIGH));
			//je kan waarde terug krijgen met func en toch nog in een lijst zetten;
			
			foreach (PriortyQueueMessage m in queue) {
				m.action.Invoke();
				
			}
			
			Assert.AreEqual("One", resultList.ElementAt(0));
			Assert.AreEqual("Three", resultList.ElementAt(1));
			Assert.AreEqual("Two", resultList.ElementAt(2));


			string SayOne ()
			{
				resultList.Add("One");
				return ("One");
			}
			string SayTwo ()
			{
				resultList.Add("Two");
				return ("Two");
			}
			string SayThree ()
			{
				resultList.Add("Three");
				return ("Three");
			}
		}

		[TestMethod]
		public void testCheckSum()
		{
			//fake resistance massage
			byte checksum = Ergometer.checksum(new byte[] { 0xa4, 0x09, 0x4e, 0x05, 0x30, 0, 0, 0, 0, 0x05, 0 });

			Assert.AreEqual(0xd3, checksum);
		}
	}
}
