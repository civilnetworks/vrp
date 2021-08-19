using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VInventory
{
	public class VInventoryItem
	{
		public VInventoryItem( long itemId, int configId )
		{
			this.ItemId = ItemId;
			this.ConfigId = configId;
		}

		public long ItemId { get; }

		public int ConfigId { get; }

		public Dictionary<string, VInventoryItemData> Data { get; set; } = new Dictionary<string, VInventoryItemData>();

		#region Setters & Getters

		public VInventoryItemData SetData( string key, object value )
		{
			this.Data.TryGetValue( key, out var current );

			if ( current != null )
			{
				this.Data.Remove( key );
			}

			if ( value == null )
			{
				return null;
			}

			current = current ?? new VInventoryItemData()
			{
				Value = value
			};

			this.Data.Add( key, current );

			return current;
		}

		public VInventoryItemData SetData( string key, VInventoryItemData data )
		{
			if ( this.Data.ContainsKey( key ) )
			{
				this.Data.Remove( key );
			}

			if ( data == null )
			{
				return null;
			}

			this.Data.Add( key, data );

			return data;
		}

		public object GetData( string key, object defaultValue = null )
		{
			if ( this.Data.TryGetValue( key, out var data ) )
			{
				return data.Value ?? defaultValue;
			}

			return defaultValue;
		}

		public float GetFloat( string key, float defaultValue = 0f )
		{
			if ( float.TryParse( this.GetData( key )?.ToString(), out var value ) )
			{
				return value;
			}

			return defaultValue;
		}

		public double GetDouble( string key, double defaultValue = 0d )
		{
			if ( double.TryParse( this.GetData( key )?.ToString(), out var value ) )
			{
				return value;
			}

			return defaultValue;
		}

		public ulong GetUnsignedLong( string key, ulong defaultValue = 0ul )
		{
			if ( ulong.TryParse( this.GetData( key )?.ToString(), out var value ) )
			{
				return value;
			}

			return defaultValue;
		}

		public bool GetBool( string key, bool defaultValue = false )
		{
			if ( bool.TryParse( this.GetData( key )?.ToString(), out var value ) )
			{
				return value;
			}

			return defaultValue;
		}

		#endregion

	}
}
