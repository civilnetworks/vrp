using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public class VGuiVerticalNavBar : Panel
	{
		private Panel _nav;
		private List<Button> _navButtons = new List<Button>();
		private Panel _page;
		private Panel _pageContainer;

		public VGuiVerticalNavBar()
		{
			StyleSheet.Load( "/VGui/UI/VGuiVerticalNavBar.scss" );

			_nav = Add.Panel( "nav-bar" );
			_nav.Style.ZIndex = 9999;

			_pageContainer = Add.Panel( "page-container" );
		}

		public T OpenPage<T>() where T : Panel, new()
		{
			_page?.Delete( true );
			var page = _pageContainer.AddChild<T>();
			_page = page;

			return page;
		}

		public Button AddNavButton( string text, string iconPath, Action onClick )
		{
			var button = _nav.Add.Button( "", () =>
			{
				onClick?.Invoke();
			} );

			var tooltip = button.Add.Label( text, "tooltip" );

			_navButtons.Add( button );

			button.Style.SetBackgroundImage( iconPath );

			return button;
		}
	}
}
