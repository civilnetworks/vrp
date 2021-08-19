using System;
using System.Collections.Generic;

namespace VRP.Character
{
	public class CharacterAppearanceSettings
	{
		public string AppearanceName { get; set; }

		public Dictionary<CharacterClothingAttachmentType, string> ClothingAttachments { get; set; } = new Dictionary<CharacterClothingAttachmentType, string>();

		public CharacterAppearanceSettings()
		{
			this.AppearanceName = this.AppearanceName ?? CharacterManager.DEFAULT_APPEARANCE;
		}

		public void SetClothingAttachmentModel( CharacterClothingAttachmentType type, string model )
		{
			if (model == null)
			{
				this.ClothingAttachments.Remove( type );
			}
			else
			{
				this.ClothingAttachments[type] = model;
			}
		}

		public void RandomizeClothingAttachments()
		{
			var appearance = this.GetAppearance();
			if ( appearance == null )
			{
				return;
			}

			this.ClothingAttachments.Clear();

			foreach ( var pair in appearance.ClothingAttachments )
			{
				var att = pair.Value;

				var rand = new Random();
				var option = att.Options[rand.Next( att.Options.Length )];
				this.SetClothingAttachmentModel( att.Type, option );
			}
		}

		public CharacterAppearance GetAppearance()
		{
			var name = this.AppearanceName ?? CharacterManager.DEFAULT_APPEARANCE;
			if ( !CharacterManager.Instance.TryGetAppearance( name, out var appearance ) )
			{
				appearance = CharacterManager.Instance.GetDefaultAppearance();
			}

			return appearance;
		}

		public bool TryGetClothingAttachmentModel( CharacterClothingAttachmentType type, out string attachmentModel )
		{
			attachmentModel = null;
			var appearance = this.GetAppearance();
			if (!appearance.ClothingAttachments.TryGetValue(type, out var attachment))
			{
				// Attachment not compatible with appearance.
				return false;
			}

			if ( this.ClothingAttachments.TryGetValue( type, out var equippedModel ) )
			{
				// Set equipped model, return true IF the equipped model exists in the appearance options.
				attachmentModel = equippedModel;
				var containsModel = false;

				foreach ( var option in attachment.Options )
				{
					if ( option.Equals( equippedModel ) )
					{
						containsModel = true;
						break;
					}
				}

				return containsModel;
			}

			// Found default
			attachmentModel = attachment.DefaultModel;
			return attachmentModel != null;
		}
	}
}
