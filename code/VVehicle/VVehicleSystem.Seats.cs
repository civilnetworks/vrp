using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VAdmin;
using VRP;
using VVehicle.UI;

namespace VVehicle
{
	public static partial class VVehicleSystem
	{
		public static bool DebugSeats { get; set; }

		[ServerCmd]
		public static void CmdSaveEditSeats( string vehicleTitle, string seatsJson )
		{
			var client = ConsoleSystem.Caller;
			if ( !VAdminSystem.TestCommand( client, "edit_vehicle_seats" ) )
			{
				return;
			}

			var data = JsonSerializer.Deserialize<VVehicleSeatData[]>( seatsJson );
			if ( data == null )
			{
				return;
			}

			TrySaveVehicleSeats( client, vehicleTitle, data );
		}

		[ClientRpc]
		public static void RpcBeginEditSeats( int networkIdent )
		{
			var vehicle = Entity.FindByIndex( networkIdent ) as VVehicle;
			VVehicleEditSeatsMenu.Open( vehicle );
		}

		[ClientCmd( "vvehicle_debug_seats" )]
		public static void CmdDebugSeats()
		{
			DebugSeats = !DebugSeats;

			Log.Info( $"DebugSeats: {DebugSeats}" );
		}
	}
}
