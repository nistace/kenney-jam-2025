using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace KJ25.GameControllers._3DButtons {
   public class Buttons3DInputController : MonoBehaviour {
      [SerializeField] private Camera _camera;
      [SerializeField] private LayerMask _interactLayerMask;
      [SerializeField] private InputActionReference _pointActionReference;
      [SerializeField] private InputActionReference _clickActionReference;

      private GameObject PointerHitObject { get; set; }
      private Button3D HoveredOverInteractable { get; set; }

      private void Start() {
         _clickActionReference.action.performed += HandleClickPerformed;
      }

      private void OnDestroy() {
         _clickActionReference.action.performed -= HandleClickPerformed;
      }

      private void HandleClickPerformed(InputAction.CallbackContext obj) {
         if (HoveredOverInteractable == null) return;

         HoveredOverInteractable.Interact();
      }

      private void Update() {
         if (EventSystem.current.IsPointerOverGameObject()) {
            PointerHitObject = null;
            HoveredOverInteractable?.HandlePointerExit();
            HoveredOverInteractable = null;
            return;
         }

         var inputPointPosition = _pointActionReference.action.ReadValue<Vector2>();
         if (Physics.Raycast(_camera.ScreenPointToRay(inputPointPosition), out var hit, _interactLayerMask)) {
            if (PointerHitObject != hit.collider.gameObject) {
               PointerHitObject = hit.collider.gameObject;
               var newHoveredOverInteractable = hit.collider.GetComponentInParent<Button3D>();
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