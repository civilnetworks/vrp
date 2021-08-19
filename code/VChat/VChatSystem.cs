using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin;
using VRP.Character;

namespace VChat
{
	public static partial class VChatEvent
	{
		public const string Say = "vchatevent.say";

		public class SayAttribute : EventAttribute
		{
			public SayAttribute() : base( Say ) { }
		}
	}

	public static partial class VChatSystem
	{
		public const int MAX_MESSAGE_LENGTH = 124;

		public static string GetClientName(Client client)
		{
			var character = CharacterManager.Instance.GetActiveCharacter( client );
			if (character == null)
			{
				return client.Name;
			}
			else
			{
				return character.Name;
			}
		}

		[ServerCmd( "say" )]
		public static void Say( string message )
		{
			var cl = ConsoleSystem.Caller;

			Assert.NotNull( cl );

			if (!SanitizeText(message, out var sanitizedMessage))
			{
				return;
			}

			var msg = new VChatMessage()
			{
				Client = cl,
				SenderName = cl.Name,
				SenderSteamId = cl.SteamId,
				Message = sanitizedMessage,
				To = To.Everyone,
			};

			VAdminSystem.ProcessVChatMessage( msg );

			// TODO: Fix this
			// Allow events to modify message
			//Event.Run( VChatEvent.Say, msg );
			Event.Run( "vhat.say", "jizz" );

			if (msg.Abort)
			{
				return;
			}

			Log.Info( $"{ConsoleSystem.Caller}: {sanitizedMessage}" );
			AddChatEntry( msg.To, msg.SenderName, sanitizedMessage, msg.SenderSteamId == null ? null : $"avatar:{msg.SenderSteamId}" );
		}

		[ClientCmd( "chat_add", CanBeCalledFromServer = true )]
		public static void AddChatEntry( string name, string message, string avatar = null )
		{
			VrpChatBox.Instance?.AddEntry( name, message, avatar );

			// Only log clientside if we're not the listen server host
			if ( !Global.IsListenServer )
			{
				Log.Info( $"{name}: {message}" );
			}
		}

		[ClientCmd( "chat_addinfo", CanBeCalledFromServer = true )]
		public static void AddInformation( string message, string avatar = null )
		{
			VrpChatBox.Instance?.AddEntry( null, message, avatar );
		}
	}
}
