using KJ25.GameControllers;
using KJ25.Tracks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KJ25.Ui.Builder.TrackChunks {
   public class BuilderTrackChunkButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
      [SerializeField] private Button _button;
      [SerializeField] private Image _chunkImage;

      private TrackChunkAmount TrackChunk { get; set; }

      private void Start() {
         _button.onClick.AddListener(HandleClick);
      }

      public void Setup(TrackChunkAmount trackChunkAmount) {
         _chunkImage.sprite = trackChunkAmount.Chunk.Sprite;

         TrackChunk = trackChunkAmount;
      }

      private void HandleClick() {
         if (TrackChunk == null) {
            return;
         }

         GameController.Instance.AppendTrackChunk(TrackChunk);
      }

      public void OnPointerEnter(PointerEventData eventData) {
         if (TrackChunk == null) {
            return;
         }

         GameController.Instance.ShowTrackChunkGhost(TrackChunk);
      }

      public void OnPointerExit(PointerEventData eventData) {
         if (TrackChunk == null) {
            return;
         }

         GameController.Instance.HideTrackChunkGhost(TrackChunk);
      }
   }
}