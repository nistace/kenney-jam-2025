using KJ25.GameControllers;
using KJ25.Saving;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KJ25.Ui.LevelSelection {
   public class LevelSelectionItem : MonoBehaviour {
      [SerializeField] private Image _lockIcon;
      [SerializeField] private Button _button;
      [SerializeField] private TMP_Text _levelName;
      [SerializeField] private string _levelNamePattern = "Level {number}";

      private int LevelIndex { get; set; }

      private void Start() {
         _button.onClick.AddListener(HandleClicked);
      }

      private void HandleClicked() {
         if (GameController.Instance.CurrentState != GameController.State.LevelSelection) {
            return;
         }

         if (LevelIndex > SaveManager.LastUnlockedLevelIndex) {
            return;
         }

         GameController.Instance.SpawnLevel(LevelIndex);
      }

      public void Setup(int levelIndex) {
         LevelIndex = levelIndex;
         Refresh();
      }

      public void Refresh() {
         var locked = LevelIndex > SaveManager.LastUnlockedLevelIndex;
         _levelName.text = locked ? string.Empty : _levelNamePattern.Replace("{number}", $"{LevelIndex + 1}");
         _lockIcon.gameObject.SetActive(locked);
         _button.interactable = !locked;
      }
   }
}