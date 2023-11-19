using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;
using System.Threading;
using UnityEngine.SceneManagement;
using Unity.Networking.Transport;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

[RequireComponent(typeof(CharacterController))]
public class FPSController : NetworkBehaviour
{
    public static NetworkVariable<int> Trigger_Goal_Count = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    private string Local_PlayerName = "Null";
    public static bool isTriggerGoalAward = false;

    #region Public Variable 
    public static int PublicPlayerID = 219;
    public static bool isWalking;
    public static bool isRunning;
    public static bool isJumping;
    public static bool isAttacking;
    public static bool isFalling;
    public static bool isDead;
    #endregion
    #region Private Variable
    //public GameObject PlayerPrefab1;
    //public GameObject PlayerPrefab2;
    [SerializeField]
    private int LocalPlayerID = PublicPlayerID;
    [Header("Game Components")]
    //components
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private TouchField touchField;
    [SerializeField]
    private AudioSource footsteps;
    [SerializeField]
    private GameObject touchFieldPenel;
    [SerializeField]
    private GameObject JoystickControllerUI;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private CharacterController characterController;


    [Header("Game Variables")]
    //Varaible
    [SerializeField]
    private float walkSpeed = 15f;
    [SerializeField]
    private float runSpeed = 25f;
    [SerializeField]
    private float jumpPower = 15f;
    [SerializeField]
    private float gravity = 10f;
    [SerializeField]
    private float lookSpeed = 3.5f;
    [SerializeField]
    private float lookXLimit = 45f;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;
    [SerializeField]
    private float rotationX = 0;

    private enum InputType
    {
        Keyboard,
        Touch
    }
    [Header("Game Mode")]
    [SerializeField]
    private InputType currentInputType = InputType.Keyboard;
    private enum PlayMode
    {
        offline,
        online
    }
    [SerializeField]
    private PlayMode currentPlayMode = PlayMode.offline;

    #endregion
    public override void OnNetworkSpawn()
    {
        gameObject.transform.position = new Vector3(0f, 100f, 0f);
       if(!IsServer) Trigger_Goal_Count.OnValueChanged += TriggerOnValueChange;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        Cursor.visible = false;

        if (IsOwner)
        {
            //asign player name via input to local string
            Local_PlayerName = HomeManager.PlayerName;
        };
    }


    void Update()
    {
        if (currentPlayMode == PlayMode.online)
        {
            if(!Application.isFocused)
            {
                footsteps.enabled = false;
                joystick.enabled = false;
                JoystickControllerUI.SetActive(false);
            }
            if (IsLocalPlayer)
            {
                playerCamera.enabled = true;
                touchField.enabled = true;
                joystick.enabled = true;
                footsteps.enabled = true;
                touchFieldPenel.SetActive(true);
                JoystickControllerUI.SetActive(true);
            }
            else
            {
                playerCamera.enabled = false;
                touchField.enabled = false;
                joystick.enabled = false;
                footsteps.enabled = false;
                touchFieldPenel.SetActive(false);
                JoystickControllerUI.SetActive(false);
            }
            if (!IsOwner || !Application.isFocused) return;
        }
        if(!IsServer)
        {

        }


        //Method
        Movement();
        AnimationState();

    }
    #region Movement
    private void Movement()
    {
        #region Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        //Keyboard Controller Input
        if (currentInputType == InputType.Touch)
        {
            touchFieldPenel.SetActive(true);
            JoystickControllerUI.SetActive(true);
            //variable movement
            float speedX = joystick.Vertical * walkSpeed;
            float speedY = joystick.Horizontal * walkSpeed;
            moveDirection = (forward * speedX) + (right * speedY);

            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            //movement apply
            characterController.Move(moveDirection * Time.deltaTime);
            rotationX += -touchField.TouchDist.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, touchField.TouchDist.x * lookSpeed, 0);
        }
        //Joystick Controller Input
        else
        {
            touchFieldPenel.SetActive(false);
            JoystickControllerUI.SetActive(false);

            // Press Left Shift to run
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical");
            float curSpeedY = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal");
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && characterController.isGrounded)
            {
                moveDirection.y = jumpPower;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            characterController.Move(moveDirection * Time.deltaTime);

            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        #endregion
    }
    #endregion
    #region Animation State
    private void AnimationState()
    {
        if (currentInputType == InputType.Keyboard)
        {
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("IsWalking", true);
                footsteps.enabled = true;
            }
            else
            {
                animator.SetBool("IsWalking", false);
                footsteps.enabled = false;
            }
            if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool("IsWalkingBack", true);
            }
            else
            {
                animator.SetBool("IsWalkingBack", false);
            }
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            {
                animator.SetBool("IsRunning", true);
                footsteps.enabled = true;
            }
            else
            {
                animator.SetBool("IsRunning", false);
                footsteps.enabled = false;
            }
        }
        else
        {
            if (joystick.Vertical > 0)
            {
                animator.SetBool("IsWalking", true);
                footsteps.enabled = true;
            }
            else
            {
                animator.SetBool("IsWalking", false);
                footsteps.enabled = false;
            }
            if (joystick.Vertical < 0)
            {
                animator.SetBool("IsWalkingBack", true);
            }
            else
            {
                animator.SetBool("IsWalkingBack", false);
            }
            if (joystick.Vertical > 0)
            {
                animator.SetBool("IsRunning", true);
            }
            else
            {
                animator.SetBool("IsRunning", false);
            }
        }
    }
    #endregion
    private void OnTriggerEnter(Collider other)
    {
        if (IsOwner)
        {
            if (other.CompareTag("Goal_Award"))
            {
                if (!isTriggerGoalAward)
                {
                    //Trigger_Goal_Count.Value += 1;
                    //PlayerWinners.WinnerCountPlayer = Trigger_Goal_Count.Value;
                    //LobbyManager.Instance.UpdatePlayer();
                    isTriggerGoalAward = true;
                    //Debu
                    Debug.Log("Trigger Goal Log" + Trigger_Goal_Count.Value);
                }
            }
            //player dead
            if (other.CompareTag("Game Zone") && !IsHost)
            {
                NetworkManager.Singleton.Shutdown();
                SceneManager.LoadScene("Home");
            }
            if (other.CompareTag("Game Zone"))
            {
                if (IsHost)
                {
                    transform.position = new Vector3(0, 10f, 0);
                }
            }
        }
    }
    void TriggerOnValueChange(int previous, int current)
    {
        Trigger_Goal_Count.Value = current;
    }

}