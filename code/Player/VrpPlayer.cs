using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Character;

namespace VRP.Player
{
	partial class VrpPlayer : SandboxPlayer
	{
		public override void Respawn()
		{
			this.EquipAppearance();

			base.Respawn();

			var controller = this.Controller as WalkController;

			if ( controller != null )
			{
				controller.WalkSpeed = 100f;
				controller.SprintSpeed = 150f;
				controller.SprintSpeed = 220f;
			}
		}

		public string GetRPName()
		{
			return VrpSystem.GetRPName( this.GetClientOwner() );
		}

		public TraceResult GetEyeTrace()
		{
			return VrpSystem.GetEyeTrace( this );
		}

		public TraceResult GetEyeTrace( float distance )
		{
			return VrpSystem.GetEyeTrace( this, distance );
		}

		public Entity GetUsable()
		{
			return this.FindUsable();
		}
	}
}
