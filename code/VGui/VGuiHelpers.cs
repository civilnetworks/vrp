using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui
{
	public static class VGuiHelpers
	{
		public static bool ScreenClickerEnabled { get; set; }

		public static Color Saturate( Color color, float saturation, float brightness = 1f, float alpha = -1f )
		{
			var r = color.r;
			var g = color.g;
			var b = color.b;
			var avg = (r + g + b) / 3f;

			r += (avg - r) * saturation;
			g += (avg - g) * saturation;
			b += (avg - b) * saturation;

			return new Color( r * brightness, g * brightness, b * brightness, alpha >= 0f ? alpha : color.a );
		}

		[ClientCmd( "vgui_screenclicker" )]
		public static void ToggleScreenClicker()
		{
			ScreenClickerEnabled = !ScreenClickerEnabled;
		}
	}
}
