using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.VGui.UI;

namespace VAdmin.UI.Elements
{
	public class VAdminArgInputPanel : Panel
	{
		private Label _header;
		private Panel _fillScroll;
		private Panel _fill;
		private Func<string> _getValue;

		public VAdminArgInputPanel()
		{
			StyleSheet.Load( "/VAdmin/UI/Elements/VAdminArgInputPanel.scss" );

			_header = Add.Label( "N/A", "arg-header" );
			_fillScroll = Add.Panel( "arg-fill-scroll" );
			_fill = _fillScroll.Add.Panel( "arg-fill" );
		}

		public Func<string> GetValue
			=> _getValue;

		public void SetArg( CommandArg arg )
		{
			_header.SetText( arg.Name );

			switch ( arg.Type )
			{
				case CommandArgType.String:
					var stringEntry = _fill.AddChild<VGuiTextEntry>();
					stringEntry.SetFontSize( 16 );
					_getValue = () =>
					{
						return stringEntry.Text;
					};
					break;
				case CommandArgType.Number:
					var numEntry = _fill.AddChild<VGuiTextEntry>();
					numEntry.SetFontSize( 16 );
					numEntry.Numeric = true;
					_getValue = () =>
					{
						return numEntry.Text;
					};
					break;
				case CommandArgType.WholeNumber:
					// TODO: Force whole numbers
					var wnumEntry = _fill.AddChild<VGuiTextEntry>();
					wnumEntry.SetFontSize( 16 );
					wnumEntry.Numeric = true;
					_getValue = () =>
					{
						return wnumEntry.Text;
					};
					break;
				case CommandArgType.Bool:
					var boolEntry = _fill.AddChild<VGuiSwitch>();
					_getValue = () =>
					{
						return boolEntry.Checked ? "true" : "false";
					};
					break;
				case CommandArgType.Client:
				case CommandArgType.ClientId:
				case CommandArgType.Permission:
				case CommandArgType.Role:
					var clientEntry = this.CreateListEntry( _fill, VAdminSystem.GetArgSuggestions( arg, null ), out var clientEntryGetter );
					_getValue = clientEntryGetter;
					break;
			}
		}

		private Panel CreateListEntry( Panel parent, string[] suggestions, out Func<string> valueGetter )
		{
			var container = parent.Add.Panel( "list-entry" );
			var value = "";

			var buttons = new List<Button>();

			foreach ( var suggestion in suggestions )
			{
				var button = container.Add.Button( suggestion );
				button.AddEventListener( "onclick", ( e ) =>
				 {
					 value = suggestion;

					 foreach ( var b in buttons )
					 {
						 b.SetClass( "selected", false );
					 }

					 button.SetClass( "selected", true );

					 e.StopPropagation();
				 } );

				buttons.Add( button );
			}

			valueGetter = () =>
			{
				return value;
			};

			return container;
		}
	}
}
