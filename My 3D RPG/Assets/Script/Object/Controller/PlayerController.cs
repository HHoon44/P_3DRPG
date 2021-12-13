using ProjectChan.Dummy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Object
{
    using static ProjectChan.Define.Actor;
    using CamView = Define.Camera.CamView;
    using Input = Define.Input;

    public class PlayerController : MonoBehaviour
    {
        private Dictionary<string, InputHandler.AxisHandler> inputAxisDic;
        private Dictionary<string, InputHandler.ButtonHandler> inputButtonDic;

        public Character PlayerCharacter { get; private set; }
        public CameraController cameraController;
        private Transform pointingTarget;
        public bool isPlayerAction { get; set; }                     // -> 현재 플레이어가 행동을 취하는 중인가?

        public bool HasPointTarget { get; private set; }
        private bool canRot;

        public void Initialize(Character character)
        {
            character.playerController = transform.GetComponent<PlayerController>();
            character.transform.parent = transform;
            character.gameObject.layer = LayerMask.NameToLayer("Player");

            PlayerCharacter = character;
            cameraController.SetTarget(PlayerCharacter.transform);

            // 축 관리
            inputAxisDic = new Dictionary<string, InputHandler.AxisHandler>();
            inputAxisDic.Add(Input.AxisX, new InputHandler.AxisHandler(GetAxisX));
            inputAxisDic.Add(Input.AxisZ, new InputHandler.AxisHandler(GetAxisZ));
            inputAxisDic.Add(Input.MouseX, new InputHandler.AxisHandler(GetMouseX));
            inputAxisDic.Add(Input.MouseY, new InputHandler.AxisHandler(GetMouseY));

            // 버튼, 마우스 관리
            inputButtonDic = new Dictionary<string, InputHandler.ButtonHandler>();
            inputButtonDic.Add(Input.MouseLeft, new InputHandler.ButtonHandler(OnPressMouseLeft, null));
            inputButtonDic.Add(Input.MouseRight, new InputHandler.ButtonHandler(OnPressMouseRight, OnNotMouseRight));
            inputButtonDic.Add(Input.FrontCam, new InputHandler.ButtonHandler(OnPressFrontCam, OnNotPressFrontCam));
        }

        private void FixedUpdate()
        {
            // -> 컨트롤러에 플레이어가 없다면
            if (PlayerCharacter == null)
            {
                return;
            }

            // -> 플레이어가 죽은 상태라면
            if (PlayerCharacter.State == Define.Actor.ActorState.Dead)
            {
                return;
            }

            // -> 플레이어가 상호작용중이라면
            if (isPlayerAction)
            {
                return;
            }

            InputUpdate();
            CheckMousePointTarget();
        }

        /// <summary>
        /// => 키 입력 함수들을 실행
        /// </summary>
        private void InputUpdate()
        {
            foreach (var input in inputAxisDic)
            {
                // -> Vertical을 누르면 여기서 반응
                var value = UnityEngine.Input.GetAxis(input.Key);

                // -> GetAxisZ(value)이렇게 값을 전달
                input.Value.GetAxisValue(value);
            }

            foreach (var input in inputButtonDic)
            {
                if (UnityEngine.Input.GetButton(input.Key))
                {
                    input.Value.OnPress();
                }
                else
                {
                    input.Value.OnNotPress();
                }
            }
        }

        /// <summary>
        /// => 마우스 포인터를 기준으로 레이를 사용하여 타겟을 찾아내는 메서드
        /// </summary>
        private void CheckMousePointTarget()
        {
            var ray = CameraController.Cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 1000f, 1 << LayerMask.NameToLayer("Monster"));

            // -> hits의 길이가 0이 아니라면, 타겟이 존재함
            // -> HasPointTarget이 True라면 hits에서 제일 첫번째 녀석의 Transform값을 가져옴
            HasPointTarget = hits.Length != 0;
            pointingTarget = HasPointTarget ? hits[0].transform : null;
        }

        #region 입력 구현

        private void GetAxisX(float value)
        {

        }

        private void GetAxisZ(float value)     
        {
            var boActor = PlayerCharacter.boActor;
            var newDir = boActor.moveDir;
            var sprint = value;
            var amount = boActor.currentEnergy / boActor.maxEnergy;

            // fillAmount 값이 0보다 커야 돌아가도록 하고
            if (UnityEngine.Input.GetAxis(Input.Sprint) > 0 && amount > 0)
            {
                // -> 저 달리는 중
                PlayerCharacter.isRun = true;
                SetSprint(UnityEngine.Input.GetAxis(Input.Sprint));
            }
            else
            {
                // -> 저 안달립니다
                PlayerCharacter.isRun = false;
                SetSprint();
            }

            // -> 로컬함수
            void SetSprint(float value = 0)
            {
                sprint += value * .5f;
                newDir.z = sprint;
                PlayerCharacter.boActor.moveDir = newDir;
            }
        }

        private void GetMouseX(float value)     // -> Y축 회전
        {
            var newDir = PlayerCharacter.boActor.rotDir;
            newDir.y = canRot ? value : 0;
            PlayerCharacter.boActor.rotDir = newDir;
        }

        private void GetMouseY(float value)
        {

        }

        private void OnPressFrontCam()      // -> 캠 회전
        {
            cameraController.camView = CamView.Front;
        }

        private void OnNotPressFrontCam()   // -> 베이스 캠
        {
            cameraController.camView = CamView.Standard;
        }

        private void OnPressMouseLeft()     // -> 공격 키
        {
            if (pointingTarget != null)
            {
                // -> Y축 회전만 실행하기 위한 작업
                var originRot = PlayerCharacter.transform.eulerAngles;

                PlayerCharacter.transform.LookAt(pointingTarget);

                var newRot = PlayerCharacter.transform.eulerAngles;
                newRot.x = originRot.x;
                newRot.z = originRot.z;

                PlayerCharacter.transform.eulerAngles = newRot;
            }

            PlayerCharacter.SetState(ActorState.Attack);
        }

        private void OnPressMouseRight()    // -> 마우스 왼쪽 클릭
        {
            canRot = true;
        }

        private void OnNotMouseRight()
        {
            canRot = false;
        }

        #endregion

        /// <summary>
        /// => 유저가 게임을 종료 했을 시 호출되는 메서드
        /// </summary>
        private void OnApplicationQuit()
        {
            // -> 플레이어의 현재 위치를 가져온다
            var playerPos = PlayerCharacter.transform.position;

            // -> 플레이어의 위치를 담아놓을 DtoStage 데이터를 가져온다
            var dtoStage = DummyServer.Instance.userData.dtoStage;
            dtoStage.lastStageIndex = GameManager.User.boStage.sdStage.index;
            dtoStage.lastPosX = playerPos.x;
            dtoStage.lastPosY = playerPos.y;
            dtoStage.lastPosZ = playerPos.z;

            DummyServer.Instance.Save();
        }

        private class InputHandler
        {
            // 축 타입의 키 입력 시, 실행할 메서드를 대리할 델리게이트
            public delegate void InputAxisDel(float value);
            // 버튼 타입의 키 입력 시, 실행할 메서드를 대리할 델리게이트
            public delegate void InputButtonDel();

            public class AxisHandler
            {
                private InputAxisDel axisDel;

                public AxisHandler(InputAxisDel axisDel)
                {
                    this.axisDel = axisDel;
                }

                public void GetAxisValue(float value)
                {
                    axisDel.Invoke(value);
                }
            }

            public class ButtonHandler
            {
                private InputButtonDel pressDel;
                private InputButtonDel notPressDel;

                public ButtonHandler(InputButtonDel pressDel, InputButtonDel notPressDel)
                {
                    this.pressDel = pressDel;
                    this.notPressDel = notPressDel;
                }

                public void OnPress()
                {
                    pressDel?.Invoke();
                }

                public void OnNotPress()
                {
                    notPressDel?.Invoke();
                }
            }
        }
    }
}