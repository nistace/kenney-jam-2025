using UnityEngine;

namespace KJ25.Vehicles {
   [RequireComponent(typeof(AudioSource))]
   public class VehicleAudio : MonoBehaviour {
      [SerializeField] private Vehicle _vehicle;
      [SerializeField] private AudioSource _audioSource;
      [SerializeField] private AnimationCurve _volumeCurve;
      [SerializeField] private AnimationCurve _pitchCurve;

      private void Update() {
         _audioSource.volume = _volumeCurve.Evaluate(_vehicle.CurrentSpeedRatio);
         _audioSource.pitch = _pitchCurve.Evaluate(_vehicle.CurrentSpeedRatio);
      }
   }
}