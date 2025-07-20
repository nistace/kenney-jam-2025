using KJ25.GameControllers;
using UnityEngine;

namespace KJ25.Ui {
   public class MainUi : MonoBehaviour {
      private static readonly int trackChunksAnimParam = Animator.StringToHash("TrackChunks");
      private static readonly int settingsAnimParam = Animator.StringToHash("Settings");
      private static readonly int levelSelectionAnimParam = Animator.StringToHash("LevelSelection");
      private static readonly int levelAnimParam = Animator.StringToHash("Level");

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
         _animator.SetBool(levelAnimParam, newState is GameController.State.Building or GameController.State.Playing or GameController.State.SpawningLevel);
      }
   }
}