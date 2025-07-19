using UnityEngine;

namespace KJ25.Tracks {
   [CreateAssetMenu]
   public class TrackChunkSpawnInfo : ScriptableObject {
      [SerializeField] private TrackChunk _chunkPrefab;
      [SerializeField] private TrackChunkGhost _ghost;
      [SerializeField] private Sprite _sprite;
      [SerializeField] private bool _flipSprite;

      public TrackChunk ChunkPrefab => _chunkPrefab;
      public TrackChunkGhost Ghost => _ghost;
      public Sprite Sprite => _sprite;
   }
}