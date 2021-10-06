using CommunicationObjects;
using CommunicationObjects.util;
using DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace VRClientTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestPriorityQueue ()
		{
			PriorityQueue<Message> queue = new PriorityQueue<Message>(new MessageComparer());
			
			queue.Add(new Message(new Action(() => SayOne()), Priority.HIGH));
			queue.Add(new Message(new Action(() => SayTwo()), Priority.LOW));
			queue.Add(new Message(new Action(() => SayThree()), Priority.HIGH));

			foreach (Message m in queue) {
				m.action.Invoke();
				
			}
			Debug.WriteLine("works");
			
			
		}
		
		private string SayOne () {
			return ("One");
		}
		private string SayTwo ()
		{
			return ("Two");
		}
		private string SayThree ()
		{
			return ("Three");
		}
	}
}
