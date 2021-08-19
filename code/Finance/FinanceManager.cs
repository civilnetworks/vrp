using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Character;
using VRP.Finance.Mediums;

namespace VRP.Finance
{
	public static partial class FinanceManager
	{
		public const string CURRENCY_SYMBOL = "£";
		public const bool CURRENCY_LEFT = true;

		private static Dictionary<string, FinanceMedium> _mediums = new Dictionary<string, FinanceMedium>();

		static FinanceManager()
		{
			CreateMediums();
		}

		public static string FormatMoney( double amt, bool includeSymbol = true, bool includeDecimal = false )
		{
			var symbol = includeSymbol ? CURRENCY_SYMBOL : String.Empty;

			if ( includeDecimal )
			{
				return CURRENCY_LEFT ? symbol + String.Format( "{0:n2}", amt ) : String.Format( "{0:n2}", amt ) + symbol;
			}
			else
			{
				return CURRENCY_LEFT ? symbol + String.Format( "{0:n0}", amt ) : String.Format( "{0:n0}", amt ) + symbol;
			}
		}

		public static string FormatMoney( string value, bool includeSymbol = true, bool includeDecimal = false )
		{
			var symbol = includeSymbol ? CURRENCY_SYMBOL : String.Empty;

			if ( double.TryParse( value, out var dval ) )
			{
				return FormatMoney( dval, includeSymbol, includeDecimal );
			}

			if ( includeDecimal )
			{
				return CURRENCY_LEFT ? symbol + String.Format( "{0:n2}", value ) : String.Format( "{0:n2}", value ) + symbol;
			}
			else
			{
				return CURRENCY_LEFT ? symbol + String.Format( "{0:n0}", value ) : String.Format( "{0:n0}", value ) + symbol;
			}
		}

		public static string GetMoneyDecimal( double amt )
		{
			var formatted = String.Format( "{0:n2}", amt );
			return formatted.Split( '.' )[1];
		}

		public static void CreateMediums()
		{
			_mediums.Clear();

			var wallet = CreateMedium( new WalletMedium( "Wallet" ) );
		}

		public static FinanceMedium GetMedium(Type mediumClassType)
		{
			return _mediums.Select( p => p.Value ).FirstOrDefault( m => m.GetType() == mediumClassType );
		}

		public static bool HasMediumAccess( Client client, string mediumId, bool ignoreDefault = false )
		{
			if ( !_mediums.TryGetValue( mediumId, out var medium ) )
			{
				return false;
			}

			if ( !ignoreDefault && medium.IsDefaultAvailable )
			{
				return true;
			}

			var character = CharacterManager.Instance.GetActiveCharacter( client );
			return character != null && character.GetBoolData( "medium_" + mediumId );
		}

		public static bool Deposit( Client client, Type mediumClassType, double amount, out string error )
		{
			var medium = _mediums.Select( p => p.Value ).FirstOrDefault( m => m.GetType() == mediumClassType );
			if ( medium == null )
			{
				error = $"Unknown medium class '{mediumClassType}'";
				return false;
			}

			return Deposit( client, medium.ID, amount, out error );
		}

		public static bool Deposit( Client client, string mediumId, double amount, out string error )
		{
			amount = Math.Round( amount, 2 );

			if ( !CanDeposit( client, mediumId, amount, out error ) )
			{
				return false;
			}

			if ( !GetBalance( client, mediumId, out var balance, out error ) )
			{
				return false;
			}

			return SetBalance( client, mediumId, Math.Round( balance + amount ), out error );
		}

		public static bool Withdraw( Client client, Type mediumClassType, double amount, out string error )
		{
			var medium = _mediums.Select( p => p.Value ).FirstOrDefault( m => m.GetType() == mediumClassType );
			if ( medium == null )
			{
				error = $"Unknown medium class '{mediumClassType}'";
				return false;
			}

			return Withdraw( client, medium.ID, amount, out error );
		}

		public static bool Withdraw( Client client, string mediumId, double amount, out string error )
		{
			amount = Math.Round( amount, 2 );

			if ( !CanWithdraw( client, mediumId, amount, out error ) )
			{
				return false;
			}

			if ( !GetBalance( client, mediumId, out var balance, out error ) )
			{
				return false;
			}

			return SetBalance( client, mediumId, Math.Round( balance - amount ), out error );
		}

		public static bool CanDeposit( Client client, string mediumId, double amount, out string error )
		{
			if ( !_mediums.TryGetValue( mediumId, out var medium ) )
			{
				error = $"Invalid medium '{mediumId}'";
				return false;
			}

			if ( amount <= 0.01 )
			{
				error = "Amount <= 0.01";
				return false;
			}

			if ( !FinanceManager.GetBalance( client, medium.ID, out var currentBalance, out error ) )
			{
				return false;
			}

			if ( currentBalance + amount > medium.MaximumBalance )
			{
				error = $"Deposit would exceed maximum {medium.Name} balance of {medium.MaximumBalance}";
				return false;
			}

			return medium.CanDeposit( client, amount, out error );
		}

		public static bool CanWithdraw( Client client, string mediumId, double amount, out string error )
		{
			if ( !_mediums.TryGetValue( mediumId, out var medium ) )
			{
				error = $"Invalid medium '{mediumId}'";
				return false;
			}

			if ( amount <= 0.01 )
			{
				error = "Amount <= 0.01";
				return false;
			}

			if ( !FinanceManager.GetBalance( client, medium.ID, out var currentBalance, out error ) )
			{
				return false;
			}

			if ( currentBalance - amount < 0 )
			{
				error = $"Insufficient money in {medium.Name}";
				return false;
			}

			return medium.CanWithdraw( client, amount, out error );
		}

		public static bool GetBalance( Client client, Type mediumClassType, out double balance, out string error )
		{
			balance = 0;

			var medium = _mediums.Select( p => p.Value ).FirstOrDefault( m => m.GetType() == mediumClassType );
			if ( medium == null )
			{
				error = $"Unknown medium class '{mediumClassType}'";
				return false;
			}

			return GetBalance( client, medium.ID, out balance, out error );
		}

		public static bool GetBalance( Client client, string mediumId, out double balance, out string error )
		{
			balance = 0;

			if ( !_mediums.TryGetValue( mediumId, out var medium ) )
			{
				error = $"Invalid medium '{mediumId}'";
				return false;
			}

			if ( !HasMediumAccess( client, mediumId ) )
			{
				error = $"No access to '{medium.Name}'";
				return false;
			}

			error = null;

			if ( medium.GetBalance( client, out balance ) )
			{
				return true;
			}

			return GetBalanceInternal( client, mediumId, out balance, out error );
		}

		#region Internal Balance

		private static bool GetBalanceInternal( Client client, string mediumId, out double balance, out string error )
		{
			balance = 0;
			error = null;

			// TODO: Core medium data
			throw new NotImplementedException();

			return true;
		}

		private static bool SetBalanceInternal( Client client, string mediumId, double balance, out string error )
		{
			error = null;

			// TODO: Core medium data
			throw new NotImplementedException();

			return true;
		}

		#endregion

		private static FinanceMedium CreateMedium( FinanceMedium medium )
		{
			_mediums.Add( medium.ID, medium );
			return medium;
		}

		[Event.Hotload]
		private static void Hotload()
		{
			CreateMediums();
		}
	}
}
