using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationObjects.util
{
	public class PriortyQueueMessage
	{
		public Action action { get; set; }
		public Priority priority { get; set; }

		public PriortyQueueMessage (Action action, Priority priority) 
		{
			this.action = action;
			this.priority = priority;
		}
		
		
	}
}
