using ProjectChan.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Util
{
    /// <summary>
    /// => 오브젝트 풀을 관리할 매니저 클래스
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        /// <summary>
        /// => 모든 풀을 보관할 딕셔너리
        /// </summary>
        private Dictionary<PoolType, object> poolDic = new Dictionary<PoolType, object>();

        /// <summary>
        /// => 딕셔너리에 새로운 풀을 생성하여 등록하는 기능
        /// </summary>
        /// <typeparam name="T"> 생성할 풀의 타입 </typeparam>
        /// <param name="type"> 딕셔너리에 사용할 풀의 키 값 </param>
        /// <param name="obj"> 원형프리팹 </param>
        /// <param name="poolCount"> 생성 개수 </param>
        public void RegistPool<T>(PoolType type, T obj, int poolCount = 1)
            where T : MonoBehaviour, IPoolableObject
        {
            ObjectPool<T> pool = null;

            // -> 이미 키 값이 있다면!
            if (poolDic.ContainsKey(type))
            {
                // -> 키 값에 해당하는 데이터는 현재 Object 타입이므로
                //    as연산자를 이용해서 캐스팅합니다!
                pool = poolDic[type] as ObjectPool<T>;
            }
            else
            {
                // -> 키 값이 등록 되어있지 않다면 새로 생성해서 등록합니다!
                pool = new ObjectPool<T>();
                poolDic.Add(type, pool);
            }

            // -> 풀에 홀더가 없다면!
            if (pool.holder == null)
            {
                // -> 사용하고자 하는 풀의 타입을 이름으로 해서 홀더를 생성합니다!
                pool.holder = new GameObject($"{type.ToString()}Holder").transform;
                pool.holder.parent = transform;
                pool.holder.position = Vector3.zero;
            }

            // -> 생성할 개수 만큼 오브젝트를 생성합니다!
            for (int i = 0; i < poolCount; i++)
            {
                var poolableObj = Instantiate(obj);
                poolableObj.name = obj.name;
                poolableObj.transform.SetParent(pool.holder);
                poolableObj.gameObject.SetActive(false);

                // -> 생성한 오브젝트를 풀에 등록합니다!
                pool.RegistPoolableObject(poolableObj);
            }
        }

        /// <summary>
        /// => 딕셔너리에 저장되어있는 풀을 반환해주는 메서드
        /// </summary>
        /// <typeparam name="T"> 반환 받고 싶은 타입 </typeparam>
        /// <param name="type"> 반환 받고 싶은 키 값 </param>
        /// <returns></returns>
        public ObjectPool<T> GetPool<T>(PoolType type)
            where T : MonoBehaviour, IPoolableObject
        {
            // -> 키 값이 딕셔너리에 없다면!
            if (!poolDic.ContainsKey(type))
            {
                return null;
            }

            return poolDic[type] as ObjectPool<T>;
        }

        /// <summary>
        /// => 특정 풀을 제거하는 메서드
        /// </summary>
        /// <typeparam name="T"> 제거 하고자 하는 풀의 타입 </typeparam>
        /// <param name="type"> 제거 하고자 하는 풀의 키 값 </param>
        public void ClearPool<T>(PoolType type)
            where T : MonoBehaviour, IPoolableObject
        {
            var pool = GetPool<T>(type)?.Pool;

            if (pool == null)
            {
                return;
            }

            // -> 풀 안에 있는 오브젝트를 다 파괴합니다!
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i] != null)
                { 
                    Destroy(pool[i].gameObject);
                }
            }

            pool.Clear();
        }
    }
}
