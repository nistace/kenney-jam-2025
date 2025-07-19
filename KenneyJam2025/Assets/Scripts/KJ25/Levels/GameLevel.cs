using System.Collections.Generic;
using System.Linq;
using KJ25.GameControllers;
using KJ25.Tracks;
using KJ25.Vehicles;
using UnityEngine;

namespace KJ25.Levels {
   public class GameLevel : MonoBehaviour {
      [SerializeField] private Transform _vehicleSpawn;
      [SerializeField] private LaunchButton _launchButton;
      [SerializeField] private TrackChunk _startTrackChunk;
      [SerializeField] private TrackChunk _endTrackChunk;
      [SerializeField] private Vehicle _playerVehicle;
      [SerializeField] private LevelFinish _finish;

      public Transform VehicleSpawn => _vehicleSpawn;
      public LaunchButton LaunchButton => _launchButton;
      public Vehicle PlayerVehicle => _playerVehicle;
      public LevelFinish Finish => _finish;
      public TrackChunk StartTrackChunk => _startTrackChunk;

      private List<TrackChunkAmount> PlacedTrackChunkAmounts { get; } = new List<TrackChunkAmount>();
      private List<TrackChunk> PlacedTrackChunks { get; } = new List<TrackChunk>();
      private Dictionary<TrackChunkGhost, TrackChunkGhost> GhostInstances { get; } = new Dictionary<TrackChunkGhost, TrackChunkGhost>();
      private TrackChunkGhost CurrentGhostPrefab { get; set; }

      private TrackChunk EvaluateLastChunk() => PlacedTrackChunks.Count > 0 ? PlacedTrackChunks.Last() : _startTrackChunk;

      private void Awake() {
         for (var childIndex = 0; childIndex < transform.childCount; childIndex++) {
            transform.GetChild(childIndex).localScale = Vector3.zero;
         }
      }

      public IEnumerable<Transform> GetAllChildrenInRandomOrder() => Enumerable.Range(0, transform.childCount).Select(t => transform.GetChild(t)).OrderBy(_ => Random.value);
      

      public void AppendTrackChunk(TrackChunkAmount trackChunkAmount) {
         var lastChunk = EvaluateLastChunk();

         var newTrackChunk = Instantiate(trackChunkAmount.Chunk.ChunkPrefab, lastChunk.NextChunkAnchor.position, lastChunk.NextChunkAnchor.rotation, transform);

         lastChunk.NextChunk = newTrackChunk;

         if (newTrackChunk.NextChunkAnchor.transform.position == _endTrackChunk.transform.position) {
            newTrackChunk.NextChunk = _endTrackChunk;
         }

         PlacedTrackChunks.Add(newTrackChunk);
         PlacedTrackChunkAmounts.Add(trackChunkAmount);
         UpdateCurrentGhostTransform();
      }

      public void RemoveLastTrackChunk() {
         if (PlacedTrackChunks.Count == 0) {
            return;
         }

         var lastTrackChunk = PlacedTrackChunks.Last();
         PlacedTrackChunks.RemoveAt(PlacedTrackChunks.Count - 1);
         PlacedTrackChunkAmounts.RemoveAt(PlacedTrackChunkAmounts.Count - 1);

         EvaluateLastChunk().NextChunk = null;
         UpdateCurrentGhostTransform();

         Destroy(lastTrackChunk.gameObject);
      }

      public void SetGhost(TrackChunkGhost chunkGhostPrefab) {
         UnsetGhost();

         if (!GhostInstances.TryGetValue(chunkGhostPrefab, out var chunkGhost)) {
            chunkGhost = Instantiate(chunkGhostPrefab, transform);
            GhostInstances.Add(chunkGhostPrefab, chunkGhost);
         }

         chunkGhost.gameObject.SetActive(true);
         CurrentGhostPrefab = chunkGhostPrefab;

         UpdateCurrentGhostTransform();
      }

      public void UnsetGhost(TrackChunkGhost chunkGhostPrefab = null) {
         if (CurrentGhostPrefab == null) {
            return;
         }

         if (chunkGhostPrefab == null || CurrentGhostPrefab == chunkGhostPrefab) {
            GhostInstances[CurrentGhostPrefab].gameObject.SetActive(false);
            CurrentGhostPrefab = null;
         }
      }

      private void UpdateCurrentGhostTransform() {
         if (CurrentGhostPrefab == null) {
            return;
         }

         if (!GhostInstances.TryGetValue(CurrentGhostPrefab, out var chunkGhost)) {
            return;
         }

         var lastChunk = EvaluateLastChunk();

         chunkGhost.transform.position = lastChunk.NextChunkAnchor.position;
         chunkGhost.transform.rotation = lastChunk.NextChunkAnchor.rotation;
      }
   }
}