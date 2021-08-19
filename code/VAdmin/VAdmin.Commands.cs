using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VAdmin.Commands;

namespace VAdmin
{
	public static partial class VAdminSystem
	{
		public const char COMMAND_PREFIX = '/';
		public const string TARGET_ALL = "*";
		public const string TARGET_SELF = "^";
		public const string KEYWORD_TARGET = "[target]";

		private static Dictionary<string, VAdminCommand> COMMANDS = new Dictionary<string, VAdminCommand>();

		public static void LoadCommands()
		{
			COMMANDS.Clear();

			LoadCommand( new Menu() );

			LoadCommand( new Kick() );
			LoadCommand( new KickId() );
			LoadCommand( new Ban() );

			LoadCommand( new Goto() );
			LoadCommand( new Bring() );
			LoadCommand( new Teleport() );
			LoadCommand( new Send() );

			LoadCommand( new Slap() );
			LoadCommand( new Slay() );

			LoadCommand( new CreateRole() );
			LoadCommand( new DeleteRole() );
			LoadCommand( new Grant() );
			LoadCommand( new Revoke() );
			LoadCommand( new RevokeId() );
			LoadCommand( new SetPermissionOverride() );
			LoadCommand( new GrantPermission() );
			LoadCommand( new RevokePermission() );

			LoadCommand( new DropMoney() );
			LoadCommand( new GiveMoney() );
			LoadCommand( new SetMoney() );
			LoadCommand( new AddMoney() );
			LoadCommand( new SetItemData() );
			LoadCommand( new PrintItemData() );
			LoadCommand( new ForceLock() );
			LoadCommand( new EditVehicleSeats() );
			LoadCommand( new VInventoryAdmin() );

			Event.Run( "vadmin.load" );
		}

		public static void LoadCommand( VAdminCommand command )
		{
			if ( COMMANDS.ContainsKey( command.Name ) )
			{
				COMMANDS.Remove( command.Name );
			}

			COMMANDS[command.Name] = command;
		}

		public static VAdminCommand[] GetCommands()
		{
			return COMMANDS.Select( p => p.Value ).ToArray();
		}

		public static VAdminCommand[] GetCommands( string category )
		{
			return GetCommands().Where( c => c.Category == category ).ToArray();
		}

		public static string[] GetCommandCategories()
		{
			return GetCommands().Where( c => c.Category != null ).Select( c => c.Category ).Distinct().OrderBy( c => c ).ToArray();
		}

		public static bool EvaluateCommandEntry( string input, out CommandEvaluation evaluation, Client caller = null, bool insertEmptyArgs = false )
		{
			evaluation = new CommandEvaluation();

			if ( input == null || input.Length <= 1 )
			{
				return false;
			}

			if ( !input.StartsWith( COMMAND_PREFIX ) )
			{
				return false;
			}

			input = input.Remove( 0, 1 );

			var pieces = GetCommandPieces( input );
			if ( pieces.Length == 0 )
			{
				return false;
			}

			evaluation.Args = pieces.ToList().Skip( 1 ).ToArray();
			var validCommands = new List<VAdminCommand>();

			foreach ( var pair in COMMANDS )
			{
				var cmd = pair.Value;

				if ( cmd.Name.ToLower().StartsWith( pieces[0].ToLower() ) )
				{
					validCommands.Add( cmd );
				}
			}

			if ( validCommands.Count == 0 || !validCommands[0].Name.ToLower().Equals( pieces[0] ) )
			{
				evaluation.Commands = validCommands.ToArray();
				evaluation.CurrentError = $"Unknown command '{COMMAND_PREFIX + pieces[0]}'";

				evaluation.Validation = ArgValidation.Warning;

				return true;
			}

			validCommands.Sort( ( a, b ) =>
			 {
				 return a.Name.Length - b.Name.Length;
			 } );

			evaluation.Validation = ArgValidation.Valid;
			evaluation.Commands = validCommands.ToArray();

			var command = evaluation.Commands[0];
			var statusList = new List<ArgInputStatus>();

			for ( var i = 1; i < Math.Max( pieces.Length, command.Args.Length + 1 ); i++ )
			{
				var piece = i < pieces.Length ? pieces[i] : null;

				if ( i > command.Args.Length )
				{
					if ( string.IsNullOrWhiteSpace( piece ) )
					{
						break;
					}

					evaluation.ArgStatusList = statusList.ToArray();
					evaluation.Validation = ArgValidation.Error;
					evaluation.CurrentError = "Too many arguments";
					return true;
				}

				var arg = command.Args[i - 1];
				var status = new ArgInputStatus()
				{
					Arg = arg
				};
				statusList.Add( status );

				if ( piece == null || piece.Length == 0 )
				{
					if ( arg.Optional )
					{
						break;
					}

					// No input for this argument
					if ( evaluation.Validation == ArgValidation.Valid )
					{
						evaluation.CurrentError = $"Missing argument '{arg.Name}'";
						evaluation.Validation = ArgValidation.ExecutionError;
					}
				}
				else if ( (status.Validation = ValidateArg( arg, piece, out var argError, caller )) == ArgValidation.Valid )
				{
					// Valid arg
					status.InProgress = true;
				}
				else
				{
					// Invalid arg
					status.InProgress = true;

					evaluation.CurrentError = evaluation.CurrentError ?? argError;

					if ( evaluation.Validation == ArgValidation.Valid )
					{
						evaluation.Validation = status.Validation;
					}
				}
			}

			evaluation.ArgStatusList = statusList.ToArray();

			// Insert empty optional args
			if ( insertEmptyArgs && evaluation.IsValid && evaluation.Args.Length < command.Args.Length )
			{
				var newArgs = evaluation.Args.ToList();

				var missing = command.Args.Length - evaluation.Args.Length;
				for ( var i = 0; i < missing; i++ )
				{
					if ( !command.Args[command.Args.Length - (missing - i)].Optional )
					{
						// If the first missing argument is not optional then input is invalid.
						break;
					}

					newArgs.Add( null );
				}

				evaluation.Args = newArgs.ToArray();
			}

			return true;
		}

