using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Character;

namespace VRP.Player
{
	partial class VrpPlayer : SandboxPlayer
	{
		private List<ModelEntity> _clothingAttachmentEntities = new List<ModelEntity>();
		private CharacterAppearanceSettings _appearanceSettings;

		public CharacterAppearanceSettings AppearanceSettings
		{
			get => _appearanceSettings;
			set
			{
				_appearanceSettings = value;
			}
		}

		public void EquipAppearance( CharacterAppearanceSettings settings = null )
		{
			foreach ( var ent in _clothingAttachmentEntities )
			{
				ent?.Delete();
			}

			_clothingAttachmentEntities.Clear();

			settings = settings ?? this.AppearanceSettings ?? new CharacterAppearanceSettings()
			{
				AppearanceName = CharacterManager.DEFAULT_APPEARANCE,
			};

			var appearance = settings.GetAppearance() ?? CharacterManager.Instance.GetDefaultAppearance();

			this.SetModel( appearance.Model );

			foreach ( var attachmentPair in appearance.ClothingAttachments )
			{
				var attachment = attachmentPair.Value;

				if ( settings.TryGetClothingAttachmentModel( attachment.Type, out var attachmentModel ) && attachmentModel.Length > 0 )
				{
					var ent = new ModelEntity();
					ent.SetModel( attachmentModel );
					ent.SetParent( this, true );
					ent.EnableShadowInFirstPerson = true;
					ent.EnableHideInFirstPerson = true;

					if ( attachment.BodygroupValue.Item2 >= 0 )
					{
						this.SetBodyGroup( attachment.BodygroupValue.Item1, attachment.BodygroupValue.Item2 );
					}

					_clothingAttachmentEntities.Add( ent );
				}
			}
		}

		/*public override void Dress()
		{
			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
				"models/citizen_clothes/trousers/trousers.jeans.vmdl",
				"models/citizen_clothes/trousers/trousers.lab.vmdl",
				"models/citizen_clothes/trousers/trousers.police.vmdl",
				"models/citizen_clothes/trousers/trousers.smart.vmdl",
				"models/citizen_clothes/trousers/trousers.smarttan.vmdl",
				"models/citizen/clothes/trousers_tracksuit.vmdl",
				"models/citizen_clothes/trousers/trousers_tracksuitblue.vmdl",
				"models/citizen_clothes/trousers/trousers_tracksuit.vmdl",
				"models/citizen_clothes/shoes/shorts.cargo.vmdl",
			} );

				_pants = new ModelEntity();
				_pants.SetModel( model );
				_pants.SetParent( this, true );
				_pants.EnableShadowInFirstPerson = true;
				_pants.EnableHideInFirstPerson = true;

				SetBodyGroup( "Legs", 1 );
			}

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
				"models/citizen_clothes/jacket/labcoat.vmdl",
				"models/citizen_clothes/jacket/jacket.red.vmdl",
				"models/citizen_clothes/jacket/jacket.tuxedo.vmdl",
				"models/citizen_clothes/jacket/jacket_heavy.vmdl",
			} );

				_jacket = new ModelEntity();
				_jacket.SetModel( model );
				_jacket.SetParent( this, true );
				_jacket.EnableShadowInFirstPerson = true;
				_jacket.EnableHideInFirstPerson = true;

				var propInfo = _jacket.GetModel().GetPropData();
				if ( propInfo.ParentBodyGroupName != null )
				{
					SetBodyGroup( propInfo.ParentBodyGroupName, propInfo.ParentBodyGroupValue );
				}
				else
				{
					SetBodyGroup( "Chest", 0 );
				}
			}

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
				"models/citizen_clothes/shoes/trainers.vmdl",
				"models/citizen_clothes/shoes/shoes.workboots.vmdl"
			} );

				_shoes = new ModelEntity();
				_shoes.SetModel( model );
				_shoes.SetParent( this, true );
				_shoes.EnableShadowInFirstPerson = true;
				_shoes.EnableHideInFirstPerson = true;

				SetBodyGroup( "Feet", 1 );
			}

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
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
			} );

				_hat = new ModelEntity();
				_hat.SetModel( model );
				_hat.SetParent( this, true );
				_hat.EnableShadowInFirstPerson = true;

			}
		}*/
	}
}
