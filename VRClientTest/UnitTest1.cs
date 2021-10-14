using CommunicationObjects;
using CommunicationObjects.util;
using DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VRClientTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestPriorityQueue ()
		{
			PriorityQueue<Message> queue = new PriorityQueue<Message>(new MessageComparer());
			List<string> resultList = new List<string>();
			queue.Add(new Message(new Action(() => SayOne()), Priority.HIGH));
			queue.Add(new Message(new Action(() => SayTwo()), Priority.LOW));
			queue.Add(new Message(new Action(() => SayThree()), Priority.HIGH));
			//je kan waarde terug krijgen met func en toch nog in een lijst zetten;
			
			foreach (Message m in queue) {
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
		
	
	}
}