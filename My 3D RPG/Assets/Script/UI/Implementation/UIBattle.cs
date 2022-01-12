using ProjectChan.Object;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.UI
{
    public class UIBattle : UIWindow
    {
        public PlayerController playerController;       // -> 플레이어 컨트롤러
        public BubbleGauge hpBubbleGauge;               // -> Hp 게이지 컴포넌트
        public BubbleGauge energyBubbleGauge;           // -> Energy 게이지 컴포넌트
        public Canvas worldCanvas;                      // -> 월드 컨버스 객체
        public Texture2D normalCursor;                  // -> 기본 마우스 커서 이미지
        public Texture2D targetPointCursor;             // -> 몬스터 타겟팅 커서 이미지

        /// <summary>
        /// => 현재 몬스터가 지니고 있는 체력바
        /// </summary>
        private List<MonHpBar> allMonHpBar = new List<MonHpBar>();

        private void Update()
        {
            PlayerCursorUpdate();
            BillboardUpdate();
            BubbleGaugeUpdate();
            MonHpBarUpdate();
        }

        /// <summary>
        /// => 플레이어의 커서를 상황에 따라 바꿔주는 메서드
        /// </summary>
        private void PlayerCursorUpdate()
        {
            Cursor.SetCursor
                (playerController.HasPointTarget ? targetPointCursor : normalCursor, new Vector2(20, 1), CursorMode.Auto);
        }

        /// <summary>
        /// => 월드 컨버스 안에 존재하는 객체들이 플레이어를 바라 보도록 설정하는 메서드
        /// </summary>
        public void BillboardUpdate()
        {
            // -> 월드 캔버스가 없다면!
            if (worldCanvas == null)
            {
                return;
            }

            // -> 플레이어를 따라다니는 카메라의 트랜스폼을 가져옵니다!
            var camTrans = CameraController.Cam.transform;

            for (int i = 0; i < worldCanvas.transform.childCount; i++)
            {
                var child = worldCanvas.transform.GetChild(i);

                /// => LookAt : 지정된 오브젝트들이 파라미터로 들어간 Target을 바라보게 해줌
                child.LookAt(camTrans, Vector3.up);

                /// => 표류하지 않거나 의도하지 않은 회전을 일으킬 수 있기때문에 직접적으로 바꾸지 않음 
                var newRot = child.eulerAngles;
                newRot.x = 0;
                newRot.z = 0;
                child.eulerAngles = newRot;
            }
        }

        /// <summary>
        /// => 플레이어의 Hp, Energy의 버블 게이지를 관리하는 메서드
        /// </summary>
        private void BubbleGaugeUpdate()
        {
            var actor = playerController.PlayerCharacter?.boActor;

            // -> 액터가 없다면
            if (actor == null)
            { 
                return;
            }

            var currentHp = actor.currentHp / actor.maxHp;
            var currentEnergy = actor.currentEnergy / actor.maxEnergy;

            hpBubbleGauge.SetGauge(currentHp);
            energyBubbleGauge.SetGauge(currentEnergy);
        }

        /// <summary>
        /// => 몬스터에게 달아줄 체력바를 생성하는 메서드
        ///    배틀매니저에 몬스터가 등록될때 생성해준다
        /// </summary>
        /// <param name="target"> 체력바를 달아줄 몬스터 </param>
        public void AddMonHpBar(Actor target)
        {
            var monHpBar = ObjectPoolManager.Instance.GetPool<MonHpBar>(Define.PoolType.MonHpBar).GetPoolableObject();
            monHpBar.transform.SetParent(worldCanvas.transform);
            monHpBar.Initialize(target);
            monHpBar.gameObject.SetActive(true);

            allMonHpBar.Add(monHpBar);
        }

        /// <summary>
        /// => 몬스터 체력바를 업데이트하는 메서드
        /// </summary>
        public void MonHpBarUpdate()
        {
            for (int i = 0; i < allMonHpBar.Count; i++)
            {
                allMonHpBar[i].MonHpBarUpdate();
            }
        }

        /// <summary>
        /// => 스테이지 전환 시 현재 스테이지에 있는 모든 체력바 객체를 풀에 반환하는 메서드
        /// </summary>
        public void Clear()
        {
            var monHpBarPool = ObjectPoolManager.Instance.GetPool<MonHpBar>(Define.PoolType.MonHpBar);

            for (int i = 0; i < allMonHpBar.Count; i++)
            {
                monHpBarPool.ReturnPoolableObject(allMonHpBar[i]);
            }

            // -> UIBattle의 몬스터 체력바 리스트를 청소 해줍니다!
            allMonHpBar.Clear();
        }
    }
}