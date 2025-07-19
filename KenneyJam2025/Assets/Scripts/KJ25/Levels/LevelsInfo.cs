using UnityEngine;

namespace KJ25.Levels {
   [CreateAssetMenu]
   public class LevelsInfo : ScriptableObject {
      [SerializeField] private GameLevel[] _levels;

      public GameLevel[] Levels => _levels;

      public GameLevel this[int index] => Levels[index];
   }
}