using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    public float jumpForce;
    public bool isGround;

    private Animator anim;
    private Rigidbody rigid;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, .05f, 1 << LayerMask.NameToLayer("Floor"));
        Debug.DrawRay(transform.position, Vector3.down * .05f, Color.red);

        if (Input.GetButtonDown("Jump"))
        {
            anim.SetBool("isJump", true);
            rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// 공격 애니메이션
    /// 1. 달리면서 공격도 가능하도록 Layer을 나누어 사용해야할듯
    /// 
    /// 점프 애니메이션
    /// 1. 점프 수정해야함 리얼 화난다~
    /// 
    /// 추가
    /// 1. 포탈 -> 트럭 메모..
    /// 
    /// 음
    /// 1. 몬스터를 죽이고 몬스터가 다시 태어나면 목적지 생성이 안됨
    /// 
    /// 점프를 하되 벨로시티 값이 음수가 될때 착지 애니메이션을 실행 시킴 
    /// 대신 또 점프 업데이트문이 돌때 다시 애니메이션이 실행되지 않도록 
    /// 땅에 닿았을때까지 다시 실행되지 않도록
    /// </summary>
}