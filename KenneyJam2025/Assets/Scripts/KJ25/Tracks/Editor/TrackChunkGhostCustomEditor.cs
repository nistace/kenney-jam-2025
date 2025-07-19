using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KJ25.Tracks.Editor {
   [CustomEditor(typeof(TrackChunkGhost))]
   public class TrackChunkGhostCustomEditor : UnityEditor.Editor {
      private static TrackChunk TrackChunk { get; set; }
      private static Material GhostMaterial { get; set; }

      public override void OnInspectorGUI() {
         base.OnInspectorGUI();

         if (Application.isPlaying) {
            return;
         }

         var ghost = target as TrackChunkGhost;

         GUILayout.Label("Editor");
         TrackChunk = EditorGUILayout.ObjectField("Source Chunk", TrackChunk, typeof(TrackChunk), true) as TrackChunk;
         GhostMaterial = EditorGUILayout.ObjectField("Ghost Material", GhostMaterial, typeof(Material), true) as Material;

         if (TrackChunk && GhostMaterial && GUILayout.Button("Generate")) {
            Undo.RecordObject(ghost, "Generate Ghost");

            while (ghost.transform.childCount > 0) {
               Undo.DestroyObjectImmediate(ghost.transform.GetChild(0).gameObject);
            }

            var instance = Instantiate(TrackChunk.gameObject, ghost.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;

            var allRenderers = instance.GetComponentsInChildren<Renderer>();
            serializedObject.Update();

            target.name = $"TrackChunkGhost_{TrackChunk.name.Split("TrackChunk_").Last()}";

            var renderersProperty = serializedObject.FindProperty("_renderers");
            renderersProperty.arraySize = allRenderers.Length;

            for (var index = 0; index < allRenderers.Length; index++) {
               var renderer = allRenderers[index];
               renderer.sharedMaterials = new[] { GhostMaterial };
               renderersProperty.GetArrayElementAtIndex(index).objectReferenceValue = renderer;
               renderer.transform.SetParent(ghost.transform);
            }

            serializedObject.ApplyModifiedProperties();

            for (var childIndex = 0; childIndex < ghost.transform.childCount; childIndex++) {
               if (!ghost.transform.GetChild(childIndex).TryGetComponent<Renderer>(out _)) {
                  DestroyImmediate(ghost.transform.GetChild(childIndex).gameObject);
                  childIndex--;
               }
            }
         }
      }
   }
}