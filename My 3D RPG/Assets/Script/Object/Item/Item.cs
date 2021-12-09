using ProjectChan.DB;
using ProjectChan.Dummy;
using ProjectChan.SD;
using ProjectChan.UI;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Object
{
    public class Item : MonoBehaviour, IPoolableObject
    {
        private SDItem sdItem;

        public bool CanRecycle { get; set; } = true;
        
        /// <summary>
        /// => 아이템 초기 설정 메서드
        /// </summary>
        /// <param name="itemNumber"> SDItem 데이터를 가져올때 사용할 인덱스값 </param>
        public void Initialize(int itemNumber)
        {
            // -> 현재 아이템의 SD데이터를 파라미터로 받은 itemNumber을 이용하여 불러온다
            sdItem =  GameManager.SD.sdItems.Where(obj => obj.index == itemNumber)?.SingleOrDefault();
        }

        private void OnTriggerEnter(Collider other)
        {
            // -> 다른 Layer와 충돌했다면 
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                return;
            }

            // -> 이미 존재한 아이템인지 비교 하거나 새로운 아이템이라면 추가하기 위해서
            //    게임매니저가 가지고 있는 BoItem 데이터를 가져온다
            var userBoItems = GameManager.User.boItems;

            // -> 아이템 슬롯에 접근하기 위해서 인벤토리를 가져온다
            var userInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            // -> 아이템이 장비 아이템인지 판단
            var isEquip = sdItem.itemType == Define.ItemType.Equipment;

            // -> 장비 아이템이 아니라면
            if (!isEquip)
            {
                // -> 이미 가지고 있는 아이템인지 판단
                var sameItem = userBoItems.Where(obj => obj.sdItem.index == sdItem.index)?.SingleOrDefault();

                // -> 이미 가지고 있는 아이템이 라면
                if (sameItem != null)
                {
                    // -> 개수값을 증가시켜주고 슬롯이 지닌 개수 텍스트 업데이트
                    sameItem.amount++;
                    userInventory.IncreaseItem(sameItem);
                }
                // -> 이미 가지고 있는 아이템이 아니라면
                else
                {
                    // -> 새롭게 추가한다
                    SetItem(new BoItem(sdItem));
                }
            }
            else
            {
                // -> 장비 아이템은 무조건 한칸을 차지하므로 새롭게 추가한다
                SetItem(new BoEquipment(sdItem));
            }

            void SetItem(BoItem boItem)
            {
                userInventory.AddItem(boItem);
                userBoItems.Add(boItem);
            }

            // -> 작업이 끝났으니 다시 풀로 반환 한다
            ObjectPoolManager.Instance.GetPool<Item>(Define.PoolType.Item).ReturnPoolableObject(this);

            // -> BoItem의 데이터에 변동이 생겼으므로 다시 DtoItem에 넣어주는 작업을 한다(나중에 변동된 데이터만 들어가게 설정하는 작업 해보도록)
            DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            DummyServer.Instance.Save();
        }
    }
}