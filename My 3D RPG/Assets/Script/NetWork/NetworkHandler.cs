using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.NetWork
{
    [Serializable]
    public class DtoBase
    {
        [HideInInspector]
        public int errorCode;
        [HideInInspector]
        public string errorMessage;
    }

    public class ResponsHandler<T> where T : DtoBase
    {
        public delegate void OnSuccess(T result);
        public delegate void OnFaild(DtoBase error);

        protected OnSuccess successDel;
        protected OnFaild failedDel;

        public ResponsHandler(OnSuccess success, OnFaild failed)
        {
            successDel = success;
            failedDel = failed;
        }

        public void HandleSuccess(string response)
        {
            T data = null;

            if (response != null)
            {
                data = SerializationUtil.JsonToObject<T>(response);

                if (CheckFail(data))
                {
                    HandleFailed(response);
                    return;
                }

                successDel?.Invoke(data);
            }
        }

        public bool CheckFail(T dtoBase)
        {
            return dtoBase.errorCode > 0;
        }

        public void HandleFailed(string response)
        {
            DtoBase data = SerializationUtil.JsonToObject<T>(response);
            failedDel?.Invoke(data);
        }
    }
}