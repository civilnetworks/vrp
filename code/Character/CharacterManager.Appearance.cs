using Sandbox;
using System.Collections.Generic;
using VRP;
using VRP.Character;

namespace VRP.Character
{
	public partial class CharacterManager : VrpManager
	{
		public static string DEFAULT_APPEARANCE = "terry";

		private Dictionary<string, CharacterAppearance> _appearances = new Dictionary<string, CharacterAppearance>();

		public void RegisterAppearances()
		{
			_appearances.Clear();

			// Default appearance ( Terry )
			var defaultAppearance = this.AddAppearance( new CharacterAppearance( DEFAULT_APPEARANCE, "models/citizen/citizen.vmdl" ) );
			defaultAppearance.AddClothingAttachment( new CharacterClothingAttachment( CharacterClothingAttachmentType.Head, "", null, true ) )
				.Options = new string[] {
					"models/citizen_clothes/hat/hat_hardhat.vmdl",
					"models/citizen_clothes/hat/hat_woolly.vmdl",
					"models/citizen_clothes/hat/hat_securityhelmet.vmdl",
					"models/citizen_clothes/hair/hair_malestyle02.vmdl",
					"models/citizen_clothes/hair/hair_femalebun.black.vmdl",
					"models/citizen_clothes/hat/hat_beret.red.vmdl",
					"models/citizen_clothes/hat/hat.tophat.vmdl",
					"models/citizen_clothes/hat/hat_beret.black.vmdl",
					"models/citizen_clothes/hat/hat_cap.vmdl",
					"models/citizen_clothes/hat/hat_leathercap.vmdl",
					"models/citizen_clothes/hat/hat_leathercapnobadge.vmdl",
					"models/citizen_clothes/hat/hat_securityhelmetnostrap.vmdl",
					"models/citizen_clothes/hat/hat_service.vmdl",
					"models/citizen_clothes/hat/hat_uniform.police.vmdl",
					"models/citizen_clothes/hat/hat_woollybobble.vmdl",
				};
			defaultAppearance.AddClothingAttachment( new CharacterClothingAttachment( CharacterClothingAttachmentType.Chest, "chest", null, true, ("Chest", 0) ) )
				.Options = new string[] {
					//"models/citizen_clothes/dress/dress.kneelength.vmdl_c",
					"models/citizen_clothes/jacket/SuitJacket/suitjacket.vmdl_c",
					"models/citizen_clothes/jacket/SuitJacket/suitjacketunbuttonedshirt.vmdl_c",
					"models/citizen_clothes/jacket/jacket.red.vmdl",
					"models/citizen_clothes/jacket/jacket.tuxedo.vmdl_c",
					"models/citizen_clothes/jacket/jacket_heavy.vmdl_c",
					"models/citizen_clothes/jacket/labcoat.vmdl_c",
				};
			defaultAppearance.AddClothingAttachment( new CharacterClothingAttachment( CharacterClothingAttachmentType.ChestUnder, "chest", "models/citizen_clothes/shirt/shirt_longsleeve.scientist.vmdl_c", false, ("Chest", 0) ) )
				.Options = new string[] {
					//"models/citizen_clothes/shirt/shirt_longsleeve.office.vmdl_c",
					"models/citizen_clothes/shirt/shirt_longsleeve.plain.vmdl_c",
					"models/citizen_clothes/shirt/shirt_longsleeve.police.vmdl_c",
					"models/citizen_clothes/shirt/shirt_longsleeve.scientist.vmdl_c",
					//"models/citizen_clothes/shirt/shirt_shortsleeve.orange.vmdl_c",
					//"models/citizen_clothes/shirt/shirt_shortsleeve.plain.vmdl_c",
					//"models/citizen_clothes/shirt/shirt_shortsleeve.service.vmdl_c",
					//"models/citizen_clothes/shirt/shirt_turtleneck.vmdl_c",
					//"models/citizen_clothes/vest/vest.highvis.vmdl_c",
					//"models/citizen_clothes/vest/vest_securitykevlar.vmdl_c",
					//"models/citizen_clothes/vest/vest_securitykevlarnobadge.vmdl_c",
				};
			defaultAppearance.AddClothingAttachment( new CharacterClothingAttachment( CharacterClothingAttachmentType.Legs, "legs", "models/citizen_clothes/shoes/shorts.cargo.vmdl", false, ("Legs", 1) ) )
				.Options = new string[] {
					"models/citizen_clothes/shoes/shorts.cargo.vmdl",
					"models/citizen_clothes/trousers/SmartTrousers/smarttrousers.vmdl_c",
					"models/citizen_clothes/trousers/trousers.jeans.vmdl_c",
					"models/citizen_clothes/trousers/trousers.lab.vmdl_c",
					"models/citizen_clothes/trousers/trousers.police.vmdl_c",
					"models/citizen_clothes/trousers/trousers.smart.vmdl_c",
					"models/citizen_clothes/trousers/trousers.smarttan.vmdl_c",
					"models/citizen_clothes/trousers/trousers_tracksuit.vmdl_c",
					"models/citizen_clothes/trousers/trousers_tracksuitblue.vmdl_c",

				};
			defaultAppearance.AddClothingAttachment( new CharacterClothingAttachment( CharacterClothingAttachmentType.Feet, "feet", "models/citizen_clothes/shoes/shoes.workboots.vmdl", false, ("Feet", 1) ) )
				.Options = new string[] {
					"models/citizen_clothes/shoes/SmartShoes/smartshoes.vmdl_c",
					"models/citizen_clothes/shoes/shoes.police.vmdl_c",
					"models/citizen_clothes/shoes/shoes.smartbrown.vmdl_c",
					"models/citizen_clothes/shoes/shoes.workboots.vmdl",
					//"models/citizen_clothes/shoes/shoes_flipflops.vmdl_c",
					//"models/citizen_clothes/shoes/shoes_heels.vmdl_c",
					//"models/citizen_clothes/shoes/shoes_securityboots.vmdl_c",
					"models/citizen_clothes/shoes/trainers.vmdl_c",

				};

			// Male_07 test
			var characterFingerScale = 0.43f;
			var male07_test = this.AddAppearance( new CharacterAppearance( "male07_test", "models/civilgamers/player/bodies/male07_test.vmdl" ) )
				.AddBoneScaleAdjustment( ("finger_ring_meta_R", "finger_ring_2_R", characterFingerScale) )
				.AddBoneScaleAdjustment( ("finger_middle_meta_R", "finger_middle_2_R", characterFingerScale) )
				.AddBoneScaleAdjustment( ("finger_index_meta_R", "finger_index_2_R", characterFingerScale) )
				.AddBoneScaleAdjustment( ("finger_thumb_0_R", "finger_thumb_2_R", characterFingerScale) )
				.AddBoneScaleAdjustment( ("hand_R", "hold_R", characterFingerScale) );
			male07_test.AddClothingAttachment( new CharacterClothingAttachment( CharacterClothingAttachmentType.Legs, "", "models/civilgamers/player/clothes/male/boxers.vmdl", false )
			{
				Options = new string[] {

				}
			} );
		}

		public Dictionary<string, CharacterAppearance> Appearances
			=> _appearances;

		public CharacterAppearance AddAppearance( CharacterAppearance appearance )
		{
			_appearances.Add( appearance.Name, appearance );
			return appearance;
		}

		public bool TryGetAppearance( string name, out CharacterAppearance appearance )
		{
			return _appearances.TryGetValue( name, out appearance );
		}

		public CharacterAppearance GetDefaultAppearance()
		{
			return _appearances[DEFAULT_APPEARANCE];
		}

		[Event.Hotload]
		private void Hotload()
		{
			this.RegisterAppearances();
		}
	}
}
