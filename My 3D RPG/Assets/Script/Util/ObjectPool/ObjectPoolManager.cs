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
    /// 오브젝트 풀을 관리할 매니저 클래스
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        /// <summary>
        /// 모든 풀을 보관할 딕셔너리
        /// </summary>
        private Dictionary<PoolType, object> poolDic = new Dictionary<PoolType, object>();

        /// <summary>
        /// 풀에 프리팹을 등록하는 메서드
        /// </summary>
        /// <typeparam name="T">        생성할 풀의 타입 </typeparam>
        /// <param name="type">         딕셔너리에 사용할 풀의 키 값 </param>
        /// <param name="obj">          원형프리팹 </param>
        /// <param name="poolCount">    생성 개수 </param>
        public void RegistPool<T>(PoolType type, T obj, int poolCount = 1)
            where T : MonoBehaviour, IPoolableObject
        {
            // 프리팹을 등록할 풀
            ObjectPool<T> pool = null;

            // 이미 풀이 존재한다면
            if (poolDic.ContainsKey(type))
            {
                // Dictionary에 들어있는 풀은 Object형태로 저장 되어있으므로
                // as연산자를 이용해서 캐스팅
                pool = poolDic[type] as ObjectPool<T>;
            }
            else
            {
                // Dictionary에 없다면 새로운 풀을 생성
                pool = new ObjectPool<T>();
                poolDic.Add(type, pool);
            }

            // 풀에 프리팹을 담을 홀더가 없다면
            if (pool.holder == null)
            {
                // 프리팹의 이름을 이용해서 홀더를 생성
                pool.holder = new GameObject($"{type.ToString()}Holder").transform;
                pool.holder.parent = transform;
                pool.holder.position = Vector3.zero;
            }

            // 생성 개수만큼 오브젝트 생성
            for (int i = 0; i < poolCount; i++)
            {
                var poolableObj = Instantiate(obj);
                poolableObj.name = obj.name;
                poolableObj.transform.SetParent(pool.holder);
                poolableObj.gameObject.SetActive(false);

                // 생성한 오브젝트를 풀에 등록
                pool.RegistPoolableObject(poolableObj);
            }
        }

        /// <summary>
        /// 딕셔너리에 저장되어있는 풀을 반환해주는 메서드
        /// </summary>
        /// <typeparam name="T">    반환 받고 싶은 타입 </typeparam>
        /// <param name="type">     반환 받고 싶은 키 값 </param>
        /// <returns></returns>
        public ObjectPool<T> GetPool<T>(PoolType type)
            where T : MonoBehaviour, IPoolableObject
        {
            // Dictionary에 사용할 풀이 없다면
            if (!poolDic.ContainsKey(type))
            {
                return null;
            }

            return poolDic[type] as ObjectPool<T>;
        }

        /// <summary>
        /// 특정 풀을 제거하는 메서드
        /// </summary>
        /// <typeparam name="T">    제거 하고자 하는 풀의 타입 </typeparam>
        /// <param name="type">     제거 하고자 하는 풀의 키 값 </param>
        public void ClearPool<T>(PoolType type) where T : MonoBehaviour, IPoolableObject
        {
            var pool = GetPool<T>(type)?.Pool;

            if (pool == null)
            {
                return;
            }

            // 풀에 있는 오브젝트를 모두 파괴하는 작업
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
