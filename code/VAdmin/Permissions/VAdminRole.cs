using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VAdmin.Permissions
{
	public class VAdminRole
	{
		private string _name;
		private bool _display;
		private bool _staff;
		private bool _enabled = true;
		private float _r, _g, _b;
		private int _level;

		private bool _dirty;

		public VAdminRole( string id, string name, bool display, Color color, int level )
		{
			this.Id = id;
			this.Name = name;
			this.Display = display;
			this.Color = color;
			this.Level = level;
		}

		public string Id { get; }

		public string Name
		{
			get => _name;
			set
			{
				if ( _name != null && _name.Equals( value ) )
				{
					return;
				}

				_name = value;
				this.MakeDirty();
			}
		}

		public bool Display
		{
			get => _display;
			set
			{
				if ( _display == value )
				{

					return;
				}

				_display = value;
				this.MakeDirty();
			}
		}

		public bool Staff
		{
			get => _staff;
			set
			{
				if ( _staff == value )
				{
					return;
				}

				_staff = value;
			}
		}

		public bool Enabled
		{
			get => _enabled;
			set
			{
				if ( _enabled == value )
				{
					return;
				}

				_enabled = value;
				this.MakeDirty();
			}
		}

		public bool Root { get; set; }

		public Color Color
		{
			get => new Color( _r, _g, _b, 1 );
			set
			{
				if ( _r == value.r && _g == value.g && _b == value.b )
				{
					return;
				}

				_r = value.r;
				_g = value.g;
				_b = value.b;

				this.MakeDirty();
			}
		}

		public int Level
		{
			get => _level;
			set
			{
				if ( _level == value )
				{
					return;
				}

				_level = value;
				this.MakeDirty();
			}
		}

		public Dictionary<string, VAdminRolePermission> Permissions { get; set; } = new Dictionary<string, VAdminRolePermission>();

		[JsonIgnore]
		public bool IsDirty
			=> _dirty;

		public void MakeDirty( bool dirty = true )
		{
			_dirty = dirty;
		}

		#region Permissions

		public void SetPermission( VAdminRolePermission permission )
		{
			if ( this.Permissions.ContainsKey( permission.Permission ) )
			{
				this.Permissions.Remove( permission.Permission );
			}

			if ( !permission.Granted && !permission.Priority && !permission.OverrideLevel )
			{
				// Permission is useless, so remove.
				return;
			}
			else
			{
				this.Permissions.Add( permission.Permission, permission );
			}
		}

		public VAdminRolePermission GetPermission( string name )
		{
			if ( this.Root )
			{
				return new VAdminRolePermission()
				{
					Permission = name,
					Granted = true,
					Priority = true,
				};
			}

			if ( this.Permissions.TryGetValue( name, out var permission ) )
			{
				return permission;
			}

			return new VAdminRolePermission()
			{
				Permission = name,
			};
		}

		#endregion
	}
}
