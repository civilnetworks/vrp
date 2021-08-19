using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Items.Interacts;
using VRP.Player;

namespace VRP.Items.UI
{
	public class VrpItemHintPanel : Panel
	{
		private const string TITLE_POSTFIX = "    ";
		private static List<VrpItemHintPanel> _hints = new List<VrpItemHintPanel>();

		private bool _deleting;
		private Vector3 _curPos = Vector3.Zero;
		private Image _locked;
		private Label _claimed;
		private List<Label> _hintItemLabels = new List<Label>();

		public VrpItemHintPanel()
		{
			StyleSheet.Load( "/Items/UI/VrpItemHintPanel.scss" );
		}

		public VrpItemEntity Entity { get; private set; }


		public override void Tick()
		{
			base.Tick();

			if ( _deleting )
			{
				return;
			}

			if ( this.Entity == null || !this.Entity.IsValid() )
			{
				this.Delete();
				_deleting = true;
				return;
			}

			if ( Local.Pawn is VrpPlayer player )
			{
				var tr = player.GetEyeTrace();

				if ( tr.Entity == this.Entity )
				{
					if ( _curPos.x == 0 )
					{
						_curPos = tr.EndPos;
					}
					else
					{
						_curPos += (tr.EndPos - _curPos) * Time.Delta * 4f;
					}

					var pos = _curPos.ToScreen();
					var screen = Local.Hud.Box.Rect;

					var x = pos.x * screen.width;
					var y = pos.y * screen.height;

					this.Style.Left = x;
					this.Style.Top = y;
					this.Style.Dirty();

					_locked?.SetClass( "is-locked", LockUnlockInteraction.IsLocked( this.Entity ) );

					for ( var i = 0; i < Math.Min( _hintItemLabels.Count, this.Entity.Item.HintItems.Length ); i++ )
					{
						var item = this.Entity.Item.HintItems[i];
						var panel = _hintItemLabels[i];

						panel.SetText( item.GetDisplayText( this.Entity.Item ) );
					}

					if ( _claimed != null )
					{
						if ( ClaimUnclaimInteraction.IsClaimed( this.Entity ) )
						{
							var cl = ClaimUnclaimInteraction.GetOwner( this.Entity );
							var display = cl != null ? VrpSystem.GetRPName( cl ) : $"SteamId {ClaimUnclaimInteraction.GetOwnerSteamId( this.Entity ).ToString()}";

							_claimed.SetText( display );
							_claimed.SetClass( "is-claimed", true );
						}
						else
						{
							_claimed.SetClass( "is-claimed", false );
						}
					}
				}
			}
		}

		public static VrpItemHintPanel CreateHint( VrpItemEntity ent )
		{
			RemoveHint( ent );

			var panel = Local.Hud.AddChild<VrpItemHintPanel>();
			panel.SetupEntity( ent );

			_hints.Add( panel );

			return panel;
		}

		public static void CreateHintIfNotExists( VrpItemEntity ent )
		{
			if ( _hints.FirstOrDefault( h => h.Entity == ent ) != null )
			{
				return;
			}

			CreateHint( ent );
		}

		public static void RemoveHint( VrpItemEntity ent )
		{
			var hint = _hints.FirstOrDefault( h => h.Entity == ent );

			if ( hint != null )
			{
				_hints.Remove( hint );
				hint.Delete();
			}
		}

		public void SetupEntity( VrpItemEntity ent )
		{
			this.Entity = ent;

			var title = Add.Label( $"{ent.Item.GetDisplayName()}{TITLE_POSTFIX}", "title" );

			if ( LockUnlockInteraction.IsLockable( ent ) )
			{
				_locked = title.Add.Image( "", "locked" );
			}

			if ( ClaimUnclaimInteraction.IsClaimable( ent ) )
			{
				_claimed = Add.Label( "", "claimed" );
				_claimed.Add.Image( "materials/vrp/interactions/claim.png" );
			}

			foreach ( var item in ent.Item.HintItems )
			{
				var value = item.GetDisplayText( ent.Item );
				var panel = Add.Label( value, "item" );

				_hintItemLabels.Add( panel );
			}
		}
	}
}
