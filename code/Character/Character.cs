using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VRP.Character
{
	public partial class Character
	{
		private Client _client;
		private bool _dirty;

		private string _name = "Character Name";
		private string _bio = "Character Bio";
		private CharacterAppearanceSettings _appearanceSettings;

		public Character(Client client)
		{
			_client = client;
		}

		[JsonIgnore]
		public bool IsDirty
			=> _dirty;

		[JsonIgnore]
		public Client Client
			=> _client;

		public string Name
		{
			get => _name;
			set
			{
				if ( _name.Equals( value ) )
				{
					return;
				}

				_name = value;
				this.MakeDirty();
			}
		}

		public string Bio
		{
			get => _bio;
			set
			{
				if ( _bio.Equals( value ) )
				{
					return;
				}

				_bio = value;
				this.MakeDirty();
			}
		}

		public CharacterAppearanceSettings AppearanceSettings
		{
			get => _appearanceSettings;
			set
			{
				// TODO: Compare properly
				if ( _appearanceSettings != null && _appearanceSettings.Equals( value ) )
				{
					return;
				}

				_appearanceSettings = value;
				this.MakeDirty();
			}
		}

		public void MakeDirty(bool dirty = true)
		{
			_dirty = dirty;
		}

		public CharacterAppearance GetAppearance()
		{
			var name = this.AppearanceSettings?.AppearanceName ?? CharacterManager.DEFAULT_APPEARANCE;
			if ( !CharacterManager.Instance.TryGetAppearance( name, out var appearance ) )
			{
				appearance = CharacterManager.Instance.GetDefaultAppearance();
			}

			return appearance;
		}

		public void CopyFrom(Character other)
		{
			this.Name = other.Name;
			this.Bio = other.Bio;
			this.AppearanceSettings = other.AppearanceSettings;
			this.StringData = other.StringData;
			this.NumberData = other.NumberData;
			this.BoolData = other.BoolData;
		}
	}
}
