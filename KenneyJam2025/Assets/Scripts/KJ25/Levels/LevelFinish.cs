using UnityEngine;
using UnityEngine.Events;

namespace KJ25.Levels {
   public class LevelFinish : MonoBehaviour {
      public UnityEvent OnEntered { get; } = new UnityEvent();

      private void OnTriggerEnter(Collider other) {
         if (other.gameObject.CompareTag("Player")) {
            OnEntered.Invoke();
         }
      }
   }
}