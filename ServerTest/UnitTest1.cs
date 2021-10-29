using CommunicationObjects;
using CommunicationObjects.util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServerTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void testMessagesAreSamePriority ()
		{
			MessageComparer comparer = new MessageComparer();
			int result = comparer.Compare(new PriortyQueueMessage(null, Priority.HIGH), new PriortyQueueMessage(null, Priority.HIGH));
			Assert.AreEqual(0, result);
		}

		[TestMethod]
		public void testMessagesOneHigherPriority()
		{
			MessageComparer comparer = new MessageComparer();
			int result = comparer.Compare(new PriortyQueueMessage(null, Priority.HIGH), new PriortyQueueMessage(null, Priority.LOW));
			Assert.AreEqual(1, result);
		}

		[TestMethod]
		public void testMessagesOneLowerPriority()
		{
			MessageComparer comparer = new MessageComparer();
			int result = comparer.Compare(new PriortyQueueMessage(null, Priority.LOW), new PriortyQueueMessage(null, Priority.HIGH));
			Assert.AreEqual(-1, result);
		}
	}
}
