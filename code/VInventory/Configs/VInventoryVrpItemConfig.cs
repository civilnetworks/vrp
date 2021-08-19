using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Items;

namespace VInventory
{
	[Library( Title = "vrp_item" )]
	public class VInventoryVrpItemConfig : VInventoryItemConfig
	{
		public string ItemId { get; }

		public override VInventoryItemType Type => VInventoryItemType.VrpItem;

		public override Type EntityType => typeof( VrpItemEntity );

		public override bool CreateEntity( VInventoryItem item, out Entity ent, out string error )
		{
			ent = null;
			if ( !VrpItems.TryCreateItemEntity( this.ItemId, Vector3.Zero, out var itemEnt, out error ) )
			{
				return false;
			}

			foreach ( var pair in item.Data )
			{
				itemEnt.Item.SetData( pair.Key, pair.Value.Value );
			}

			ent = itemEnt;

			return true;
		}

		public override bool CreateItem( Entity entity, out VInventoryItem item, out string error )
		{
			if ( !VInventorySystem.TryCreateItem( this.Id, out item, out error ) )
			{
				return false;
			}

			var itemEnt = (VrpItemEntity)entity;

			foreach ( var pair in itemEnt.Item.Data )
			{
				var data = item.SetData( pair.Key, pair.Value );
				var template = itemEnt.Item.DataTemplates.FirstOrDefault( t => t.Key == pair.Key );

				data.Networked = (template?.Networked ?? false);
			}

			return true;
		}

		protected override bool CanCreateConfigInternal( out string error )
		{
			foreach ( var config in VInventorySystem.GetItemConfigs<VInventoryVrpItemConfig>() )
			{
				if ( config.ItemId == this.ItemId )
				{
					error = $"ItemId '{this.ItemId}' already exists in config";
					return false;
				}
			}

			error = null;
			return true;
		}

		public override bool IsConfigValid( out string error )
		{
			if ( string.IsNullOrWhiteSpace( this.ItemId ) )
			{
				error = "ItemId not set";
				return false;
			}

			var attribute = Library.GetAllAttributes<VrpItem>().FirstOrDefault( a => a.Title == this.ItemId );
			if ( attribute == null )
			{
				error = $"VrpItem with Title '{this.ItemId}' does not exist";
				return false;
			}

			return base.IsConfigValid( out error );
		}

		public override VInventoryItemConfigAttribute[] GetCustomItemAttributes()
		{
			return new VInventoryItemConfigAttribute[]
			{

			};
		}
	}
}
