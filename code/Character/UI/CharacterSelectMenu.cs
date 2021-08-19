using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using VRP.Character.UI;
using VRP.Character;
using VRP.VGui.UI;

[Library]
public partial class CharacterSelectMenu : VGuiBasePanel
{
	public static CharacterSelectMenu Instance;

	private List<CharacterSlotCard> _characterCards = new List<CharacterSlotCard>();
	private CharacterSlotCard _selectedCard;
	private Panel _left;
	private Panel _right;
	private CharacterModelPanel _modelPanel;

	private bool _menuOpen;

	public CharacterSelectMenu() : base()
	{
		Instance = this;

		StyleSheet.Load( "/character/ui/CharacterSelectMenu.scss" );

		_left = Add.Panel( "left" );

		_right = Add.Panel( "right" );
		var rightHeader = _right.AddChild<VGuiHeader>();
		rightHeader.SetTitle( "" );
		rightHeader.SetBarWidth( 0 );
		rightHeader.SetIcon( "materials/vrp/icon.png" );

		_modelPanel = _right.AddChild<CharacterModelPanel>();
		_modelPanel.SetDefaultModel();
		_modelPanel.ModelPosition = new Vector3( 0, 0, 2f );

		this.CreateCards( _left );

		this.MenuOpen = CharacterManager.Instance?.GetActiveCharacter( Local.Client ) == null;
	}

	public bool MenuOpen
	{
		get => _menuOpen;
		set
		{
			_menuOpen = value;

			_selectedCard?.SetCharacter( _selectedCard?.Character );
			this.SelectCard( null );

			if ( _menuOpen )
			{
				this.Style.Opacity = 1;
				this.Style.PointerEvents = null;
				this.Style.Dirty();
			}
			else
			{
				this.Style.Opacity = 0;
				this.Style.PointerEvents = "none";
				this.Style.Dirty();
			}
		}
	}

	public override void Tick()
	{
		base.Tick();
	}

	public void ReloadCharacterCards()
	{
		this.CreateCards( _left );
	}

	private void CreateCards( Panel parent )
	{
		foreach (var card in _characterCards )
		{
			card?.Delete();
		}

		_characterCards.Clear();

		this.ShowModel( false );

		if ( CharacterManager.Instance == null || !CharacterManager.Instance.TryGetCharacterData( Local.Client, out var data ) )
		{
			// Loading characters...

			return;
		}

		for ( var i = 0; i < data.CharacterSlots; i++ )
		{
			data.Characters.TryGetValue( i, out var character );

			var slot = parent.AddChild<CharacterSlotCard>();
			slot.SetCharacter( character );
			slot.Slot = i;

			slot.AddEventListener( "onclick", ( e ) =>
			{
				this.SelectCard( slot );
			} );

			_characterCards.Add( slot );
		}
	}

	private void SelectCard( CharacterSlotCard card )
	{
		if ( _selectedCard == card )
		{
			return;
		}

		foreach ( var c in _characterCards )
		{
			c.SetClass( "character-card-selected", c == card );
			c.HideContent( c == card );
		}

		_selectedCard = card;

		if ( card == null )
		{
			this.ShowModel( false );
			return;
		}

		var fill = card.ClearFillContent();
		var header = fill.AddChild<VGuiHeader>();
		header.ShowCloseButton( true, () =>
		{
			card?.SetCharacter( card?.Character );
			this.SelectCard( null );
		} );

		if ( card.HasCharacter() )
		{
			// Character summary menu
			header.SetTitle( "Character Summary" );
			header.SetIcon( "materials/cg_icons/player.png" );
			var summaryMenu = fill.AddChild<CharacterSummaryMenu>();
			summaryMenu.Character = card.Character;
			summaryMenu.Slot = card.Slot;

			_modelPanel.SetCharacterAppearance( card.Character.GetAppearance(), card.Character.AppearanceSettings );

			this.ShowModel( true );
		}
		else
		{
			// Create character menu
			header.SetTitle( "Create Character" );
			header.SetIcon( "materials/cg_icons/add_circle.png" );

			var createMenu = fill.AddChild<CreateCharacterMenu>();
			var appearanceSettings = new CharacterAppearanceSettings();
			appearanceSettings.RandomizeClothingAttachments();
			createMenu.AppearanceSettings = appearanceSettings;
			createMenu.Slot = card.Slot;

			_modelPanel.SetCharacterAppearance( appearanceSettings.GetAppearance(), appearanceSettings );
			createMenu.OnAppearanceUpdated += (CharacterAppearanceSettings settings) => {
				_modelPanel.SetCharacterAppearance( settings.GetAppearance(), settings );
			};

			this.ShowModel( true );
		}
	}

	private void ShowModel( bool showModel )
	{
		_right.SetClass( "right-visible", showModel );
	}

	[ClientRpc]
	public static void RpcDisplaySelectMenu(bool display)
	{
		if ( Instance != null)
		{
			Instance.MenuOpen = display;
		}
	}
}
