using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin
{
	public static partial class VAdminSystem
	{
		public static bool Teleport(Client a, Client b, out string error)
		{
			if (a.Pawn == null)
			{
				error = $"{a.Name} has no Pawn";
				return false;
			}

			if (b.Pawn == null)
			{
				error = $"{b.Name} has no Pawn";
				return false;
			}

			return Teleport(a, b.Pawn.Position, out error);
		}

		public static bool Teleport (Client client, Vector3 pos, out string error)
		{
			if (client.Pawn == null)
			{
				error = $"{client.Name} has no Pawn";
				return false;
			}

			client.Pawn.Position = pos;

			error = null;
			return true;
		}
	}
}
