using KJ25.GameControllers;
using KJ25.Levels;
using KJ25.Tracks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KJ25.Ui.Builder.TrackChunks {
   public class BuilderTrackChunkButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
      [SerializeField] private Button _button;
      [SerializeField] private Image _chunkImage;
      [SerializeField] private TMP_Text _amountText;

      private GameLevel Level { get; set; }
      private TrackChunkAmount TrackChunk { get; set; }
      private bool Interactable => _button.interactable;

      private void Start() {
         _button.onClick.AddListener(HandleClick);
      }

      public void Setup(GameLevel level, TrackChunkAmount trackChunkAmount) {
         if (Level) {
            Level.OnTrackChunkPlaced.RemoveListener(HandleTrackChunkPlaced);
            Level.OnTrackChunkRemoved.RemoveListener(HandleTrackChunkRemoved);
         }

         TrackChunk = trackChunkAmount;
         Level = level;

         level.OnTrackChunkPlaced.AddListener(HandleTrackChunkPlaced);

         _chunkImage.sprite = trackChunkAmount.Chunk.Sprite;
         _chunkImage.transform.rotation = Quaternion.Euler(0, trackChunkAmount.Chunk.FlipSprite ? 180 : 0, 0);
         RefreshCounters();
      }

      private void HandleTrackChunkRemoved(TrackChunkAmount removedChunk) {
         if (removedChunk != TrackChunk) return;

         RefreshCounters();
      }

      private void RefreshCounters() {
         var consumedAmount = Level ? Level.CountConsumed(TrackChunk) : 0;
         var interactable = Level && consumedAmount < TrackChunk.Amount;

         _button.interactable = interactable;
         _amountText.text = Level ? $"{consumedAmount}/{TrackChunk.Amount}" : string.Empty;

         if (!interactable) {
            GameController.Instance.HideTrackChunkGhost(TrackChunk);
         }
      }

      private void HandleTrackChunkPlaced(TrackChunkAmount placedChunk) {
         if (placedChunk != TrackChunk) return;

         RefreshCounters();
      }

      private void HandleClick() {
         if (TrackChunk == null) {
            return;
         }

         if (!Interactable) {
            return;
         }

         GameController.Instance.AppendTrackChunk(TrackChunk);
      }

      public void OnPointerEnter(PointerEventData eventData) {
         if (TrackChunk == null) {
            return;
         }

         if (!Interactable) {
            return;
         }

         GameController.Instance.ShowTrackChunkGhost(TrackChunk);
      }

      public void OnPointerExit(PointerEventData eventData) {
         if (TrackChunk == null) {
            return;
         }

         if (!Interactable) {
            return;
         }

         GameController.Instance.HideTrackChunkGhost(TrackChunk);
      }
   }
}