using System.Collections.Generic;
using KJ25.GameControllers;
using UnityEngine;

namespace KJ25.Ui.LevelSelection {
   public class LevelSelector : MonoBehaviour {
      [SerializeField] private Transform _container;
      [SerializeField] private LevelSelectionItem _itemPrefab;

      private List<LevelSelectionItem> Items { get; } = new List<LevelSelectionItem>();

      private void Start() {
         var levels = GameController.Instance.LevelsInfo.Levels;

         for (var index = 0; index < levels.Count; index++) {
            var item = Instantiate(_itemPrefab, _container);
            item.Setup(index);
            Items.Add(item);
         }

         GameController.OnStateChanged.AddListener(HandleGameStateChanged);
      }

      private void HandleGameStateChanged(GameController.State newState) {
         if (newState == GameController.State.LevelSelection) {
            Refresh();
         }
      }

      private void Refresh() {
         foreach (var item in Items) {
            item.Refresh();
         }
      }
   }
}