using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Commands;

namespace VAdmin.UI.Elements
{
	public class VAdminCommandCategoryDropdown : Panel
	{
		private Label _header;
		private Panel _content;
		private bool _open = true;

		public VAdminCommandCategoryDropdown()
		{
			StyleSheet.Load( "/VAdmin/UI/Elements/VAdminCommandCategoryDropdown.scss" );

			_header = Add.Label( "N/A", "header" );
			_header.AddEventListener( "onclick", ( e ) =>
			 {
				 this.Toggle();
				 e.StopPropagation();
			 } );
		}

		public bool Open
		{
			get => _open;
			set
			{
				_open = value;
				_header.SetClass( "closed", !_open );
				_content.SetClass( "closed", !_open );
			}
		}

		public Action<VAdminCommand> OnSelect { get; set; }

		public void SetCategory( string category )
		{
			_header.SetText( category ?? "Other" );

			_content?.Delete( true );
			_content = Add.Panel( "content" );

			var commands = VAdminSystem.GetCommands( category );

			foreach ( var command in commands )
			{
				var button = _content.Add.Button( command.Name );
				button.AddEventListener( "onclick", ( e ) =>
				 {
					 e.StopPropagation();
					 this.OnSelect?.Invoke( command );
				 } );
			}
		}

		public void Toggle()
		{
			this.Open = !this.Open;
		}
	}
}
