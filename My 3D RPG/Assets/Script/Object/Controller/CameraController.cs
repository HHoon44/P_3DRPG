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

    public class CameraController : MonoBehaviour
    {
        public CamView camView;

        public float smooth = 3f;

        private Transform standardPos;
        private Transform frontPos;
        private Transform target;   /// 따라 다닐 타겟

        public static Camera Cam { get; private set; }

        private void Start()
        {
            Cam = GetComponent<Camera>();
        }

        private void FixedUpdate()
        {
            if (target == null)
            {
                return;
            }

            switch (camView)
            {
                case CamView.Standard:
                    SetPosition(true, standardPos);
                    break;

                case CamView.Front:
                    SetPosition(false, frontPos);
                    break;
            }
        }

        public void SetForceStandarView()
        {
            SetPosition(false, standardPos);
        }

        /// PlayerController에서 호출됨
        public void SetTarget(Transform target)
        {
            this.target = target;

            var camPos = Instantiate(ResourceManager.Instance.LoadObject(Define.Camera.CamPosPath)).transform;

            camPos.parent = this.target.transform;
            camPos.localPosition = Vector3.zero;

            standardPos = camPos.Find("StandardPos");
            frontPos = camPos.Find("FrontPos");

            /// StandardPos의 Position으로 설정
            transform.position = standardPos.position;
            transform.forward = standardPos.forward;
        }

        private void SetPosition(bool isLerp, Transform viewPos)
        {
            if (isLerp)
            {
                transform.position = Vector3.Lerp(transform.position, viewPos.position, Time.fixedDeltaTime * smooth);
                transform.forward = Vector3.Lerp(transform.forward, viewPos.forward, Time.fixedDeltaTime * smooth);
            }
            else
            {
                transform.position = viewPos.position;
                transform.forward = viewPos.forward;
            }
        }

    }
}