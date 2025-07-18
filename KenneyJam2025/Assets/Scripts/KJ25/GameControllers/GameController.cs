using UnityEngine;
using UnityEngine.InputSystem;

namespace KJ25.GameControllers {
   public class GameController : MonoBehaviour {
      [SerializeField] private Camera _camera;
      [SerializeField] private LaunchButton _launchButton;
      [SerializeField] private LayerMask _interactLayerMask;
      [SerializeField] private InputActionReference _pointActionReference;
      [SerializeField] private InputActionReference _clickActionReference;
      [SerializeField] private InputActionReference _quickLaunchActionReference;

      private GameObject PointerHitObject { get; set; }
      private I3DInteractable HoveredOverInteractable { get; set; }

      private void Start() {
         _quickLaunchActionReference.action.performed += HandleQuickLaunchPerformed;
         _clickActionReference.action.performed += HandleClickPerformed;
      }

      private void OnDestroy() {
         _quickLaunchActionReference.action.performed -= HandleQuickLaunchPerformed;
         _clickActionReference.action.performed -= HandleClickPerformed;
      }

      private void HandleQuickLaunchPerformed(InputAction.CallbackContext obj) => _launchButton.Interact();

      private void HandleClickPerformed(InputAction.CallbackContext obj) => HoveredOverInteractable?.Interact();

      private void Update() {
         var inputPointPosition = _pointActionReference.action.ReadValue<Vector2>();
         if (Physics.Raycast(_camera.ScreenPointToRay(inputPointPosition), out var hit, _interactLayerMask)) {
            if (PointerHitObject != hit.collider.gameObject) {
               PointerHitObject = hit.collider.gameObject;
               var newHoveredOverInteractable = hit.collider.GetComponentInParent<I3DInteractable>();
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
   }
}