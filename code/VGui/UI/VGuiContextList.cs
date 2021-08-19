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
	public class VGuiContextList : Panel
	{
		public static VGuiContextList Instance;

		private const int BUTTON_HEIGHT = 30;
		private const int BUTTON_MARGIN = 3;

		private int _numOptions;

		public VGuiContextList()
		{
			StyleSheet.Load( "/VGui/UI/VGuiContextList.scss" );

			this.Style.PaddingBottom = BUTTON_MARGIN;

			Instance?.Delete( true );
			Instance = this;
		}

		public Panel ContextParent { get; set; }

		public Action<string, string> Callback { get; set; }

		public override void Tick()
		{
			base.Tick();

			if (this.ContextParent == null || this.ContextParent.IsDeleting || !this.ContextParent.IsVisible)
			{
				this.Delete( true );
			}
		}

		public static VGuiContextList Create(Panel parent, Action<string, string> callback)
		{
			var context = Local.Hud.AddChild<VGuiContextList>();
			context.Callback = callback;
			context.ContextParent = parent;

			var self = parent.Box.Rect;
			var rect = context.Box.Rect;

			context.Style.Top = self.top + self.height;
			context.Style.Left = self.left;

			return context;
		}

		public override string GetClipboardValue( bool cut )
		{
			this.Delete( true );

			return base.GetClipboardValue( cut );
		}

		public void AddOption(string name, string value)
		{
			// TODO: Make button (when copy to clipboard is added)
			//var button = Add.Button( name );
			var button = Add.TextEntry( value ?? name );
			button.Style.Height = BUTTON_HEIGHT;
			button.Style.MinHeight = BUTTON_HEIGHT;
			button.Style.Margin = BUTTON_MARGIN;
			button.Style.MarginBottom = 0;
			button.AddEventListener( "onclick", ( e ) =>
			 {
				 /*this.Callback?.Invoke( name, value );
				 this.Delete( true );*/

				 e.StopPropagation();
			 } );

			_numOptions++;

			this.Style.Height = _numOptions * (BUTTON_HEIGHT + BUTTON_MARGIN);
		}
	}
}
