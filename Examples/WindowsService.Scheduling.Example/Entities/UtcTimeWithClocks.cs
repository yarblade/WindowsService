using System;
using System.Collections.Generic;



namespace WindowsService.Scheduling.Example.Entities
{
	internal class UtcTimeWithClocks
	{
		public long Time { get; set; }
		public DateTime DateTime { get { return new DateTime(Time); } }
		public IDictionary<int, CityClock> Clocks { get; set; }
	}
}
