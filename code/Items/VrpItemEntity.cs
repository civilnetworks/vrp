using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VRP.Items.UI;
using VRP.Player;

namespace VRP.Items
{
	public partial class VrpItemEntity : ModelEntity, IUse
	{
		private static readonly List<VrpItemEntity> ITEMS = new List<VrpItemEntity>();

		private float _dataDirtyTime = 0f;
		private Dictionary<ulong, float> _dataLastSent = new Dictionary<ulong, float>();

		public VrpItemEntity()
		{
			ITEMS.Add( this );
		}

		[Net]
		public VrpItem Item { get; set; }

		[Event.Tick]
		public void OnTick()
		{
			if ( Host.IsClient )
			{
				return;
			}

			if ( this.Item == null )
			{
				Log.Warning( "VrpItemEntity deleted for missing _item!" );
				this.Delete();
				return;
			}

			var itemModel = this.Item.GetModelPath();

			if ( this.GetModelName() != itemModel )
			{
				this.SetModel( itemModel );
				//this.SetupPhysicsFromModel
			}

			this.ProcessClientDataUpdates();

			this.Item.Tick( this );
		}

		[Event.Frame]
		public void OnFrame()
		{
			// TODO: Check this is only called when entity is rendered

			if ( this.Item != null && this.Item.DisplayHint && Local.Pawn is VrpPlayer player )
			{
				var tr = player.GetEyeTrace();

				if ( tr.Entity == this && tr.Distance <= this.Item.DisplayHintDistance )
				{
					VrpItemHintPanel.CreateHintIfNotExists( this );
				}
				else
				{
					VrpItemHintPanel.RemoveHint( this );
				}
			}
		}

		public void InitializeItem( VrpItem item )
		{
			if ( this.Item != null )
			{
				this.Item.OnDataSet -= this.OnItemDataSet;
			}

			this.Item = item;
			this.Item.OnDataSet += this.OnItemDataSet;

			this.SetModel( item.GetModelPath() );

			if ( item.DataTemplates != null )
			{
				foreach ( var req in item.DataTemplates )
				{
					if ( !item.HasData( req.Key ) )
					{
						item.SetData( req.Key, req.DefaultValue );
					}
				}
			}

			this.Item.Initialize( this );
		}

		public static VrpItemEntity[] GetItems()
		{
			return ITEMS.ToArray();
		}

		public void NetworkData()
		{
			this.NetworkData( To.Everyone );
		}

		public void NetworkData( To to )
		{
			var data = this.Item.SerializeData();
			this.RpcNetworkData( to, data );
		}

		[ClientRpc]
		public void RpcNetworkData( string serializedData )
		{
			var data = this.Item.DeserializeData( serializedData );
			this.Item.Data = data;
		}

		private void ProcessClientDataUpdates()
		{
			foreach ( var cl in Client.All )
			{
				if ( !_dataLastSent.TryGetValue( cl.SteamId, out var lastUpdated ) )
				{
					lastUpdated = -1f;
				}

				if ( lastUpdated < _dataDirtyTime )
				{
					this.UpdateClientData( cl );
				}
			}
		}

		private void UpdateClientData( Client client )
		{
			if ( _dataLastSent.ContainsKey( client.SteamId ) )
			{
				_dataLastSent.Remove( client.SteamId );
			}

			_dataLastSent.Add( client.SteamId, Time.Now );

			this.NetworkData( To.Single( client ) );
		}

		private void OnItemDataSet( string key, object value )
		{
			_dataDirtyTime = Time.Now;
		}
	}
}
