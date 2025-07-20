using UnityEngine;
using UnityEngine.Splines;

namespace KJ25.Tracks {
   public class TrackChunk : MonoBehaviour {
      [SerializeField] private SplineContainer _splineContainer;
      [SerializeField] private float _curveLength;
      [SerializeField] private Transform _nextChunkAnchor;
      [SerializeField] private TrackChunk _nextChunk;

      public float CurveLength => _curveLength;
      public Transform NextChunkAnchor => _nextChunkAnchor;

      public TrackChunk NextChunk {
         get => _nextChunk;
         set => _nextChunk = value;
      }

      public bool TryGetInfoAtDistance(float distance, out Vector3 worldPosition, out Vector3 worldTangent, out Vector3 worldUp) {
         var clampedDistance = Mathf.Clamp(distance, 0, _curveLength);

         if (!_splineContainer.Spline.Evaluate(clampedDistance / _curveLength, out var splinePosition, out var splineTangent, out var splineUp)) {
            worldPosition = default;
            worldTangent = default;
            worldUp = default;
            return false;
         }

         worldPosition = transform.TransformPoint(splinePosition);
         worldTangent = transform.TransformDirection(splineTangent);
         worldUp = transform.TransformDirection(splineUp);

         return Mathf.Approximately(distance, clampedDistance);
      }

      private void OnValidate() {
         var newLength = _splineContainer ? _splineContainer.CalculateLength() : 0;
         if (Mathf.FloorToInt(_curveLength * 1000) != Mathf.FloorToInt(newLength * 1000)) {
            _curveLength = newLength;
         }
      }

      private void OnDrawGizmos() {
         if (_nextChunkAnchor) {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = _nextChunkAnchor.localToWorldMatrix;
            Gizmos.DrawLine(Vector3.zero, Vector3.forward);
            Gizmos.DrawLine((Vector3.forward + Vector3.left) * .5f, Vector3.forward);
            Gizmos.DrawLine((Vector3.forward + Vector3.right) * .5f, Vector3.forward);
            Gizmos.DrawSphere(Vector3.zero, .3f);
         }
      }

      public void SetCollidersEnabled(bool enable) {
         foreach (var colliderToDisable in GetComponentsInChildren<Collider>()) {
            colliderToDisable.enabled = enable;
         }
      }
   }
}