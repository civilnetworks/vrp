using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VInventory
{
	public static partial class VInventorySystem
	{
		private static Dictionary<int, VInventoryItemConfig> sItemConfigs = new Dictionary<int, VInventoryItemConfig>();
		private static Dictionary<long, VInventoryItem> sItems = new Dictionary<long, VInventoryItem>();

		static VInventorySystem()
		{
			if ( Host.IsServer && sItemConfigs.Count == 0 )
			{
				LoadItemConfigs();
			}
		}

		public static VInventoryItemConfig[] GetItemConfigs()
		{
			return sItemConfigs.Select( p => p.Value ).ToArray();
		}

		public static T[] GetItemConfigs<T>() where T : VInventoryItemConfig
		{
			return (T[])GetItemConfigs().Where( c => c is T ).ToArray();
		}

		public static string[] GetItemConfigCategories()
		{
			return sItemConfigs.Select( p => p.Value.Category ).Distinct().OrderBy( c => c ).ToArray();
		}

		public static bool TryDeserializeItemConfig( string configTitle, string json, out VInventoryItemConfig config, out string error )
		{
			config = null;

			var attribute = Library.GetAllAttributes<VInventoryItemConfig>().FirstOrDefault( a => a.Title.Equals( configTitle ) );
			if ( attribute == null )
			{
				error = $"Unknown config {configTitle}'";
				return false;
			}

			var instance = attribute.Create<VInventoryItemConfig>();
			var instanceType = instance.GetType();

			config = JsonSerializer.Deserialize( json, instanceType ) as VInventoryItemConfig;

			error = null;
			return true;
		}

		public static bool TryCreateItemConfig( string configTitle, out VInventoryItemConfig config, out string error )
		{
			config = null;

			var configAttribute = Library.GetAllAttributes<VInventoryItemConfig>().FirstOrDefault( v => v.Title.Equals( configTitle ) );
			if ( configAttribute == null )
			{
				error = $"Unknown config title {configTitle}";
				return false;
			}

			config = configAttribute.Create<VInventoryItemConfig>();
			config.AttributeTitle = configTitle;

			error = null;

			return true;
		}

		[ClientRpc]
		public static void RpcNetworkItemConfig( string configJson )
		{
			var config = JsonSerializer.Deserialize<VInventoryItemConfig>( configJson );

			if ( sItemConfigs.ContainsKey( config.Id ) )
			{
				sItemConfigs.Remove( config.Id );
			}

			sItemConfigs.Add( config.Id, config );
		}
	}
}
