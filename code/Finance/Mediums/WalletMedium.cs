using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Character;

namespace VRP.Finance.Mediums
{
	public class WalletMedium : FinanceMedium
	{
		public WalletMedium( string name ) : base( name )
		{
		}

		public override string ID => "wallet";

		public override bool IsDefaultAvailable => true;

		public override int MaximumBalance => 10000;

		public override int DefaultBalance => 200;

		public override bool Networked => false;

		public override bool CustomDataStorage => true;

		public override bool GetBalance( Client client, out double balance )
		{
			var character = CharacterManager.Instance.GetActiveCharacter( client );
			if (character == null)
			{
				balance = 0;
				return true;
			}

			character.TryGetNumberData( "wallet", out balance );

			return true;
		}

		public override bool SetBalance( Client client, double balance )
		{
			var character = CharacterManager.Instance.GetActiveCharacter( client );
			if ( character == null )
			{
				return true;
			}

			character.SetNumberData( "wallet", balance );

			return true;
		}
	}
}
