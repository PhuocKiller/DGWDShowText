using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class RoboController : NetworkBehaviour, ICanTakeDamage
{
    private TextMeshProUGUI playerLifeText;
    private bool isDetectReady = false;
    private bool isDetectInput = false;
    [Networked(OnChanged = nameof(LifeChanged))]
    private int lives { get; set; }
    public int Lives
    {
        get
        {
            if (Object.IsValid) { return lives; }
            else { return -1; }
        }
    }
    [SerializeField]
    private float maxHealth;
    [Networked]
    private float health { get; set; }
    RoboInput roboInput;
    Vector2 mousePos { get; set; }
    [Networked]
    Vector3 headDirection { get; set; }
    [Networked]
    Vector2 inputDirection { get; set; }
    [SerializeField]
    private LayerMask groundLayerMask;
    [SerializeField]
    private Transform headTransform;
    [SerializeField]
    private Transform bodyTransform;
    [SerializeField]
    private float angle;

    private Transform playerLifeParent;
    [SerializeField]
    private GameObject playerLifeNumber;

    private NetworkCharacterControllerPrototype characterControllerPrototype;

    [SerializeField]
    private Material[] headColors;
    [SerializeField]
    private MeshRenderer headMeshRenderer;
    private TrackingReady trackingReady;
    [SerializeField]
    Transform bulletPoint;
    [SerializeField]
    GameObject bullet;

    private bool isFire;
    private float countDownFire = .5f;
    private bool visualChanged = false;

    //0 la binh thuong
    //1 la respawn visualchange = false;
    //2 la respawn visualchange = true;
    [Networked(OnChanged = nameof(listenState))]
    public int state { get; set; }
    public int GetCurrentState()
    {
        return state;
    }
    private TickTimer respawnCount { get; set; }
    private Vector3 spawnPointLocal = new Vector3(0, 10, 0);
    private bool flagState = false;
    public void SetTrackingReady(TrackingReady tracking)
    {
        this.trackingReady = tracking;
    }
    public void SetSpawnPoint(Vector3 setSpawnPoint)
    {
        spawnPointLocal = setSpawnPoint;
    }
    static void LifeChanged(Changed<RoboController> changed)
    {
        if (changed.Behaviour.playerLifeText != null)
        {
            changed.Behaviour.playerLifeText.text
           = $"Player {changed.Behaviour.Object.InputAuthority.PlayerId} has life: {changed.Behaviour.lives}";
        }
        Singleton<PlayerManager>.Instance.CheckWinner();
    }
    protected static void listenState(Changed<RoboController> changed)
    {

        if (changed.Behaviour.state == 1)
        {
            changed.Behaviour.headTransform.gameObject.SetActive(true);
            changed.Behaviour.bodyTransform.gameObject.SetActive(true);
            if (changed.Behaviour.Object.HasStateAuthority)
            {
                changed.Behaviour.characterControllerPrototype.TeleportToPosition(
                changed.Behaviour.spawnPointLocal);
            }
            changed.Behaviour.CheckCamera(changed.Behaviour.Object.InputAuthority, true);
            changed.Behaviour.StartCoroutine(changed.Behaviour.CheckFlag());
            return;
        }
        if (changed.Behaviour.state == 2)
        {
            changed.Behaviour.headTransform.gameObject.SetActive(false);
            changed.Behaviour.bodyTransform.gameObject.SetActive(false);
            changed.Behaviour.flagState = true;
            if (changed.Behaviour.Object.HasStateAuthority)
            {
                changed.Behaviour.characterControllerPrototype.TeleportToPosition(
                new Vector3(-500, -500, -500)
                );
            }

            changed.Behaviour.CheckCamera(changed.Behaviour.Object.InputAuthority, false);
            return;
        }
    }
    public IEnumerator CheckFlag()
    {
        yield return new WaitForSeconds(0.1f);
        flagState = false;
    }
    public void CheckCamera(PlayerRef player, bool IsFollow)
    {
        if (player == Runner.LocalPlayer)
        {
            if (IsFollow)
            {
                Singleton<CameraController>.Instance.SetFollowRobo(transform);
            }
            else
            {
                Singleton<CameraController>.Instance.RemoveFollowRobo();
            }
        }
    }
    private void Awake()
    {
        roboInput = new RoboInput();
        playerLifeParent = GameObject.FindGameObjectWithTag("PlayerLife").transform;
    }
    #region life input 
    private void OnEnable()
    {
        roboInput.Enable();
    }
    private void OnDisable()
    {
        roboInput.Disable();
    }
    #endregion 


    void Update()
    {
        if (HasStateAuthority && Object.IsValid)
        {
            if (isDetectInput == false)
            {
                return;
            }
        }
        if (HasInputAuthority && HasStateAuthority)
        {
            mousePos = roboInput.RoboActions.MousePosition.ReadValue<Vector2>();
            inputDirection = roboInput.RoboActions.Move.ReadValue<Vector2>();

            if (isFire == false)
            {
                isFire = roboInput.RoboActions.Fire.triggered;
            }
        }

        if (HasInputAuthority && isDetectReady && Object.IsValid
            && roboInput.RoboActions.Ready.triggered && trackingReady != null
            )
        {
            trackingReady.OnChangedReady();
        }

    }
    private void FixedUpdate()
    {
    }
    private void CalculateMove()
    {
        if (flagState) return;
        Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
        characterControllerPrototype.Move(moveDirection * 4 * Runner.DeltaTime);

    }
    private void CalculateBodyRotation()
    {
        /*Vector3 axisRotate = new Vector3(inputDirection.y, 0, -inputDirection.x);
        bodyTransform.rotation = Quaternion.AngleAxis(angle, axisRotate);*/


        Vector3 direction = inputDirection.sqrMagnitude > 0
            ? new Vector3(inputDirection.y, 0, -inputDirection.x)
            : Vector3.zero;
        Quaternion rotate = Quaternion.Euler(direction * angle);
        bodyTransform.rotation = Quaternion.RotateTowards(bodyTransform.rotation,
            rotate, 360 * Runner.DeltaTime);
    }
    private void CalculateHeadRotation()
    {
        if (HasInputAuthority && HasStateAuthority)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 200, groundLayerMask))
            {
                headDirection = (new Vector3(hitInfo.point.x, 0, hitInfo.point.z)
                    - new Vector3(headTransform.position.x, 0, headTransform.position.z)).normalized;
            }
        }

        Quaternion look = Quaternion.LookRotation(headDirection);
        headTransform.rotation = Quaternion.RotateTowards(
            headTransform.rotation, look, 720 * Runner.DeltaTime
            );

    }


    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority)
        {
            lives = 1;
        }


        if (HasStateAuthority) state = 0;
        health = maxHealth;
        characterControllerPrototype = GetComponent<NetworkCharacterControllerPrototype>();
        headMeshRenderer.material = headColors[Object.InputAuthority.PlayerId];

        if (Object.InputAuthority.PlayerId == Runner.LocalPlayer.PlayerId)
        {
            Singleton<CameraController>.Instance.SetFollowRobo(transform);
            FindObjectOfType<GameManager>().RegisterOnGameStateChanged(OnGameStateChanged);
        }
        Singleton<PlayerManager>.Instance.AddRobo(this);
        FindObjectOfType<GameManager>().RegisterOnGameStateChanged(OnAllGameStateChanged);
    }
    public void OnAllGameStateChanged(GameState oldState, GameState newState)
    {
        if (newState == GameState.InGame)
        {
            GameObject playerLife = Instantiate(playerLifeNumber, playerLifeParent);
            playerLifeText = playerLife.GetComponent<TextMeshProUGUI>();
            playerLifeText.text
                = $"Player {Object.InputAuthority.PlayerId} has life: {lives}";
        }
    }
    public void OnGameStateChanged(GameState oldState, GameState newState)
    {
        if (HasStateAuthority)
        {
            isDetectInput = newState == GameState.Lobby || newState == GameState.InGame;
            isDetectReady = newState == GameState.Lobby;
            if (newState != GameState.Lobby && trackingReady != null) trackingReady.HideReady();
        }



    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (HasStateAuthority)
        {
            if (respawnCount.RemainingTicks(Runner) <= 1 && visualChanged == true)
            {
                state = 1;
                visualChanged = false;
            }

            if (respawnCount.IsRunning && !respawnCount.Expired(Runner))
            {
                if (visualChanged) return;
                state = 2;
                visualChanged = true;
            }
        }


        if (visualChanged) return;

        CalculateMove();
        CalculateHeadRotation();
        CalculateBodyRotation();

        if (isFire && HasInputAuthority && countDownFire == .5f)
        {
            Runner.Spawn(bullet, bulletPoint.position, Quaternion.identity, inputAuthority: Object.InputAuthority,
                        onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                        {
                            obj.GetComponent<Bullet>().SetDirection(headTransform.forward);
                        }
                        );
            countDownFire -= Runner.DeltaTime;
        }
        isFire = false;
        if (countDownFire > 0 && countDownFire < .5f)
        {
            countDownFire -= Runner.DeltaTime;
        }
        else
        {
            countDownFire = .5f;
        }

    }
    public override void Render()
    {
        base.Render();

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void CalculateHealthRPC(int damage, PlayerRef player)
    {
        if (HasStateAuthority)
        {
            if (health - damage > 0)
            {
                health -= damage;
            }
            else
            {
                lives -= 1;
                health = maxHealth;
                if (lives == 0)
                {
                    RespawnRobo(second: 999);
                }
                else
                {
                    //Respawn
                    if (HasStateAuthority)
                    {
                        RespawnRobo(second: 3);
                    }

                }
            }
        }

    }
    private void RespawnRobo(int second)
    {
        respawnCount = TickTimer.CreateFromSeconds(Runner, second);
    }
    public void ApplyDamage(int damage, PlayerRef player, Action callback = null)
    {

        CalculateHealthRPC(damage, player);
        callback?.Invoke();
    }
}
