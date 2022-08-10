using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using System;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";
        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();


        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public object CaptrueState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {

                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;


        }
 
        public void Restore(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach(ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if (stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                }


            }

        }

#if UNITY_EDITOR
        private void Update()
        {
            //为每个saveableEntity生成UUID
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject serializedobject = new SerializedObject(this);
            SerializedProperty property = serializedobject.FindProperty("uniqueIdentifier");

            print("edit");

            if(string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedobject.ApplyModifiedProperties();

            }
            // 不写在if中：解决IsUnique中场景加载的问题-只重新在Dict中注册UUID和entity而不重新生成UUID
            globalLookup[property.stringValue] = this;

        }
#endif

        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate)) return true;

            if (globalLookup[candidate] == this) return true;

            //Dictionary为静态变量，跨场景，场景加载时，entity被删除，UUID仍存在于Dict中,使得返回值为False,UUID重新生成
            if(globalLookup[candidate] == null)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            //场景：在Editor中改变了entity的UUID，不同UUID指向同一个entity,移除旧的键值对
            if(globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }
    }

}

