using System;
using System.Collections.Generic;
using System.Linq;
using KJ25.GameControllers;
using KJ25.Tracks;
using KJ25.Vehicles;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace KJ25.Levels {
   public class GameLevel : MonoBehaviour {
      [SerializeField] private CinemachineCamera _wholeLevelCameraAnchor;
      [SerializeField] private CinemachineCamera _vehicleCamera;
      [SerializeField] private CinemachineCamera _buildCamera;
      [SerializeField] private Transform _buildCameraTarget;
      [SerializeField] private LaunchButton _launchButton;
      [SerializeField] private TrackChunk _startTrackChunk;
      [SerializeField] private TrackChunk _endTrackChunk;
      [SerializeField] private Vehicle _playerVehicle;
      [SerializeField] private LevelFinish _finish;

      [SerializeField] private SpawnData _spawnData;
      [SerializeField] private SpawnData _despawnData;

      public CinemachineCamera WholeLevelCameraAnchor => _wholeLevelCameraAnchor;
      public CinemachineCamera VehicleCamera => _vehicleCamera;
      public CinemachineCamera BuildCamera => _buildCamera;

      public LaunchButton LaunchButton => _launchButton;
      public Vehicle PlayerVehicle => _playerVehicle;
      public LevelFinish Finish => _finish;
      public TrackChunk StartTrackChunk => _startTrackChunk;

      private List<TrackChunkAmount> PlacedTrackChunkAmounts { get; } = new List<TrackChunkAmount>();
      private List<TrackChunk> PlacedTrackChunks { get; } = new List<TrackChunk>();
      private Dictionary<TrackChunkGhost, TrackChunkGhost> GhostInstances { get; } = new Dictionary<TrackChunkGhost, TrackChunkGhost>();
      private TrackChunkGhost CurrentGhostPrefab { get; set; }

      public UnityEvent<TrackChunkAmount> OnTrackChunkPlaced { get; } = new UnityEvent<TrackChunkAmount>();
      public UnityEvent<TrackChunkAmount> OnTrackChunkRemoved { get; } = new UnityEvent<TrackChunkAmount>();

      private TrackChunk EvaluateLastChunk() => PlacedTrackChunks.Count > 0 ? PlacedTrackChunks.Last() : _startTrackChunk;

      private void Awake() {
         foreach (var spawnableChild in GetChildrenForSpawn()) {
            spawnableChild.localScale = Vector3.zero;
         }

         _playerVehicle.transform.SetParent(transform);
         _playerVehicle.Respawn(_startTrackChunk);
      }

      public List<Transform> GetChildrenForSpawn() => _spawnData.GenerateRandomizedListOfChildren(transform);
      public List<Transform> GetChildrenForDespawn() => _despawnData.GenerateRandomizedListOfChildren(transform);

      public bool AppendTrackChunk(TrackChunkAmount trackChunkAmount) {
         var ghost = SetGhost(trackChunkAmount.Chunk.Ghost);
         if (!ghost.IsValid) {
            return false;
         }

         var lastChunk = EvaluateLastChunk();

         var newTrackChunk = Instantiate(trackChunkAmount.Chunk.ChunkPrefab, lastChunk.NextChunkAnchor.position, lastChunk.NextChunkAnchor.rotation, transform);

         lastChunk.NextChunk = newTrackChunk;

         if (newTrackChunk.NextChunkAnchor.transform.position == _endTrackChunk.transform.position) {
            newTrackChunk.NextChunk = _endTrackChunk;
         }

         _buildCameraTarget.position = newTrackChunk.NextChunkAnchor.position;

         PlacedTrackChunks.Add(newTrackChunk);
         PlacedTrackChunkAmounts.Add(trackChunkAmount);
         UpdateCurrentGhost();

         OnTrackChunkPlaced.Invoke(trackChunkAmount);

         return true;
      }

      public void RemoveLastTrackChunk() {
         if (PlacedTrackChunks.Count == 0) {
            return;
         }

         var lastTrackChunk = PlacedTrackChunks.Last();
         var removedTrackChunkAmount = PlacedTrackChunkAmounts.Last();
         PlacedTrackChunks.RemoveAt(PlacedTrackChunks.Count - 1);
         PlacedTrackChunkAmounts.RemoveAt(PlacedTrackChunkAmounts.Count - 1);

         var newLastChunk = EvaluateLastChunk();
         newLastChunk.NextChunk = null;

         _buildCameraTarget.position = newLastChunk.NextChunkAnchor.position;

         UpdateCurrentGhost();

         Destroy(lastTrackChunk.gameObject);

         OnTrackChunkPlaced.Invoke(removedTrackChunkAmount);
      }

      public TrackChunkGhost SetGhost(TrackChunkGhost chunkGhostPrefab) {
         UnsetGhost();

         if (!GhostInstances.TryGetValue(chunkGhostPrefab, out var chunkGhost)) {
            chunkGhost = Instantiate(chunkGhostPrefab, transform);
            GhostInstances.Add(chunkGhostPrefab, chunkGhost);
         }

         chunkGhost.gameObject.SetActive(true);
         CurrentGhostPrefab = chunkGhostPrefab;

         UpdateCurrentGhost();

         return chunkGhost;
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

      private void UpdateCurrentGhost() {
         if (CurrentGhostPrefab == null) {
            return;
         }

         if (!GhostInstances.TryGetValue(CurrentGhostPrefab, out var chunkGhost)) {
            return;
         }

         var lastChunk = EvaluateLastChunk();

         chunkGhost.transform.position = lastChunk.NextChunkAnchor.position;
         chunkGhost.transform.rotation = lastChunk.NextChunkAnchor.rotation;

         chunkGhost.RefreshValid();
      }

      [Serializable]
      private class SpawnData {
         [SerializeField] private Transform[] _spawnedFirst;
         [SerializeField] private Transform[] _spawnedLast;
         [SerializeField] private Transform[] _notToSpawn;

         public List<Transform> GenerateRandomizedListOfChildren(Transform parent) {
            var result = new List<Transform>();

            result.AddRange(_spawnedFirst);
            result.AddRange(Enumerable.Range(0, parent.childCount).Select(parent.GetChild).Except(_spawnedFirst).Except(_spawnedLast).Except(_notToSpawn));
            result.AddRange(_spawnedLast);

            return result;
         }
      }

      public int CountConsumed(TrackChunkAmount trackChunk) => PlacedTrackChunkAmounts.Count(t => t == trackChunk);
   }
}