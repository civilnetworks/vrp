using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace VRP.VGui.UI
{
	public class VGuiHeader : VGuiBasePanel
	{
		private string _title = "VGui Panel";
		private int _height = 40;
		private string _iconPath;

		private Panel _bar;
		private Image _icon;
		private Label _label;
		private Button _closeButton;

		public VGuiHeader()
		{
			StyleSheet.Load( "/vgui/ui/VGuiHeader.scss" );
		}

		public VGuiHeader( string title )
		{
			this.SetTitle( title );
		}

		public void SetTitle( string title )
		{
			_title = title;
			_label?.SetText( title );
		}

		public string GetTitle()
		{
			return _title;
		}

		public void SetBarWidth( int width )
		{
			_bar.Style.Width = width;
		}

		public void SetHeight( int height )
		{
			this.Style.Height = height;
			_icon.Style.Height = height * 0.75f;
			_icon.Style.Width = height * 0.75f;
			_label.Style.FontSize = height * 0.5f;

			_height = height;
		}

		public void SetIcon(string iconPath)
		{
			if (iconPath == null)
			{
				_icon.Style.Width = 0;
			}
			else
			{
				_icon.SetTexture( iconPath );
				_icon.Style.Width = _height * 0.75f;
			}

			_iconPath = iconPath;
		}

		public void ShowCloseButton(bool show, Action closeButtonAction = null)
		{
			_closeButton?.Delete();

			if (show)
			{
				_closeButton = this.Add.Button( "" );
				_closeButton.SetClass( "close", true );
				_closeButton.AddEventListener( "onclick", ( e ) =>
				{
					e.StopPropagation();
					closeButtonAction?.Invoke();
				} );
			}
		}

		protected override void OnLoad()
		{
			base.OnLoad();

			this.SetClass( "v-header", true );

			_bar = Add.Panel( "bar" );
			_icon = Add.Image( "", "icon" );
			_label = Add.Label( this.GetTitle(), "title" );

			this.SetHeight( _height );
			this.SetIcon( _iconPath );
		}
	}
}
