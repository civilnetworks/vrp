using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VRP.Items;
using VRP.Player;

namespace VRP.Weapons
{
	[Library( "weapon_hands", Title = "Hands", Spawnable = true )]
	partial class Hands : VrpBaseWeapon
	{
		private const string WIDGET_MODEL_RIGHT = "models/civilgamers/hands/right_hand.vmdl";
		private const string WIDGET_MODEL_LEFT = "models/civilgamers/hands/left_hand.vmdl";
		private const string WIDGET_MODEL_GRABBED = "models/civilgamers/pickup_widget.vmdl";
		private const float WIDGET_SCALE = 0.7f;
		private const float WIDGET_NORMAL_OFFSET = 4f;
		private const float WIDGET_NORMAL_OFFSET_GRABBED = 1f;
		private static readonly Vector3 WIDGET_ROT_OFFSET = new Vector3( -90, 180, 0 );

		private ModelEntity[] _widgets;
		private VrpItemPickupPoint[] _pickupPoints = new VrpItemPickupPoint[0];
		private Material _widgetMaterial;
		private Vector3 _curLookNormal = Vector3.Zero;
		private bool _grabbed;

		public override float PrimaryRate => 0.1f;

		public override float SecondaryRate => 0.1f;

		public override string ViewModelPath => "";

		public override void Spawn()
		{
			base.Spawn();

			SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );

			if ( IsClient )
			{
				_widgetMaterial = Material.Load( "materials/civilgamers/widget_glow.vmat" );
			}
		}

		public override void SimulateAnimator( PawnAnimator anim )
		{
			anim.SetParam( "holdtype", 0 );
			anim.SetParam( "aimat_weight", 0f );
		}

		public override void Simulate( Client owner )
		{
			base.Simulate( owner );

			if ( IsClient )
			{
				if ( !(Local.Pawn is VrpPlayer player) )
				{
					return;
				}

				_curLookNormal += (player.EyeRot.Forward - _curLookNormal) * Time.Delta * 6f;
				_curLookNormal = _curLookNormal.Normal;

				var itemEnt = VrpItems.FindPickupItem( owner, _curLookNormal );
				var pickupPoints = VrpItems.FindPickupPoints( owner, itemEnt, _curLookNormal );
				var wasGrabbed = _grabbed;

				if ( VrpItems.TryGetClientPickup( Local.Client, out var clientPickup ) )
				{
					itemEnt = clientPickup.Entity;
					pickupPoints = clientPickup.Points;
					_grabbed = true;
				}
				else
				{
					_grabbed = false;
				}

				if ( _grabbed != wasGrabbed )
				{
					this.CreatePickupWidgets();
				}

				var numPickups = pickupPoints?.Length;

				if ( pickupPoints != null )
				{
					_pickupPoints = pickupPoints;
				}
				else if ( _pickupPoints.Length > 0 )
				{
					_pickupPoints = new VrpItemPickupPoint[0];
				}

				if ( (_widgets?.Length ?? 0) == 0 && (numPickups ?? 0) > 0 )
				{
					this.CreatePickupWidgets();
				}
				else if ( (_widgets?.Length ?? 0) > 0 && (numPickups ?? 0) == 0 )
				{
					this.DestroyPickupWidgets();
				}

				this.UpdatePickupWidgets();
			}
		}

		[Event.Tick]
		public void OnFrame()
		{
			if ( Host.IsServer )
			{
				return;
			}

			if ( IsClient && Input.Pressed( InputButton.Attack1 ) )
			{
				if ( !(Local.Pawn is VrpPlayer player) )
				{
					return;
				}

				if ( (_pickupPoints?.Length ?? 0) == 0 )
				{
					return;
				}

				var ent = _pickupPoints[0].Entity;

				foreach ( var pickup in _pickupPoints )
				{
					var pos = pickup.Entity.Transform.PointToWorld( pickup.Location.GetPos() );
					var holdPos = new Transform( player.EyePos, player.EyeRot ).PointToLocal( pos );
					pickup.HoldPosition = holdPos;
				}

				var json = JsonSerializer.Serialize( _pickupPoints );
				VrpItems.CmdStartPickup( ent.NetworkIdent, json );

				return;
			}

			if ( IsClient && Input.Pressed( InputButton.Attack2 ) )
			{
				VrpItems.CmdEndPickup();
			}
		}

