using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ProjectChan.Define.Actor;

namespace ProjectChan.Battle
{
    /// <summary>
    /// 무기 장착/해제를 관리하는 클래스
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        // public
        public bool isWeapon;           // -> 플레이어는 현재 무기를 가지고 있는가?

        // private
        private ActorType actorType;    // -> 캐릭터 타입

        /// <summary>
        /// 캐릭터 모델이 지닌 무기위치를 담아놓을 리스트
        /// [0]MountingPos = 장착 위치
        /// [1]ReleasePos = 해제 위치 
        /// </summary>
        private List<GameObject> weaponPos = new List<GameObject>();

        /// <summary>
        /// 캐릭터 모델마다 무기위치를 관리하기 위해서 담아놓을 딕셔너리
        /// </summary>
        private Dictionary<ActorType, List<GameObject>> weaponDic = new Dictionary<ActorType, List<GameObject>>();

        /// <summary>
        /// WeaponController의 초기화 하는 메서드
        /// </summary>
        /// <param name="actorType"> 현재 캐릭터의 타입 </param>
        public void Initialize(ActorType actorType)
        {
            isWeapon = false;
            this.actorType = actorType;

            // -> 캐릭터 모델에 WeaponPos 태그를 지닌 오브젝트를 찾는다
            var weaponPosList = GameObject.FindGameObjectsWithTag("WeaponPos");

            // 찾은 Pos를 모두 리스트에 저장한다
            foreach (var pos in weaponPosList)
            {
                weaponPos.Add(pos);
            }

            /*
             *  무기를 장착한 상태에서 변신 했다가, 다시 변신을 하게 되었을 때
             *  이미 무기를 장착한 상태 이므로 무기를 장착한 상태라고 알려주는 작업
             *  GetChild(0)는 무기를 의미
             */

            // 캐릭터의 Pos가 이미 딕셔너리에 존재한다면
            if (weaponDic.ContainsKey(actorType))
            {
                // 무기를 장착한 상태라면
                if (weaponDic[actorType][0].transform.GetChild(0).gameObject.activeSelf)
                {
                    isWeapon = true;
                }

                return;
            }

            // 액터타입과 현재 캐릭터의 WeaponPos가 들어있는 리스트를 딕셔너리에 저장
            weaponDic.Add(this.actorType, weaponPos);
        }

        /// <summary>
        /// 애니메이션 이벤트로 사용
        /// 무기 장착 상태에 따라, 무기를 장착/해제 하는 메서드
        /// </summary>
        public void SetWeapon()
        {
            // 캐릭터의 무기 Pos가 딕셔너리에 없다면
            if (!weaponDic.ContainsKey(actorType))
            {
                return;
            }

            switch (actorType)
            {
                case ActorType.Character:
                    // 무기를 장착하지 않았다면
                    if (!isWeapon)
                    {
                        // -> 무기를 장착
                        weaponDic[actorType][0].transform.GetChild(0).gameObject.SetActive(true);
                        isWeapon = true;
                    }
                    // 무기를 장착 했다면
                    else if (isWeapon)
                    {
                        // 무기를 해제
                        weaponDic[actorType][0].transform.GetChild(0).gameObject.SetActive(false);
                        isWeapon = false;
                    }
                    break;

                case ActorType.Form:
                    // 무기를 장착하지 않았다면
                    if (!isWeapon)
                    {
                        // 무기를 장착합니다
                        weaponDic[actorType][1].transform.GetChild(0).gameObject.SetActive(false);
                        weaponDic[actorType][0].transform.GetChild(0).gameObject.SetActive(true);
                        isWeapon = true;
                    }
                    // 무기를 장착 했다면
                    else if (isWeapon)
                    {
                        // 무기를 해제 합니다
                        weaponDic[actorType][0].transform.GetChild(0).gameObject.SetActive(false);
                        weaponDic[actorType][1].transform.GetChild(0).gameObject.SetActive(true);
                        isWeapon = false;
                    }
                    break;
            }
        }

        /// <summary>
        /// 무기 Pos를 모두 삭제하는 메서드
        /// </summary>
        public void PosClear()
        {
            weaponPos.Clear();
        }
    }
}