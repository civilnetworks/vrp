using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public class VGuiComboBoxWang : Panel
	{
		private VGuiComboBox _combo;
		private Button _prevButton;
		private Button _nextButton;

		public VGuiComboBoxWang()
		{
			StyleSheet.Load( "/VGui/UI/VGuiComboBoxWang.scss" );

			_combo = this.AddChild<VGuiComboBox>();
			_prevButton = Add.Button( "<", "wang" );
			_prevButton.AddEventListener( "onclick", ( e ) =>
			 {
				 var index = this.Combo.SelectedIndex - 1;
				 if ( index < 0 )
				 {
					 index = this.Combo.NumItems - 1;
				 }

				 this.Combo.Select( index );

				 e.StopPropagation();
			 } );

			_nextButton = Add.Button( ">", "wang" );
			_nextButton.AddEventListener( "onclick", ( e ) =>
			{
				var index = this.Combo.SelectedIndex + 1;
				if ( index >= this.Combo.NumItems )
				{
					index = 0;
				}

				this.Combo.Select( index );

				e.StopPropagation();
			} );
		}

		public VGuiComboBox Combo
			=> _combo;
	}
}