		public override void CreateViewModel()
		{
			base.CreateViewModel();
		}

		public override bool CanReload()
		{
			return false;
		}

		public override bool CanPrimaryAttack()
		{
			return base.CanPrimaryAttack();
		}

		public override bool AttackPrimary()
		{
			return false;
		}

		public override bool CanSecondaryAttack()
		{
			return base.CanSecondaryAttack();
		}

		public override bool AttackSecondary()
		{
			return false;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			this.DestroyPickupWidgets();
		}

		public override void OnCarryDrop( Entity dropper )
		{
			base.OnCarryDrop( dropper );

			this.DestroyPickupWidgets();
		}

		public override void ActiveEnd( Entity ent, bool dropped )
		{
			base.ActiveEnd( ent, dropped );

			this.DestroyPickupWidgets();
		}

		private void UpdatePickupWidgets()
		{
			if ( _pickupPoints.Length <= 0 )
			{
				return;
			}

			if ( !(Local.Pawn is VrpPlayer player) )
			{
				return;
			}

			var left = _pickupPoints.FirstOrDefault( p => !p.RightHand );
			var right = _pickupPoints.FirstOrDefault( p => p.RightHand );

			for ( var i = 0; i < _widgets.Length; i++ )
			{
				var pickup = i == 0 ? left : right;
				var widget = _widgets[i];

				if ( pickup != null )
				{
					var pos = pickup.Entity.Transform.PointToWorld( pickup.Location.GetPos() );
					var hitRot = pickup.Entity.Transform.RotationToWorld( pickup.Location.GetAngles().ToRotation() );
					pos += hitRot.Forward * (_grabbed ? WIDGET_NORMAL_OFFSET_GRABBED : WIDGET_NORMAL_OFFSET);

					var rot = _grabbed ? hitRot : Rotation.LookAt( hitRot.Forward, player.EyeRot.Up );
					rot = rot.RotateAroundAxis( Vector3.Right, WIDGET_ROT_OFFSET.x );
					rot = rot.RotateAroundAxis( Vector3.Up, WIDGET_ROT_OFFSET.y );
					rot = rot.RotateAroundAxis( Vector3.Forward, WIDGET_ROT_OFFSET.z );

					widget.Position = pos;
					widget.Rotation = rot;

					widget.Scale += (WIDGET_SCALE - widget.Scale) * MathF.Min( 1f, Time.Delta * 5f );
				}
				else
				{
					widget.Scale -= widget.Scale * MathF.Min( 1f, Time.Delta * 16f );
				}
			}
		}

		private void CreatePickupWidgets()
		{
			this.DestroyPickupWidgets( false );

			if ( IsClient )
			{
				if ( !(Local.Pawn is VrpPlayer player) )
				{
					return;
				}

				_widgets = new ModelEntity[2];

				for ( var i = 0; i < _widgets.Length; i++ )
				{
					var ent = new ModelEntity();
					ent.SetModel( _grabbed ? WIDGET_MODEL_GRABBED : (i == 0 ? WIDGET_MODEL_LEFT : WIDGET_MODEL_RIGHT) );
					ent.Scale = 0;
					ent.SceneObject.SetMaterialOverride( _widgetMaterial );

					_widgets[i] = ent;
				}

				_curLookNormal = player.EyeRot.Forward;
			}
		}

		private void DestroyPickupWidgets( bool clear = true )
		{
			if ( _widgets != null )
			{
				foreach ( var widget in _widgets )
				{
					widget?.Delete();
				}
			}

			if ( clear && _pickupPoints.Length > 0 )
			{

			}

			_widgets = null;
		}
	}
}
