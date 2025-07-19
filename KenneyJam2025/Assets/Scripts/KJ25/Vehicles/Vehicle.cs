using KJ25.PowerUps;
using KJ25.Tracks;
using UnityEngine;

namespace KJ25.Vehicles {
   public class Vehicle : MonoBehaviour, IPowerReceiver {
      [SerializeField] private Rigidbody _rigidbody;
      [SerializeField] private VehicleData _vehicleData;
      [SerializeField] private TrackChunk _currentTrackChunk;

      private float DistanceOnCurrentTrackChunk { get; set; }
      public float CurrentSpeed { get; private set; }
      private float Power { get; set; }

      [ContextMenu("Launch")]
      public void Launch() => PowerUp();

      public void PowerUp() {
         CurrentSpeed = _vehicleData.MaxSpeed;
         Power = _vehicleData.MaxPower;
      }

      private void FixedUpdate() {
         if (!TryUpdateToKinematic()) return;

         UpdatePowerAndSpeed();
         UpdatePosition();
      }

      private void UpdatePosition() {
         var distanceTraveled = CurrentSpeed * Time.deltaTime;
         if (_currentTrackChunk) {
            DistanceOnCurrentTrackChunk += distanceTraveled;

            while (_currentTrackChunk && DistanceOnCurrentTrackChunk > _currentTrackChunk.CurveLength) {
               DistanceOnCurrentTrackChunk -= _currentTrackChunk.CurveLength;
               _currentTrackChunk = _currentTrackChunk.NextChunk;
            }

            if (_currentTrackChunk && _currentTrackChunk.TryGetInfoAtDistance(DistanceOnCurrentTrackChunk, out var position, out var tangent, out var up)) {
               transform.position = position;
               if (tangent != Vector3.zero && up != Vector3.zero) {
                  transform.rotation = Quaternion.LookRotation(tangent, up);
               }
            }
            else {
               transform.position += transform.forward * DistanceOnCurrentTrackChunk;
            }
         }
         else {
            transform.position += transform.forward * distanceTraveled;
         }
      }

      private void UpdatePowerAndSpeed() {
         var defaultDeceleration = _vehicleData.Deceleration * Time.deltaTime;
         var angleWithUp = Vector3.Angle(transform.forward, Vector3.up);
         var expectedDeceleration = _vehicleData.TransformDecelerationWithVerticalAngle(defaultDeceleration, angleWithUp);

         if (expectedDeceleration < 0) {
            CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, _vehicleData.MaxSpeed, -expectedDeceleration);
            return;
         }

         var powerCost = _vehicleData.PowerForSpeedCost * expectedDeceleration;

         if (Power >= powerCost) {
            Power -= powerCost;
            return;
         }

         CurrentSpeed -= expectedDeceleration - Power / _vehicleData.PowerForSpeedCost;
         Power = 0;
      }

      private bool TryUpdateToKinematic() {
         var shouldBeKinematic = ShouldBeKinematic();

         if (shouldBeKinematic != _rigidbody.isKinematic) {
            _rigidbody.isKinematic = shouldBeKinematic;
            if (!_rigidbody.isKinematic) {
               _rigidbody.linearVelocity = transform.forward * CurrentSpeed;
            }
         }

         return _rigidbody.isKinematic;
      }

      private bool ShouldBeKinematic() {
         if (!_currentTrackChunk) return false;

         if (Power > 0) return true;
         if (CurrentSpeed > 1) return true;

         return false;
      }
   }
}