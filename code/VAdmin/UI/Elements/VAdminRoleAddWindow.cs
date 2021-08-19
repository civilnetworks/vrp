using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.VGui;

namespace VAdmin.UI.Elements
{
	public class VAdminRoleAddWindow : Panel
	{
		public static VAdminRoleAddWindow Instance;

		public VAdminRoleAddWindow()
		{
			StyleSheet.Load( "/VAdmin/UI/Elements/VAdminRoleAddWindow.scss" );
		}

		public Panel MenuParent { get; set; }

		public Action<string> OnSelect { get; set; }

		public int CreateRoles( string[] ignoreRoles )
		{
			var numRoles = 0;

			foreach ( var roleId in VAdminSystem.GetRoles().Select( r => r.Id ).Except( ignoreRoles ) )
			{
				if ( !VAdminSystem.TryGetRole( roleId, out var role ) )
				{
					continue;
				}

				var button = Add.Button( role.Name );
				button.Style.BackgroundColor = VGuiHelpers.Saturate( role.Color, 0.5f, 0.7f, 0.5f );
				button.Style.BorderBottomColor = role.Color;
				button.AddEventListener( "onclick", ( e ) =>
				 {
					 this.OnSelect?.Invoke( role.Id );
					 this.Delete();

					 e.StopPropagation();
				 } );

				numRoles++;
			}

			return numRoles;
		}

		public override void Tick()
		{
			base.Tick();

			if ( this.MenuParent == null || this.MenuParent.IsDeleting )
			{
				this.Delete( true );
			}
		}

		public static VAdminRoleAddWindow Create( Panel parent, Action<string> onSelect, string[] ignoreRoles = null )
		{
			Instance?.Delete( true );

			var screen = Local.Hud.Box.Rect;
			var self = parent.Box.Rect;

			var menu = Local.Hud.AddChild<VAdminRoleAddWindow>();
			menu.MenuParent = parent;
			menu.OnSelect = onSelect;

			var numRoles = menu.CreateRoles( ignoreRoles );
			var width = 200;
			var height = Math.Min(180, numRoles * (30 + 6) + 6);

			menu.Style.Width = width;
			menu.Style.Height = height;

			menu.Style.Left = self.left - width + self.width;
			menu.Style.Top = Math.Min( self.top + self.height, screen.height - height );

			Instance = menu;

			return menu;
		}
	}
}
