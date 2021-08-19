using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public delegate void VGuiSwitchChangedEvent(VGuiSwitch element, bool value);

	public class VGuiSwitch : Panel
	{
		public VGuiSwitchChangedEvent OnValueChanged;
		public VGuiSwitchChangedEvent OnValueChangedInternal;

		private bool _acceptsInput = true;
		private bool _checked;
		private Panel _widget;
		private Color _onColor = new Color( 0.27f, 0.81f, 0.27f );

		public VGuiSwitch()
		{
			StyleSheet.Load( "/VGui/UI/VGuiSwitch.scss" );

			_widget = Add.Panel( "widget" );

			this.AddEventListener( "onclick", ( e ) =>
			 {
				 this.Toggle();
				 e.StopPropagation();
			 } );

			this.AddEventListener( "onmouseover", ( e ) =>
			{
				if ( this.Tooltip != null && this.Tooltip.Length > 0 )
				{
					VGuiTooltip.CreateTooltip( this, this.Tooltip );
				}

				e.StopPropagation();
			} );

			this.AddEventListener( "onmouseout", ( e ) =>
			{
				VGuiTooltip.RemoveTooltip( this );

				e.StopPropagation();
			} );
		}

		public bool Checked
		{
			get => _checked;
			set
			{
				if (!this.AcceptsInput)
				{
					return;
				}

				_checked = value;
				_widget.SetClass( "checked", value );
				this.SetClass( "checked", value );
				this.UpdateColor();

				this.OnValueChangedInternal?.Invoke( this, value );
			}
		}

		public bool AcceptsInput
		{
			get => _acceptsInput;
			set
			{
				_acceptsInput = value;

				if (value)
				{
					this.Style.PointerEvents = null;
				}
				else
				{
					this.Style.PointerEvents = "none";
				}
			}
		}

		public string Tooltip { get; set; }

		public void Toggle()
		{
			this.Checked = !this.Checked;
			this.OnValueChanged?.Invoke( this, this.Checked );
		}

		public void SetOnColor(Color color)
		{
			_onColor = color;
			this.UpdateColor();
		}

		private void UpdateColor()
		{
			if (this.Checked)
			{
				var bgColor = VGuiHelpers.Saturate( _onColor, 0.15f, 0.5f);

				this.Style.BackgroundColor = bgColor;
				_widget.Style.BackgroundColor = _onColor;
			}
			else
			{
				this.Style.BackgroundColor = null;
				_widget.Style.BackgroundColor = null;
			}
		}
	}
}
