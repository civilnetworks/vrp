using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public class VGuiConfirmBox : Panel
	{
		private Panel _popup;
		private Label _message;
		private Button _yes;
		private Button _no;

		public VGuiConfirmBox()
		{
			StyleSheet.Load( "/VGui/UI/VGuiConfirmBox.scss" );

			_popup = Add.Panel( "popup" );

			var width = 400;
			var height = 72;

			_popup.Style.Width = width;
			_popup.Style.Height = height;

			var screen = Local.Hud.Box.Rect;
			_popup.Style.Left = (screen.width - width) * 0.5f;
			_popup.Style.Top = (screen.height - height) * 0.5f;

			_message = _popup.Add.Label( "Message", "message" );

			var bottom = _popup.Add.Panel();

			_no = bottom.Add.Button( "No", "no", () =>
			{
				this.OnResult?.Invoke( false );
				this.Delete();
			} );

			_yes = bottom.Add.Button( "Yes", "yes", () =>
			{
				this.OnResult?.Invoke( true );
				this.Delete();
			} );
		}

		public Label Label
			=> _message;

		public Action<bool> OnResult { get; set; }

		public static VGuiConfirmBox CreateConfirmBox( string message, Action<bool> callback )
		{
			var box = Local.Hud.AddChild<VGuiConfirmBox>();
			box.Label.SetText( message );
			box.OnResult = callback;

			return box;
		}
	}
}
