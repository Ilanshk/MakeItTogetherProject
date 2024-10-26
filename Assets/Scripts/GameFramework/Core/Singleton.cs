using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameFramework.Core
{

    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                    if(_instance == null)
                    {
                        T[] objs = FindObjectsOfType<T>();
                        if(objs.Length > 0)
                        {
                            T instance = objs[0];
                            _instance = Instance;
                        }
                        else
                        {
                            GameObject go = new GameObject();
                            go.name = typeof(T).Name;
                            _instance = go.AddComponent<T>();
                            DontDestroyOnLoad(go);
                        }

                    }
                return _instance;
            }
        }
    }

}
