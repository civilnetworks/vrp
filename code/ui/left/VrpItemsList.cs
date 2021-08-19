using Sandbox;
using Sandbox.UI;
using System.Linq;
using vrp.Items.UI;
using VRP.Items;
using VRP.VGui.UI;

[Library]
public partial class VrpItemsList : Panel
{
	public VrpItemsList()
	{
		AddClass( "spawnpage" );

		var spawnlist = this.AddChild<VGuiSpawnList>();
		var ents = Library.GetAllAttributes<VrpItem>().OrderBy( x => x.Title ).ToArray();

		foreach ( var entry in ents )
		{
			var typeName = typeof( VrpItem ).ToString();
			if ( entry.FullName.Equals( typeName ) )
			{
				// Prevent creating instance of abstract base class.
				continue;
			}

			var itemInstance = entry.Create<VrpItem>();

			spawnlist.AddButton( itemInstance.Name, () =>
			{
				if ( itemInstance.DataTemplates != null && itemInstance.DataTemplates.Length > 0 )
				{
					VrpItemSpawnMenu.Open( itemInstance );
				}
				else
				{
					VrpItems.CmdCreateItem( itemInstance.Id );
				}
			} );
		}
	}
}

