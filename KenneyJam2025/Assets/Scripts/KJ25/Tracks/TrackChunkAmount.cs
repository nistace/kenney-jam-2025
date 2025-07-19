using System;
using UnityEngine;

namespace KJ25.Tracks {
   [Serializable]
   public class TrackChunkAmount {
      [SerializeField] private TrackChunkSpawnInfo _chunk;
      [SerializeField] private bool _infinite;
      [SerializeField] private int _amount;

      public TrackChunkSpawnInfo Chunk => _chunk;
      public bool Infinite => _infinite;
      public int Amount => _amount;
   }
}