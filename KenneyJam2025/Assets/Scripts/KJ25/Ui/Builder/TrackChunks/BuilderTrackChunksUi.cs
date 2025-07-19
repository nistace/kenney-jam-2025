using System.Collections.Generic;
using KJ25.GameControllers;
using KJ25.Levels;
using UnityEngine;

namespace KJ25.Ui.Builder.TrackChunks {
   public class BuilderTrackChunksUi : MonoBehaviour {
      [SerializeField] private BuilderTrackChunkButton _buttonPrefab;

      private List<BuilderTrackChunkButton> Buttons { get; } = new List<BuilderTrackChunkButton>();

      private void Start() {
         GameController.OnCurrentLevelChanged.AddListener(HandleCurrentLevelChanged);
      }

      private void HandleCurrentLevelChanged(int levelIndex, GameLevel level) {
         for (var index = 0; index < level.TrackChunkAmounts.Count; index++) {
            var trackChunkAmount = level.TrackChunkAmounts[index];

            if (index >= Buttons.Count) {
               Buttons.Add(Instantiate(_buttonPrefab, transform));
            }

            var button = Buttons[index];
            button.Setup(level, trackChunkAmount);
            button.gameObject.SetActive(true);
         }

         for (var index = level.TrackChunkAmounts.Count; index < Buttons.Count; index++) {
            Buttons[index].gameObject.SetActive(false);
         }
      }
   }
}