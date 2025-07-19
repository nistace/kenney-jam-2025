using System.Linq;
using UnityEngine;

namespace KJ25.Tracks {
   public class TrackChunkGhost : MonoBehaviour {
      [SerializeField] private Material _ghostValidMaterial;
      [SerializeField] private Material _ghostInvalidMaterial;
      [SerializeField] private Renderer[] _renderers;
      [SerializeField] private BoxCollider[] _colliders;
      [SerializeField] private LayerMask _collisionMask;

      private static Collider[] overlapNonAllocResult { get; } = new Collider[1];

      public bool IsValid { get; private set; }

      public void RefreshValid() {
         IsValid = !HasCollisions();
         var material = IsValid ? _ghostValidMaterial : _ghostInvalidMaterial;
         foreach (var ghostRenderer in _renderers) {
            ghostRenderer.material = material;
         }
      }

      private bool HasCollisions() => _colliders.Any(HasCollisionsFrom);

      private bool HasCollisionsFrom(BoxCollider box) {
         var worldCenter = box.transform.TransformPoint(box.center);
         var halfSize = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);
         return Physics.OverlapBoxNonAlloc(worldCenter, halfSize, overlapNonAllocResult, box.transform.rotation, _collisionMask) > 0;
      }
   }
}