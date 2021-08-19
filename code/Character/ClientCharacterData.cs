using Sandbox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Character
{
	public class ClientCharacterData
	{
		private Client _client;

		private Dictionary<int, Character> _characters = new Dictionary<int, Character>();
		private int _characterSlots = CharacterManager.DEFAULT_CHARACTER_SLOTS;

		public ClientCharacterData( Client client )
		{
			_client = client;
		}

		public Client Client
			=> _client;

		public int CharacterSlots
		{
			get => _characterSlots;
			set
			{
				_characterSlots = value;

				if ( Host.IsServer )
				{
					// TODO: Network
				}
			}
		}

		public Dictionary<int, Character> Characters
			=> _characters;

		public int GetCharacterSlot( Character character )
		{
			foreach ( var characterPair in _characters )
			{
				if ( characterPair.Value == character )
				{
					return characterPair.Key;
				}
			}

			return -1;
		}
		public Character SetCharacter( int slot, Character character )
		{
			_characters[slot] = character;
			return character;
		}
	}
}
