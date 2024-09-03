using System;
using System.Collections.Generic;
using System.Linq;
using RC.Main;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RC.Editor
{
    [CustomEditor(typeof(ReferenceCollector))]
    public class ReferenceCollectorEditor : UnityEditor.Editor
    {
        private ReferenceCollector _referenceCollector;
        private Object _heroPrefab;
        private string _searchKey = "";

        private string SearchKey
        {
            get => _searchKey;
            set
            {
                if (_searchKey == value) return;
                _searchKey = value;
                _heroPrefab = _referenceCollector.GetSearchGameObject(SearchKey);
            }
        }

        private void DelNullReference()
        {
            var dataProperty = serializedObject.FindProperty("data");
            for (var i = dataProperty.arraySize - 1; i >= 0; i--)
            {
                var gameObjectProperty = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("gameObject");
                if (gameObjectProperty.objectReferenceValue != null) continue;
                dataProperty.DeleteArrayElementAtIndex(i);
                EditorUtility.SetDirty(_referenceCollector);
                serializedObject.ApplyModifiedProperties();
                serializedObject.UpdateIfRequiredOrScript();
            }
        }

        private void OnEnable() => _referenceCollector = (ReferenceCollector)target;

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(_referenceCollector, "Changed Settings");
            var dataProperty = serializedObject.FindProperty("data");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("添加引用")) AddReference(dataProperty, Guid.NewGuid().GetHashCode().ToString(), null);
            if (GUILayout.Button("全部删除")) _referenceCollector.Clear();
            if (GUILayout.Button("删除空引用")) DelNullReference();
            if (GUILayout.Button("排序")) _referenceCollector.Sort();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            SearchKey = EditorGUILayout.TextField(SearchKey);
            EditorGUILayout.ObjectField(_heroPrefab, typeof(Object), false);
            if (GUILayout.Button("删除"))
            {
                _referenceCollector.Remove(SearchKey);
                _heroPrefab = null;
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            var delList = new List<int>();
            for (var i = _referenceCollector.data.Count - 1; i >= 0; i--)
            {
                GUILayout.BeginHorizontal();
                var property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("key");
                property.stringValue = EditorGUILayout.TextField(property.stringValue, GUILayout.Width(150));
                property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("gameObject");
                property.objectReferenceValue =
                    EditorGUILayout.ObjectField(property.objectReferenceValue, typeof(Object), true);
                if (GUILayout.Button("X")) delList.Add(i);

                GUILayout.EndHorizontal();
            }

            var eventType = Event.current.type;
            if (eventType is EventType.DragUpdated or EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (eventType == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (var o in DragAndDrop.objectReferences)
                    {
                        var canAdd = true;
                        if (_referenceCollector.data.Any(t => t.GameObject == o))
                        {
                            Debug.LogError("这个GameObject已经被引用了");
                            canAdd = false;
                        }

                        if (canAdd)
                        {
                            AddReference(dataProperty, o.name, o);
                        }
                    }
                }

                Event.current.Use();
            }

            foreach (var i in delList) dataProperty.DeleteArrayElementAtIndex(i);

            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        private static void AddReference(SerializedProperty dataProperty, string key, Object obj)
        {
            var index = dataProperty.arraySize;
            dataProperty.InsertArrayElementAtIndex(index);
            var element = dataProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("key").stringValue = key;
            element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
        }
    }
}