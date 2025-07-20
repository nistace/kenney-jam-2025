using UnityEngine;
using UnityEngine.Events;

namespace KJ25.Levels {
   public class LevelFinish : MonoBehaviour {
      [SerializeField] private UnityEvent _onEntered = new UnityEvent();

      public UnityEvent OnEntered => _onEntered;

      private void OnTriggerEnter(Collider other) {
         if (!enabled) return;

         if (other.gameObject.CompareTag("Player")) {
            enabled = false;
            OnEntered.Invoke();
         }
      }
   }
}