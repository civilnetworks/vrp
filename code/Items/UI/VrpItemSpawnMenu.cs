using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Items;
using VRP.VGui.UI;

namespace vrp.Items.UI
{
	public class VrpItemSpawnMenu : Panel
	{
		public static VGuiFrame Current;

		private VrpItem _item;

		public VrpItemSpawnMenu()
		{
			StyleSheet.Load( "/Items/UI/VrpItemSpawnMenu.scss" );
		}

		public VrpItem Item
			=> _item;

		public Action<string, List<string>> OnSpawned { get; set; }

		public static VrpItemSpawnMenu Open( VrpItem item )
		{
			Current?.Delete();

			Current = VGuiFrame.Create( 300, 400 );
			Current.Header.SetTitle( $"Create item: {item.GetDisplayName()}" );
			var panel = Current.AddChild<VrpItemSpawnMenu>();
			panel.SetItem( item );

			return panel;
		}

		public void SetItem( VrpItem item )
		{
			_item = item;

			var defaults = new Func<object>[item.DataTemplates.Length];
			for ( var i = 0; i < defaults.Length; i++ )
			{
				defaults[i] = null;
			}

			var x = 0;
			foreach ( var itemData in item.DataTemplates )
			{
				this.Add.Label( itemData.Key );

				if ( bool.TryParse( itemData.DefaultValue?.ToString(), out var bval ) )
				{
					var check = this.AddChild<VGuiSwitch>();
					check.Checked = bval;
					defaults[x] = () =>
					{
						return check.Checked;
					};
				}
				else
				{
					var entry = this.AddChild<VGuiTextEntry>();

					if ( double.TryParse( itemData.DefaultValue?.ToString(), out var dval ) )
					{
						entry.Numeric = true;
						defaults[x] = () =>
						{
							if ( entry.Text?.Length == 0 )
							{
								return null;
							}

							return double.Parse( entry.Text );
						};
					}
					else
					{
						defaults[x] = () =>
						{
							return entry.Text;
						};
					}

					entry.SetText( itemData.DefaultValue.ToString() );
				}

				x++;
			}

			var create = Add.Button( "Create", "create" );
			create.AddEventListener( "onclick", ( e ) =>
			 {
				 var data = new object[item.DataTemplates.Length];
				 for ( var i = 0; i < defaults.Length; i++ )
				 {
					 data[i] = defaults[i].Invoke();
				 }

				 var args = data.Select( i => i.ToString() ).ToList();
				 this.OnSpawned?.Invoke( item.Id, args );
				 VrpItems.CmdCreateItem( item.Id, args );
				 this.Parent.Delete( );
				 e.StopPropagation();
			 } );
		}
	}
}
