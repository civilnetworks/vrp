using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.VGui.UI;
using VVehicle;

[Library]
public class VrpVehiclesList : Panel
{
	public VrpVehiclesList()
	{
		AddClass( "spawnpage" );

		var spawnlist = this.AddChild<VGuiSpawnList>();

		var vehicles = Library.GetAllAttributes<VVehicleConfig>();

		foreach ( var vehicle in vehicles )
		{
			var instance = vehicle.Create<VVehicleConfig>();

			spawnlist.AddModel( instance.Model, () =>
			{
				VVehicleSystem.CmdSpawnVehicle( vehicle.Title );
			} );
		}
	}
}
