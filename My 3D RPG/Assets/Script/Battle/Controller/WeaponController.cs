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
    public class WeaponController : MonoBehaviour
    {
        // -> 무기를 장착, 해제할때 무기의 포지션이 되어줄 오브젝트
        // -> [0]MountingPos = 장착 [1]ReleasePos = 해제
        private List<GameObject> weaponPos = new List<GameObject>();
        private Dictionary<ActorType, List<GameObject>> weaponDic = new Dictionary<ActorType, List<GameObject>>();
        private ActorType actorType;
        private SDWeapon sdWeapon;

        public bool isWeapon;       // -> 플레이어는 현재 무기를 가지고 있는가?

        /// <summary>
        /// => WeaponPos 태그를 지닌 객체를 찾아서 관리
        /// </summary>
        /// <param name="actorType"> 딕셔너리에서 사용할 Key 값 </param>
        /// <param name="sdWeapon"> 무기 프리팹 객체를 생성할때 파라미터로 보내줄 데이터 </param>
        public void Initialize(ActorType actorType, SDWeapon sdWeapon)
        {
            isWeapon = false;

            // -> 현재 객체에서 WeaponPos를 찾는 작업
            var WeaponPos = GameObject.FindGameObjectsWithTag("WeaponPos");

            // -> 모든 WeaponPos를 List에 저장
            foreach (var pos in WeaponPos)
            {
                weaponPos.Add(pos);
            }

            this.actorType = actorType;
            this.sdWeapon = sdWeapon;

            // -> 찾으면 파라미터로 받은 Key값과 WeaponPos가 들어있는 List를 담아놓는다 
            weaponDic.Add(this.actorType, weaponPos);

            // -> 무기를 든 상태에서 변신 -> 다시 변신을 하면 무기를 든 상태지만 isWeapon이 false로 설정되므로 예외 처리
            if (weaponDic[actorType][0].transform.GetChild(0).gameObject.activeSelf)
            {
                isWeapon = true;
            }
        }

        public void SetWeapon()
        {
            switch (actorType)
            {
                case ActorType.Character:
                    if (weaponDic.ContainsKey(actorType) && weaponDic[actorType][0] != null && !isWeapon)
                    {
                        weaponDic[actorType][0].transform.GetChild(0).gameObject.SetActive(true);
                        isWeapon = true;
                    }
                    else if (isWeapon)
                    {
                        weaponDic[actorType][0].transform.GetChild(0).gameObject.SetActive(false);

                        isWeapon = false;
                    }
                    break;

                case ActorType.Form:
                    if (weaponDic.ContainsKey(actorType) && weaponDic[actorType][0] != null && !isWeapon)
                    {
                        weaponDic[actorType][1].transform.GetChild(0).gameObject.SetActive(false);
                        weaponDic[actorType][0].transform.GetChild(0).gameObject.SetActive(true);

                        isWeapon = true;
                    }
                    else if (weaponDic.ContainsKey(actorType) && weaponDic[actorType][1] != null && isWeapon)
                    {
                        weaponDic[actorType][0].transform.GetChild(0).gameObject.SetActive(false);
                        weaponDic[actorType][1].transform.GetChild(0).gameObject.SetActive(true);

                        isWeapon = false;
                    }
                    break;
            }
        }

        /// <summary>
        /// => List와 Dic에 담겨있는 모든 데이터 제거
        /// </summary>
        public void PosClear()
        {
            weaponPos.Clear();
            weaponDic.Clear();
        }
    }
}