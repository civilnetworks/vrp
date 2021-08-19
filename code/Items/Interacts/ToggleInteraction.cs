using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Items.Interacts
{
	public class ToggleInteraction : VrpItemInteraction
	{
		private string _onDisplayName;
		private string _offDisplayName;

		public ToggleInteraction( string key, string name, string onDisplayName = null, string offDisplayName = null ) : base( key, name )
		{
			_onDisplayName = onDisplayName;
			_offDisplayName = offDisplayName;
		}

		/// <summary>
		/// If set, uses bool value of the given Vrp Item Data key to toggle.
		/// </summary>
		public string DataToggleKey { get; set; }

		/// <summary>
		/// If set, uses this action to toggle on for the entity. Takes priority over data key.
		/// </summary>
		public Action<Client, VrpItemEntity> OnAction { get; set; }

		/// <summary>
		/// If set, uses this action to toggle off for the entity. Takes priority over data key.
		/// </summary>
		public Action<Client, VrpItemEntity> OffAction { get; set; }

		/// <summary>
		/// If set, uses this func to determine the toggle state. Takes priority over data key.
		/// </summary>
		public Func<Client, VrpItemEntity, bool> IsOnFunc { get; set; }

		public Func<Client, VrpItemEntity, bool, VrpItemInteractResult> InteractFunc { get; set; }

		public string OnIconPath { get; set; } = "materials/vrp/interactions/on.png";

		public string OffIconPath { get; set; } = "materials/vrp/interactions/on.png";

		public virtual bool CanToggle( Client caller, VrpItemEntity ent, bool newValue, out string reason )
		{
			reason = null;
			return true;
		}

		public void ToggleOn( Client client, VrpItemEntity ent )
		{
			if ( this.OnAction == null )
			{
				if ( this.DataToggleKey == null )
				{
					return;
				}

				ent.Item.SetData( this.DataToggleKey, true );
				return;
			}

			this.OnAction( client, ent );
		}

		public void ToggleOff( Client client, VrpItemEntity ent )
		{
			if ( this.OffAction == null )
			{
				if ( this.DataToggleKey == null )
				{
					return;
				}

				ent.Item.SetData( this.DataToggleKey, false );
				return;
			}

			this.OffAction( client, ent );
		}

		public bool IsOn( Client client, VrpItemEntity ent )
		{
			if ( this.IsOnFunc == null )
			{
				if ( bool.TryParse( ent.Item.GetData( this.DataToggleKey, false ).ToString(), out var open ) )
				{
					return open;
				}
				else
				{
					return false;
				}
			}

			return this.IsOnFunc( client, ent );
		}

		public override string GetDisplayName( Client caller, VrpItemEntity ent )
		{
			if ( this.IsOn( caller, ent ) )
			{
				return _onDisplayName ?? this.Name;
			}
			else
			{
				return _offDisplayName ?? this.Name;
			}
		}

		public override string GetIconPath( Client caller, VrpItemEntity ent )
		{
			return this.IsOn( caller, ent ) ? this.OnIconPath : this.OffIconPath;
		}

		public override void ClientInteract( Client caller, VrpItemEntity ent, string[] args )
		{

		}

		public override VrpItemInteractResult Interact( Client caller, VrpItemEntity ent, string[] args )
		{
			var isOn = this.IsOn( caller, ent );

			var funcResult = this.InteractFunc?.Invoke( caller, ent, !isOn );
			if (funcResult != null)
			{
				return funcResult;
			}

			if ( !this.CanToggle( caller, ent, !isOn, out var reason ) )
			{
				return new VrpItemInteractResult( false, reason );
			}

			if ( isOn )
			{
				this.ToggleOff( caller, ent );
			}
			else
			{
				this.ToggleOn( caller, ent );
			}

			return new VrpItemInteractResult( true );
		}
	}
}
