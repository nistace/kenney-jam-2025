using KJ25.GameControllers;
using Unity.Cinemachine;
using UnityEngine;

namespace KJ25.Cameras {
   public class GameCameraSystem : MonoBehaviour {
      [SerializeField] private CinemachineCamera _titleCamera;

      private void Awake() {
         _titleCamera.enabled = true;
      }

      private void Start() {
         GameController.OnStateChanged.AddListener(HandleGameStateChanged);
      }

      private void HandleGameStateChanged(GameController.State newState) {
         var currentLevel = GameController.Instance.CurrentLevel;
         if (currentLevel) {
            currentLevel.WholeLevelCameraAnchor.enabled = newState is GameController.State.SpawningLevel or GameController.State.DespawningLevel;
            currentLevel.BuildCamera.enabled = newState == GameController.State.Building;
            currentLevel.VehicleCamera.enabled = newState == GameController.State.Playing;
         }

         _titleCamera.enabled = newState == GameController.State.LevelSelection;
      }
   }
}