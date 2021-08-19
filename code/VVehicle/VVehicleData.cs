using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVehicle
{
	public class VVehicleData
	{
		public VVehicleSeatData[] Seats { get; set; }

		public static VVehicleData CreateDefaultData()
		{
			var data = new VVehicleData()
			{
				Seats = new VVehicleSeatData[]
				{
					new VVehicleSeatData(),
				},
			};

			return data;
		}
	}
}
