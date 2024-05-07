using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public enum PlayerStateName
{
    IDLE, WALK, ASSASING,DEAD
}

public enum PlayerAttackType
{
    NOMAL,ASSASING,AIMMING
}


public class PlayerController : Singleton<PlayerController>
{
    public float moveSpeed;
    public int hp = 100;
    public bool isAssasing;
    public bool isGround = true;
    public CinemachineVirtualCamera overView;
    public CinemachineVirtualCamera aimView;
    public GameObject aim;
    public GameObject gun;
    public GameObject cross;
    public GameObject skin;
    public GameObject gunSkin;

    public GameObject sss;

    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public Animator playerAnimator;
    public PlayerStateName playerState = PlayerStateName.IDLE;
    public GameObject assasingPos;
    protected CharacterController characterController;

    [SerializeField]
    private GameObject leftHandAttackPos;
    [SerializeField]
    private GameObject rightHandAttackPos;

    private StateMachine playerStateMachine;
    private Vector3 moveDirection;
    public Vector3 jumpDirection = new Vector3(0, 0, 0);
    private PlayerAttackType attackType = PlayerAttackType.NOMAL;
    private Transform aaa;

    private float jumpForce = 3.0f;
    private float gravtyScale = -0.02f;
    private float attackTime;
    private float maxComboInputTime = 0.5f;
    private float attakingTime = 3.0f;
    private float rootSpeed = 3.0f;
    private float yRotate;
    private float xRotate;
    private int comboCount = 0;
    private bool isCrouching = false;
    public bool isJump = false;
    private bool isAttack = false;
    private bool isAimming = false;
    private bool isAssasingAttack = false;

    public GameObject Gun;
    private GunController gunController;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
        playerStateMachine = gameObject.AddComponent<StateMachine>();
        overView.Priority = 10;
        aimView.Priority = 0;
        playerStateMachine.AddState(PlayerStateName.IDLE, new IdleState(this));
        playerStateMachine.AddState(PlayerStateName.WALK, new WalkState(this));
        playerStateMachine.AddState(PlayerStateName.DEAD, new DeadState(this));
        playerStateMachine.InitState(PlayerStateName.IDLE);
        aaa = playerAnimator.GetBoneTransform(HumanBodyBones.Spine);
        leftHandAttackPos.SetActive(false);
        rightHandAttackPos.SetActive(false);
        gun.SetActive(false);
        cross.SetActive(false);
        jumpDirection.y = jumpForce;

