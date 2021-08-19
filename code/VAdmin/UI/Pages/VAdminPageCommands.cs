using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Commands;
using VAdmin.UI.Elements;

namespace VAdmin.UI
{
	public class VAdminPageCommands : Panel
	{
		private Panel _list;
		private Panel _fill;
		private List<VAdminArgInputPanel> _inputPanels = new List<VAdminArgInputPanel>();
		private Button _runButton;
		private VAdminCommand _command;

		public VAdminPageCommands()
		{
			StyleSheet.Load( "/VAdmin/UI/Pages/VAdminPageCommands.scss" );

			_list = Add.Panel( "list" );
			var content = _list.Add.Panel( "scroll-content" );

			var categories = VAdminSystem.GetCommandCategories();

			foreach ( var category in categories )
			{
				var dropdown = content.AddChild<VAdminCommandCategoryDropdown>();
				dropdown.SetCategory( category );
				dropdown.OnSelect = ( command ) =>
				{
					this.SetCommand( command );
				};
			}

			var other = content.AddChild<VAdminCommandCategoryDropdown>();
			other.SetCategory( null );
		}

		public override void Tick()
		{
			base.Tick();

			if ( _runButton != null && !_runButton.IsDeleting )
			{


				_runButton.SetText( "Run: " + this.GetCommandString() );
			}
		}

		public void SetCommand( VAdminCommand command )
		{
			_command = command;

			_fill?.Delete( true );
			_fill = Add.Panel( "command-fill" );

			_inputPanels.Clear();

			foreach ( var arg in command.Args )
			{
				var input = _fill.AddChild<VAdminArgInputPanel>();
				input.SetArg( arg );

				_inputPanels.Add( input );
			}

			_runButton = _fill.Add.Button( "Run", "run-cmd-button" );
			_runButton.AddEventListener( "onclick", ( e ) =>
			 {
				 VAdminSystem.CmdRunCommand( this.GetCommandString() );
				 e.StopPropagation();
			 } );
		}

		public string GetCommandString()
		{
			if ( _command == null )
			{
				return String.Empty;
			}

			var cmd = $"{VAdminSystem.COMMAND_PREFIX}{_command.Name}";

			foreach ( var input in _inputPanels )
			{
				var value = input.GetValue();

				if ( value != null && value.Contains( " " ) && !value.StartsWith( "\"" ) )
				{
					cmd += $" \"{value}\"";
				}
				else
				{
					cmd += " " + value;
				}
			}

			return cmd;
		}
	}
}
