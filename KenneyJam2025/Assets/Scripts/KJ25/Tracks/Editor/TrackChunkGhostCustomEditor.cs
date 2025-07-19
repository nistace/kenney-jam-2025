using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KJ25.Tracks.Editor {
   [CustomEditor(typeof(TrackChunkGhost))]
   public class TrackChunkGhostCustomEditor : UnityEditor.Editor {
      private static TrackChunk TrackChunk { get; set; }

      public override void OnInspectorGUI() {
         base.OnInspectorGUI();

         if (Application.isPlaying) {
            return;
         }

         var ghost = target as TrackChunkGhost;

         GUILayout.Label("Editor");
         TrackChunk = EditorGUILayout.ObjectField("Source Chunk", TrackChunk, typeof(TrackChunk), true) as TrackChunk;

         if (TrackChunk && GUILayout.Button("Generate")) {
            Undo.RecordObject(ghost, "Generate Ghost");

            var indexToDestroy = 0;
            while (ghost.transform.childCount > indexToDestroy) {
               var child = ghost.transform.GetChild(indexToDestroy);
               if (child.GetComponent<Renderer>() || child.gameObject.layer == LayerMask.NameToLayer("GhostColliders") && child.GetComponent<BoxCollider>()) {
                  Undo.DestroyObjectImmediate(child.gameObject);
               }
               else {
                  indexToDestroy++;
               }
            }

            var instance = Instantiate(TrackChunk.gameObject, ghost.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;

            var allRenderers = instance.GetComponentsInChildren<Renderer>();
            var allColliders = instance.GetComponentsInChildren<BoxCollider>().Where(t => t.gameObject.layer == LayerMask.NameToLayer("Obstacles")).ToArray();

            serializedObject.Update();

            target.name = $"TrackChunkGhost_{TrackChunk.name.Split("TrackChunk_").Last()}";

            var renderersProperty = serializedObject.FindProperty("_renderers");
            renderersProperty.arraySize = allRenderers.Length;

            var ghostMaterial = serializedObject.FindProperty("_ghostValidMaterial").objectReferenceValue as Material;

            for (var index = 0; index < allRenderers.Length; index++) {
               var renderer = allRenderers[index];
               renderer.sharedMaterials = new[] { ghostMaterial };
               renderersProperty.GetArrayElementAtIndex(index).objectReferenceValue = renderer;
               renderer.transform.SetParent(ghost.transform);
            }

            var collidersProperty = serializedObject.FindProperty("_colliders");
            collidersProperty.arraySize = allColliders.Length;

            for (var index = 0; index < allColliders.Length; index++) {
               var ghostCollider = allColliders[index];
               ghostCollider.gameObject.layer = LayerMask.NameToLayer("GhostColliders");
               collidersProperty.GetArrayElementAtIndex(index).objectReferenceValue = ghostCollider;
               ghostCollider.transform.SetParent(ghost.transform);
            }

            serializedObject.ApplyModifiedProperties();

            DestroyImmediate(instance);
         }
      }
   }
}