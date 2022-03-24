using ProjectChan.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Object
{
    using CamView = Define.Camera.CamView;

    /// <summary>
    /// 인 게임 카메라를 컨트롤 하는 클래스
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        // public
        public CamView camView;         // 현재 카메라 뷰

        // private
        private Transform standardPos;  // 기본 카메라 포지션
        private Transform frontPos;     // 정면 카메라 포지션
        private Transform target;       // 카메라가 따라 다닐 타겟

        /// <summary>
        /// 씬에 존재하는 카메라
        /// </summary>
        public static Camera Cam { get; private set; }

        private void Start()
        {
            Cam = GetComponent<Camera>();
        }

        private void FixedUpdate()
        {
            // 조종 타겟이 없다면
            if (target == null)
            {
                return;
            }

            // CamView에 따라 캠 각도를 설정
            switch (camView)
            {
                case CamView.Standard:
                    SetPosition(standardPos);
                    break;

                case CamView.Front:
                    SetPosition(frontPos);
                    break;
            }
        }

        /// <summary>
        /// 요청에 따라 카메라의 위치를 변경하는 메서드
        /// </summary>
        /// <param name="viewPos"> 카메라 Pos </param>
        private void SetPosition(Transform viewPos)
        {
            transform.position = viewPos.position;
            transform.forward = viewPos.forward;
        }

        /// <summary>
        /// 카메라가 따라 다닐 타겟을 설정하는 메서드
        /// </summary>
        /// <param name="target"> 타겟 </param>
        public void SetTarget(Transform target)
        {
            this.target = target;

            // 리소스 폴더에 있는 CamPos 프리팹을 가져옴
            var camPos = Instantiate(ResourceManager.Instance.LoadObject(Define.Camera.CamPosPath)).transform;

            camPos.parent = this.target.transform;
            camPos.localPosition = Vector3.zero;

            standardPos = camPos.Find("StandardPos");
            frontPos = camPos.Find("FrontPos");

            // 기본 카메라 포지션으로 설정
            transform.position = standardPos.position;
            transform.forward = standardPos.forward;
        }

        /// <summary>
        /// 기본 카메라로 설정하는 메서드
        /// </summary>
        public void SetForceStandarView()
        {
            SetPosition(standardPos);
        }
    }
}