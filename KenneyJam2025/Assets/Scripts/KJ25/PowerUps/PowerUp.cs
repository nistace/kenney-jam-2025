using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KJ25.PowerUps {
   public class PowerUp : MonoBehaviour {
      private static readonly int emissionColorShaderProperty = Shader.PropertyToID("_EmissionColor");

      [SerializeField] private Renderer _renderer;
      [SerializeField] private Color _emissionColor = Color.white;
      [SerializeField] private Color _fadeEmissionColor = Color.black;
      [SerializeField] private AnimationCurve _emissionCurve;
      [SerializeField] private AnimationCurve _collectedEmissionCurve;

      private CancellationTokenSource _animationCancellationTokenSource;

      private void OnDestroy() {
         _animationCancellationTokenSource?.Cancel();
         _animationCancellationTokenSource?.Dispose();
         _animationCancellationTokenSource = null;
      }

      public void ResetPower(bool force) {
         if (enabled && !force) return;

         _animationCancellationTokenSource?.Cancel();
         _animationCancellationTokenSource?.Dispose();
         _animationCancellationTokenSource = new CancellationTokenSource();

         enabled = true;
         AnimateEmission(_animationCancellationTokenSource.Token).Forget();
      }

      private async UniTask AnimateEmission(CancellationToken cancellationToken) {
         var time = Random.value * _emissionCurve.keys.Last().time;
         while (enabled) {
            time += Time.deltaTime;
            _renderer.material.SetVector(emissionColorShaderProperty, _emissionColor * _emissionCurve.Evaluate(time));

            await UniTask.NextFrame(cancellationToken);
         }

         var fadeDuration = _collectedEmissionCurve.keys.Last().time;
         for (time = 0; time < fadeDuration; time += Time.deltaTime) {
            var intensity = _collectedEmissionCurve.Evaluate(time);
            var color = intensity > 0 ? _emissionColor * intensity : Color.Lerp(_emissionColor, _fadeEmissionColor, -intensity);

            _renderer.material.SetVector(emissionColorShaderProperty, color);

            await UniTask.NextFrame(cancellationToken);
         }

         _renderer.material.SetVector(emissionColorShaderProperty, Color.black);
      }

      private void OnTriggerEnter(Collider other) {
         if (!enabled) {
            return;
         }

         var otherAsPowerReceiver = other.GetComponent<IPowerReceiver>();
         if (otherAsPowerReceiver == null) {
            return;
         }

         otherAsPowerReceiver.PowerUp();
         enabled = false;
      }
   }
}