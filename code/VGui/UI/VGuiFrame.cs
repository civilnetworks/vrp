using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using VRP.VGui.UI;

namespace VRP.VGui.UI
{
	public class VGuiFrame : VGuiCenterablePanel
	{
		private VGuiHeader _header;
		private bool _screenClickerEnabled;

		protected override void OnLoad()
		{
			base.OnLoad();

			this.SetClass( "v-frame", true );

			_header = this.AddChild<VGuiHeader>();
			_header.ShowCloseButton( true, () =>
			 {
				 this.Delete();
			 } );

			var screenSize = Local.Hud.Box.Rect;
			this.SetWidth( screenSize.width * 0.9 );
			this.SetHeight( screenSize.height * 0.9 );
			this.Center();
		}

		public VGuiHeader Header
			=> _header;

		public bool EnableClickerToggle { get; set; }

		public override void Tick()
		{
			base.Tick();

			var clickerEnabled = this.EnableClickerToggle ? VGuiHelpers.ScreenClickerEnabled : true;

			if ( _screenClickerEnabled != clickerEnabled )
			{
				_screenClickerEnabled = clickerEnabled;
				this.SetClass( "clicker", _screenClickerEnabled );
			}
		}

		public static VGuiFrame Create( int? width = null, int? height = null )
		{
			var frame = Local.Hud.AddChild<VGuiFrame>();

			if ( width != null )
			{
				frame.SetWidth( (double)width );
			}
			if ( height != null )
			{
				frame.SetHeight( (double)height );
			}

			frame.Center();

			return frame;
		}
	}
}
