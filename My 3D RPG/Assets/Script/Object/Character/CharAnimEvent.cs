using ProjectChan.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan
{
    public class CharAnimEvent : MonoBehaviour
    {
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

        public void OnJumpStart()
        {

        }

        public void OnJumpEnd()
        {
        }

        public void OnDeadEnd() 
        {
            myParent.OnDeadEnd();
        }
        
        public void OnChangeWeapon()
        {
            myParent.OnChangeWeapon();
        }
    }
}
