using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.VGui.UI;

namespace VRP.Character.UI
{
	[Library]
	public class CharacterSlotCard : VGuiBasePanel
	{
		private Character _character;
		private Panel _fill;
		private VGuiLabel _createLabel;

		public CharacterSlotCard()
		{
			StyleSheet.Load( "/character/ui/CharacterSlotCard.scss" );

			this.SetClass( "character-slot-card", true );
			this.SetShadow( true );

			this.SetCharacter( _character );
		}

		public Character Character
			=> _character;

		public int Slot { get; set; }

		public Panel ClearFillContent()
		{
			this.CreateFill();

			return _fill;
		}

		public void SetCharacter(Character character)
		{
			_character = character;

			this.CreateFill();

			if ( character != null )
			{
				_fill.Add.Label( character.Name, "character-name" );

				var model = _fill.AddChild<CharacterModelPanel>("model");
				model.SetCharacterAppearance( character.GetAppearance(), character.AppearanceSettings );
				model.ModelPosition = new Vector3( -50, 0, -15 );
				// TODO
			}
			else
			{
				var icon = _fill.Add.Image( "materials/cg_icons/add_circle.png", "hint-icon" );

				_createLabel = _fill.AddChild<VGuiLabel>( "hint" );
				_createLabel.SetText( "Create a character" );
			}
		}

		public void HideContent( bool hide )
		{
			if ( _fill == null )
			{
				return;
			}

			_fill.Style.Opacity = hide ? 0f : 1f;
		}

		public bool HasCharacter()
		{
			return this.Character != null;
		}

		private void CreateFill()
		{
			_fill?.Delete();
			_fill = Add.Panel( "v-fill" );
			_fill.SetClass( "character-slot-fill", true );
		}
	}
}
