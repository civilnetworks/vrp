using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Player;

namespace VRP.Character
{
	public partial class CharacterManager : VrpManager
	{
		private static Dictionary<string, Dictionary<int, Vector3>> _modelBoneOffsets = new Dictionary<string, Dictionary<int, Vector3>>();

		[Event.Tick]
		public static void BoneTick()
		{
			if ( Host.IsServer )
			{
				return;
			}

			foreach ( var cl in Client.All )
			{
				if ( !(cl.Pawn is VrpPlayer player) )
				{
					return;
				}

				var character = Instance.GetActiveCharacter( cl );

				if ( character != null )
				{
					var appearance = character.GetAppearance();

					player.ResetBones();
				}
			}
		}

		[Event.Frame]
		public static void BoneFrame()
		{
			if ( Host.IsServer )
			{
				return;
			}

			foreach ( var cl in Client.All )
			{
				if ( !(cl.Pawn is VrpPlayer player) )
				{
					return;
				}

				var character = Instance.GetActiveCharacter( cl );

				if ( character != null )
				{
					var appearance = character.GetAppearance();

					foreach ( var offset in appearance.BoneScaleAdjustments )
					{
						Instance.ScaleBonePositions( player, offset.Item1, offset.Item2, offset.Item3 );
					}
				}
			}
		}

		public void ScaleBonePositions( AnimEntity ent, string startBoneName, string endBoneName, float scale )
		{
			this.ScaleBonePositionsInternal( ent, startBoneName, endBoneName, scale, 0 );
		}

		private void ScaleBonePositionsInternal( AnimEntity ent, string startBoneName, string endBoneName, float scale, int iteration )
		{
			iteration++;

			if ( iteration > 100 )
			{
				return;
			}

			if ( endBoneName == startBoneName )
			{
				return;
			}

			if ( !_modelBoneOffsets.TryGetValue( ent.GetModel().Name, out var boneOffsets ) )
			{
				boneOffsets = new Dictionary<int, Vector3>();
				_modelBoneOffsets.Add( ent.GetModel().Name, boneOffsets );
			}

			var boneIndex = ent.GetBoneIndex( endBoneName );
			var parent = ent.GetBoneParent( boneIndex );
			var parentBoneName = ent.GetBoneName( parent );
			var parentTx = ent.GetBoneTransform( parentBoneName );
			var tx = ent.GetBoneTransform( endBoneName );

			if ( !boneOffsets.TryGetValue( boneIndex, out var localPos ) )
			{
				localPos = parentTx.PointToLocal( tx.Position ) * scale;

				//boneOffsets.Add( boneIndex, localPos );
			}

			var newPos = parentTx.PointToWorld( localPos );
			tx.Position = newPos;
			ent.SetBone( boneIndex, tx );
			//ent.ResetBones();

			if ( parentBoneName == null || parentBoneName == startBoneName )
			{
				return;
			}

			ScaleBonePositionsInternal( ent, startBoneName, parentBoneName, scale, iteration++ );
		}
	}
}