        gunController = Gun.GetComponent<GunController>();
    }

    private void LateUpdate()
    {
        if (isAimming)
        {
            //aaa.rotation = Quaternion.Euler(0f, 0, yRotate * rootSpeed);
           // gun.transform.Rotate(yRotate * rootSpeed, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState != PlayerStateName.DEAD)
        {
            if (!isGround)
            {
                jumpDirection.y += gravtyScale;
                characterController.Move(jumpDirection * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.K))
            {
                hp = 0;
            }


            AttackingTimeCheck();
            MoveAssasingTarget();
            PlayerRotate();
            if (isAimming)
            {
                //aaa.rotation = Quaternion.Euler(0f, 0, yRotate * rootSpeed);
                //transform.Rotate(0f, Input.GetAxis("Mouse X") * rootSpeed, 0f);
                gun.transform.Rotate(yRotate, 0f, 0f);
            }
            //aimView.LookAt = aim.transform;

            if (hp <= 0)
            {
                playerStateMachine.ChangeState(PlayerStateName.DEAD);
            }
        }
        
    }

    private void OnAimming()
    {
        if(playerState != PlayerStateName.DEAD)
        {
            if (isAimming)
            {
                gun.SetActive(false);
                cross.SetActive(false);
                gunSkin.SetActive(true);
                skin.GetComponent<SkinnedMeshRenderer>().enabled = true;
                overView.Priority = 10;
                aimView.Priority = 0;
                // overView.gameObject.SetActive(true);
                // aimView.gameObject.SetActive(false);
                attackType = PlayerAttackType.NOMAL;
                isAimming = false;
            }
            else
            {
                gun.SetActive(true);
                gunSkin.SetActive(false);
                cross.SetActive(true);
                skin.GetComponent<SkinnedMeshRenderer>().enabled = false;
                attackType = PlayerAttackType.AIMMING;
                overView.Priority = 0;
                aimView.Priority = 10;
                //aimView.gameObject.SetActive(true);
                // overView.gameObject.SetActive(false);
                isAimming = true;
            }
        }
        
    }

    private void OnDash()
    {
        if (!isAssasingAttack)
        {
            if (moveSpeed == 1)
            {
                moveSpeed = 3;
            }
            else if (moveSpeed == 3)
            {
                moveSpeed = 1;
            }
        }
        
    }

    private void AimmingAttack()
    {
        attackTime = Time.time;
        GameManger.Instance.isBattle = true;
        isAttack = true;
        gun.GetComponent<GunController>().TryFire();
    }
     public void Hit(int damge)
    {
        hp -= damge;
    }

    private void PlayerRotate()
    {
        transform.Rotate(0f, Input.GetAxis("Mouse X") * rootSpeed, 0f);
        gun.transform.rotation = transform.rotation;
        yRotate += -Input.GetAxis("Mouse Y") * rootSpeed;
        yRotate = Mathf.Clamp(yRotate, -60, 10);
        


    }

    public void OnReload()
    {
        if(!gunController.isReload && gunController.gun.currentBulletCount < gunController.gun.reloadBulletCount)
        playerAnimator.SetBool("Reload", true);
    }

    public void ReloadEnd()
    {
        gunController.Reload();
        playerAnimator.SetBool("Reload", false);
    }

    private void PlayerYRoatate()
    {

    }

    void OnCrouching()
    {
        if (!isAssasingAttack)
        {
            if (!isCrouching)
            {
                isCrouching = true;
                moveSpeed = 1.0f;
                //�ִϼ���
                playerAnimator.SetBool("IsCrouching", true);
                //�ɱ��� ��� 0, 0.49 ,0
                characterController.center = new Vector3(0, 0.49f, 0);
                //���� 1
                characterController.height = 1;

            }
            else
            {
                isCrouching = false;
                moveSpeed = 1.0f;
                //�ִϼ���
                playerAnimator.SetBool("IsCrouching", false);
                //ĳ���� ��Ʈ�ѷ� �ݸ��� ���� ���Ͱ� 0 , 0.99 ,0
                characterController.center = new Vector3(0, 0.99f, 0);
                //���� 1.8
                characterController.height = 1.8f;
            }

        }



    }

    private void OnAssasing()
    {
        if (isAssasing&&!isAttack)
        {
            attackType = PlayerAttackType.ASSASING;
            isCrouching = false;
            moveSpeed = 1.0f;
            //�ִϼ���
            //playerAnimator.SetLayerWeight(1, 1);
            playerAnimator.SetBool("IsCrouching", false);
            playerAnimator.SetTrigger("AssasingAttackStart");
            //ĳ���� ��Ʈ�ѷ� �ݸ��� ���� ���Ͱ� 0 , 0.99 ,0
            characterController.center = new Vector3(0, 0.99f, 0);
            //���� 1.8
            characterController.height = 1.8f;
            target.gameObject.GetComponent<CapsuleCollider>().isTrigger = true;

            target.gameObject.GetComponent<Monster>().isAmbushed = true;
        }
        
    }

    private void OnAttack()
    {

        switch (attackType)
        {
            case PlayerAttackType.NOMAL:
                NomalAttack();
                break;
            case PlayerAttackType.ASSASING:
                AssaingAttack();
                break;
            case PlayerAttackType.AIMMING:
                AimmingAttack();
                break;
        }

    }

    public void SetAssingAttackingAni()
    {
        playerAnimator.SetTrigger("AssasinAttacking");
    }
    private void MoveAssasingTarget()
    {
        if (attackType == PlayerAttackType.ASSASING)
        {
            target.transform.position = assasingPos.transform.position;
            target.transform.rotation = assasingPos.transform.rotation;
        }
    }

    private void AssaingAttack()
    {
        playerAnimator.SetTrigger("AssasingAttackFinish");
        target.GetComponent<Monster>().Hurt(100);
        target.GetComponent<Monster>().isAmbushed = false;
        attackType = PlayerAttackType.NOMAL;

        PlayerController.Instance.isAssasing = false;
        PlayerController.Instance.target = null;
    }

    private void NomalAttack()
    {
        isAttack = true;
       // playerAnimator.SetLayerWeight(1, 1);
        playerAnimator.SetBool("IsAttack", true);
        attackTime = Time.time;

        if (Time.time - attackTime <= maxComboInputTime)
        {
            if (comboCount == 0)
            {
                playerAnimator.SetTrigger("LeftAttack");
                comboCount++;
            }
            else if (comboCount == 1)
            {
                playerAnimator.SetTrigger("RightAttack");
                comboCount--;
            }
        }
    }

    public void LeftAttack()
    {
        if (leftHandAttackPos.activeSelf == false)
        {
            leftHandAttackPos.SetActive(true);
        }
        else
        {
            leftHandAttackPos.SetActive(false);
        }
    }

    public void RightAttack()
    {
        if (rightHandAttackPos.activeSelf == false)
        {
            rightHandAttackPos.SetActive(true);
        }
        else
        {
            rightHandAttackPos.SetActive(false);
        }
    }

    private void AttackingTimeCheck()
    {
        if (Time.time - attackTime > attakingTime)
        {
            isAttack = false;
            comboCount = 0;
            hp = 100;
            playerAnimator.SetBool("IsAttack", false);
            //playerAnimator.SetLayerWeight(1, 0);
        }

    }

    private void OnJump()
    {
        if (isGround && !isAssasing&&!isJump)
        {
            jumpDirection.y = jumpForce;
            isGround = false;
            isJump = true;
            playerAnimator.SetBool("IsGround", isGround);
        }

    }

    private void OnMove(InputValue input)
    {
        if(!isAssasingAttack)
        {
            Vector2 moveVector = input.Get<Vector2>();
            moveDirection = new Vector3(moveVector.x, 0, moveVector.y);
            playerStateMachine.ChangeState(PlayerStateName.WALK);
        }
        
    }

    private void SetAnimatorFloat(float Xvalue, float Zvalue)
    {
        playerAnimator.SetFloat("Xspeed", Xvalue);
        playerAnimator.SetFloat("Zspeed", Zvalue);
    }


    private class PlayerBaseState : BaseState
    {
        protected PlayerController player;

        public PlayerBaseState(PlayerController player)
        {
            this.player = player;
        }
    }



    private class IdleState : PlayerBaseState
    {
        public IdleState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            player.playerState = PlayerStateName.IDLE;
            //�ִϸ��̼� ����
            player.SetAnimatorFloat(0, 0);
            
            
        }

        public override void Update()
        {
        }
    }

    private class WalkState : PlayerBaseState
    {
        public WalkState(PlayerController player) : base(player) { }

        public override void Enter()
        {
        }

        public override void FixedUpdate()
        {
        }

        public override void Update()
        {
            if(player.playerState != PlayerStateName.DEAD)
            {
                player.characterController.Move(player.transform.TransformDirection(player.moveDirection) * player.moveSpeed * Time.deltaTime);
                if (player.isGround)
                {
                    player.SetAnimatorFloat(player.moveDirection.x * player.moveSpeed, player.moveDirection.z * player.moveSpeed);

                }


                if (player.characterController.velocity.magnitude == 0)
                {
                    player.playerStateMachine.ChangeState(PlayerStateName.IDLE);
                }
            }
           
        }
    }
    private class DeadState : PlayerBaseState
    {
        public DeadState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            player.gun.SetActive(true);
            player.gunSkin.SetActive(false);
            player.cross.SetActive(true);
            player.skin.GetComponent<SkinnedMeshRenderer>().enabled = false;

            player.playerState = PlayerStateName.DEAD;
            player.playerAnimator.SetTrigger("isDead");
            //�ִϸ��̼� ����

            player.isAimming = false;
        }

        public override void Update()
        {
        }
    }

}
