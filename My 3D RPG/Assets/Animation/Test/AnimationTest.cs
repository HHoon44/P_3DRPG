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
    /// ���� �ִϸ��̼�
    /// 1. �޸��鼭 ���ݵ� �����ϵ��� Layer�� ������ ����ؾ��ҵ�
    /// 
    /// ���� �ִϸ��̼�
    /// 1. ���� �����ؾ��� ���� ȭ����~
    /// 
    /// �߰�
    /// 1. ��Ż -> Ʈ�� �޸�..
    /// 
    /// ��
    /// 1. ���͸� ���̰� ���Ͱ� �ٽ� �¾�� ������ ������ �ȵ�
    /// 
    /// ������ �ϵ� ���ν�Ƽ ���� ������ �ɶ� ���� �ִϸ��̼��� ���� ��Ŵ 
    /// ��� �� ���� ������Ʈ���� ���� �ٽ� �ִϸ��̼��� ������� �ʵ��� 
    /// ���� ����������� �ٽ� ������� �ʵ���
    /// </summary>
}