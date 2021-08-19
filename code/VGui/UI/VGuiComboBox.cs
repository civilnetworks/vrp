using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public delegate void VGuiComboBoxSelectEvent( VGuiComboBoxItem item );

	public class VGuiComboBox : Button
	{
		private static List<VGuiComboBox> COMBO_BOXES = new List<VGuiComboBox>();

		private int _selectedIndex;
		private bool _expanded;
		private Panel _panel;
		private Panel _panelContent;

		private List<VGuiComboBoxItem> _items = new List<VGuiComboBoxItem>();

		public VGuiComboBoxSelectEvent OnSelect;

		public VGuiComboBox()
		{
			StyleSheet.Load( "/VGui/UI/VGuiComboBox.scss" );

			var img = Add.Image( "", "arrow" );
			img.SetTexture( "materials/cg_icons/chevron_down.png" );

			_panel = Local.Hud.Add.Panel( "v-combo-content" );
			_panel.StyleSheet.Load( "/VGui/UI/VGuiComboBox.scss" );
			_panel.Style.Height = 0;
			_panel.Style.Opacity = 0;

			_panelContent = _panel.Add.Panel( "v-combo-content-inner" );

			this.PositionContent();

			COMBO_BOXES.Add( this );
		}

		public int SelectedIndex
			=> _selectedIndex;

		public int NumItems
			=> _items.Count;

		public override void Tick()
		{
			base.Tick();

			this.PositionContent();
		}

		public void Select(int index)
		{
			if (index < 0 || index >= _items.Count)
			{
				return;
			}

			var item = _items[index];
			this.SetText( item.Text );
			this.Collapse();
			_selectedIndex = index;
			this.OnSelect?.Invoke( item );

			for (var i = 0; i < _items.Count; i++)
			{
				_items[i].SetSelected( index == i );
			}
		}

		public VGuiComboBoxItem AddOption( string name, object value = null )
		{
			var item = _panelContent.AddChild<VGuiComboBoxItem>();
			item.SetText( name );
			item.Value = value;
			item.Index = _items.Count;
			item.OnSelect += () =>
			{
				this.Select( item.Index );
			};

			_items.Add( item );

			return item;
		}

		public void Expand()
		{
			if ( _expanded )
			{
				return;
			}

			foreach (var combo in COMBO_BOXES )
			{
				combo.Collapse();
			}

			_expanded = true;

			_panel.Style.Opacity = 1;
			_panel.SetClass( "v-combo-content-expanded", true );
			this.SetClass( "v-combo-expanded", true );
		}

		public void Collapse()
		{
			if ( !_expanded )
			{
				return;
			}

			_expanded = false;

			_panel.Style.Height = 0;
			_panel.Style.Opacity = 0;

			_panel.SetClass( "v-combo-content-expanded", false );
			this.SetClass( "v-combo-expanded", false );
		}

		public void Toggle()
		{
			if ( _expanded )
			{
				this.Collapse();
			}
			else
			{
				this.Expand();
			}
		}

		protected override void OnClick( MousePanelEvent e )
		{
			this.Toggle();
			e.StopPropagation();
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			_panel?.Delete();
			COMBO_BOXES.Remove( this );
		}

		private void PositionContent()
		{
			var rect = this.Box.Rect;
			var screen = Local.Hud.Box.Rect;

			_panel.Style.MarginLeft = rect.left;
			_panel.Style.MarginTop = rect.top + rect.height;
			_panel.Style.Width = rect.width;

			if (_expanded && _items.Count > 0 )
			{
				var panelHeight = _items.Count * _items[0].ItemHeight;

				_panelContent.Style.Height = panelHeight;
				_panel.Style.Height = Math.Min( Math.Max( 0, screen.height - (rect.top + rect.height) ) + 3, panelHeight );
			}
		}
	}
}
