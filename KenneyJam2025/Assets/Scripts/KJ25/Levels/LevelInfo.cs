using KJ25.Tracks;
using UnityEngine;

namespace KJ25.Levels {
   [CreateAssetMenu]
   public class LevelInfo : ScriptableObject {
      [SerializeField] private GameLevel _levelPrefab;
      [SerializeField] private TrackChunkAmount[] _trackChunkAmounts;

      public GameLevel LevelPrefab => _levelPrefab;
      public TrackChunkAmount[] TrackChunkAmounts => _trackChunkAmounts;
   }
}