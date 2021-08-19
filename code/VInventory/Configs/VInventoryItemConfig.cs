using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VInventory
{
	public abstract class VInventoryItemConfig
	{
		private int _width;
		private int _height;

		public VInventoryItemConfig() { }

		public int Id { get; set; }

		[JsonIgnore]
		public abstract VInventoryItemType Type { get; }

		public string Name { get; set; }

		public string Category { get; set; }

		public string AttributeTitle { get; set; }

		public int Width
		{
			get => _width;
			set
			{
				_width = Math.Max( 1, value );
			}
		}

		public int Height
		{
			get => _height;
			set
			{
				_height = Math.Max( 1, value );
			}
		}

		[JsonIgnore]
		public abstract Type EntityType { get; }

		public abstract bool CreateEntity( VInventoryItem item, out Entity ent, out string error );

		public abstract bool CreateItem( Entity entity, out VInventoryItem item, out string error );

		protected abstract bool CanCreateConfigInternal( out string error );

		public bool CanCreateConfig( out string error )
		{
			if ( !this.IsConfigValid( out error ) )
			{
				return false;
			}

			return this.CanCreateConfigInternal( out error );
		}

		public virtual bool IsConfigValid( out string error )
		{
			if ( this.Width <= 0 )
			{
				error = "Width <= 0";
				return false;
			}

			if ( this.Height <= 0 )
			{
				error = "Width <= 0";
				return false;
			}

			if ( this.Id <= 0 )
			{
				error = "Id not set";
				return false;
			}

			error = null;
			return true;
		}

		public virtual VInventoryItemConfigAttribute[] GetCustomItemAttributes()
		{
			return null;
		}

		public Vector2 GetSize()
		{
			return new Vector2( this.Width, this.Height );
		}

		public void SetSize( Vector2 size )
		{
			this.Width = (int)size.x;
			this.Height = (int)size.y;
		}
	}
}
