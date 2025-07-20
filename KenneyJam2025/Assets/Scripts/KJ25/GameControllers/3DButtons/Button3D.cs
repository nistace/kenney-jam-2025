using KJ25.Levels;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace KJ25.GameControllers._3DButtons {
   public class Button3D : MonoBehaviour {
      private static readonly int hoveredAnimParam = Animator.StringToHash("Hovered");
      private static readonly int interactableAnimParam = Animator.StringToHash("Interactable");
      private static readonly int clickedAnimParam = Animator.StringToHash("Clicked");

      [SerializeField] private GameController.Action _action;
      [SerializeField] private InputActionReference _shortcut;
      [SerializeField] private Animator _animator;
      [SerializeField] private Renderer[] _coloredParts;
      [SerializeField] private Material _interactableMaterial;
      [SerializeField] private Material _notInteractableMaterial;
      [SerializeField] private float _cooldown = .1f;
      [SerializeField] private UnityEvent _onPointerEntered = new UnityEvent();
      [SerializeField] private UnityEvent _onPointerExited = new UnityEvent();
      [SerializeField] private UnityEvent _onInteracted = new UnityEvent();
      [SerializeField] private UnityEvent _onNotInteractableInteracted = new UnityEvent();

      private float NextInteractionTime { get; set; }
      private bool Interactable => GameController.CanPerform(_action);

      protected virtual void OnEnable() {
         if (_shortcut) {
            _shortcut.action.performed += HandleShortcut;
         }
      }

      private void Start() {
         GameController.OnStateChanged.AddListener(HandleGameStateChanged);
         GameController.OnCurrentLevelTrackChanged.AddListener(HandleTrackChanged);

         RefreshVisuals();
      }

      private void HandleTrackChanged(GameLevel level) => RefreshVisuals();

      private void RefreshVisuals() {
         _animator.SetBool(interactableAnimParam, Interactable);

         var material = Interactable ? _interactableMaterial : _notInteractableMaterial;
         foreach (var coloredPart in _coloredParts) {
            coloredPart.material = material;
         }
      }

      private void HandleGameStateChanged(GameController.State newState) => RefreshVisuals();

      protected void OnDisable() {
         if (_shortcut) {
            _shortcut.action.performed -= HandleShortcut;
         }
      }

      private void HandleShortcut(InputAction.CallbackContext obj) => Interact();

      public void HandlePointerEnter() {
         _animator.SetBool(hoveredAnimParam, true);
         _onPointerEntered.Invoke();
      }

      public void HandlePointerExit() {
         _animator.SetBool(hoveredAnimParam, false);
         _onPointerExited.Invoke();
      }

      public void Interact() {
         if (NextInteractionTime > Time.time) {
            return;
         }

         if (!Interactable) {
            _onNotInteractableInteracted.Invoke();
            return;
         }

         GameController.Instance.TryPerform(_action);
         _animator.SetTrigger(clickedAnimParam);
         NextInteractionTime = Time.time + _cooldown;
         _onInteracted.Invoke();
      }
   }
}