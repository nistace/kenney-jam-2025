using UnityEngine;

namespace KJ25.Vehicles {
   [CreateAssetMenu]
   public class VehicleData : ScriptableObject {
      [SerializeField] private float _maxSpeed = 5;
      [SerializeField] private float _deceleration = 1;
      [SerializeField] private float _maxPower = 3;
      [SerializeField] private float _powerForSpeedCost = 1;
      [SerializeField] private AnimationCurve _decelerationCoefficientPerForwardAngleWithUp = AnimationCurve.Linear(0, 20, 100, 0);

      public float MaxSpeed => _maxSpeed;
      public float Deceleration => _deceleration;
      public float MaxPower => _maxPower;
      public float PowerForSpeedCost => _powerForSpeedCost;

      public float TransformDecelerationWithVerticalAngle(float deceleration, float forwardAngleWithUp) => deceleration * _decelerationCoefficientPerForwardAngleWithUp.Evaluate(forwardAngleWithUp);
   }
}