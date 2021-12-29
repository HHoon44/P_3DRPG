using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Util
{
    public class ObjectPool<T>
        where T : MonoBehaviour, IPoolableObject
    {
        public List<T> Pool { get; private set; } = new List<T>();

        public Transform holder;

        public bool canRecycle => Pool.Find(obj => obj.CanRecycle) != null;

        public void RegistPoolableObject(T obj)
        {
            Pool.Add(obj);
        }

        public void ReturnPoolableObject(T obj)
        {
            obj.transform.SetParent(holder);
            obj.gameObject.SetActive(false);
            obj.CanRecycle = true;
        }

        public T GetPoolableObject(Func<T, bool> pred = null)
        {
            if (!canRecycle)
            {
                var protoObj = pred == null ? 
                    (Pool.Count > 0 ? Pool[0] : null) :
                    Pool.Find(obj => pred(obj));

                if (protoObj != null)
                {
                    var newObj = GameObject.Instantiate(protoObj.gameObject, holder);
                    newObj.name = protoObj.name;
                    newObj.SetActive(false);

                    RegistPoolableObject(newObj.GetComponent<T>());
                }
                else
                {
                    return null;
                }
            }

            T recycleObj = pred == null ?
                (Pool.Count > 0 ? Pool[0] : null) : 
                Pool.Find(obj => pred(obj) && obj.CanRecycle);

            if (recycleObj == null)
            { 
                return null;
            }

            recycleObj.CanRecycle = false;

            return recycleObj;
        }
    }
}