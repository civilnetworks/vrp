using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.VGui.UI;

namespace VVehicle.UI
{
	public class VVehicleSeatPanel : Panel
	{
		private Label _title;
		private VGuiTextEntry _x;
		private VGuiTextEntry _y;
		private VGuiTextEntry _z;
		private VGuiTextEntry _pitch;
		private VGuiTextEntry _yaw;
		private VGuiTextEntry _roll;
		private AnimEntity _previewEnt;

		public VVehicleSeatPanel()
		{
			StyleSheet.Load( "/VVehicle/UI/VVehicleSeatPanel.scss" );
		}

		public VVehicleEditSeatsMenu Menu { get; private set; }

		public VVehicleSeatData Data { get; private set; }

		public override void Tick()
		{
			base.Tick();

			var index = -1;

			for ( var i = 0; i < this.Menu.Data.Seats.Length; i++ )
			{
				if ( this.Menu.Data.Seats[i] == this.Data )
				{
					index = i;
					break;
				}
			}

			_title.SetText( $"Seat [ {index} ]" );

			if ( float.TryParse( _x.Text, out var x ) )
			{
				this.Data.Location.PosX = x;
			}
			if ( float.TryParse( _y.Text, out var y ) )
			{
				this.Data.Location.PosY = y;
			}
			if ( float.TryParse( _z.Text, out var z ) )
			{
				this.Data.Location.PosZ = z;
			}

			if ( float.TryParse( _pitch.Text, out var pitch ) )
			{
				this.Data.Location.Pitch = pitch;
			}
			if ( float.TryParse( _yaw.Text, out var yaw ) )
			{
				this.Data.Location.Yaw = yaw;
			}
			if ( float.TryParse( _roll.Text, out var roll ) )
			{
				this.Data.Location.Roll = roll;
			}

			var pos = this.Menu.Vehicle.Transform.PointToWorld( this.Data.Location.GetPos() );
			var ang = this.Menu.Vehicle.Transform.RotationToWorld( this.Data.Location.GetAngles().ToRotation() );
			_previewEnt.Position = pos;
			_previewEnt.Rotation = ang;
			_previewEnt.SetAnimBool( "b_sit", true );
		}

		public void SetSeat( VVehicleEditSeatsMenu menu, VVehicleSeatData data, bool enableDelete = true )
		{
			this.Menu = menu;
			this.Data = data;

			var banner = Add.Panel( "banner" );
			_title = banner.Add.Label( "N/A", "label" );

			if ( enableDelete )
			{
				var delete = banner.Add.Button( "X", "delete" );
				delete.AddEventListener( "onclick", ( e ) =>
				 {
					 VGuiConfirmBox.CreateConfirmBox( "Delete seat?", ( confirmed ) =>
						{
							if ( confirmed )
							{
								var list = this.Menu.Data.Seats.ToList();
								list.Remove( this.Data );
								this.Menu.Data.Seats = list.ToArray();

								menu.Rebuild();
							}
						} );

					 e.StopPropagation();
				 } );
			}

			var top = this.Add.Panel();
			var bottom = this.Add.Panel();

			var posLabel = top.Add.Label( "Pos", "label" );

			_x = top.AddChild<VGuiTextEntry>( "entry" );
			_x.Numeric = true;
			_x.SetText( data.Location.PosX.ToString() );

			_y = top.AddChild<VGuiTextEntry>( "entry" );
			_y.Numeric = true;
			_y.SetText( data.Location.PosY.ToString() );

			_z = top.AddChild<VGuiTextEntry>( "entry" );
			_z.Numeric = true;
			_z.SetText( data.Location.PosZ.ToString() );

			var angLabel = bottom.Add.Label( "Ang", "label" );

			_pitch = bottom.AddChild<VGuiTextEntry>( "entry" );
			_pitch.Numeric = true;
			_pitch.SetText( data.Location.Pitch.ToString() );

			_yaw = bottom.AddChild<VGuiTextEntry>( "entry" );
			_yaw.Numeric = true;
			_yaw.SetText( data.Location.Yaw.ToString() );

			_roll = bottom.AddChild<VGuiTextEntry>( "entry" );
			_roll.Numeric = true;
			_roll.SetText( data.Location.Roll.ToString() );

			_previewEnt = new AnimEntity();
			_previewEnt.SetModel( "models/citizen/citizen.vmdl" );
			_previewEnt.Position = this.Menu.Vehicle.Position;
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			_previewEnt?.Delete();
		}
	}
}
