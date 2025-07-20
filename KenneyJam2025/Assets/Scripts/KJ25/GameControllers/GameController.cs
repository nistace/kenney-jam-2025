using System;
using Cysharp.Threading.Tasks;
using KJ25.Levels;
using KJ25.Saving;
using KJ25.Tracks;
using UnityEngine;
using UnityEngine.Events;

namespace KJ25.GameControllers {
   public class GameController : MonoBehaviour {
      public static GameController Instance { get; private set; }

      public enum State {
         Title = 6,
         GameQuit = 5,
         Thanks = 7,
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

      public LevelsInfo LevelsInfo => _levelsInfo;
      public State CurrentState { get; private set; }
      private int CurrentLevelIndex { get; set; }
      public GameLevel CurrentLevel { get; private set; }
      public static UnityEvent<int, GameLevel> OnCurrentLevelChanged { get; } = new UnityEvent<int, GameLevel>();
      public static UnityEvent<GameLevel> OnCurrentLevelTrackChanged { get; } = new UnityEvent<GameLevel>();
      public static UnityEvent<State> OnStateChanged { get; } = new UnityEvent<State>();

      private void Awake() {
         Instance = this;
      }

      private void Start() {
         foreach (var debugLevel in FindObjectsByType<GameLevel>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            Destroy(debugLevel.gameObject);
         }
         StartTitle().Forget();
      }

      private async UniTaskVoid StartTitle() {
         ChangeState(State.Title);

         await UniTask.WaitForSeconds(1, cancellationToken: destroyCancellationToken);

         StartLevelSelection();
      }

      public void StartLevelSelection() {
         CleanUpCurrentLevel();

         ChangeState(State.LevelSelection);
      }

      private void CleanUpCurrentLevel() {
         if (CurrentLevel) {
            CurrentLevel.OnTrackChanged.RemoveListener(HandleTrackChanged);
            CurrentLevel.Finish.OnEntered.RemoveListener(HandleCurrentLevelFinishEntered);
            Destroy(CurrentLevel.gameObject);
         }
      }

      public void SpawnLevel(int index) {
         CleanUpCurrentLevel();

         CurrentLevelIndex = index;
         CurrentLevel = Instantiate(_levelsInfo[index]);
         CurrentLevel.Finish.OnEntered.AddListener(HandleCurrentLevelFinishEntered);
         CurrentLevel.OnTrackChanged.AddListener(HandleTrackChanged);

         _levelSpawner.Spawn(CurrentLevel, StartBuilderState).Forget();
         ChangeState(State.SpawningLevel);
         OnCurrentLevelChanged.Invoke(CurrentLevelIndex, CurrentLevel);
      }

      public void QuitGame() {
         DespawnCurrentLevel(Application.Quit);
         ChangeState(State.GameQuit);
      }

      private void HandleTrackChanged() => OnCurrentLevelTrackChanged.Invoke(CurrentLevel);

      private void StartBuilderState() {
         CurrentLevel.ResetGameLevel();
         ChangeState(State.Building);
      }

      private void ChangeState(State newState) {
         CurrentState = newState;

         OnStateChanged.Invoke(newState);
      }

      private void HandleCurrentLevelFinishEntered() {
         if (CurrentState != State.Playing) return;

         SaveManager.SetUnlockedLevelIndex(CurrentLevelIndex + 1);

         DespawnCurrentLevel(ContinueToNextLevel);
      }

      private void DespawnCurrentLevel(UnityAction then) {
         ChangeState(State.DespawningLevel);

         if (CurrentLevel) {
            _levelSpawner.Despawn(CurrentLevel, then).Forget();
         }
         else {
            then?.Invoke();
         }
      }

      private void ContinueToNextLevel() {
         CleanUpCurrentLevel();

         var nextLevelIndex = CurrentLevelIndex + 1;
         if (nextLevelIndex >= _levelsInfo.Levels.Count) {
            ChangeState(State.Thanks);
            return;
         }

         SpawnLevel(nextLevelIndex);
      }

      private void SpawnCurrentLevel() => SpawnLevel(CurrentLevelIndex);

      public void AppendTrackChunk(TrackChunkAmount trackChunk) {
         CurrentLevel.AppendTrackChunk(trackChunk);
      }

      public void ShowTrackChunkGhost(TrackChunkAmount trackChunk) => CurrentLevel.SetGhost(trackChunk.Chunk.Ghost);

      public void HideTrackChunkGhost(TrackChunkAmount trackChunk) => CurrentLevel.UnsetGhost(trackChunk.Chunk.Ghost);

      private bool Launch() {
         if (!CanPerform(Action.Launch)) return false;

         CurrentLevel.LaunchPlayerVehicle();
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