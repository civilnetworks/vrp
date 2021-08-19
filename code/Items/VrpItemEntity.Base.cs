using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Items
{
	// Copied from the prop entity
	public partial class VrpItemEntity : ModelEntity, IUse
	{
		public override void Spawn()
		{
			base.Spawn();

			MoveType = MoveType.Physics;
			CollisionGroup = CollisionGroup.Interactive;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;
		}

		public override void TakeDamage( DamageInfo info )
		{
			var body = info.Body;
			if ( !body.IsValid() )
				body = PhysicsBody;

			if ( body.IsValid() && !info.Flags.HasFlag( DamageFlags.PhysicsImpact ) )
			{
				body.ApplyImpulseAt( info.Position, info.Force * 100 );
			}

			base.TakeDamage( info );
		}

		protected virtual ModelPropData GetModelPropData()
		{
			var model = GetModel();
			if ( model != null && !model.IsError && model.HasPropData() )
			{
				return model.GetPropData();
			}

			ModelPropData defaultData = new ModelPropData();
			defaultData.Health = -1;
			defaultData.ImpactDamage = 10;
			if ( PhysicsGroup != null )
			{
				defaultData.ImpactDamage = PhysicsGroup.Mass / 10;
			}
			defaultData.MinImpactDamageSpeed = 500;
			return defaultData;
		}

		protected override void OnPhysicsCollision( CollisionEventData eventData )
		{
			var propData = GetModelPropData();

			var minImpactSpeed = propData.MinImpactDamageSpeed;
			if ( minImpactSpeed <= 0.0f ) minImpactSpeed = 500;

			var impactDmg = propData.ImpactDamage;
			if ( impactDmg <= 0.0f ) impactDmg = 10;

			float speed = eventData.Speed;

			if ( speed > minImpactSpeed )
			{
				// I take damage from high speed impacts
				if ( Health > 0 )
				{
					var damage = speed / minImpactSpeed * impactDmg;
					TakeDamage( DamageInfo.Generic( damage ).WithFlag( DamageFlags.PhysicsImpact ) );
				}

				// Whatever I hit takes more damage
				if ( eventData.Entity.IsValid() && eventData.Entity != this )
				{
					var damage = speed / minImpactSpeed * impactDmg * 1.2f;
					eventData.Entity.TakeDamage( DamageInfo.Generic( damage )
						.WithFlag( DamageFlags.PhysicsImpact )
						.WithAttacker( this )
						.WithPosition( eventData.Pos )
						.WithForce( eventData.PreVelocity ) );
				}
			}

			this.Item.OnPhysicsCollision( this, eventData );
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			ITEMS.Remove( this );
		}
	}
}
