using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Permissions
{
	public enum TargetDefault
	{
		/// <summary>
		/// Target all clients.
		/// </summary>
		All,
		/// <summary>
		/// Target clients equal-to or below sender.
		/// </summary>
		Relative,
		/// <summary>
		/// Target clients below sender.
		/// </summary>
		RelativeBelow,
	}
}
