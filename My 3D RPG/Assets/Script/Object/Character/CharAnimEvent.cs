using ProjectChan.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan
{
    /// <summary>
    /// => 캐릭터 클래스가 지닌 애니메이션 이벤트를 지닌 클래스
    /// </summary>
    public class CharAnimEvent : MonoBehaviour
    {
        // private
        private Character myParent;     // -> Character를 가진 부모 객체를 가져온다

        private  void Start()
        {
            myParent = transform.parent.GetComponent<Character>();
        }

        public void OnAttackHit()
        {
            myParent.OnAttackHit();
        }

        public void OnAttackEnd()
        {
            myParent.OnAttackEnd();
        }

        public void OnChangeWeapon()
        {
            myParent.OnChangeWeapon();
        }

        public void OnChangeWeaponEnd()
        {
            myParent.OnChangeWeaponEnd();
        }

        public void OnDeadEnd() 
        {
            myParent.OnDeadEnd();
        }
    }
}
