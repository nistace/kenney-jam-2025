using KJ25.GameControllers;
using KJ25.Levels;
using TMPro;
using UnityEngine;

namespace KJ25.Ui.Level {
   public class LevelInfo : MonoBehaviour {
      [SerializeField] private TMP_Text _levelName;
      [SerializeField] private TMP_Text _levelDescription;

      private void Start() {
         GameController.OnCurrentLevelChanged.AddListener(HandleCurrentLevelChanged);
      }

      private void HandleCurrentLevelChanged(int index, GameLevel level) {
         _levelName.text = $"{level.LevelName} ({index + 1})";
         _levelDescription.text = level.LevelInfo;
      }
   }
}