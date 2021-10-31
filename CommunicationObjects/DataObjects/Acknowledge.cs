using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationObjects.DataObjects
{
	public class Acknowledge
	{
		public string subtype { get; set; }
		public int status { get; set; }
		public string statusmessage { get; set; }
	}
}
