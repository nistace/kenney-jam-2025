using Cysharp.Threading.Tasks;
using KJ25.Levels;
using KJ25.Tracks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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

      [SerializeField] private LevelsInfo _levelsInfo;
      [SerializeField] private Camera _camera;
      [SerializeField] private GameLevelSpawner _levelSpawner;
      [SerializeField] private LayerMask _interactLayerMask;
      [SerializeField] private InputActionReference _pointActionReference;
      [SerializeField] private InputActionReference _clickActionReference;
      [SerializeField] private InputActionReference _quickLaunchActionReference;

      private State CurrentState { get; set; }
      private int CurrentLevelIndex { get; set; }
      public GameLevel CurrentLevel { get; private set; }
      private GameObject PointerHitObject { get; set; }
      private ILevelInteractable HoveredOverInteractable { get; set; }
      public UnityEvent<LevelInfo, GameLevel> OnCurrentLevelChanged { get; } = new UnityEvent<LevelInfo, GameLevel>();
      public UnityEvent<State> OnStateChanged { get; } = new UnityEvent<State>();

      private void Awake() {
         Instance = this;
      }

      private void Start() {
         CurrentLevelIndex = 0;
         SpawnLevel(_levelsInfo.Levels[CurrentLevelIndex]);

         _quickLaunchActionReference.action.performed += HandleQuickLaunchPerformed;
         _clickActionReference.action.performed += HandleClickPerformed;
      }

      private void OnDestroy() {
         _quickLaunchActionReference.action.performed -= HandleQuickLaunchPerformed;
         _clickActionReference.action.performed -= HandleClickPerformed;
      }

      private void HandleQuickLaunchPerformed(InputAction.CallbackContext obj) {
         if (CurrentState == State.Building && CurrentLevel) {
            CurrentLevel.LaunchButton.Interact();
         }
      }

      private void SpawnLevel(LevelInfo levelInfo) {
         if (CurrentLevel) {
            Destroy(CurrentLevel.gameObject);
         }

         CurrentLevel = Instantiate(levelInfo.LevelPrefab);
         CurrentLevel.Finish.OnEntered.AddListener(HandleCurrentLevelFinishEntered);
         CurrentLevel.LaunchButton.OnConsumed.AddListener(HandleLaunchButtonConsumed);

         _levelSpawner.Spawn(CurrentLevel, StartBuilderState).Forget();
         ChangeState(State.SpawningLevel);
         OnCurrentLevelChanged.Invoke(levelInfo, CurrentLevel);
      }

      private void StartBuilderState() {
         ChangeState(State.Building);
      }

      private void HandleLaunchButtonConsumed() {
         CurrentLevel.LaunchButton.OnConsumed.RemoveListener(HandleLaunchButtonConsumed);

         ChangeState(State.Playing);
      }

      private void ChangeState(State newState) {
         CurrentState = newState;

         OnStateChanged.Invoke(newState);
      }

      private void HandleCurrentLevelFinishEntered() {
         CurrentLevel.Finish.OnEntered.RemoveListener(HandleCurrentLevelFinishEntered);

         _levelSpawner.Despawn(CurrentLevel, ContinueToNextLevel).Forget();
         ChangeState(State.DespawningLevel);
      }

      private void ContinueToNextLevel() {
         Destroy(CurrentLevel.gameObject);

         CurrentLevelIndex++;
         CurrentLevelIndex %= _levelsInfo.Levels.Length;
         SpawnLevel(_levelsInfo.Levels[CurrentLevelIndex]);
      }

      private void HandleClickPerformed(InputAction.CallbackContext obj) => HoveredOverInteractable?.Interact();

      private void Update() {
         var inputPointPosition = _pointActionReference.action.ReadValue<Vector2>();
         if (Physics.Raycast(_camera.ScreenPointToRay(inputPointPosition), out var hit, _interactLayerMask)) {
            if (PointerHitObject != hit.collider.gameObject) {
               PointerHitObject = hit.collider.gameObject;
               var newHoveredOverInteractable = hit.collider.GetComponentInParent<ILevelInteractable>();
               if (newHoveredOverInteractable != HoveredOverInteractable) {
                  HoveredOverInteractable?.HandlePointerExit();
                  HoveredOverInteractable = newHoveredOverInteractable;
                  HoveredOverInteractable?.HandlePointerEnter();
               }
            }
         }
         else {
            PointerHitObject = null;
            HoveredOverInteractable?.HandlePointerExit();
            HoveredOverInteractable = null;
         }
      }

      public void AppendTrackChunk(TrackChunkAmount trackChunk) {
         CurrentLevel.AppendTrackChunk(trackChunk);
      }

      public void ShowTrackChunkGhost(TrackChunkAmount trackChunk) => CurrentLevel.SetGhost(trackChunk.Chunk.Ghost);

      public void HideTrackChunkGhost(TrackChunkAmount trackChunk) => CurrentLevel.UnsetGhost(trackChunk.Chunk.Ghost);
   }
}