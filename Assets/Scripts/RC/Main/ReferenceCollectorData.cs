using System;
using UnityEngine;

namespace RC.Main
{
    [Serializable]
    public class ReferenceCollectorData
    {
        [SerializeField] private string key;
        [SerializeField] private UnityEngine.Object gameObject;

        public string Key
        {
            get => key;
            set => key = value;
        }

        public UnityEngine.Object GameObject
        {
            get => gameObject;
            set => gameObject = value;
        }
    }
}