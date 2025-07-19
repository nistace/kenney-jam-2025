using UnityEditor;
using UnityEngine;

namespace KJ25.Levels.Editor {
   [CustomEditor(typeof(GameLevel))]
   public class GameLevelCustomEditor : UnityEditor.Editor {
      private static string environmentPrefix { get; set; } = "Obstacle_";

      public override void OnInspectorGUI() {
         base.OnInspectorGUI();

         if (Application.isPlaying) {
            return;
         }

         GUILayout.Space(1);
         GUILayout.Space(1);

         environmentPrefix = EditorGUILayout.TextField("Environment Prefix", environmentPrefix);

         var targetTransform = (target as GameLevel).transform;

         if (GUILayout.Button("SnapEnvironment")) {
            for (var i = 0; i < targetTransform.childCount; i++) {
               var child = targetTransform.GetChild(i);
               if (child.name.StartsWith(environmentPrefix)) {
                  child.localPosition = new Vector3(Mathf.RoundToInt(child.localPosition.x), Mathf.RoundToInt(child.localPosition.y), Mathf.RoundToInt(child.localPosition.z));
                  EditorUtility.SetDirty(child);
               }
            }
         }
      }
   }
}