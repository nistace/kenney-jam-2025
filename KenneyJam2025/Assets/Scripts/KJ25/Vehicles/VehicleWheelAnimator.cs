using UnityEngine;

namespace KJ25.Vehicles {
   [RequireComponent(typeof(Vehicle))]
   public class VehicleWheelAnimator : MonoBehaviour {
      [SerializeField] private Vehicle _vehicle;
      [SerializeField] private Transform[] _wheels;
      [SerializeField] private Vector3 _wheelRotationPerSpeed = new Vector3(5, 0, 0);

      private void Reset() {
         _vehicle = GetComponent<Vehicle>();
      }

      private void LateUpdate() {
         var rotation = _wheelRotationPerSpeed * (_vehicle.CurrentSpeed * Time.deltaTime);

         if (_wheelRotationPerSpeed == Vector3.zero) {
            return;
         }

         foreach (var wheel in _wheels) {
            wheel.Rotate(rotation.x, rotation.y, rotation.z);
         }
      }
   }
}