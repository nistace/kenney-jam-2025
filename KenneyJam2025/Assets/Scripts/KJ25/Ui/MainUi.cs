using KJ25.GameControllers;
using UnityEngine;

namespace KJ25.Ui {
   public class MainUi : MonoBehaviour {
      private static readonly int trackChunksAnimParam = Animator.StringToHash("TrackChunks");

      [SerializeField] private Animator _animator;

      private void Start() {
         GameController.OnStateChanged.AddListener(HandleGameStateChanged);
      }

      private void OnDestroy() {
         GameController.OnStateChanged.RemoveListener(HandleGameStateChanged);
      }

      private void HandleGameStateChanged(GameController.State newState) {
         _animator.SetBool(trackChunksAnimParam, newState == GameController.State.Building);
      }
   }
}