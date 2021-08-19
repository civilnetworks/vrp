using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public class VGuiCenterablePanel : VGuiBasePanel
	{
		private int _width;
		private int _height;

		public VGuiCenterablePanel()
		{

		}

		public void SetWidth( double pixels )
		{
			_width = (int)pixels;
			this.Style.Width = (int)pixels;
		}

		public void SetHeight( double pixels )
		{
			_height = (int)pixels;
			this.Style.Height = (int)pixels;
		}

		public void SetPosX( double posX )
		{
			this.Style.Left = (int)posX;
		}

		public void SetPosY( double posY )
		{
			this.Style.Top = (int)posY;
		}

		public void Center()
		{
			var screenSize = Local.Hud.Box.Rect;
			this.SetPosX( (screenSize.width - _width) * 0.5 );
			this.SetPosY( (screenSize.height - _height) * 0.5 );
		}
	}
}
