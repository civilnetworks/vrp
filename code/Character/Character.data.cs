using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Character
{
	public partial class Character
	{
		public Dictionary<string, string> StringData { get; set; } = new Dictionary<string, string>();

		public Dictionary<string, double> NumberData { get; set; } = new Dictionary<string, double>();

		public Dictionary<string, bool> BoolData { get; set; } = new Dictionary<string, bool>();

		public void SetStringData( string key, string value )
		{
			if ( this.StringData.ContainsKey( key ) )
			{
				this.StringData.Remove( key );
			}

			this.MakeDirty();

			if ( value == null )
			{
				return;
			}

			this.StringData[key] = value;
		}

		public string GetStringData( string key )
		{
			return this.StringData[key];
		}

		public void SetNumberData(string key, double value)
		{
			if ( this.NumberData.ContainsKey( key ) )
			{
				this.NumberData.Remove( key );
			}

			this.NumberData[key] = value;
			this.MakeDirty();
		}

		public void RemoveNumberData(string key)
		{
			this.NumberData.Remove( key );
			this.MakeDirty();
		}

		public bool TryGetNumberData(string key, out double value)
		{
			return this.NumberData.TryGetValue( key, out value );
		}

		public void SetBoolData(string key, bool value)
		{
			this.BoolData[key] = value;
		}

		public bool GetBoolData(string key)
		{
			if (this.BoolData.TryGetValue(key, out var value))
			{
				return value;
			}

			return false;
		}
	}
}
