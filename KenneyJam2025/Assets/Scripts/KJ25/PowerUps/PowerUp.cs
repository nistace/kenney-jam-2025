using UnityEngine;

namespace KJ25.PowerUps {
   public class PowerUp : MonoBehaviour {
      private void OnTriggerEnter(Collider other) {
         if (!enabled) {
            return;
         }

         var otherAsPowerReceiver = other.GetComponent<IPowerReceiver>();
         if (otherAsPowerReceiver == null) {
            return;
         }

         otherAsPowerReceiver.PowerUp();
         enabled = false;
      }
   }
}