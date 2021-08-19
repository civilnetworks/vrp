using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Finance.Mediums
{
	public abstract class FinanceMedium
	{
		public FinanceMedium(string name)
		{
			this.Name = name;
		}

		public abstract string ID { get; }

		public string Name { get; }

		public abstract bool IsDefaultAvailable { get; }

		public abstract int MaximumBalance { get; }

		public abstract int DefaultBalance { get; }

		public abstract bool Networked { get; }

		public abstract bool CustomDataStorage { get; }

		public virtual bool CanDeposit( Client client, double amount, out string error )
		{
			error = null;
			return true;
		}

		public virtual bool CanWithdraw( Client client, double amount, out string error )
		{
			error = null;
			return true;
		}

		public virtual bool GetBalance( Client client, out double balance )
		{
			balance = 0;
			return false;
		}

		public virtual bool SetBalance( Client client, double balance )
		{
			return false;
		}
	}
}
