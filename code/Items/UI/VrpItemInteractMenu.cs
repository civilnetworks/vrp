using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Player;
using VRP.VGui.UI;

namespace VRP.Items.UI
{
	public class VrpItemInteractMenu : VGuiCenterablePanel
	{
		public const int MENU_SIZE = 260;
		public const int ITEM_SIZE = 73;
		public const float MENU_ANIM_TIME = .2f;

		public static VrpItemInteractMenu Instance;
		private static bool _wasUseDown;

		private Label _label;
		private float _lastLookEnt;
		private bool _deleting = false;
		private List<float> _positioned = new List<float>();
		private List<Panel> _items = new List<Panel>();

		public VrpItemInteractMenu()
		{
			StyleSheet.Load( "/Items/UI/VrpItemInteractMenu.scss" );

			_label = this.Add.Label( "", "radial-label" );

			this.SetWidth( MENU_SIZE );
			this.SetHeight( MENU_SIZE );
			this.Center();

			_lastLookEnt = Time.Now;
		}

		public VrpItemEntity Entity { get; set; }

		public List<Panel> Items
			=> _items;

		public override void Tick()
		{
			base.Tick();

			if ( _deleting )
			{
				return;
			}

			var interactions = this.Entity.Item.GetInteractions().Where( i => i.VisibleInInteractMenu ).ToArray();
			var numInteractions = _positioned.Count;

			var radians = (Math.PI / 180f) * (360f / numInteractions);
			var angOffset = -(Math.PI / 180f) * 90f;
			var radius = MENU_SIZE * 0.45f - ITEM_SIZE * 0.5f;
			var posOffset = MENU_SIZE * 0.5f - ITEM_SIZE * 0.5f;

			for ( var i = 0; i < numInteractions; i++ )
			{
				var interaction = interactions[i];

				var item = _items[i];
				var position = Math.Min( 1f, _positioned[i] / MENU_ANIM_TIME );
				position = position * (2f - position);

				_positioned[i] = Math.Min( 1f, _positioned[i] + Time.Delta );

				if ( position <= 0f )
				{
					continue;
				}

				var x = Math.Cos( radians * i + angOffset ) * radius * position + posOffset;
				var y = Math.Sin( radians * i + angOffset ) * radius * position + posOffset;
				var pnlTr = new PanelTransform();
				pnlTr.AddTranslateX( (float)x );
				pnlTr.AddTranslateY( (float)y );

				item.Children.ElementAt( 0 ).Style.SetBackgroundImage( interaction.GetIconPath( Local.Client, this.Entity ) );
				item.Style.Transform = pnlTr;
				item.SetClass( "show", true );
				item.Style.Dirty();
			}

			if ( this.Entity == null || !this.Entity.IsValid() )
			{
				this.Delete( true );
				return;
			}

			var user = Local.Pawn;

			if ( user is VrpPlayer player && player.GetUsable() == this.Entity )
			{
				_lastLookEnt = Time.Now;
			}
			else if ( Time.Now - _lastLookEnt > 0.5f )
			{
				Close();
				this.Delete();
				_deleting = true;
			}
		}

		public static VrpItemInteractMenu Open( VrpItemEntity ent )
		{
			var currentlyOpen = !(Instance?.IsDeleting ?? false) && Instance?.Entity == ent;

			Close();

			if ( ent == null || currentlyOpen )
			{
				return null;
			}

			var menu = Local.Hud.AddChild<VrpItemInteractMenu>();
			menu.SetEntity( ent );

			Instance = menu;

			return menu;
		}

		public static void Close()
		{
			if ( Instance != null )
			{
				Instance.Style.PointerEvents = "none";
				Instance.Style.Dirty();

				foreach ( var item in Instance.Items )
				{
					item.SetClass( "show", false );
					item.Style.PointerEvents = "none";
					item.Style.Dirty();
				}
			}

			Instance?.Delete();
		}

		[Event.Frame]
		public static void OnFrame()
		{
			if ( Input.Down( InputButton.Use ) )
			{
				if ( !_wasUseDown )
				{
					_wasUseDown = true;

					var user = Local.Pawn;

					if ( user is VrpPlayer player && player.GetUsable() is VrpItemEntity )
					{
						var ent = (VrpItemEntity)player.GetUsable();

						Open( ent );
					}
				}
			}
			else
			{
				_wasUseDown = false;
			}
		}

		public void SetEntity( VrpItemEntity ent )
		{
			this.Entity = ent;

			var interactions = ent.Item.GetInteractions().Where( i => i.VisibleInInteractMenu ).ToArray();
			var numInteractions = interactions.Length;

			if ( numInteractions == 0)
			{
				Close();
				return;
			}
			
			var delay = 0.1f;

			var contentPanel = this.AddChild<Panel>( "radial-content" );

			for ( var i = 0; i < numInteractions; i++ )
			{
				var interaction = interactions[i];

				var item = this.AddChild<Panel>( "radial-item" );
				item.Style.Width = ITEM_SIZE;
				item.Style.Height = ITEM_SIZE;

				item.AddEventListener( "onmouseout", ( e ) =>
				 {
					 _label.SetText( "" );
				 } );
				item.AddEventListener( "onmouseover", ( e ) =>
				 {
					 _label.SetText( interaction.GetDisplayName( Local.Client, ent ) );
				 } );
				item.AddEventListener( "onclick", ( e ) =>
				 {
					 VrpItemEntity.CmdInteract( ent.NetworkIdent, interaction.Key );

					 Close();
					 e.StopPropagation();
				 } );

				var icon = item.AddChild<Panel>( "icon" );
				icon.Style.SetBackgroundImage( interaction.GetIconPath( Local.Client, ent ) );

				_items.Add( item );
				_positioned.Add( delay );
			}
		}
	}
}
