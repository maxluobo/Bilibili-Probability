using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RC.Main
{
    [DisallowMultipleComponent]
    public class ReferenceCollector : MonoBehaviour, ISerializationCallbackReceiver
    {
        public List<ReferenceCollectorData> data = new();

        private readonly Dictionary<string, Object> _dict = new();

        #region Public Methods

        public T GetComponent<T>(string key) where T : Component => GetGameObject(key).GetComponent<T>();

        public GameObject GetGameObject(string key) => GetObject<GameObject>(key);

        public T GetObject<T>(string key) where T : Object => _dict.TryGetValue(key, out var value) ? value as T : null;

        #endregion

        #region Editor Methods

#if UNITY_EDITOR
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public void Add(string key, Object obj)
        {
            var serializedObject = new UnityEditor.SerializedObject(this);
            var dataProperty = serializedObject.FindProperty("data");
            int i;
            for (i = 0; i < data.Count; i++)
            {
                if (data[i].Key == key) break;
            }

            if (i != data.Count)
            {
                var element = dataProperty.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
            }
            else
            {
                dataProperty.InsertArrayElementAtIndex(i);
                var element = dataProperty.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("key").stringValue = key;
                element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
            }

            UnityEditor.EditorUtility.SetDirty(this);
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            var serializedObject = new UnityEditor.SerializedObject(this);
            var dataProperty = serializedObject.FindProperty("data");
            var i = 0;
            for (; i < data.Count; i++)
            {
                if (data[i].Key == key) break;
            }

            if (i != data.Count) dataProperty.DeleteArrayElementAtIndex(i);

            UnityEditor.EditorUtility.SetDirty(this);
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        public void Clear()
        {
            var serializedObject = new UnityEditor.SerializedObject(this);
            var dataProperty = serializedObject.FindProperty("data");
            dataProperty.ClearArray();
            UnityEditor.EditorUtility.SetDirty(this);
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        public void Sort()
        {
            var serializedObject = new UnityEditor.SerializedObject(this);
            data.Sort(new ReferenceCollectorDataComparer());
            UnityEditor.EditorUtility.SetDirty(this);
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        /// <summary>
        /// 搜索元素
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public GameObject GetSearchGameObject(string n)
        {
            if (_dict.TryGetValue(n, out var value))
            {
                return (GameObject)value;
            }

            return null;
        }
#endif

        #endregion

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            _dict.Clear();
            foreach (var referenceCollectorData in data.Where(referenceCollectorData =>
                         !_dict.ContainsKey(referenceCollectorData.Key)))
            {
                _dict.Add(referenceCollectorData.Key, referenceCollectorData.GameObject);
            }
        }

        #endregion
    }
}