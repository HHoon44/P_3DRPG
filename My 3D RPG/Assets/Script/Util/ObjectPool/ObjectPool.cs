using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Util
{
    /// <summary>
    /// 오브젝트 풀링을 수행할 클래스
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> where T : MonoBehaviour, IPoolableObject
    {
        public Transform holder;

        /// <summary>
        /// 풀에 담겨있는 오브젝트 리스트
        /// </summary>
        public List<T> Pool { get; private set; } = new List<T>();

        /// <summary>
        /// 풀에서 재사용 가능한 객체가 존재하는지
        /// </summary>
        public bool canRecycle => Pool.Find(obj => obj.CanRecycle) != null;

        /// <summary>
        /// 풀링할 새로운 오브젝트를 등록하는 메서드
        /// </summary>
        /// <param name="obj">풀에 등록할 객체 </param>
        public void RegistPoolableObject(T obj)
        {
            Pool.Add(obj);
        }

        /// <summary>
        /// 오브젝트를 다시 풀에 반환하는 메서드
        /// </summary>
        /// <param name="obj"> 풀에 반환할 메서드 </param>
        public void ReturnPoolableObject(T obj)
        {
            obj.transform.SetParent(holder);
            obj.gameObject.SetActive(false);
            obj.CanRecycle = true;
        }

        /// <summary>
        /// 사용할 오브젝트를 풀에서 반환해주는 메서드
        /// </summary>
        /// <param name="pred"></param>
        /// <returns></returns>
        public T GetPoolableObject(Func<T, bool> pred = null)
        {
            // 재사용할 오브젝트가 없다면
            if (!canRecycle)
            {
                // 풀에 오브젝트가 존재한다면 0번째 오브젝트의 정보를 가져옴
                var protoObj = Pool.Count > 0 ? Pool[0] : null;

                // 프로토 타입 오브젝트가 있다면
                if (protoObj != null)
                {
                    // 오브젝트를 이용해서 새로운 오브젝트를 생성
                    var newObj = GameObject.Instantiate(protoObj.gameObject, holder);
                    newObj.name = protoObj.name;
                    newObj.SetActive(false);

                    // 생성한 오브젝트를 풀에 등록
                    RegistPoolableObject(newObj.GetComponent<T>());
                }
                else
                {

                    return null;
                }
            }

            // 풀에서 가져올 때 사용할 조건식의 여부에 따라
            // 풀에서 오브젝트 가져오는 조건을 다르게 함
            T recycleObj = (pred == null) ? (Pool.Count > 0 ? Pool[0] : null) :
                (Pool.Find(obj => pred(obj) && obj.CanRecycle));

            if (recycleObj == null)
            {
                return null;
            }

            recycleObj.CanRecycle = false;

            // 오브젝트를 반환
            return recycleObj;
        }
    }
}