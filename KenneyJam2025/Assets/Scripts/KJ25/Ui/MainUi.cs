using KJ25.GameControllers;
using UnityEngine;

namespace KJ25.Ui {
   public class MainUi : MonoBehaviour {
      private static readonly int trackChunksAnimParam = Animator.StringToHash("TrackChunks");
      private static readonly int settingsAnimParam = Animator.StringToHash("Settings");
      private static readonly int levelSelectionAnimParam = Animator.StringToHash("LevelSelection");

      [SerializeField] private Animator _animator;

      private void Start() {
         GameController.OnStateChanged.AddListener(HandleGameStateChanged);
      }

      private void OnDestroy() {
         GameController.OnStateChanged.RemoveListener(HandleGameStateChanged);
      }

      private void HandleGameStateChanged(GameController.State newState) {
         _animator.SetBool(trackChunksAnimParam, newState is GameController.State.Building);
         _animator.SetBool(settingsAnimParam, newState is not GameController.State.GameQuit and not GameController.State.Title);
         _animator.SetBool(levelSelectionAnimParam, newState is GameController.State.LevelSelection);
      }
   }
}