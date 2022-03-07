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
    /// <summary>
    /// => 아이템 객체가 지닐 클래스
    /// </summary>
    public class Item : MonoBehaviour, IPoolableObject
    {
        // public
        public MeshRenderer meshRender;     // -> 아이템에 세팅할 머티리얼

        // private
        private SDItem sdItem;              // -> 현재 아이템의 기획 데이터

        /// <summary>
        /// => 재사용 가능 여부
        /// </summary>
        public bool CanRecycle { get; set; } = true;

        /// <summary>
        /// => 아이템 초기 설정 메서드
        /// </summary>
        /// <param name="itemNumber"> SDItem 데이터를 가져올때 사용할 인덱스값 </param>
        public void Initialize(int itemNumber)
        {
            // -> 현재 아이템의 기획 데이터를 불러오기 위해 파라미터로 받은 itemNumber을 이용합니다!
            sdItem = GameManager.SD.sdItems.Where(obj => obj.index == itemNumber)?.SingleOrDefault();

            // -> 현재 아이템의 용도에 맞는 머티리얼을 가져와 세팅 합니다!
            var materials = meshRender.materials;
            var material = Resources.Load<Material>(sdItem.resourcePath);
            materials[0] = material;
            meshRender.materials = materials;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                return;
            }

            // -> 이미 존재한 아이템인지 비교 하거나 새로운 아이템이라면 추가하기 위해서
            //    GM이 지닌 BoItem을 가져옵니다!
            var userBoItems = GameManager.User.boItems;

            // -> 아이템 슬롯에 접근하기 위해서 UIInventory를 가져옵니다!
            var userInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            // -> 아이템이 장비 아이템인지 판별합니다!
            var isEquip = (sdItem.itemType == Define.ItemType.Equipment);

            // -> 장비 아이템이 아니라면!
            if (!isEquip)
            {
                // -> 이미 가지고 있는 아이템인지 확인 합니다!
                var sameItem = userBoItems.Where(obj => obj.sdItem.index == sdItem.index)?.SingleOrDefault();

                // -> 이미 가지고 있는 아이템이 라면!
                if (sameItem != null)
                {
                    // -> 개수값을 증가시켜주고 슬롯이 지닌 개수 텍스트를 업데이트 합니다!
                    sameItem.amount++;
                    userInventory.IncreaseItem(sameItem);
                }
                // -> 이미 가지고 있는 아이템이 아니라면!
                else
                {
                    // -> 새롭게 추가 합니다!
                    SetItem(new BoItem(sdItem));
                }
            }
            else
            {
                // -> 장비 아이템은 무조건 한칸을 차지하므로 새롭게 추가합니다!
                SetItem(new BoEquipment(sdItem));
            }

            void SetItem(BoItem boItem)
            {
                userInventory.AddItem(boItem);
                userBoItems.Add(boItem);
            }

            // -> 작업이 끝났으니 다시 풀로 반환 합니다!
            ObjectPoolManager.Instance.GetPool<Item>(Define.PoolType.Item).ReturnPoolableObject(this);

            // -> BoItem의 데이터에 변동이 생겼으므로 다시 DtoItem에 넣어주는 작업을 합니다!
            DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            DummyServer.Instance.Save();
        }
    }
}