using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VAdmin;
using VRP;

namespace VVehicle
{
	public static partial class VVehicleSystem
	{
		private static Dictionary<string, VVehicleData> sVehicleData = new Dictionary<string, VVehicleData>();

		static VVehicleSystem()
		{
			if ( Host.IsServer )
			{
				LoadVehiclesData();
			}
		}

		public static string[] GetVehicleTitles()
		{
			return Library.GetAllAttributes<VVehicleConfig>().Select( c => c.Title ).ToArray();
		}

		public static VVehicleData GetVehicleData( string vehicleTitle )
		{
			if ( sVehicleData.TryGetValue( vehicleTitle, out var data ) )
			{
				return data;
			}

			return VVehicleData.CreateDefaultData();
		}

		private static void LoadVehicleData( string vehicleTitle, VVehicleData data )
		{
			if ( sVehicleData.ContainsKey( vehicleTitle ) )
			{
				sVehicleData.Remove( vehicleTitle );
			}

			sVehicleData[vehicleTitle] = data;

			if ( Host.IsServer )
			{
				NetworkVehicleData( To.Everyone, vehicleTitle, data );
			}
		}

		public static void NetworkAllVehicleData( To to )
		{
			foreach ( var pair in sVehicleData )
			{
				NetworkVehicleData( to, pair.Key, pair.Value );
			}
		}

		public static void NetworkVehicleData( string vehicleTitle )
		{
			if ( !sVehicleData.TryGetValue( vehicleTitle, out var data ) )
			{
				return;
			}

			NetworkVehicleData( vehicleTitle, data );
		}

		public static void NetworkVehicleData( string vehicleTitle, VVehicleData data )
		{
			NetworkVehicleData( To.Everyone, vehicleTitle, data );
		}

		public static void NetworkVehicleData( To to, string vehicleTitle, VVehicleData data )
		{
			RpcNetworkVehicleData( to, vehicleTitle, JsonSerializer.Serialize( data ) );
		}

		[ServerCmd]
		public static void CmdSpawnVehicle( string vehicleTitle )
		{
			var client = ConsoleSystem.Caller;

			if ( !VAdminSystem.TestPermission( client, "vrp_spawn_vehicles" ) )
			{
				return;
			}

			var vehicle = Library.GetAllAttributes<VVehicleConfig>().FirstOrDefault( v => v.Title.Equals( vehicleTitle ) );
			if ( vehicle == null )
			{
				return;
			}

			var instance = vehicle.Create<VVehicleConfig>();
			instance.VehicleTitle = vehicleTitle;
			var pos = VrpSystem.GetEyeTrace( client ).EndPos;

			if ( !TrySpawnVehicle( instance, pos, out var ent, out var error ) )
			{
				VrpSystem.SendChatMessage( To.Single( client ), $"Error: {error}" );
			}
		}

		[ClientRpc]
		public static void RpcNetworkVehicleData( string vehicleTitle, string dataJson )
		{
			var data = JsonSerializer.Deserialize<VVehicleData>( dataJson );

			if ( sVehicleData.ContainsKey( vehicleTitle ) )
			{
				sVehicleData.Remove( vehicleTitle );
			}

			sVehicleData.Add( vehicleTitle, data );
		}
	}
}