		public static ArgValidation ValidateArg( CommandArg arg, string value, out string error, Client caller = null )
		{
			var argValidationType = ValidateArgType( value, arg.Type, out var customError, caller );
			if ( argValidationType != ArgValidation.Valid )
			{
				error = customError ?? $"'{arg.Name}' expects '{arg.Type}'";
				return argValidationType;
			}

			return arg.Validate( value, out error );
		}

		public static ArgValidation ValidateArgType( string value, CommandArgType type, out string customError, Client caller = null )
		{
			customError = null;

			switch ( type )
			{
				case CommandArgType.Bool:
					return bool.TryParse( value, out var bval ) ? ArgValidation.Valid : ArgValidation.Error;
				case CommandArgType.String:
					return value.Length > 0 ? ArgValidation.Valid : ArgValidation.Error;
				case CommandArgType.Number:
					return double.TryParse( value, out var nval ) ? ArgValidation.Valid : ArgValidation.Error;
				case CommandArgType.WholeNumber:
					if ( !double.TryParse( value, out var wnval ) )
					{
						return ArgValidation.Error;
					}

					return Math.Round( wnval ) == wnval ? ArgValidation.Valid : ArgValidation.Error;
				case CommandArgType.Client:
					if ( FindClients( value, caller ).Length == 0 )
					{
						if ( value.Equals( TARGET_ALL ) || value.Equals( TARGET_SELF ) )
						{
							customError = $"No clients found";
						}
						else
						{
							customError = $"No clients found with name '{value}'";
						}

						return ArgValidation.Error;
					}

					return ArgValidation.Valid;
				case CommandArgType.ClientId:
					if ( FindClientTargetById( value ) == null )
					{
						customError = $"No clients found with steamid {value}";
						return ArgValidation.Error;
					}

					return ArgValidation.Valid;
				case CommandArgType.Role:
					if ( VAdminSystem.GetRoles().FirstOrDefault( r => r.Name.Equals( value ) ) == null )
					{
						customError = $"Unknown role '{value}'";
						return ArgValidation.Warning;
					}
					else
					{
						return ArgValidation.Valid;
					}
				case CommandArgType.Permission:
					if ( TryGetPermission( value, out var permission ) )
					{
						return ArgValidation.Valid;
					}
					else
					{
						customError = $"Unknown permission '{value}'";
						return ArgValidation.Warning;
					}
				case CommandArgType.Timeframe:
					customError = "Invalid timeframe";
					return ParseTimeframe( value, out var timeframe ) ? ArgValidation.Valid : ArgValidation.Error;
				default:
					return ArgValidation.Error;
			}
		}

		public static Client[] FindClients( string value, Client caller = null )
		{
			var clients = new List<Client>();

			foreach ( var cl in Client.All )
			{
				if ( value == null || value.Equals( TARGET_ALL ) || (value.Equals( TARGET_SELF ) && cl == caller) || cl.Name.ToLower().Contains( value.ToLower() ) )
				{
					if ( clients.FirstOrDefault( c => c.Name.Equals( cl.Name ) ) != null )
					{
						// Two players with the same name??? Abort
						continue;
					}

					clients.Add( cl );
				}
			}

			return clients.ToArray();
		}

		public static Client FindClientTarget( string value, Client caller = null )
		{
			var clients = FindClients( value, caller );
			if ( clients.Length == 0 )
			{
				return null;
			}

			return clients[0];
		}

		public static Client FindClientTargetById( string value )
		{
			var sid = value.Split( " " )[0];

			return Client.All.FirstOrDefault( c => c.SteamId.ToString().Equals( sid ) );
		}

		/// <summary>
		/// Finds a target Client for the given command arguments.
		/// NOTE: Assumes the first argument is the target.
		/// </summary>
		/// <param name="args">The command arguments.</param>
		/// <returns>The target Client.</returns>
		public static Client GetCommandTarget( string[] args, Client caller = null )
		{
			return FindClientTarget( args[0], caller );
		}

