using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationObjects.util
{
	public class Message
	{
		public Action action { get; set; }
		public Priority priority { get; set; }

		public Message (Action action, Priority priority) 
		{
			this.action = action;
			this.priority = priority;
		}
		
		
	}
}
