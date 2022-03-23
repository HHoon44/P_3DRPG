using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.Util
{
    /// <summary>
    /// 오브젝트 풀링을 사용하는 클래스에서 구현해야하는 인터페이스
    /// 오브젝트 풀링을 사용할 객체는 해당인터페이스를 상속 받아야만 오브젝트 풀링을 사용할 수 있다
    /// </summary>
    public interface IPoolableObject
    {
        /// <summary>
        /// 오브젝트가 재사용 될 수 있음을 나타내는 프로퍼티
        /// 오브젝트 풀에서 꺼내서 사용 할 수 있는 상태인지를 나타내는 필드
        /// </summary>
        bool CanRecycle { get; set; }
    }
}