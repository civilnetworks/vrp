using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.VGui.UI;

namespace VChat
{
	public partial class VrpChatBox : Panel
	{
		public static VrpChatBox Instance;

		public Panel Canvas { get; protected set; }
		public VGuiTextEntry Input { get; protected set; }

		private bool _open;

		public VrpChatBox()
		{
			Instance = this;

			StyleSheet.Load( "/VChat/VrpChatBox.scss" );

			Canvas = Add.Panel( "chat_canvas" );

			Input = this.AddChild<VGuiTextEntry>( "" );
			Input.AddEventListener( "onsubmit", () => Submit() );
			Input.AddEventListener( "onblur", () => Close() );
			Input.AddEventListener( "onbuttontyped", ( e ) =>
			{
				if ( e.Value.ToString() == "tab" )
				{
					this.AutoCompleteHint();
				}
				else if ( e.Value.ToString() == "up" )
				{
					this.NextArgHint();
				}
				else if ( e.Value.ToString() == "down" )
				{
					this.PrevArgHint();
				}
			} );
			Input.AcceptsFocus = true;
			Input.AllowEmojiReplace = true;
			Input.PreventUpDown = true;

			Sandbox.Hooks.Chat.OnOpenChat += Open;
		}

		public bool IsOpen
			=> _open;


		void Open()
		{
			AddClass( "open" );
			Input.Focus();
			_open = true;
		}

		void Close()
		{
			RemoveClass( "open" );
			Input.Blur();
			Input.SetText( "" );
			this.ShowHints( false );
			_open = false;
		}

		void Submit()
		{
			var msg = Input.Text?.Trim() ?? string.Empty;

			Close();

			if ( string.IsNullOrWhiteSpace( msg ) )
				return;

			VChatSystem.Say( msg );
		}

		public void AddEntry( string name, string message, string avatar )
		{
			var e = Canvas.AddChild<ChatEntry>();
			//e.SetFirstSibling();
			e.Message.Text = message;
			e.NameLabel.Text = name;
			e.Avatar.SetTexture( avatar );

			e.SetClass( "noname", string.IsNullOrEmpty( name ) );
			e.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );
		}
	}
}

namespace Sandbox.Hooks
{
	public static partial class Chat
	{
		public static event Action OnOpenChat;

		[ClientCmd( "openchat" )]
		internal static void MessageMode()
		{
			OnOpenChat?.Invoke();
		}

	}
}
