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
    /// => 무기 장착, 해제를 관리하는 클래스
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        public bool isWeapon;           // -> 플레이어는 현재 무기를 가지고 있는가?

        private ActorType actorType;    // -> 캐릭터 타입

        /// <summary>
        /// => 오브젝트가 지닌 WeaponPos를 담아놓을 리스트
        /// => [0]MountingPos = 장착 위치
        /// => [1]ReleasePos = 해제 위치 
        /// </summary>
        private List<GameObject> weaponPos = new List<GameObject>();

        /// <summary>
        /// => 현재 캐릭터의 WeaponPos 정보를 담아놓을 딕셔너리
        /// </summary>
        private Dictionary<ActorType, List<GameObject>> weaponDic = new Dictionary<ActorType, List<GameObject>>();

        /// <summary>
        /// => WeaponPos 태그를 지닌 객체를 찾아서 관리
        /// </summary>
        /// <param name="actorType"> 현재 캐릭터의 타입 </param>
        public void Initialize(ActorType actorType)
        {
            // -> 아직 무기 장착을 하지 않았습니다!
            isWeapon = false;
            this.actorType = actorType;

            // -> 캐릭터 프리팹안에서 WeaponPos 태그를 지닌 오브젝트를 찾습니다!
            var weaponPosList = GameObject.FindGameObjectsWithTag("WeaponPos");

            // -> 찾은 WeaponPos를 리스트에 저장합니다!
            foreach (var pos in weaponPosList)
            {
                weaponPos.Add(pos);
            }

            /*
                -> 무기를 장착한 상태에서 변신 했다가 다시 변신을 하게 되었을 때 
                   이미 무기를 장착한 상태이므로 무기를 장착한 상태라고 알려주는 작업
                   GetChild(0)는 무기를 의미
            */
            if (weaponDic.ContainsKey(actorType))
            {
                if (weaponDic[actorType][0].transform.GetChild(0).gameObject.activeSelf)
                {
                    isWeapon = true;
                }

                return;
            }

            // -> 액터타입과 현재 캐릭터의 WeaponPos가 들어있는 리스트를 딕셔너리에 저장합니다!
            weaponDic.Add(this.actorType, weaponPos);
        }

        /// <summary>
        /// => 애니메이션 이벤트로 사용되며 무기를 장착한 상태가 아니라면 장착하고
        ///    이미 장착한 상태라면 해제 하는 메서드
        /// </summary>
        public void SetWeapon()
        {
            // -> 현재 캐릭터가 딕셔너리에 존재하지 않는다면!
            if (!weaponDic.ContainsKey(actorType))
            {
                return;
            }

            // -> GetChild(0)은 무기 입니다!
            switch (actorType)
            {
                case ActorType.Character:
                    // -> 무기를 장착하지 않았다면!
                    if (!isWeapon)
                    {
                        // -> 무기를 장착합니다!
                        weaponDic[actorType][0].transform.GetChild(0).gameObject.SetActive(true);
                        isWeapon = true;
                    }
                    // -> 무기를 장착 했다면!
                    else if (isWeapon)
                    {
                        // -> 무기를 해제 합니다!
                        weaponDic[actorType][0].transform.GetChild(0).gameObject.SetActive(false);
                        isWeapon = false;
                    }
                    break;

                case ActorType.Form:
                    // -> 무기를 장착하지 않았다면!
                    if (!isWeapon)
                    {
                        // -> 무기를 장착합니다!
                        weaponDic[actorType][1].transform.GetChild(0).gameObject.SetActive(false);
                        weaponDic[actorType][0].transform.GetChild(0).gameObject.SetActive(true);
                        isWeapon = true;
                    }
                    // -> 무기를 장착 했다면!
                    else if (isWeapon)
                    {
                        // -> 무기를 해제 합니다!
                        weaponDic[actorType][0].transform.GetChild(0).gameObject.SetActive(false);
                        weaponDic[actorType][1].transform.GetChild(0).gameObject.SetActive(true);
                        isWeapon = false;
                    }
                    break;
            }
        }

        /// <summary>
        /// => 리스트에 담겨 있는 Pos들을 초기화 하는 메서드
        /// </summary>
        public void PosClear()
        {
            weaponPos.Clear();
        }
    }
}