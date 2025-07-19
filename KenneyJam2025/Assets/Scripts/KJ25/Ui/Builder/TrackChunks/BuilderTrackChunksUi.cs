using System.Collections.Generic;
using KJ25.GameControllers;
using KJ25.Levels;
using UnityEngine;

namespace KJ25.Ui.Builder.TrackChunks {
   public class BuilderTrackChunksUi : MonoBehaviour {
      [SerializeField] private BuilderTrackChunkButton _buttonPrefab;

      private List<BuilderTrackChunkButton> Buttons { get; } = new List<BuilderTrackChunkButton>();

      private void Start() {
         GameController.Instance.OnCurrentLevelChanged.AddListener(HandleCurrentLevelChanged);
      }

      private void HandleCurrentLevelChanged(LevelInfo levelInfo, GameLevel level) {
         for (var index = 0; index < levelInfo.TrackChunkAmounts.Length; index++) {
            var trackChunkAmount = levelInfo.TrackChunkAmounts[index];

            if (index >= Buttons.Count) {
               Buttons.Add(Instantiate(_buttonPrefab, transform));
            }

            var button = Buttons[index];
            button.Setup(level, trackChunkAmount);
            button.gameObject.SetActive(true);
         }

         for (var index = levelInfo.TrackChunkAmounts.Length; index < Buttons.Count; index++) {
            Buttons[index].gameObject.SetActive(false);
         }
      }
   }
}