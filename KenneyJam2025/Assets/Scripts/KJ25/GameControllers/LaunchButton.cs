using KJ25.Vehicles;
using UnityEngine;

namespace KJ25.GameControllers {
   public class LaunchButton : MonoBehaviour, I3DInteractable {
      [SerializeField] private Vehicle _vehicleToLaunch;

      private bool Consumed { get; set; }

      public void Interact() {
         if (Consumed) {
            return;
         }

         _vehicleToLaunch.Launch();
         Consumed = true;
      }

      public void HandlePointerEnter() {
         Debug.Log("Enter");
      }

      public void HandlePointerExit() {
         Debug.Log("Exit");
      }
   }
}