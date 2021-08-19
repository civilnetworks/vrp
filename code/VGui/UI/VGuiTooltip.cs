using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public class VGuiTooltip : Label
	{
		private static VGuiTooltip CURRENT;

		public VGuiTooltip()
		{
			StyleSheet.Load( "/VGui/UI/VGuiTooltip.scss" );
		}

		public Panel TooltipParent { get; set; }

		public static VGuiTooltip CreateTooltip( Panel parent, string text )
		{
			CURRENT?.Delete();

			var tooltip = Local.Hud.AddChild<VGuiTooltip>();
			tooltip.SetText( text );
			tooltip.TooltipParent = parent;

			var rect = parent.Box.Rect;
			var screen = Local.Hud.Box.Rect;

			tooltip.Style.MarginLeft = rect.left;
			tooltip.Style.MarginTop = rect.top - 42;

			CURRENT = tooltip;

			return tooltip;
			/*if ( _expanded && _items.Count > 0 )
			{
				var panelHeight = _items.Count * _items[0].ItemHeight;

				_panelContent.Style.Height = panelHeight;
				_panel.Style.Height = Math.Min( Math.Max( 0, screen.height - (rect.top + rect.height) ) + 3, panelHeight );
			}*/
		}

		public static void RemoveTooltip( Panel parent )
		{
			if ( CURRENT != null && CURRENT.TooltipParent == parent )
			{
				CURRENT.Delete( true );
			}
		}

		public override void Tick()
		{
			base.Tick();

			if ( this.Parent == null || this.Parent.IsDeleting )
			{
				this.Delete( true );
			}
		}
	}
}
