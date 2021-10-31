using CommunicationObjects.util;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CommunicationObjects
{
	public class MessageComparer : IComparer<PriortyQueueMessage>
	{
		public int Compare([AllowNull] PriortyQueueMessage x, [AllowNull] PriortyQueueMessage y)
		{

			if (x.priority == y.priority) return 0;
			else if (x.priority == Priority.LOW && y.priority == Priority.HIGH) return -1;
			else return 1;
			
		}
	}

	public enum Priority
	{
		LOW,
		HIGH
	}
}
