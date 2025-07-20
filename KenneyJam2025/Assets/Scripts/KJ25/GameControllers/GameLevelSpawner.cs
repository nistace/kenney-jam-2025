using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using KJ25.Levels;
using UnityEngine;
using UnityEngine.Events;

namespace KJ25.GameControllers {
   public class GameLevelSpawner : MonoBehaviour {
      [SerializeField] private AnimationData _spawnAnimationData;
      [SerializeField] private AnimationData _despawnAnimationData;

      public async UniTask Spawn(GameLevel level, UnityAction spawnedCallback) {
         await _spawnAnimationData.PerformOnItems(level.GetChildrenForSpawn(), destroyCancellationToken, spawnedCallback);
      }

      public async UniTask Despawn(GameLevel level, UnityAction despawnedCallback) {
         await _despawnAnimationData.PerformOnItems(level.GetChildrenForDespawn(), destroyCancellationToken, despawnedCallback);
      }

      [Serializable] private class AnimationData {
         [SerializeField] private float _delay;
         [SerializeField] private AnimationCurve _normalizedStartCurve;
         [SerializeField] private float _startDuration = 1;
         [SerializeField] private AnimationCurve _itemScaleCurve;
         [SerializeField] private float _itemDuration = .2f;
         [SerializeField] private float _finalValue;

         public async UniTask PerformOnItems(IReadOnlyList<Transform> items, CancellationToken cancellationToken, UnityAction callback) {
            await UniTask.WaitForSeconds(_delay, cancellationToken: cancellationToken);

            var spawnedAmount = 0;

            for (var time = 0f; time < _startDuration + _itemDuration; time += Time.deltaTime) {
               var newSpawnedAmount = _normalizedStartCurve.Evaluate(time / _startDuration) * items.Count;

               while (newSpawnedAmount > spawnedAmount) {
                  var child = items[spawnedAmount];
                  PerformOnItem(child, cancellationToken).Forget();
                  spawnedAmount++;
               }

               await UniTask.NextFrame(cancellationToken);
            }

            await UniTask.NextFrame(cancellationToken);

            callback?.Invoke();
         }

         public async UniTask PerformOnItem(Transform item, CancellationToken cancellationToken) {
            for (var t = 0f; item && t < _itemDuration; t += Time.deltaTime) {
               item.localScale = Vector3.one * _itemScaleCurve.Evaluate(t / _itemDuration);
               await UniTask.NextFrame(cancellationToken);
            }
            if (item) {
               item.localScale = Vector3.one * _finalValue;
            }
         }
      }
   }
}