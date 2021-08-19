using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using VRP.Finance;

namespace VRP.ui
{
	public class VrpHud : Panel
	{
		private Label _moneySymbol;
		private Label _moneyLabel;
		private Label _moneyLabelDecimal;
		private Label _moneyChange;
		private double prevMoney;
		private double moneyChange;
		private float changeTimeEnd = 0;

		public VrpHud()
		{
			StyleSheet.Load( "/ui/VrpHud.scss" );

			var top = Add.Panel( "vrp-hud-top" );

			var wrapper = top.Add.Panel( "vrp-hud-money-wrapper" );
			var money = wrapper.Add.Panel( "vrp-hud-money-container" );
			_moneySymbol = money.Add.Label( "", "vrp-hud-money-small" );
			_moneySymbol.Style.MarginRight = 2;
			_moneySymbol.Style.MarginBottom = 1;

			_moneyLabel = money.Add.Label( "", "vrp-hud-money" );

			_moneyLabelDecimal = money.Add.Label( "", "vrp-hud-money-small" );
			_moneyLabelDecimal.Style.MarginBottom = 2;

			_moneyChange = wrapper.Add.Label( "Test", "vrp-hud-money-small" );
		}

		public override void Tick()
		{
			base.Tick();

			if ( FinanceManager.GetBalance( Local.Client, "wallet", out var balance, out var error ) )
			{
				var formatted = FinanceManager.FormatMoney( balance, false, false );
				var decimals = FinanceManager.GetMoneyDecimal( balance );
				_moneySymbol.SetText( FinanceManager.CURRENCY_SYMBOL );
				_moneyLabel.SetText( formatted );
				_moneyLabelDecimal.SetText( "." + decimals.ToString() );

				if ( prevMoney != balance )
				{
					moneyChange += (balance - prevMoney);

					prevMoney = balance;
					changeTimeEnd = Time.Now + 5f;
				}

				if ( Time.Now < changeTimeEnd )
				{
					_moneyChange.Style.Opacity = 1;
					_moneyChange.SetText( (moneyChange > 0 ? "+" : "-") + FinanceManager.FormatMoney( Math.Abs( moneyChange ), true, true ) );

					if ( moneyChange > 0 )
					{
						_moneyChange.Style.FontColor = Color.Green;
					}
					else
					{
						_moneyChange.Style.FontColor = Color.Red;
					}

					_moneyChange.Style.Dirty();
				}
				else
				{
					_moneyChange.SetText( "" );
					_moneyChange.Style.Opacity = 0;
					moneyChange = 0;
				}
			}
			else
			{
				_moneySymbol.SetText( "" );
				_moneyLabel.SetText( "" );
				_moneyLabelDecimal.SetText( "" );
			}
		}
	}
}
