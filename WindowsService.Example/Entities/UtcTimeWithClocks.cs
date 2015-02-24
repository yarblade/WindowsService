using System;
using System.Collections.Generic;



namespace WindowsService.Example.Entities
{
	internal class UtcTimeWithClocks
	{
		public DateTime Time { get; set; }

		public IDictionary<int, CityClock> Clocks { get; set; }
	}
}
