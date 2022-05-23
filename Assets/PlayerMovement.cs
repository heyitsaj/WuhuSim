using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;
[RequireComponent(typeof(NetworkObject))]
public class PlayerMovement : NetworkBehaviour
{
    public NetworkVariable<Vector3> charpos = new NetworkVariable<Vector3>(default,NetworkVariableBase.DefaultReadPerm,NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> charrot = new NetworkVariable<Quaternion>(default,NetworkVariableBase.DefaultReadPerm,NetworkVariableWritePermission.Owner);
    [SerializeField]
    NetworkVariable<ForceNetworkSerializeByMemcpy<FixedString64Bytes>> charname = new NetworkVariable<ForceNetworkSerializeByMemcpy<FixedString64Bytes>>(default,NetworkVariableBase.DefaultReadPerm,NetworkVariableWritePermission.Owner);
    public float speed = 12f;
    public float gravity = -19.62f;
    public float jumpHeight = 2f;
    public Transform groundCheck;
    public Transform player;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask deathMask;
    public Vector3 position;
    public Quaternion rotation;
    public Animator animate;
    Transform cameraTransform;
    CharacterController cc;
    float speedMult = 1.5f;
    Vector3 velocity;
    bool isGrounded;
    bool isDead;
    float xRotation = 0f;
    public float mouseSensitivity = 200f;
    public GameObject FloatText;
    GameObject script;

    //Set first person view
    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        cameraTransform = GetComponentInChildren<Camera>().transform;
        if (IsLocalPlayer){
            cc = GetComponent<CharacterController>();
        }else{
            cameraTransform.gameObject.SetActive(false);
        }
        script = GameObject.Find("DecideScript");
        FloatText.GetComponent<TextMesh>().text = script.GetComponent<DecideScript>().username;
    }
    //Input movement (jumping, turning, etc.)
    void Move()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isDead = Physics.CheckSphere(groundCheck.position, groundDistance, deathMask);
        if(isGrounded && velocity.y < 0){
            velocity.y = -9.81f;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float avgvel = Mathf.Abs(x) + Mathf.Abs(z);
        Vector3 move = transform.right * x + transform.forward * z;
        cc.Move(move * speed * Time.deltaTime);
        if(Input.GetButtonDown("Jump") && isGrounded){
            velocity.y = Mathf.Sqrt(jumpHeight * -1f * gravity);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            speed *= 1.5f;
            speedMult = 2.5f;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift)){
            speed /= 1.5f;
            speedMult = 1.5f;
        }
        velocity.y += gravity * Time.deltaTime;
        animate.SetFloat("velocity",avgvel);
        cc.Move(velocity * Time.deltaTime);
        AudioSource step = GetComponent<AudioSource>();
        
        if(isGrounded && avgvel > .5 && !step.isPlaying){
            step.volume = Random.Range(0.8f, 1);
            step.pitch = Random.Range(0.95f, 1.05f) * speedMult;
            step.Play();
        }
        if(isDead){
            player.transform.SetPositionAndRotation(position,rotation);
        }
    }
    //Mouse movement for first person view
    void Look(){
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }
    void Update(){
        //Updates server
        if (IsOwner){
            Move();
            Look();
            charname.Value = new ForceNetworkSerializeByMemcpy<FixedString64Bytes>(new FixedString64Bytes(FloatText.GetComponent<TextMesh>().text));
            charrot.Value = transform.rotation;
            charpos.Value = transform.position;
        }else{
            FloatText.GetComponent<TextMesh>().text = charname.Value.Value.ToString();
            transform.position = charpos.Value;
            transform.rotation = charrot.Value;


        }
    }
}
