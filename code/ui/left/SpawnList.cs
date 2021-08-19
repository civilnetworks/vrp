using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Tests;
using VRP.VGui.UI;

[Library]
public partial class SpawnList : Panel
{
	public SpawnList()
	{
		AddClass( "spawnpage" );

		var spawnlist = this.AddChild<VGuiSpawnList>();

		var models = FileSystem.Mounted.FindFile( "models", "*.vmdl_c", true );

		foreach ( var file in models )
		{
			if ( string.IsNullOrWhiteSpace( file ) ) continue;
			//if ( file.Contains( "_lod0" ) ) continue;
			//if ( file.Contains( "clothes" ) ) continue;

			spawnlist.AddModel( "models/" + file.Remove( file.Length - 2 ) );
		}
	}
}
