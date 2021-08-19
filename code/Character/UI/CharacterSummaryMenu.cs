using Sandbox.UI;
using Sandbox.UI.Construct;
using VRP.VGui.UI;

namespace VRP.Character.UI
{
	public class CharacterSummaryMenu : Panel, IClosablePanel
	{
		public event OnPanelClosed OnClosed;

		private Character _character;
		private Panel _fill;
		private VGuiTextEntry _nameEntry;
		private VGuiTextEntry _bioEntry;
		private VGuiButton _submitButton;

		public CharacterSummaryMenu()
		{
			StyleSheet.Load( "/character/ui/CharacterSummaryMenu.scss" );

			_fill = Add.Panel( "scroll-content" );

			var characterSection = _fill.Add.Label( "Character", "section" );
			var characterBody = _fill.Add.Panel( "section-body" );

			_nameEntry = characterBody.AddChild<VGuiTextEntry>();
			_nameEntry.AllowEmojiReplace = false;
			_nameEntry.AcceptsFocus = false;

			_bioEntry = characterBody.AddChild<VGuiTextEntry>();
			_bioEntry.AllowEmojiReplace = false;
			_bioEntry.Style.Height = 300;
			_bioEntry.AcceptsFocus = false;

			_submitButton = this.AddChild<VGuiButton>( "create" );
			_submitButton.SetText( "Play" );
			_submitButton.AddEventListener( "onclick", ( e ) =>
			{
				CharacterManager.PlayCharacter( this.Slot );

				e.StopPropagation();
			} );
		}

		public int Slot { get; set; }

		public Character Character
		{
			get => _character;
			set
			{
				_character = value;

				_nameEntry.SetText( value?.Name );
				_bioEntry.SetText( value?.Bio );
				_submitButton.SetText($"Play as {value?.Name ?? "N/A"}");
			}
		}
	}
}
