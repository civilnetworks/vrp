using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Permissions;

namespace VAdmin.UI.Elements
{
	public class VAdminRolePanel : Panel
	{
		private VAdminRole _role;
		private Image _img;
		private Panel _container;
		private Label _label;

		public VAdminRolePanel()
		{
			StyleSheet.Load( "/VAdmin/UI/Elements/VAdminRolePanel.scss" );

			VAdminSystem.OnRoleLoaded += this.OnRoleLoaded;
		}

		public VAdminRole Role
			=> _role;

		public void SetRole(VAdminRole role)
		{
			_role = role;

			_img?.Delete( true );
			_container?.Delete( true );
			_label?.Delete( true );

			_img = Add.Image( "materials/vrp/permission.png", "icon" );
			_container = Add.Panel( "container" );
			_label = _container.Add.Label( role.Name, "label" );

			this.UpdateColor();
		}

		public void UpdateColor()
		{
			var c = _role.Color;
			var r = c.r;
			var g = c.g;
			var b = c.b;

			// Desaturate
			var avg = (r + g + b) / 3f;
			var saturation = 0.5f;
			r += (avg - r) * saturation;
			g += (avg - g) * saturation;
			b += (avg - b) * saturation;
			var m = 0.7f;

			this.Style.BackgroundColor = new Color(r* m, g * m, b * m, 0.3f);

			var tint = $"rgb({c.r*255f}, {c.g*255f}, {c.b*255f})";
			_img.Style.Set( "background-image-tint", tint );
			this.Style.BorderColor = new Color( c.r, c.g, c.b, 0.5f );
		}

		private void OnRoleLoaded(VAdminRole role)
		{
			if (role.Id == _role?.Id)
			{
				this.SetRole( role );
			}
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			VAdminSystem.OnRoleLoaded -= this.OnRoleLoaded;
		}
	}
}
