using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Weapons
{
	public partial class VrpBaseWeapon : BaseCarriable
	{
		public virtual float PrimaryRate => 5.0f;

		public virtual float SecondaryRate => 15.0f;

		public virtual float ReloadTime => 3.0f;

		public PickupTrigger PickupTrigger { get; protected set; }

		[Net, Predicted]
		public TimeSince TimeSincePrimaryAttack { get; set; }

		[Net, Predicted]
		public TimeSince TimeSinceSecondaryAttack { get; set; }

		[Net, Predicted]
		public TimeSince TimeSinceReload { get; set; }

		[Net, Predicted]
		public bool IsReloading { get; set; }

		[Net, Predicted]
		public TimeSince TimeSinceDeployed { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
			SetInteractsAs( CollisionLayer.Debris ); // so player movement doesn't walk into it

			PickupTrigger = new PickupTrigger
			{
				Parent = this,
				Position = Position,
				EnableTouch = true,
				EnableSelfCollisions = false
			};

			PickupTrigger.PhysicsBody.EnableAutoSleeping = false;
		}

		public override void ActiveStart( Entity ent )
		{
			base.ActiveStart( ent );

			TimeSinceDeployed = 0;
		}

		public override void Simulate( Client owner )
		{
			if ( this.CanReload() )
			{
				this.Reload();
			}

			//
			// Reload could have deleted us
			//
			if ( !this.IsValid() )
				return;

			if ( this.CanPrimaryAttack() )
			{
				if ( this.AttackPrimary() )
				{
					TimeSincePrimaryAttack = 0;
				}
			}

			//
			// AttackPrimary could have deleted us
			//
			if ( !owner.IsValid() )
				return;

			if ( this.CanSecondaryAttack() )
			{
				if ( this.AttackSecondary() )
				{
					TimeSinceSecondaryAttack = 0;
				}
			}

			if ( TimeSinceDeployed < 0.6f )
				return;

			if ( !IsReloading )
			{
				base.Simulate( owner );
			}

			if ( IsReloading && TimeSinceReload > ReloadTime )
			{
				this.OnReloadFinish();
			}
		}

		public virtual bool CanReload()
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.Reload ) ) return false;

			return true;
		}

		public virtual void Reload()
		{
			if ( IsReloading )
				return;

			TimeSinceReload = 0;
			IsReloading = true;

			(Owner as AnimEntity)?.SetAnimBool( "b_reload", true );

			StartReloadEffects();
		}

		public virtual bool CanPrimaryAttack()
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.Attack1 ) ) return false;

			var rate = PrimaryRate;
			if ( rate <= 0 ) return true;

			return TimeSincePrimaryAttack > (1 / rate);
		}

		public virtual bool AttackPrimary()
		{
			return false;
		}

		public virtual bool CanSecondaryAttack()
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.Attack2 ) ) return false;

			var rate = SecondaryRate;
			if ( rate <= 0 ) return true;

			return TimeSinceSecondaryAttack > (1 / rate);
		}

		public virtual bool AttackSecondary()
		{
			return false;
		}

		public virtual void OnReloadFinish()
		{
			IsReloading = false;
		}

		[ClientRpc]
		public virtual void StartReloadEffects()
		{
			ViewModelEntity?.SetAnimBool( "reload", true );

			// TODO - player third person model reload
		}

		public override void CreateViewModel()
		{
			Host.AssertClient();

			if ( string.IsNullOrEmpty( ViewModelPath ) )
				return;

			ViewModelEntity = new ViewModel
			{
				Position = Position,
				Owner = Owner,
				EnableViewmodelRendering = true
			};

			ViewModelEntity.SetModel( ViewModelPath );
		}

		public bool OnUse( Entity user )
		{
			if ( Owner != null )
				return false;

			if ( !user.IsValid() )
				return false;

			user.StartTouch( this );

			return false;
		}

		public virtual bool IsUsable( Entity user )
		{
			if ( Owner != null ) return false;

			if ( user.Inventory is Inventory inventory )
			{
				return inventory.CanAdd( this );
			}

			return true;
		}

		public void Remove()
		{
			PhysicsGroup?.Wake();
			Delete();
		}

		[ClientRpc]
		protected virtual void ShootEffects()
		{
			Host.AssertClient();

			Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

			if ( IsLocalPawn )
			{
				_ = new Sandbox.ScreenShake.Perlin();
			}

			ViewModelEntity?.SetAnimBool( "fire", true );
			CrosshairPanel?.CreateEvent( "fire" );
		}

		/// <summary>
		/// Shoot a single bullet
		/// </summary>
		public virtual void ShootBullet( Vector3 pos, Vector3 dir, float spread, float force, float damage, float bulletSize )
		{
			var forward = dir;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			//
			// ShootBullet is coded in a way where we can have bullets pass through shit
			// or bounce off shit, in which case it'll return multiple results
			//
			foreach ( var tr in TraceBullet( pos, pos + forward * 5000, bulletSize ) )
			{
				tr.Surface.DoBulletImpact( tr );

				if ( !IsServer ) continue;
				if ( !tr.Entity.IsValid() ) continue;

				//
				// We turn predictiuon off for this, so any exploding effects don't get culled etc
				//
				using ( Prediction.Off() )
				{
					var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100 * force, damage )
						.UsingTraceResult( tr )
						.WithAttacker( Owner )
						.WithWeapon( this );

					tr.Entity.TakeDamage( damageInfo );
				}
			}
		}

		/// <summary>
		/// Shoot a single bullet from owners view point
		/// </summary>
		public virtual void ShootBullet( float spread, float force, float damage, float bulletSize )
		{
			ShootBullet( Owner.EyePos, Owner.EyeRot.Forward, spread, force, damage, bulletSize );
		}

		/// <summary>
		/// Shoot a multiple bullets from owners view point
		/// </summary>
		public virtual void ShootBullets( int numBullets, float spread, float force, float damage, float bulletSize )
		{
			var pos = Owner.EyePos;
			var dir = Owner.EyeRot.Forward;

			for ( var i = 0; i < numBullets; i++ )
			{
				ShootBullet( pos, dir, spread, force / numBullets, damage, bulletSize );
			}
		}

		/// <summary>
		/// Does a trace from start to end, does bullet impact effects. Coded as an IEnumerable so you can return multiple
		/// hits, like if you're going through layers or ricocet'ing or something.
		/// </summary>
		public virtual IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f )
		{
			var InWater = Physics.TestPointContents( start, CollisionLayer.Water );

			var tr = Trace.Ray( start, end )
					.UseHitboxes()
					.HitLayer( CollisionLayer.Water, !InWater )
					.Ignore( Owner )
					.Ignore( this )
					.Size( radius )
					.Run();

			yield return tr;

			//
			// Another trace, bullet going through thin material, penetrating water surface?
			//
		}
	}
}
