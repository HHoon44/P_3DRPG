using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Object
{
    public class Weapon : MonoBehaviour
    {
        public SDWeapon sdWeapon { get; private set; }      // -> 무기 데이터

        public void Initialize(SDWeapon sdWeapon )
        {
            this.sdWeapon = sdWeapon;
        }
    }
}