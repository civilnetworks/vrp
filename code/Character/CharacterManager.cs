using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace VRP.Character
{
	public partial class CharacterManager : VrpManager
	{
		public const int DEFAULT_CHARACTER_SLOTS = 3;

		public static CharacterManager Instance;

		private Dictionary<ulong, ClientCharacterData> _clientCharacterData = new Dictionary<ulong, ClientCharacterData>();
		private Dictionary<ulong, Character> _activeCharacters = new Dictionary<ulong, Character>();

		public CharacterManager()
		{
			Instance = this;

			Transmit = TransmitType.Always;

			this.RegisterAppearances();
		}

		public override void InitializeClient( Client client )
		{
			this.LoadCharacterData( client );
		}

		public bool TryGetCharacterData( Client client, out ClientCharacterData data )
		{
			return _clientCharacterData.TryGetValue( client.SteamId, out data );
		}

		public void LoadCharacterData( Client client )
		{
			var isBot = client.SteamId.ToString().StartsWith( "9007" );
			if ( isBot )
			{
				// Bot??? :what meme:
				var appearance = new CharacterAppearanceSettings();
				appearance.RandomizeClothingAttachments();

				var botCharacter = new Character( client )
				{
					Name = "[Bot] " + client.Name,
					AppearanceSettings = appearance
				};

				var data = new ClientCharacterData( client );
				data.SetCharacter( 0, botCharacter );
				_clientCharacterData[client.SteamId] = data;
			}
			else
			{
				// Real player
				// TODO: Load real data or create sensible data
				var data = new ClientCharacterData( client );
				_clientCharacterData[client.SteamId] = data;

				data.SetCharacter( 1, new Character( client )
				{
					Name = "Poo man",
				} );
			}

			this.NetworkCharacterData( client );
			TryGetCharacterData( client, out var clientData );

			foreach ( var pair in clientData.Characters )
			{
				this.NetworkCharacter( client, pair.Value );
			}

			if ( isBot )
			{
				if ( !TryPlayCharacter( client, 0, out var error ) )
				{
					Log.Warning( $"VCharacterManager.LoadCharacterData failed to play Bot character: {error}" );
				}
			}
		}

		public Character GetActiveCharacter( Client client )
		{
			if ( _activeCharacters.ContainsKey( client.SteamId ) )
			{
				return _activeCharacters[client.SteamId];
			}

			return null;
		}

		public bool HasActiveCharacter( Client client )
		{
			return this.GetActiveCharacter( client ) != null;
		}

		#region ClientRpc

		[ClientRpc]
		public void NetworkActiveCharacter( ulong steamid, string characterJson )
		{
			Character character = null;

			if ( characterJson != null && characterJson.Length > 0 )
			{
				character = JsonSerializer.Deserialize<Character>( characterJson );
			}

			if ( _activeCharacters.ContainsKey( steamid ) )
			{
				_activeCharacters.Remove( steamid );
			}

			if ( character != null )
			{
				_activeCharacters[steamid] = character;
			}
		}

		public void NetworkCharacterData( Client client )
		{
			if ( !this.TryGetCharacterData( client, out var data ) )
			{
				return;
			}


			this.NetworkCharacterData( client.NetworkIdent, data.CharacterSlots );
		}

		[ClientRpc]
		public void NetworkCharacterData( int clientNetworkIdent, int characterSlots )
		{
			var client = Client.All.FirstOrDefault( cl => cl.NetworkIdent == clientNetworkIdent );
			if ( client == null )
			{
				return;
			}

			if ( !this.TryGetCharacterData( client, out var data ) )
			{
				// Create new character data on client.
				data = new ClientCharacterData( client );
				_clientCharacterData[client.SteamId] = data;
			}

			data.CharacterSlots = characterSlots;
		}

		public void NetworkCharacter( Client client, Character character )
		{
			if ( !this.TryGetCharacterData( client, out var data ) )
			{
				return;
			}

			this.NetworkCharacter( client.NetworkIdent, data.GetCharacterSlot( character ), JsonSerializer.Serialize( character ) );
		}

		public void NetworkCharacter( To to, Client client, Character character )
		{
			if ( !this.TryGetCharacterData( client, out var data ) )
			{
				return;
			}

			this.NetworkCharacter( to, client.NetworkIdent, data.GetCharacterSlot( character ), JsonSerializer.Serialize( character ) );
		}

		[ClientRpc]
		public void NetworkCharacter( int clientNetworkIdent, int slot, string characterJson )
		{
			var client = Client.All.FirstOrDefault( cl => cl.NetworkIdent == clientNetworkIdent );
			if ( client == null )
			{
				return;
			}

			if ( !this.TryGetCharacterData( client, out var data ) )
			{
				return;
			}

			if ( !data.Characters.TryGetValue( slot, out var character ) )
			{
				character = data.SetCharacter( slot, new Character( client ) );
			}

			// Update the existing cached character with the new data.
			var dataChar = JsonSerializer.Deserialize<Character>( characterJson );
			character.CopyFrom( dataChar );

			// Check if the active character object needs updating too.
			var activeCharacter = this.GetActiveCharacter( client );
			if ( activeCharacter != null && activeCharacter.Name.Equals( character.Name ) )
			{
				activeCharacter.CopyFrom( dataChar );
			}

			if ( client == Local.Client && activeCharacter == null )
			{
				CharacterSelectMenu.Instance?.ReloadCharacterCards();
			}
		}

		#endregion

		[Event.Tick]
		public void Tick()
		{

		}

		#region Commands

		[ClientCmd( "character_manager_info" )]
		public static void CmdInfo()
		{
			var manager = CharacterManager.Instance;

			Log.Info( "" );
			Log.Info( "== Begin Character Manager Info ==" );
			Log.Info( "" );

			foreach ( var pair in manager._clientCharacterData )
			{
				var data = pair.Value;

				Log.Info( $"	= Character Data [{pair.Key}] =" );
				Log.Info( $"	Slot: {data.CharacterSlots}" );
				Log.Info( $"	Characters ( {data.Characters.Count} ):" );

				foreach ( var characterPair in data.Characters )
				{
					var character = characterPair.Value;

					Log.Info( $"		= Character [{characterPair.Key}] =" );
					Log.Info( $"		Name: {character.Name}" );
				}

				Log.Info( "" );
			}

			Log.Info( "== End Character Manager Info ==" );
			Log.Info( "" );
		}

		#endregion
	}
}