		public static string[] GetArgSuggestions( CommandArg arg, string value, Client caller = null )
		{
			var suggestions = new List<string>();

			if ( arg.Suggestions != null )
			{
				var argSuggests = arg.Suggestions( value );

				if ( argSuggests != null )
				{
					foreach ( var suggest in argSuggests )
					{
						suggestions.Add( suggest );
					}
				}
			}

			var alphabetically = false;

			switch ( arg.Type )
			{
				case CommandArgType.Bool:
					suggestions.Add( "true" );
					suggestions.Add( "false" );
					break;
				case CommandArgType.Client:
					foreach ( var cl in FindClients( value, caller ) )
					{
						suggestions.Add( $"\"{cl.Name}\"" );
					}
					break;
				case CommandArgType.ClientId:
					foreach ( var cl in Client.All )
					{
						if ( value == null || cl.SteamId.ToString().StartsWith( value ) )
						{
							suggestions.Add( $"\"{cl.SteamId} ( {cl.Name} )\"" );
						}
					}

					alphabetically = true;
					break;
				case CommandArgType.Role:
					foreach ( var role in VAdminSystem.GetRoles().Select( r => r.Name ) )
					{
						suggestions.Add( $"\"{role}\"" );
					}
					break;
				case CommandArgType.Permission:
					foreach ( var permId in VAdminSystem.GetPermissions().Select( r => r.Name ) )
					{
						suggestions.Add( $"\"{permId}\"" );
					}
					break;
			}

			// Sort & Filter results
			var validSuggestions = new List<string>();
			foreach ( var suggest in suggestions )
			{
				if ( value != null && !suggest.ToLower().Replace( "\"", "" ).StartsWith( value.ToLower() ) )
				{
					continue;
				}
				validSuggestions.Add( suggest );
			}

			foreach ( var suggest in suggestions )
			{
				if ( value != null && !suggest.ToLower().Replace( "\"", "" ).Contains( value.ToLower() ) )
				{
					continue;
				}
				validSuggestions.Add( suggest );
			}

			if ( (value?.Length ?? 0) == 0 || alphabetically )
			{
				// Sort alphabetically, removing digits from the sort.
				return validSuggestions.Distinct().OrderBy( s => new string( s.ToCharArray().Where( c => !char.IsDigit( c ) ).ToArray() ) ).ToArray();
			}
			else
			{
				return validSuggestions.Distinct().OrderBy( s => s.Length ).ToArray();
			}
		}

		public static string[] GetCommandPieces( string input )
		{
			var pieces = new List<string>();
			var currentWord = String.Empty;
			var inQuotes = false;

			foreach ( var c in input )
			{
				if ( char.IsWhiteSpace( c ) )
				{
					if ( inQuotes )
					{
						currentWord += c;
					}
					else
					{
						pieces.Add( currentWord );
						currentWord = String.Empty;
					}
				}
				else if ( c.Equals( '"' ) )
				{
					pieces.Add( currentWord );
					currentWord = String.Empty;

					inQuotes = !inQuotes;
				}
				else
				{
					currentWord += c;
				}
			}

			pieces.Add( currentWord );

			pieces = pieces.Where( s => !string.IsNullOrWhiteSpace( s ) ).Select( s => s.Trim() ).ToList();

			if ( input.EndsWith( " " ) )
			{
				pieces.Add( "" );
			}

			return pieces.ToArray();
		}

		public static string CreateCommandString( string command, string[] args )
		{
			var str = $"{VAdminSystem.COMMAND_PREFIX}{command}";

			foreach ( var arg in args )
			{
				if ( arg.Contains( " " ) )
				{
					str += $" \"{arg.Replace( "\"", "" )}\"";
				}
				else
				{
					str += $" {arg}";
				}
			}

			return str;
		}

		public static void CmdRunCommand( string command, List<string> args )
		{
			CmdRunCommand( CreateCommandString( command, args.ToArray() ) );
		}

		[ServerCmd]
		public static void CmdRunCommand( string commandString )
		{
			RunCommand( ConsoleSystem.Caller, commandString );
		}

		/// <summary>
		/// Compute the distance between two strings.
		/// </summary>
		public static int ComputeLevenshteinDistance( string s, string t )
		{
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];

			// Step 1
			if ( n == 0 )
			{
				return m;
			}

			if ( m == 0 )
			{
				return n;
			}

			// Step 2
			for ( int i = 0; i <= n; d[i, 0] = i++ )
			{
			}

			for ( int j = 0; j <= m; d[0, j] = j++ )
			{
			}

			// Step 3
			for ( int i = 1; i <= n; i++ )
			{
				//Step 4
				for ( int j = 1; j <= m; j++ )
				{
					// Step 5
					int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

					// Step 6
					d[i, j] = Math.Min(
						Math.Min( d[i - 1, j] + 1, d[i, j - 1] + 1 ),
						d[i - 1, j - 1] + cost );
				}
			}
			// Step 7
			return d[n, m];
		}
	}
}
