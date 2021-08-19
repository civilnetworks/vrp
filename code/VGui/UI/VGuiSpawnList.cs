using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public class VGuiSpawnList : Panel
	{
		private Panel _content;
		private List<(string, VGuiSpawnIcon)> _icons = new List<(string, VGuiSpawnIcon)>();

		public VGuiSpawnList()
		{
			StyleSheet.Load( "/VGui/UI/VGuiSpawnList.scss" );

			this.ClearSpawnlist();
		}

		public override void Tick()
		{
			base.Tick();

			var self = this.Box.Rect;

			var toRemove = new List<(string, VGuiSpawnIcon)>();

			foreach ( var icon in _icons )
			{
				var panel = icon.Item2;

				var rect = panel.Box.Rect;
				var visible = rect.bottom > self.top && rect.top < self.bottom;

				if ( visible )
				{
					toRemove.Add( icon );
					panel.SetModel( icon.Item1 );
				}
			}

			foreach ( var remove in toRemove )
			{
				_icons.Remove( remove );
			}
		}

		public void AddModel( string model, Action customCallback = null )
		{
			var icon = _content.AddChild<VGuiSpawnIcon>();
			icon.CustomCallback = customCallback;
			_icons.Add( (model, icon) );
		}

		public void AddButton( string text, Action onClick )
		{
			_content.Add.Button( text, onClick );
		}

		public void ClearSpawnlist()
		{
			_content?.Delete();
			_content = Add.Panel( "spawnlist-content" );

			_icons.Clear();
		}
	}
}
