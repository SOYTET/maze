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

    #region Public Variable 
    public static bool isDead;
    #endregion
    #region Private Variable
    [Header("Game Components")]
    //components
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private TouchField touchField;
    //[SerializeField]
    //private AudioSource footsteps;
    [SerializeField]
    private GameObject touchFieldPenel;
    [SerializeField]
    private GameObject JoystickControllerUI;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private CharacterController characterController;

    [SerializeField]
    private GameObject Local_Hide;



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

    //booolean
    public static bool isTriggerGoalAward = false;
    public static bool isBadMan = false;
    public static bool isDoor = false;
    public static bool box = false;



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
        gameObject.transform.position = new Vector3(Random.Range(-10, 10), 5f, Random.Range(-10, 10));
       if(!IsServer) Trigger_Goal_Count.OnValueChanged += TriggerOnValueChange;

        if (IsLocalPlayer)
        {
            Local_Hide.SetActive(false);
        }

    }

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        Cursor.visible = false;


    }


    void Update()
    {
        if (currentPlayMode == PlayMode.online)
        {
            if(!Application.isFocused)
            {
                joystick.enabled = false;
                JoystickControllerUI.SetActive(false);
                playerCamera.GetComponentInChildren<AudioListener>().enabled = false;
            }
            if (IsLocalPlayer)
            {
                playerCamera.enabled = true;
                touchField.enabled = true;
                joystick.enabled = true;
                touchFieldPenel.SetActive(true);
                JoystickControllerUI.SetActive(true);
                playerCamera.GetComponentInChildren<AudioListener>().enabled = true;
            }
            else
            {
                playerCamera.enabled = false;
                touchField.enabled = false;
                joystick.enabled = false;
                touchFieldPenel.SetActive(false);
                JoystickControllerUI.SetActive(false);
                playerCamera.GetComponentInChildren<AudioListener>().enabled = false;
            }
            if (!IsOwner || !Application.isFocused) return;
        }
        if (IsOwner)
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                NetworkManager.Singleton.Shutdown();
                SceneManager.LoadScene("Home");
            }
            else if (Input.GetKey(KeyCode.F2))
            {
                LobbyManager.Instance.StartGame();
            }
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
            }
            else
            {
                animator.SetBool("IsWalking", false);
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
            }
            else
            {
                animator.SetBool("IsRunning", false);
            }
        }
        else
        {
            if (joystick.Vertical > 0)
            {
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
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
               isTriggerGoalAward = true;
               Debug.Log("Trigger touch Target");
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
        if (other.CompareTag("door"))
        {
            isDoor = true;
            Debug.Log("touch door");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("door"))
        {
            isDoor = false;
        }
    }
    void TriggerOnValueChange(int previous, int current)
    {
        Trigger_Goal_Count.Value = current;
    }

    //check and asign role of player


}