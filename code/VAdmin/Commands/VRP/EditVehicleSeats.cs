using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP;
using VVehicle;

namespace VAdmin.Commands
{
	public class EditVehicleSeats : VAdminCommand
	{
		public override string Name => "edit_vehicle_seats";

		public override string Description => "Opens the vehicle seat edit menu.";

		public override string Category => "VRP";

		public override EchoType Echo => EchoType.Silent;

		public override CommandArg[] Args => new CommandArg[] { };

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var tr = VrpSystem.GetEyeTrace( caller );
			if ( !tr.Hit || !(tr.Entity is VVehicle.VVehicle vehicle) )
			{
				error = "Must be looking at a valid VVehicle.";
				return false;
			}

			VVehicleSystem.RpcBeginEditSeats( To.Single( caller ), vehicle.NetworkIdent );

			error = null;
			return true;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			return null;
		}
	}
}
