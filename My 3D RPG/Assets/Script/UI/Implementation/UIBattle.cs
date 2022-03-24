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
    /// <summary>
    /// 인 게임 플레이를 할 때 사용되는 UI를 관리하는 클래스
    /// </summary>
    public class UIBattle : UIWindow
    {
        // public 
        public PlayerController playerController;       // 플레이어 컨트롤러
        public BubbleGauge hpBubbleGauge;               // Hp 게이지 컴포넌트
        public BubbleGauge energyBubbleGauge;           // Energy 게이지 컴포넌트
        public Canvas worldCanvas;                      // 월드 컨버스 객체
        public Texture2D normalCursor;                  // 기본 마우스 커서 이미지
        public Texture2D targetPointCursor;             // 몬스터 타겟팅 커서 이미지

        /// <summary>
        /// 필드 위의 몬스터들이 지닌 체력바
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
        /// 월드 컨버스의 하위 객체들이 플레이어를 바라 보도록 설정하는 메서드
        /// </summary>
        public void BillboardUpdate()
        {
            if (worldCanvas == null)
            {
                return;
            }

            // 플레이어를 타겟으로 하는 카메라의 Transform을 가져온다
            var camTrans = CameraController.Cam.transform;

            // 하위 객체들이 카메라를 바라보도록 하는 작업
            for (int i = 0; i < worldCanvas.transform.childCount; i++)
            {
                var child = worldCanvas.transform.GetChild(i);

                /// LookAt : 지정된 오브젝트들이 파라미터로 들어간 Target을 바라보게 해줌
                child.LookAt(camTrans, Vector3.up);

                /// 표류하지 않거나 의도하지 않은 회전을 일으킬 수 있기때문에 직접적으로 바꾸지 않음 
                var newRot = child.eulerAngles;
                newRot.x = 0;
                newRot.z = 0;
                child.eulerAngles = newRot;
            }
        }

        /// <summary>
        /// 플레이어의 현재 체력/기력을 게이지에 업데이트하는 메서드
        /// </summary>
        private void BubbleGaugeUpdate()
        {
            var actor = playerController.PlayerCharacter?.boActor;

            // 캐릭터가 없다면
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
        /// 몬스터에게 체력바를 달아주는 메서드
        /// </summary>
        /// <param name="target"> 체력바를 달아줄 몬스터 </param>
        public void AddMonHpBar(Actor target)
        {
            // 풀에서 사용가능한 몬스터 체력바를 가져온다
            var monHpBar = ObjectPoolManager.Instance.GetPool<MonHpBar>
                (Define.PoolType.MonHpBar).GetPoolableObject(obj => obj.CanRecycle);

            monHpBar.transform.SetParent(worldCanvas.transform);
            monHpBar.Initialize(target);
            monHpBar.gameObject.SetActive(true);

            // 리스트에 넣어둔다
            allMonHpBar.Add(monHpBar);
        }

        /// <summary>
        /// 인 게임 커서를 상황에 맞게 업데이트 해주는 메서드
        /// </summary>
        private void PlayerCursorUpdate()
        {
            Cursor.SetCursor
                (playerController.HasPointTarget ? targetPointCursor : normalCursor, new Vector2(20, 1), CursorMode.Auto);
        }

        /// <summary>
        /// 몬스터의 현재 체력을 체력바에 업데이트 하는 메서드
        /// </summary>
        public void MonHpBarUpdate()
        {
            for (int i = 0; i < allMonHpBar.Count; i++)
            {
                allMonHpBar[i].MonHpBarUpdate();
            }
        }

        /// <summary>
        /// 스테이지 전환 시, 현재 스테이지에 존재하는 체력바 객체를 풀에 반환하는 메서드
        /// </summary>
        public void Clear()
        {
            // 몬스터 체력바 풀을 가져온다
            var monHpBarPool = ObjectPoolManager.Instance.GetPool<MonHpBar>(Define.PoolType.MonHpBar);

            // 풀에 체력바를 반환하는 작업
            for (int i = 0; i < allMonHpBar.Count; i++)
            {
                monHpBarPool.ReturnPoolableObject(allMonHpBar[i]);
            }

            // 몬스터 체력바 담겨있었던 리스트를 청소한다
            allMonHpBar.Clear();
        }
    }
}