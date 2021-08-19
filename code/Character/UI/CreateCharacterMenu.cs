using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using VRP.VGui.UI;

namespace VRP.Character.UI
{
	public delegate void CharacterMenuAppearanceUpdated( CharacterAppearanceSettings settings );

	public class CreateCharacterMenu : Panel, IClosablePanel
	{
		public event OnPanelClosed OnClosed;

		private Panel _fill;
		private VGuiTextEntry _nameEntry;
		private VGuiTextEntry _bioEntry;
		private Panel _appearanceContent;
		private VGuiButton _submitButton;
		private CharacterAppearanceSettings _appearanceSettings;
		private List<VGuiComboBoxWang> _appearanceOptions = new List<VGuiComboBoxWang>();

		public CharacterMenuAppearanceUpdated OnAppearanceUpdated;

		public CreateCharacterMenu()
		{
			StyleSheet.Load( "/character/ui/CreateCharacterMenu.scss" );

			_fill = Add.Panel( "scroll-content" );

			var characterSection = _fill.Add.Label( "Character", "section" );
			var characterBody = _fill.Add.Panel( "section-body" );

			_nameEntry = characterBody.AddChild<VGuiTextEntry>();
			_nameEntry.Placeholder = "Name...";
			_nameEntry.AllowEmojiReplace = false;

			_bioEntry = characterBody.AddChild<VGuiTextEntry>();
			_bioEntry.Placeholder = "Bio...";
			_bioEntry.AllowEmojiReplace = false;
			_bioEntry.Style.Height = 300;

			var appearanceSection = _fill.Add.Label( "Appearance", "section" );

			this.LoadAppearancePanel();

			_submitButton = this.AddChild<VGuiButton>( "create" );
			_submitButton.SetText( "Create Character" );
			_submitButton.AddEventListener( "onclick", ( e ) =>
			 {
				 var character = new Character( Local.Client )
				 {
					 Name = _nameEntry.Text ?? string.Empty,
					 Bio = _bioEntry.Text ?? string.Empty,
					 AppearanceSettings = this.AppearanceSettings
				 };

				 CharacterManager.CreateCharacter( this.Slot, character );

				 e.StopPropagation();
			 } );
		}

		public int Slot { get; set; }

		public CharacterAppearanceSettings AppearanceSettings
		{
			get => _appearanceSettings;
			set
			{
				_appearanceSettings = value;

				this.LoadAppearancePanel();
			}
		}

		public void LoadAppearancePanel()
		{
			_appearanceContent?.Delete();

			_appearanceContent = _fill.Add.Panel( "section-body" );

			if ( _appearanceSettings == null )
			{
				return;
			}

			var appearanceCombo = _appearanceContent.AddChild<VGuiComboBoxWang>();
			appearanceCombo.Combo.SetText( "Model" );
			appearanceCombo.Combo.OnSelect += ( VGuiComboBoxItem item ) =>
			{
				var appearance = item.Value as CharacterAppearance;
				_appearanceSettings.AppearanceName = appearance.Name;
				_appearanceSettings.RandomizeClothingAttachments();
				this.OnAppearanceUpdated?.Invoke( _appearanceSettings );
				this.ShowAppearanceOptions();
			};

			foreach ( var pair in CharacterManager.Instance.Appearances )
			{
				var listedAppearance = pair.Value;
				appearanceCombo.Combo.AddOption( listedAppearance.Model, listedAppearance );
			}

			this.ShowAppearanceOptions();
		}

		private void ShowAppearanceOptions()
		{
			foreach ( var opt in _appearanceOptions )
			{
				opt?.Delete();
			}

			_appearanceOptions.Clear();

			var appearance = _appearanceSettings.GetAppearance();
			if ( appearance == null )
			{
				return;
			}

			foreach ( var pair in appearance.ClothingAttachments )
			{
				var attachment = pair.Value;

				var clothingAttachmentCombo = _appearanceContent.AddChild<VGuiComboBoxWang>();
				clothingAttachmentCombo.Combo.SetText( attachment.Type.ToString() );
				clothingAttachmentCombo.Combo.OnSelect += ( VGuiComboBoxItem item ) =>
				{
					var appearance = item.Value as string;
					_appearanceSettings.SetClothingAttachmentModel( attachment.Type, appearance );
					this.OnAppearanceUpdated?.Invoke( _appearanceSettings );
				};

				if ( attachment.AllowNone )
				{
					clothingAttachmentCombo.Combo.AddOption( "None", null );
				}

				for ( var i = 0; i < attachment.Options.Length; i++ )
				{
					var option = attachment.Options[i];
					clothingAttachmentCombo.Combo.AddOption( option, option );
				}

				_appearanceOptions.Add( clothingAttachmentCombo );
			}
		}
	}
}
