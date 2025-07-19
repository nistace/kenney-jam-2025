using UnityEngine;

namespace KJ25.Levels {
   [CreateAssetMenu]
   public class LevelsInfo : ScriptableObject {
      [SerializeField] private LevelInfo[] _levels;

      public LevelInfo[] Levels => _levels;
   }
}