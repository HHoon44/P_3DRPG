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
    /// => 통신에 사용되는 모든 데이터셋의 베이스 클래스
    /// </summary>
    [Serializable]
    public class DtoBase
    {
        [HideInInspector]
        public int errorCode;           // -> 통신 결과에 대한 에러코드

        [HideInInspector]               
        public string errorMessage;     // -> 에러에 대한 내용
    }

    /// <summary>
    /// => 서버 통신 후 받는 데이터에 관한 처리를 일반화한 다음 수행할 클래스
    /// </summary>
    /// <typeparam name="T"> 통신에 사용되는 데이터들 </typeparam>
    public class ResponsHandler<T> where T : DtoBase
    {
        /// <summary>
        /// => 응답 성공 시 호출될 델리게이트
        /// </summary>
        /// <param name="result"> 요청해서 받은 데이터 셋 </param>
        public delegate void OnSuccess(T result);

        /// <summary>
        /// => 응답 실패 시 호출될 델리게이트
        /// </summary>
        /// <param name="error"> 에러 코드와 에러 메세지 </param>
        public delegate void OnFaild(DtoBase error);

        protected OnSuccess successDel;
        protected OnFaild failedDel;

        /// <summary>
        /// => ResponsHandler 생성자
        /// => 생성 시 응답 성공, 실패 시 응답 실패에 따른 델리게이트를 받아온다
        /// </summary>
        /// <param name="success"> 성공 시 실행할 메서드 </param>
        /// <param name="failed"> 실패 시 실행할 메서드 </param>
        public ResponsHandler(OnSuccess success, OnFaild failed)
        {
            successDel = success;
            failedDel = failed;
        }

        /// <summary>
        /// => 서버에 데이터 요청 성공 시 응답 처리
        /// </summary>
        /// <param name="response"> Json 데이터 </param>
        public void HandleSuccess(string response)
        {
            T data = null;

            // -> 서버에서 받은 Json이 존재한다면!
            if (response != null)
            {
                // -> Json을 임의 T타입으로 변환(역직렬화) 합니다!
                // -> EX: DtoAccount, DtoCharacter등등..
                data = SerializationUtil.JsonToObject<T>(response);

                // -> 에러 코드가 존재한다면!
                if (CheckFail(data))
                {
                    // -> 에러 코드가 존재하므로 실패 입니다!
                    HandleFailed(response);
                    return;
                }

                // -> 요청 성공에 따른 성공 시 델리게이트가 존재 한다면 실행합니다!
                successDel?.Invoke(data);
            }
        }

        /// <summary>
        /// => 서버에 데이터 요청 시 응답 에러코드를 통한 에러 체크 메서드
        /// </summary>
        /// <param name="dtoBase"></param>
        /// <returns></returns>
        public bool CheckFail(T dtoBase)
        {
            return dtoBase.errorCode > 0;
        }

        /// <summary>
        /// => 서버에 데이터 요청 실패 시 에러메세지 처리 메서드
        /// </summary>
        /// <param name="response"></param>
        public void HandleFailed(string response)
        {
            // -> Json 파일을 다시 T 타입 데이터로 변형
            DtoBase data = SerializationUtil.JsonToObject<T>(response);

            // -> 요청 실패에 따른 실패 시 델리게이트가 존재 한다면 실행
            failedDel?.Invoke(data);
        }
    }
}