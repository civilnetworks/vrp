using Sandbox;
using System.Linq;
using System.Text.Json;

namespace VRP.Character
{
	public partial class CharacterManager : VrpManager
	{
		public const int MAX_NAME_LENGTH = 25;
		public const int MIN_NAME_LENGTH = 3;
		public static readonly char[] VALID_NAME_CHARACTERS = "-.".ToCharArray();
		public const int MIN_BIO_LENGTH = 0;
		public const int MAX_BIO_LENGTH = 3000;
		public static readonly char[] VALID_BIO_CHARACTERS = "0123456789-.!\"£$%^&*()-=_+\\|[]{}#';~@:/.,?><".ToCharArray();

		#region Sanity Checks

		public static bool IsNameValid( string name, out string error )
		{
			if ( name == null )
			{
				error = "Name is null";
				return false;
			}

			if ( name.Length < MIN_NAME_LENGTH )
			{
				error = $"Name is too short ( < {MIN_NAME_LENGTH} )";
				return false;
			}

			if ( name.Length > MAX_NAME_LENGTH )
			{
				error = $"Name is too long ( > {MAX_NAME_LENGTH} )";
				return false;
			}

			foreach ( var c in name.ToCharArray() )
			{
				if ( !char.IsLetter( c ) && !c.Equals(' ') && !VALID_NAME_CHARACTERS.Contains( c ) )
				{
					error = $"Invalid character ( {c} )";
					return false;
				}
			}

			error = null;
			return true;
		}

		public static bool IsBioValid( string bio, out string error )
		{
			if ( bio == null )
			{
				error = "Bio is null";
				return false;
			}

			if ( bio.Length < MIN_BIO_LENGTH )
			{
				error = $"Bio is too short ( < {MIN_BIO_LENGTH} )";
				return false;
			}

			if ( bio.Length > MAX_BIO_LENGTH )
			{
				error = $"Bio is too long ( > {MAX_BIO_LENGTH} )";
				return false;
			}

			foreach ( var c in bio.ToCharArray() )
			{
				if ( !char.IsLetter( c ) && !c.Equals( ' ' ) && !VALID_NAME_CHARACTERS.Contains( c ) )
				{
					error = $"Invalid character ( {c} )";
					return false;
				}
			}

			error = null;
			return true;
		}

		#endregion

		public static void CreateCharacter( int slot, Character character )
		{
			CreateCharacter( slot, JsonSerializer.Serialize( character ) );
		}

		[ServerCmd]
		public static void CreateCharacter( int slot, string characterJson )
		{
			var client = ConsoleSystem.Caller;
			var character = JsonSerializer.Deserialize<Character>( characterJson );

			if ( CharacterManager.Instance.TryCreateCharacter( client, slot, character, out var error ) )
			{
				Log.Info( "Create success" );
			}
			else
			{
				Log.Info( "Create error " + error );
			}
		}

		[ServerCmd]
		public static void PlayCharacter(int slot)
		{
			var client = ConsoleSystem.Caller;

			if (CharacterManager.Instance.TryPlayCharacter(client, slot, out var error))
			{
				Log.Info( "Play character success" );
			}
			else
			{
				Log.Info( "Play character error " + error );
			}
		}

		[ServerCmd("vrp_character_quit")]
		public static void QuitCharacter()
		{
			var client = ConsoleSystem.Caller;
			
			if ( CharacterManager.Instance.TryQuitCharacter( client, out var error ))
			{
				Log.Info( "Quit character success" );
			}
			else
			{
				Log.Info( "Quit character error " + error );
			}
		}
	}
}
