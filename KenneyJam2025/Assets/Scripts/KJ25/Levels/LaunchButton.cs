using KJ25.Levels;
using KJ25.Vehicles;
using UnityEngine;
using UnityEngine.Events;

namespace KJ25.GameControllers {
   public class LaunchButton : MonoBehaviour, ILevelInteractable {
      [SerializeField] private Vehicle _vehicleToLaunch;

      private bool Consumed { get; set; }
      public UnityEvent OnConsumed { get; } = new UnityEvent();

      public void Interact() {
         if (Consumed) {
            return;
         }

         _vehicleToLaunch.Launch();
         Consumed = true;

         OnConsumed.Invoke();
      }

      public void HandlePointerEnter() { }

      public void HandlePointerExit() { }
   }
}