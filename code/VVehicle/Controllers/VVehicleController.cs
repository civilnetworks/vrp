using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Player;

namespace VVehicle
{
	public class VVehicleController : PawnController
	{
		public override void FrameSimulate()
		{
			base.FrameSimulate();

			this.Simulate();
		}

		public override void Simulate()
		{
			var player = Pawn as VrpPlayer;
			if ( !player.IsValid() ) return;

			var car = player.Vehicle as VVehicle;
			if ( !car.IsValid() ) return;

			car.Simulate( Client );

			if ( player.Vehicle == null )
			{
				Position = player.Position;
				Velocity = car.Velocity;

				return;
			}

			EyeRot = Input.Rotation;
			EyePosLocal = Vector3.Up * (64 - 10) * car.Scale;
			Velocity = car.Velocity;

			SetTag( "noclip" );
			SetTag( "sitting" );
		}

		public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );


		}
	}
}
