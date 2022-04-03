using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.NetWork
{
    /// <summary>
    /// 통신에 사용되는 모든 데이터셋의 베이스 클래스
    /// </summary>
    [Serializable]
    public class DtoBase
    {
        [HideInInspector]
        public int errorCode;           // 통신 결과에 대한 에러코드

        [HideInInspector]               
        public string errorMessage;     // 에러 내용
    }

    /*
     *  DtoBase 클래스?
     *  서버와의 통신에 사용되는 데이터를 클래스로 전부 만들어 둘건데
     *  이 통신에 사용될 데이터를 Dto라고 표현 == Dto는 통신 데이터 이다
     *  통신에 사용되는 모든 데이터들은 DtoBase 라는 클래스에서 파생되어 만들어짐
     *  결과적으로 해당 클래스의 T 타입에는 통신에 사용되는 데이터들만 올 수 있음
     */

    /// <summary>
    /// 서버 통신 후 받은 데이터에 대한 처리를 일반화하여 수행할 클래스
    /// </summary>
    /// <typeparam name="T"> 통신에 사용되는 데이터들 </typeparam>
    public class ResponsHandler<T> where T : DtoBase
    {
        /// <summary>
        /// 응답 성공 시 호출될 델리게이트
        /// </summary>
        /// <param name="result"> 요청해서 받은 데이터 셋 </param>
        public delegate void OnSuccess(T result);

        /// <summary>
        /// 응답 실패 시 호출될 델리게이트
        /// </summary>
        /// <param name="error"> 에러 코드와 에러 메세지 </param>
        public delegate void OnFaild(DtoBase error);

        protected OnSuccess successDel;
        protected OnFaild failedDel;

        /// <summary>
        /// 생성 시 응답 성공 / 실패에 따른 델리게이트를 받아온다
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failed"></param>
        public ResponsHandler(OnSuccess success, OnFaild failed)
        {
            // 전달받은 응답 성공 / 실패에 따른 델리게이트를 저장
            successDel = success;
            failedDel = failed;
        }

        /// <summary>
        /// 서버에 데이터 요청 성공 시 응답 처리
        /// </summary>
        /// <param name="response"> Json 데이터 </param>
        public void HandleSuccess(string response)
        {
            T data = null;

            // 서버에서 받은 Json이 존재한다면
            if (response != null)
            {
                // DtoBase의 파생 클래스 ( Dto~ ) 타입으로 역직렬화 
                data = SerializationUtil.JsonToObject<T>(response);

                // 에러 코드가 존재한다면
                if (CheckFail(data))
                {
                    // 실패 시 응답 처리
                    HandleFailed(response);
                    return;
                }

                // 요청 성공 시 실행할 델리게이트가 존재 한다면 실행
                successDel?.Invoke(data);
            }
        }

        /// <summary>
        /// 서버에 데이터 요청 실패 시 에러메세지 처리 메서드
        /// </summary>
        /// <param name="response"></param>
        public void HandleFailed(string response)
        {
            // 서버에서 받은 Json 데이터를 에러코드, 메세지를 갖는 데이터 셋으로 변환
            DtoBase data = SerializationUtil.JsonToObject<T>(response);

            // 요청 실패에 따른 델리게이트가 있다면 실행
            failedDel?.Invoke(data);
        }

        /// <summary>
        /// 서버에 데이터 요청 시 응답 에러코드를 통한 에러 체크 메서드
        /// </summary>
        /// <param name="dtoBase"></param>
        /// <returns></returns>
        public bool CheckFail(T dtoBase)
        {
            return dtoBase.errorCode > 0;
        }
    }
}