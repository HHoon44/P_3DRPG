using ProjectChan.NetWork;
using ProjectChan.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace ProjectChan.Dummy
{
    /// <summary>
    /// => 더미 서버를 지닌 클래스
    /// </summary>
    public class DummyServer : Singleton<DummyServer>
    {
        public INetworkClient dummyModule;      // -> 더미 서버가 가지고 있는 유저의 데이터
        public UserDataSo userData;             // -> ServerModuleFactory에서 생산되며 프로토콜 메서드를 사용할 수 있음

        public void Initialize()
        {
            // -> 프로토콜 함수가 선언 되어 있는 모듈더미를 사용하기 위해서 만들어 줍니다!
            dummyModule = new ServerModuleDummy(this);
        }

        /// <summary>
        /// => 유저의 Dto데이터를 저장하는 메서드
        /// </summary>
        public void Save()
        {
            /*  
             *  왜 비동기 방식으로 데이터를 저장하는가?
             *  유저의 데이터를 쓰는 과정에서 일반적인 메서드로 저장을 진행 했을 시
             *  동기화 상태에서 데이터를 쓰는 행위가 데이터가 클수록 시간이 오래걸림
             *  이 때 프레임이 일시적으로 떨어지고 화면이 잠시 멈추는 것처럼 보임
             *  따라서, 데이터를 쓰는 행위를 동기에서 비동기로 변경하기 위해서 코루틴으로 처리함
             *  (실제 비동기는 아니고 코루틴이므로 비동기처럼 보이게 하는 것 이다)
             */

            StartCoroutine(OnSaveProgress());

            /// => SaveAssets : 함수가 호출되면 더티 플래그가 선 오브젝트와 에셋 오브젝트인 경우에 에셋을 업데이트 한다
            /// => SetDirty   : 파라미터로 받은 오브젝트에 더티 플래그를 설정하여 디스크에 저장될 수 있도록 해주는 메서드
            /// => 더티 플래그 : 하나하나의 요소가 변경될 때 마다 저장하는 것은 너무 값이 비싸니 변경된 요소에 표시를 해놓고
            ///                 나중에 저장할 떄 마다 표시가 있는 것만 저장하는 작업

            IEnumerator OnSaveProgress()
            {
                EditorUtility.SetDirty(userData);
                AssetDatabase.SaveAssets();

                yield return null;
            }
        }
    }
}
