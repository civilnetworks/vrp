using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Character
{
	public class CharacterClothingAttachment
	{
		private string[] _options = new string[] { };

		public CharacterClothingAttachment( CharacterClothingAttachmentType type, string attachment, string defaultModel, bool allowNone, (string, int) bodygroupValue = default )
		{
			this.Type = type;
			this.Attachment = attachment;
			this.DefaultModel = defaultModel?.Replace( ".vmdl_c", ".vmdl" );
			this.AllowNone = allowNone;
			this.BodygroupValue = bodygroupValue;
		}

		public CharacterClothingAttachmentType Type { get; }

		public string Attachment { get; }

		public string DefaultModel { get; }

		public bool AllowNone { get; }

		public (string, int) BodygroupValue { get; }

		public string[] Options
		{
			get => _options;
			set
			{
				for ( var i = 0; i < value.Length; i++ )
				{
					value[i] = value[i].Replace( ".vmdl_c", ".vmdl" );
				}

				var hasDefault = this.DefaultModel == null || value.FirstOrDefault( s => s.Equals( this.DefaultModel ) ) != null;
				if ( !hasDefault )
				{
					var temp = value.ToList();
					temp.Add( this.DefaultModel );
					value = temp.ToArray();
				}

				_options = value;
			}
		}
	}
}
