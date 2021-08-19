using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Character
{
	public class CharacterAppearance
	{
		public CharacterAppearance( string name, string model )
		{
			this.Name = name;
			this.Model = model;
		}

		public string Name { get; }

		public string Model { get; }

		public Dictionary<CharacterClothingAttachmentType, CharacterClothingAttachment> ClothingAttachments { get; } = new Dictionary<CharacterClothingAttachmentType, CharacterClothingAttachment>();

		public List<(string, string, float)> BoneScaleAdjustments { get; } = new List<(string, string, float)>();

		public CharacterClothingAttachment AddClothingAttachment( CharacterClothingAttachment attachment )
		{
			this.ClothingAttachments.Add( attachment.Type, attachment );
			return attachment;
		}

		public CharacterAppearance AddBoneScaleAdjustment( (string, string, float) adjustment )
		{
			this.BoneScaleAdjustments.Add( adjustment );
			return this;
		}
	}
}
