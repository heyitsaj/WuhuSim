using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(NetworkObject))]
public class PlayerMovement : NetworkBehaviour
{
    public NetworkVariable<Vector3> charpos = new NetworkVariable<Vector3>(default,NetworkVariableBase.DefaultReadPerm,NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> charrot = new NetworkVariable<Quaternion>(default,NetworkVariableBase.DefaultReadPerm,NetworkVariableWritePermission.Owner);
    [SerializeField]
    public NetworkVariable<ForceNetworkSerializeByMemcpy<FixedString64Bytes>> charname = new NetworkVariable<ForceNetworkSerializeByMemcpy<FixedString64Bytes>>(default,NetworkVariableBase.DefaultReadPerm,NetworkVariableWritePermission.Owner);
    public NetworkVariable<Color> outfitColor = new NetworkVariable<Color>(default,NetworkVariableBase.DefaultReadPerm,NetworkVariableWritePermission.Owner);
    public NetworkVariable<Color> skinColor = new NetworkVariable<Color>(default,NetworkVariableBase.DefaultReadPerm,NetworkVariableWritePermission.Owner);
    public NetworkVariable<Color> eyeColor = new NetworkVariable<Color>(default,NetworkVariableBase.DefaultReadPerm,NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> pEyeType = new NetworkVariable<int>(default,NetworkVariableBase.DefaultReadPerm,NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> pMouthType = new NetworkVariable<int>(default,NetworkVariableBase.DefaultReadPerm,NetworkVariableWritePermission.Owner);
    public float speed = 12f;
    public GameObject CurrPlayer;
    public GameObject model1;
    public GameObject model2;
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
    public Camera playerCam;
    public GameObject settingsMenu;
    CharacterController cc;
    float speedMult = 1.5f;
    Vector3 velocity;
    bool isGrounded;
    bool isDead;
    float xRotation = 0f;
    public float mouseSensitivity = 200f;
    public GameObject FloatText;
    public Canvas pauseMenu;
    public Button exitMenuButton;
    public Button SettingsButton;
    public Button ContinueButton;
    public Button exitButton;
    public Slider sensitivitySlider;
    public TMP_InputField sensitivityNumber;
    public Slider ROSlider;
    public Slider GOSlider;
    public Slider BOSlider;
    public TMP_InputField ROInput;
    public TMP_InputField GOInput;
    public TMP_InputField BOInput;
    public GameObject playerShirt;
    public RawImage outfitDemo;
    public Slider RSSlider;
    public Slider GSSlider;
    public Slider BSSlider;
    public TMP_InputField RSInput;
    public TMP_InputField GSInput;
    public TMP_InputField BSInput;
    public GameObject playerFace1;
    public GameObject playerFace2;
    public RawImage skinDemo;
    public Slider RESlider;
    public Slider GESlider;
    public Slider BESlider;
    public TMP_InputField eyeType;
    public TMP_InputField mouthType;
    public TMP_InputField REInput;
    public TMP_InputField GEInput;
    public TMP_InputField BEInput;
    public GameObject playerEyeL1;
    public GameObject playerEyeL2;
    public GameObject playerEyeR1;
    public GameObject playerEyeR2;
    public GameObject playerMouth;
    public RawImage eyeColorDemo;
    public RawImage eyeLDemo;
    public RawImage eyeLDemo2;
    public RawImage eyeRDemo;
    public RawImage eyeRDemo2;
    public RawImage mouthDemo;
    public Button eyeLeft;
    public Button eyeRight;
    public Button mouthLeft;
    public Button mouthRight;
    Material playerShirtMaterial;
    Material playerFaceMaterial1;
    Material playerFaceMaterial2;
    RawImage playerEyeLI1;
    RawImage playerEyeLI2;
    RawImage playerEyeRI1;
    RawImage playerEyeRI2;
    RawImage playerMouthI;
    public Color playerOColor = new Color(1,0,0,1);
    public Color playerSColor = new Color(1f,202f/255f,153f/255f,1f);
    public Color playerEColor = new Color(123f/255f,59/255f,0f/255f,1f);
    public int playerEType = 1;
    public int playerMType = 1;
    GameObject script;
    bool paused = false;
    void closeSettings(){
        settingsMenu.gameObject.SetActive(false);
    }
    void updateSensitivityNumber(float number){
        sensitivityNumber.text = number.ToString();
        mouseSensitivity = number;
    }
    void updateSlider(string number){
        float result;
        if(!float.TryParse(number,out result)){
            sensitivityNumber.text = mouseSensitivity.ToString();
            sensitivitySlider.value = mouseSensitivity;
        }else{
            float rounded = Mathf.Floor(result);
            if(rounded < 0){
                rounded = 0;
            }else if(rounded > 1000){
                rounded = 1000;
            }
            sensitivityNumber.text = rounded.ToString();
            sensitivitySlider.value = rounded;
            mouseSensitivity = rounded;
        }
    }
    void updateOutfit(){
        outfitDemo.color = playerOColor;
        playerShirtMaterial.color = playerOColor;
    }
    void updateROnumber(float red){
        ROInput.text = red.ToString();
        playerOColor.r = red/255;
        updateOutfit();
    }
    void updateROSlider(string red){
        float parsed;
        if(!float.TryParse(red,out parsed)){
            ROInput.text = (playerOColor.r * 255).ToString();
            ROSlider.value = playerOColor.r*255;
        }else{
            float rounded = Mathf.Floor(parsed);
            if(rounded < 0){
                rounded = 0;
            }else if(rounded > 255){
                rounded = 255;
            }
            ROInput.text = rounded.ToString();
            ROSlider.value = rounded;
            playerOColor.r = rounded/255;
            updateOutfit();
        }

    }
    void updateGOnumber(float green){
        GOInput.text = green.ToString();
        playerOColor.g = green/255;
        updateOutfit();

    }
    void updateGOSlider(string green){
        float parsed;
        if(!float.TryParse(green,out parsed)){
            GOInput.text = (playerOColor.g*255).ToString();
            GOSlider.value = playerOColor.g * 255;
        }else{
            float rounded = Mathf.Floor(parsed);
            if(rounded < 0){
                rounded = 0;
            }else if(rounded > 255){
                rounded = 255;
            }
            GOInput.text = rounded.ToString();
            GOSlider.value = rounded;
            playerOColor.g = rounded/255;
            updateOutfit();
        }
    }
    void updateBOnumber(float blue){
        BOInput.text = blue.ToString();
        playerOColor.b = blue/255;
        updateOutfit();
    }
    void updateBOslider(string blue){
        float parsed;
        if(!float.TryParse(blue,out parsed)){
            BOInput.text = (playerOColor.b*255).ToString();
            BOSlider.value = playerOColor.b*255;
        }else{
            float rounded = Mathf.Floor(parsed);
            if(rounded < 0){
                rounded = 0;
            }else if(rounded > 255){
                rounded = 255;
            }
            BOInput.text = rounded.ToString();
            BOSlider.value = rounded;
            playerOColor.b = rounded/255;
            updateOutfit();
        }
    }
    void updateSkin(){
        skinDemo.color = playerSColor;
        playerFaceMaterial1.color = playerSColor;
        playerFaceMaterial2.color = playerSColor;
    }
    void updateRSnumber(float red){
        RSInput.text = red.ToString();
        playerSColor.r = red/255;
        updateSkin();
    }
    void updateRSslider(string red){
        float parsed;
        if(!float.TryParse(red,out parsed)){
            RSInput.text = (playerSColor.r * 255).ToString();
            RSSlider.value = playerSColor.r*255;
        }else{
            float rounded = Mathf.Floor(parsed);
            if(rounded < 0){
                rounded = 0;
            }else if(rounded > 255){
                rounded = 255;
            }
            RSInput.text = rounded.ToString();
            RSSlider.value = rounded;
            playerSColor.r = rounded/255;
            updateSkin();
        }
    }
    void updateGSnumber(float green){
        GSInput.text = green.ToString();
        playerSColor.g = green/255;
        updateSkin();
    }
    void updateGSslider(string green){
        float parsed;
        if(!float.TryParse(green,out parsed)){
            GSInput.text = (playerSColor.g * 255).ToString();
            GSSlider.value = playerSColor.g*255;
        }else{
            float rounded = Mathf.Floor(parsed);
            if(rounded < 0){
                rounded = 0;
            }else if(rounded > 255){
                rounded = 255;
            }
            GSInput.text = rounded.ToString();
            GSSlider.value = rounded;
            playerSColor.g = rounded/255;
            updateSkin();
        }
    }
    void updateBSnumber(float blue){
        BSInput.text = blue.ToString();
        playerSColor.b = blue/255;
        updateSkin();
    }
    void updateBSslider(string blue){
        float parsed;
        if(!float.TryParse(blue,out parsed)){
            BSInput.text = (playerSColor.b * 255).ToString();
            BSSlider.value = playerSColor.b*255;
        }else{
            float rounded = Mathf.Floor(parsed);
            if(rounded < 0){
                rounded = 0;
            }else if(rounded > 255){
                rounded = 255;
            }
            BSInput.text = rounded.ToString();
            BSSlider.value = rounded;
            playerSColor.b = rounded/255;
            updateSkin();
        }
    }
    void updateFace(){
        eyeColorDemo.color = playerEColor;
        eyeLDemo.texture = Resources.Load("Eyes/"+playerEType.ToString()+"/1") as Texture2D;
        eyeLDemo2.texture = Resources.Load("Eyes/"+playerEType.ToString()+"/2") as Texture2D;
        eyeLDemo2.color = playerEColor;
        eyeRDemo.texture = Resources.Load("Eyes/"+playerEType.ToString()+"/1") as Texture2D;
        eyeRDemo2.texture = Resources.Load("Eyes/"+playerEType.ToString()+"/2") as Texture2D;
        eyeRDemo2.color = playerEColor;
        mouthDemo.texture = Resources.Load("Mouths/"+playerMType.ToString()) as Texture2D;
        playerEyeLI1.texture = Resources.Load("Eyes/"+playerEType.ToString()+"/1") as Texture2D;
        playerEyeLI2.texture = Resources.Load("Eyes/"+playerEType.ToString()+"/2") as Texture2D;
        playerEyeLI2.color = playerEColor;
        playerEyeRI1.texture = Resources.Load("Eyes/"+playerEType.ToString()+"/1") as Texture2D;
        playerEyeRI2.texture = Resources.Load("Eyes/"+playerEType.ToString()+"/2") as Texture2D;
        playerEyeRI2.color = playerEColor;
        playerMouthI.texture = Resources.Load("Mouths/"+playerMType.ToString()) as Texture2D;
    }
    void updateREnumber(float red){
        REInput.text = red.ToString();
        playerEColor.r = red/255;
        updateFace();
    }
    void updateRESlider(string red){
        float parsed;
        if(!float.TryParse(red,out parsed)){
            REInput.text = (playerEColor.r * 255).ToString();
            RESlider.value = playerEColor.r*255;
        }else{
            float rounded = Mathf.Floor(parsed);
            if(rounded < 0){
                rounded = 0;
            }else if(rounded > 255){
                rounded = 255;
            }
            REInput.text = rounded.ToString();
            RESlider.value = rounded;
            playerEColor.r = rounded/255;
            updateFace();
        }

    }
    void updateGEnumber(float green){
        GEInput.text = green.ToString();
        playerEColor.g = green/255;
        updateFace();

    }
    void updateGESlider(string green){
        float parsed;
        if(!float.TryParse(green,out parsed)){
            GEInput.text = (playerEColor.g*255).ToString();
            GESlider.value = playerEColor.g * 255;
        }else{
            float rounded = Mathf.Floor(parsed);
            if(rounded < 0){
                rounded = 0;
            }else if(rounded > 255){
                rounded = 255;
            }
            GEInput.text = rounded.ToString();
            GESlider.value = rounded;
            playerEColor.g = rounded/255;
            updateFace();
        }
    }
    void updateBEnumber(float blue){
        BEInput.text = blue.ToString();
        playerEColor.b = blue/255;
        updateFace();
    }
    void updateBEslider(string blue){
        float parsed;
        if(!float.TryParse(blue,out parsed)){
            BEInput.text = (playerEColor.b*255).ToString();
            BESlider.value = playerEColor.b*255;
        }else{
            float rounded = Mathf.Floor(parsed);
            if(rounded < 0){
                rounded = 0;
            }else if(rounded > 255){
                rounded = 255;
            }
            BEInput.text = rounded.ToString();
            BESlider.value = rounded;
            playerEColor.b = rounded/255;
            updateFace();
        }
    }
    void decreaseEyes(){
        playerEType--;
        if(playerEType < 1){
            playerEType = 76;
        }
        eyeType.text = playerEType.ToString();
        updateFace();
    }
    void increaseEyes(){
        playerEType++;
        if(playerEType > 76){
            playerEType = 1;
        }
        eyeType.text = playerEType.ToString();
        updateFace();
    }
    void decreaseMouth(){
        playerMType--;
        if(playerMType < 1){
            playerMType = 42;
        }
        mouthType.text = playerMType.ToString();
        updateFace();
    }
    void increaseMouth(){
        playerMType++;
        if(playerMType > 42){
            playerMType = 1;
        }
        mouthType.text = playerMType.ToString();
        updateFace();
    }
    void eyeInput(string input){
        int parsed;
        if(!int.TryParse(input,out parsed)){
            eyeType.text = playerEType.ToString();
        }else{
            if(parsed < 1){
                parsed = 1;
            }else if(parsed > 76){
                parsed = 76;
            }
            eyeType.text = parsed.ToString();
            playerEType = parsed;
            updateFace();
        }
    }
    void mouthInput(string input){
        int parsed;
        if(!int.TryParse(input,out parsed)){
            mouthType.text = playerMType.ToString();
        }else{
            if(parsed < 1){
                parsed = 1;
            }else if(parsed > 42){
                parsed = 42;
            }
            mouthType.text = parsed.ToString();
            playerMType = parsed;
            updateFace();
        }
    }
    void chooseExit(){
        script.GetComponent<DecideScript>().ExitServer();
    }
    void chooseSettings(){
        settingsMenu.SetActive(true);
    }
    void chooseContinue(){
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        settingsMenu.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
    }
    //Set first person view
    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        if (IsLocalPlayer){
            cc = GetComponent<CharacterController>();
        }else{
            playerCam.transform.gameObject.SetActive(false);
        }
        script = GameObject.Find("DecideScript");
        sensitivitySlider.onValueChanged.AddListener(updateSensitivityNumber);
        sensitivityNumber.onSubmit.AddListener(updateSlider);
        exitButton.onClick.AddListener(closeSettings);
        ROSlider.onValueChanged.AddListener(updateROnumber);
        GOSlider.onValueChanged.AddListener(updateGOnumber);
        BOSlider.onValueChanged.AddListener(updateBOnumber);
        ROInput.onSubmit.AddListener(updateROSlider);
        GOInput.onSubmit.AddListener(updateGOSlider);
        BOInput.onSubmit.AddListener(updateBOslider);
        exitMenuButton.onClick.AddListener(chooseExit);
        SettingsButton.onClick.AddListener(chooseSettings);
        ContinueButton.onClick.AddListener(chooseContinue);
        RSSlider.onValueChanged.AddListener(updateRSnumber);
        GSSlider.onValueChanged.AddListener(updateGSnumber);
        BSSlider.onValueChanged.AddListener(updateBSnumber);
        RSInput.onSubmit.AddListener(updateRSslider);
        GSInput.onSubmit.AddListener(updateGSslider);
        BSInput.onSubmit.AddListener(updateBSslider);
        RESlider.onValueChanged.AddListener(updateREnumber);
        GESlider.onValueChanged.AddListener(updateGEnumber);
        BESlider.onValueChanged.AddListener(updateBEnumber);
        REInput.onSubmit.AddListener(updateRESlider);
        GEInput.onSubmit.AddListener(updateGESlider);
        BEInput.onSubmit.AddListener(updateBEslider);
        eyeLeft.onClick.AddListener(decreaseEyes);
        eyeRight.onClick.AddListener(increaseEyes);
        mouthLeft.onClick.AddListener(decreaseMouth);
        mouthRight.onClick.AddListener(increaseMouth);
        eyeType.onSubmit.AddListener(eyeInput);
        mouthType.onSubmit.AddListener(mouthInput);
        FloatText.GetComponent<TextMesh>().text = script.GetComponent<DecideScript>().username;
        playerShirtMaterial = playerShirt.GetComponent<SkinnedMeshRenderer>().materials[0];
        playerFaceMaterial1 = playerFace1.GetComponent<SkinnedMeshRenderer>().materials[0];
        playerFaceMaterial2 = playerFace2.GetComponent<SkinnedMeshRenderer>().materials[0];
        playerEyeLI1 = playerEyeL1.GetComponent<RawImage>();
        playerEyeLI2 = playerEyeL2.GetComponent<RawImage>();
        playerEyeRI1 = playerEyeR1.GetComponent<RawImage>();
        playerEyeRI2 = playerEyeR2.GetComponent<RawImage>();
        playerMouthI = playerMouth.GetComponent<RawImage>();
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
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }
    void PauseCheck(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(paused){
                paused = false;
                Cursor.lockState = CursorLockMode.Locked;
                pauseMenu.gameObject.SetActive(false);
                settingsMenu.gameObject.SetActive(false);
            }else{
                paused = true;
                Cursor.lockState = CursorLockMode.None;
                pauseMenu.gameObject.SetActive(true);
                settingsMenu.gameObject.SetActive(false);
            }
        }
    }
    void Update(){
        //Updates server
        if (IsOwner){
            PauseCheck();
            if(!paused){
                Move();
                Look();
            }
            model1.gameObject.SetActive(false);
            model2.gameObject.SetActive(false);
            charname.Value = new ForceNetworkSerializeByMemcpy<FixedString64Bytes>(new FixedString64Bytes(FloatText.GetComponent<TextMesh>().text));
            charrot.Value = transform.rotation;
            charpos.Value = transform.position;
            outfitColor.Value = playerShirtMaterial.color;
            skinColor.Value = playerFaceMaterial1.color;
            eyeColor.Value = playerEyeL2.GetComponent<RawImage>().color;
            pEyeType.Value = playerEType;
            pMouthType.Value = playerMType;

        }else{
            model1.gameObject.SetActive(true);
            model2.gameObject.SetActive(true);
            FloatText.GetComponent<TextMesh>().text = charname.Value.Value.ToString();
            transform.position = charpos.Value;
            transform.rotation = charrot.Value;
            playerShirtMaterial.color = outfitColor.Value;
            playerFaceMaterial1.color = skinColor.Value;
            playerFaceMaterial2.color = skinColor.Value;
            playerEyeLI1.texture = Resources.Load("Eyes/"+pEyeType.Value.ToString()+"/1") as Texture2D;
            playerEyeLI2.texture = Resources.Load("Eyes/"+pEyeType.Value.ToString()+"/2") as Texture2D;
            playerEyeLI2.color = eyeColor.Value;
            playerEyeRI1.texture = Resources.Load("Eyes/"+pEyeType.Value.ToString()+"/1") as Texture2D;
            playerEyeRI2.texture = Resources.Load("Eyes/"+pEyeType.Value.ToString()+"/2") as Texture2D;
            playerEyeRI2.color = eyeColor.Value;
            playerMouthI.texture = Resources.Load("Mouths/"+pMouthType.Value.ToString()) as Texture2D;
        }
    }
}
