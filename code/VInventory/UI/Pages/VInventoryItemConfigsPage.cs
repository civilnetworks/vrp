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

namespace VInventory.UI
{
	public class VInventoryItemConfigsPage : Panel
	{
		private Panel _scroll;

		public VInventoryItemConfigsPage()
		{
			StyleSheet.Load( "/VInventory/UI/Pages/VInventoryItemConfigsPage.scss" );
			_scroll = Add.Panel( "config-scroll" );

			var newDrop = this.AddChild<VGuiComboBox>( "new-drop" );
			newDrop.SetText( "Create New" );

			var configs = Library.GetAllAttributes<VInventoryItemConfig>();

			foreach ( var attribute in configs )
			{
				newDrop.AddOption( attribute.Title, attribute.Title );
			}

			newDrop.OnSelect += ( VGuiComboBoxItem item ) =>
			{
				var title = item.Value as string;
				if ( !VInventorySystem.TryCreateItemConfig( title, out var config, out var error ) )
				{
					Log.Error( error );
					return;
				}

				config.Name = "New Item Config";
				config.Width = 1;
				config.Height = 1;

				var page = VInventoryAdminMenu.Instance.Nav.OpenPage<VInventoryEditItemConfigPage>();
				page.SetItemConfig( config, ( newConfig ) =>
				{
					VInventorySystem.CmdAddItemConfig( newConfig.AttributeTitle, JsonSerializer.Serialize( newConfig ) );
				} );
			};
		}
	}
}
