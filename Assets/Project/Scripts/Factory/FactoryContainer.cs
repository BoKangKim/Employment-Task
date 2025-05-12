using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Factory
{
    // Factory들을 관리하는 클래스
    public class FactoryContainer : MonoBehaviour
    {
        #region Singleton
        private static FactoryContainer instance = null;

        public static FactoryContainer Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindAnyObjectByType<FactoryContainer>();
                }

                return instance;
            }
        }
        #endregion
        
        private Dictionary<string, IFactory> factoryDict = new Dictionary<string, IFactory>();

        public T GetFactory<T>() where T : MonoBehaviour, IFactory
        {
            string key = typeof(T).ToString();

            IFactory targetFactory = null;

            if (!factoryDict.TryGetValue(key, out targetFactory))
            {
                targetFactory = GetComponentInChildren<T>();

                if (targetFactory == null)
                {
                    Debug.LogError($"Not Found {key}");
                    return null;
                }

                factoryDict.Add(key, targetFactory);
            }

            return targetFactory as T;
        }
    }
}

