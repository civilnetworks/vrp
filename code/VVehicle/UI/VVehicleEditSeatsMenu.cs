using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VRP.VGui.UI;

namespace VVehicle.UI
{
	public class VVehicleEditSeatsMenu : VGuiFrame
	{
		public static VVehicleEditSeatsMenu Instance;

		private List<VVehicleSeatPanel> _seatPanels = new List<VVehicleSeatPanel>();
		private Button _addSeat;
		private Button _save;
		private Panel _content;

		public VVehicleEditSeatsMenu()
		{
			this.EnableClickerToggle = true;

			StyleSheet.Load( "/VVehicle/UI/VVehicleEditSeatsMenu.scss" );

			_content = Add.Panel( "content" );
		}

		public VVehicleData Data { get; set; }

		public VVehicle Vehicle { get; private set; }

		public static VVehicleEditSeatsMenu Open( VVehicle vehicle )
		{
			Close();

			var menu = Local.Hud.AddChild<VVehicleEditSeatsMenu>();
			menu.SetWidth( 300 );
			menu.SetHeight( 700 );
			menu.Center();
			menu.SetPosX( 10 );

			Instance = menu;

			menu.SetVehicle( vehicle );

			return menu;
		}

		public static void Close()
		{
			Instance?.Delete();
		}

		public void SetVehicle( VVehicle vehicle )
		{
			this.Header.SetTitle( $"Edit Seats [{vehicle.VehicleTitle}]" );

			this.Vehicle = vehicle;
			this.Data = VVehicleSystem.GetVehicleData( vehicle.VehicleTitle );

			this.Rebuild();
		}

		public void Rebuild()
		{
			_addSeat?.Delete( true );

			foreach ( var panel in _seatPanels )
			{
				panel.Delete( true );
			}

			_seatPanels.Clear();

			var index = 0;
			foreach ( var seat in this.Data.Seats )
			{
				var panel = _content.AddChild<VVehicleSeatPanel>();
				panel.SetSeat( this, seat, index != 0 );
				_seatPanels.Add( panel );

				index++;
			}

			_addSeat = _content.Add.Button( "Add Seat" );
			_addSeat.AddEventListener( "onclick", ( e ) =>
			{
				var seat = new VVehicleSeatData();
				var list = this.Data.Seats.ToList();
				list.Add( seat );
				this.Data.Seats = list.ToArray();

				this.Rebuild();

				e.StopPropagation();
			} );

			_save = _content.Add.Button( "Save" );
			_save.AddEventListener( "onclick", ( e ) =>
			 {
				 VVehicleSystem.CmdSaveEditSeats( this.Vehicle.VehicleTitle, JsonSerializer.Serialize( this.Data.Seats ) );
			 } );
		}
	}
}
