using System;
using Cysharp.Threading.Tasks;
using KJ25.Levels;
using KJ25.Tracks;
using UnityEngine;
using UnityEngine.Events;

namespace KJ25.GameControllers {
   public class GameController : MonoBehaviour {
      public static GameController Instance { get; private set; }

      public enum State {
         LevelSelection = 0,
         SpawningLevel = 1,
         Building = 2,
         Playing = 3,
         DespawningLevel = 4,
      }

      public enum Action {
         Launch = 0,
         Reset = 1,
         Restart = 2,
         Undo = 3
      }

      [SerializeField] private LevelsInfo _levelsInfo;
      [SerializeField] private GameLevelSpawner _levelSpawner;

      private State CurrentState { get; set; }
      private int CurrentLevelIndex { get; set; }
      public GameLevel CurrentLevel { get; private set; }
      public static UnityEvent<LevelInfo, GameLevel> OnCurrentLevelChanged { get; } = new UnityEvent<LevelInfo, GameLevel>();
      public static UnityEvent<GameLevel> OnCurrentLevelTrackChanged { get; } = new UnityEvent<GameLevel>();
      public static UnityEvent<State> OnStateChanged { get; } = new UnityEvent<State>();

      private void Awake() {
         Instance = this;
      }

      private void Start() {
         CurrentLevelIndex = 0;
         SpawnLevel(_levelsInfo.Levels[CurrentLevelIndex]);
      }

      private void CleanUpCurrentLevel() {
         if (CurrentLevel) {
            CurrentLevel.OnTrackChanged.RemoveListener(HandleTrackChanged);
            CurrentLevel.Finish.OnEntered.RemoveListener(HandleCurrentLevelFinishEntered);
            Destroy(CurrentLevel.gameObject);
         }
      }

      private void SpawnLevel(LevelInfo levelInfo) {
         CleanUpCurrentLevel();

         CurrentLevel = Instantiate(levelInfo.LevelPrefab);
         CurrentLevel.Finish.OnEntered.AddListener(HandleCurrentLevelFinishEntered);
         CurrentLevel.OnTrackChanged.AddListener(HandleTrackChanged);

         _levelSpawner.Spawn(CurrentLevel, StartBuilderState).Forget();
         ChangeState(State.SpawningLevel);
         OnCurrentLevelChanged.Invoke(levelInfo, CurrentLevel);
      }

      private void HandleTrackChanged() => OnCurrentLevelTrackChanged.Invoke(CurrentLevel);

      private void StartBuilderState() {
         CurrentLevel.RespawnPlayerVehicle();
         ChangeState(State.Building);
      }

      private void ChangeState(State newState) {
         CurrentState = newState;

         OnStateChanged.Invoke(newState);
      }

      private void HandleCurrentLevelFinishEntered() {
         DespawnCurrentLevel(ContinueToNextLevel);
      }

      private void DespawnCurrentLevel(UnityAction then) {
         _levelSpawner.Despawn(CurrentLevel, then).Forget();

         ChangeState(State.DespawningLevel);
      }

      private void ContinueToNextLevel() {
         CleanUpCurrentLevel();

         CurrentLevelIndex++;
         CurrentLevelIndex %= _levelsInfo.Levels.Length;
         SpawnLevel(_levelsInfo.Levels[CurrentLevelIndex]);
      }

      private void SpawnCurrentLevel() => SpawnLevel(_levelsInfo.Levels[CurrentLevelIndex]);

      public void AppendTrackChunk(TrackChunkAmount trackChunk) {
         CurrentLevel.AppendTrackChunk(trackChunk);
      }

      public void ShowTrackChunkGhost(TrackChunkAmount trackChunk) => CurrentLevel.SetGhost(trackChunk.Chunk.Ghost);

      public void HideTrackChunkGhost(TrackChunkAmount trackChunk) => CurrentLevel.UnsetGhost(trackChunk.Chunk.Ghost);

      private bool Launch() {
         if (!CanPerform(Action.Launch)) return false;

         CurrentLevel.PlayerVehicle.Launch();
         ChangeState(State.Playing);

         return true;
      }

      private bool ResetToBuildingState() {
         if (!CanPerform(Action.Reset)) return false;

         StartBuilderState();

         return true;
      }

      private bool RestartCurrentLevel() {
         if (!CanPerform(Action.Restart)) return false;

         DespawnCurrentLevel(SpawnCurrentLevel);

         return true;
      }

      public bool TryPerform(Action action) {
         switch (action) {
            case Action.Launch: return Launch();
            case Action.Reset: return ResetToBuildingState();
            case Action.Restart: return RestartCurrentLevel();
            case Action.Undo: return CurrentLevel.RemoveLastTrackChunk();
            default: throw new ArgumentOutOfRangeException(nameof(action), action, null);
         }
      }

      public static bool CanPerform(Action action) {
         if (!Instance) return false;
         if (!Instance.CurrentLevel) return false;

         switch (action) {
            case Action.Launch when Instance.CurrentState != State.Building:

            case Action.Reset when Instance.CurrentState != State.Playing:

            case Action.Restart when Instance.CurrentState != State.Building:
            case Action.Restart when !Instance.CurrentLevel.HasAddedTrackChunks():

            case Action.Undo when Instance.CurrentState != State.Building:
            case Action.Undo when !Instance.CurrentLevel.HasAddedTrackChunks():
               return false;

            default: return true;
         }
      }
   }
}