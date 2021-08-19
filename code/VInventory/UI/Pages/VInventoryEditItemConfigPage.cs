using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.VGui.UI;

namespace VInventory.UI
{
	public class VInventoryEditItemConfigPage : Panel
	{
		private VInventoryItemConfig _config;

		private Panel _scroll;
		private Button _save;

		public VInventoryEditItemConfigPage()
		{
			StyleSheet.Load( "/VInventory/UI/Pages/VInventoryEditItemConfigPage.scss" );

			_scroll = Add.Panel( "config-scroll" );
		}

		public void SetItemConfig( VInventoryItemConfig config, Action<VInventoryItemConfig> onSave )
		{
			_save?.Delete();

			_config = config;

			var categories = VInventorySystem.GetItemConfigCategories();

			var typePanel = _scroll.Add.Panel( "element" );
			var typeLabel = typePanel.Add.Label( $"Type: {config.Type}", "element-name" );
			typeLabel.Style.Width = Length.Parse( "100%" );

			var namePanel = _scroll.Add.Panel( "element" );
			var nameLabel = namePanel.Add.Label( "Name", "element-name" );
			var nameEntry = namePanel.AddChild<VGuiTextEntry>();
			nameEntry.SetText( config.Name );
			nameEntry.SetFontSize( 16 );

			var categoryPanel = _scroll.Add.Panel( "element" );
			var categoryLabel = categoryPanel.Add.Label( "Category", "element-name" );
			var categoryDrop = categoryPanel.AddChild<VGuiComboBox>();
			categoryDrop.SetText( $"Categories ( {categories.Length} )" );

			foreach ( var category in categories )
			{
				categoryDrop.AddOption( category, categoryDrop );
			}

			var categoryEntry = categoryPanel.AddChild<VGuiTextEntry>();
			categoryEntry.SetText( config.Category );
			categoryEntry.SetFontSize( 16 );

			categoryDrop.OnSelect += ( VGuiComboBoxItem item ) =>
			{
				var cat = item.Value as string;
				categoryEntry.SetText( cat );
			};

			var sizePanel = _scroll.Add.Panel( "element" );
			var sizeLabel = sizePanel.Add.Label( "Size", "element-name" );
			var widthEntry = sizePanel.AddChild<VGuiTextEntry>();
			widthEntry.SetText( config.Width.ToString() );
			widthEntry.Numeric = true;
			widthEntry.SetFontSize( 16 );
			var heightEntry = sizePanel.AddChild<VGuiTextEntry>();
			heightEntry.SetText( config.Height.ToString() );
			heightEntry.Numeric = true;
			heightEntry.SetFontSize( 16 );

			_save = Add.Button( "Save", "save" );
			_save.AddEventListener( "onclick", ( e ) =>
			 {
				 config.Name = nameEntry.Text;
				 config.Category = categoryEntry.Text;
				 config.Width = int.Parse( widthEntry.Text );
				 config.Height = int.Parse( heightEntry.Text );

				 onSave.Invoke(config);
			 } );
		}
	}
}
