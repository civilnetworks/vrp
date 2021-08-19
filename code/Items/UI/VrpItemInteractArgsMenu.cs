using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VRP.VGui.UI;

namespace VRP.Items.UI
{
	public class VrpItemInteractArgsMenu : VGuiCenterablePanel
	{
		public static VrpItemInteractArgsMenu Instance;

		private bool _deleting;

		public VrpItemInteractArgsMenu()
		{
			StyleSheet.Load( "/Items/UI/VrpItemInteractArgsMenu.scss" );
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
		}

		public static VrpItemInteractArgsMenu Open( VrpItemEntity ent, string interactionKey )
		{
			Close();

			Instance = Local.Hud.AddChild<VrpItemInteractArgsMenu>();
			Instance.Initialize( ent, interactionKey );

			return Instance;
		}

		public static void Close()
		{
			Instance?.Delete();
		}

		public void Initialize( VrpItemEntity ent, string interactionKey )
		{
			var interaction = ent.Item.GetInteraction( interactionKey );
			var args = interaction.Args;
			if ( args == null )
			{
				return;
			}

			this.Entity = ent;

			var title = Add.Label( interaction.GetDisplayName( Local.Client, ent ), "title" );
			var getters = new List<Func<string>>();

			foreach ( var arg in args )
			{
				var label = Add.Label( arg.Name, "arg-title" );

				switch ( arg.Type )
				{
					case VrpItemInteractArgType.String:
						var stringInput = this.AddChild<VGuiTextEntry>( "input" );
						stringInput.SetFontSize( 14 );
						stringInput.SetText( arg.Default?.ToString() ?? "" );
						stringInput.Focus();
						getters.Add( () =>
						{
							return stringInput.Text;
						} );

						break;
					case VrpItemInteractArgType.Number:
						var numberInput = this.AddChild<VGuiTextEntry>( "input" );
						numberInput.Numeric = true;
						numberInput.SetFontSize( 14 );
						numberInput.SetText( arg.Default?.ToString() ?? "" );
						numberInput.Focus();
						getters.Add( () =>
						 {
							 return numberInput.Text;
						 } );

						break;
					default:
						throw new Exception( $"Unsupported VrpItemInteractArgType '{arg.Type}" );
				}
			}

			var submit = this.Add.Button( interaction.GetDisplayName( Local.Client, ent ), "submit" );
			submit.AddEventListener( "onclick", ( e ) =>
			 {
				 var args = new List<string>();
				 foreach ( var getter in getters )
				 {
					 args.Add( getter() );
				 }

				 VrpItemEntity.CmdInteractArgs( ent.NetworkIdent, interaction.Key, JsonSerializer.Serialize( args ) );
				 Close();
			 } );

			var height = 30 + (args.Length * (24 + 3 + 30 + 3)) + 40 + 12*2;
			this.SetWidth( 280 );
			this.SetHeight( height );
			this.Center();
		}
	}
}
