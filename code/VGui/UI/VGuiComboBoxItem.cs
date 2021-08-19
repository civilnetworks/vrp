using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public delegate void VGuiComboBoxItemSelectEvent();

	public class VGuiComboBoxItem : Label
	{
		public VGuiComboBoxItemSelectEvent OnSelect;

		public VGuiComboBoxItem()
		{
			StyleSheet.Load( "/VGui/UI/VGuiComboBoxItem.scss" );

			this.Style.Height = this.ItemHeight;
		}

		public int ItemHeight { get; } = 30;

		public object Value { get; set; }

		public int Index { get; set; }

		public void SetSelected( bool selected )
		{
			this.SetClass( "v-combo-item-selected", selected );
		}

		protected override void OnClick( MousePanelEvent e )
		{
			OnSelect?.Invoke();
			e.StopPropagation();
		}
	}
}
