using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.UI.Elements
{
	public class RoleColorPicker : Panel
	{
		private Color[] paletteColors = new Color[]
		{
			Color.White,
			new Color(0.8f, 0.8f, 0.8f),
			new Color(0.6f, 0.6f, 0.6f),
			new Color(1, 0.22f, 0.22f),
			new Color(0.9f, 0.3f, 0.28f),
			new Color(0.7f, 0.22f, 0.22f),
			new Color(0.2f, 0.82f, 0.2f),
			new Color(0.62f, 0.9f, 0.22f),
			new Color(0.15f, 0.35f, 0.8f),
			new Color(0.05f, 0.45f, 0.8f),
			new Color(1f, 0.8f, 0.3f),
			new Color(0.9f, 0.7f, 0.27f),
			new Color(0.8f, 0.5f, 0.27f),
			new Color(0.5f, 0.27f, 0.92f),
			new Color(0.7f, 0.35f, 0.9f),
			new Color(1f, 0.35f, 0.8f),
			new Color(0.9f, 0.3f, 0.7f),
		};

		private Color _color;
		private Panel _preview;
		private Panel _palette;

		public RoleColorPicker()
		{
			StyleSheet.Load( "/VAdmin/UI/Elements/RoleColorPicker.scss" );

			_preview = Add.Panel( "preview" );
			_palette = Add.Panel( "palette" );

			foreach (var color in paletteColors )
			{
				this.AddColor( color );
			}

			this.SetColor( Color.White );
		}

		public Color Color
			=> _color;

		public void SetColor( Color color )
		{
			_color = color;
			_preview.Style.BackgroundColor = color;
			_preview.Style.Dirty();
		}

		public Panel AddColor(Color color)
		{
			var option = _palette.Add.Panel( "option" );
			option.Style.BackgroundColor = color;
			option.AddEventListener( "onclick", ( e ) =>
			{
				this.SetColor( color );
				e.StopPropagation();
			} );

			return option;
		}
	}
}
