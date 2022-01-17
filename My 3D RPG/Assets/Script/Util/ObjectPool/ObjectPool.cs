using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Util
{
    /// <summary>
    /// => 오브젝트 풀링을 수행할 클래스
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T>
        where T : MonoBehaviour, IPoolableObject
    {
        public Transform holder;

        /// <summary>
        /// => 오브젝트 풀 리스트
        /// </summary>
        public List<T> Pool { get; private set; } = new List<T>();

        /// <summary>
        /// => 풀에서 재사용 가능한 객체가 존재하는지
        /// </summary>
        public bool canRecycle => Pool.Find(obj => obj.CanRecycle) != null;

        /// <summary>
        /// => 풀링할 새로운 오브젝트를 등록하는 메서드
        /// </summary>
        /// <param name="obj">풀에 등록할 객체 </param>
        public void RegistPoolableObject(T obj)
        {
            Pool.Add(obj);
        }

        /// <summary>
        /// => 객체를 다시 풀에 반환하는 메서드
        /// </summary>
        /// <param name="obj"> 풀에 반환할 메서드 </param>
        public void ReturnPoolableObject(T obj)
        {
            obj.transform.SetParent(holder);
            obj.gameObject.SetActive(false);
            obj.CanRecycle = true;
        }

        /// <summary>
        /// => 사용할 풀을 반환해주는 메서드
        /// </summary>
        /// <param name="pred"></param>
        /// <returns></returns>
        public T GetPoolableObject(Func<T, bool> pred = null)
        {
            // -> 재사용 할 수 없다면!
            if (!canRecycle)
            {
                // ->                           True      False
                var protoObj = Pool.Count > 0 ? Pool[0] : null;

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

            // ->                           True                                False
            T recycleObj = (pred == null) ? (Pool.Count > 0 ? Pool[0] : null) : (Pool.Find(obj => pred(obj) && obj.CanRecycle));

            if (recycleObj == null)
            {
                return null;
            }

            recycleObj.CanRecycle = false;

            return recycleObj;
        }
    }
}