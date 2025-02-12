using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Linq;

public class PlayerHandlerScript : NetworkBehaviour
{
    [SerializeField] private float moveAllowance = 0.1f;
    private Camera cam;
    private Vector3 lastPosition;
    private bool lastMovingState = false;
    private FieldStates[,] field;
    public NavMeshAgent agent { get; private set; }
    private Animator animator;
    public string playerName { get; private set; }
    public int playerId { get; private set; }
    private List<Vector3> path;
    private Vector3 previousStep;
    private int respawnTime = 10;
    public float animationSpeed = 0.1f;
    public PlayerProperty thisPlayer;
    public static bool IsStarted = false;
    public bool IsAllowedToMove = true;
    public Vector3 SpawnPoint;
    public void SetPlayerName(string name)
    {
        playerName = name;
        gameObject.name = name;
    }

    private void Awake()
    {
        field = new FieldStates[GlobalVariableHandler.Instance.FieldSizeY, GlobalVariableHandler.Instance.FieldSizeX];
        for (int y = 0; y < GlobalVariableHandler.Instance.FieldSizeY; y++)
        {
            for (int x = 0; x < GlobalVariableHandler.Instance.FieldSizeX; x++)
            {
                if (GlobalVariableHandler.Instance.TerrainField[y, x] >= -moveAllowance && GlobalVariableHandler.Instance.TerrainField[y, x] <= moveAllowance)
                {
                    field[y, x] = FieldStates.Empty;
                }
                else
                {
                    field[y, x] = FieldStates.Wall;
                }
            }
        }
    }
    public void AssignCamera(Camera assignedCamera)
    {
        cam = assignedCamera;
    }
    private void InitializeServerAgent()
    {
        if (IsServer)
        {
            if (agent == null)
                agent = gameObject.AddComponent<NavMeshAgent>();

            agent.enabled = true;
            agent.areaMask = NavMesh.AllAreas;
            for (int i = 0; i < GlobalVariableHandler.Instance.Players.Count; i++)
            {
                if (GlobalVariableHandler.Instance.Players[i].Id == playerId)
                {
                    agent.speed = GlobalVariableHandler.Instance.Players[i].MoveSpeed;
                    break;
                }
            }
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError($"NavMeshAgent is null for {gameObject.name} on server.");
            }
        }

        if (IsOwner)
        {
            InitializeLocalPlayer();
        }
        else
        {
            DisableRemotePlayerComponents();
        }
    }
    private void InitializeLocalPlayer()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is missing.");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.Warp(hit.position);
            }
        }

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        for (int i = 0; i < GlobalVariableHandler.Instance.Players.Count; i++)
        {
            if (GlobalVariableHandler.Instance.Players[i].Id == playerId)
            {
                agent.speed = GlobalVariableHandler.Instance.Players[i].MoveSpeed;
                break;
            }
        }
        animator = GetComponent<Animator>();

        MenuHandler.SetCurrentPlayer(this.transform);
        BaseMenuHandler.SetCurrentPlayer(this.transform);
        if (string.IsNullOrEmpty(playerName))
        {
            SetPlayerName($"Player{OwnerClientId}");
        }
    }
    private void DisableRemotePlayerComponents()
    {
        if (TryGetComponent(out NavMeshAgent navAgent))
        {
            navAgent.enabled = false;
        }

        if (TryGetComponent(out Renderer renderer))
        {
            renderer.material.color = Color.gray;
        }
    }
    private void Update()
    {
        if (!IsOwner || agent == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1) && IsAllowedToMove)
        {
            HandleMouseClick();
        }
    }
    private void Start()
    {
        AssignCamera(Camera.main);
        animator = GetComponent<Animator>();

    }
    private void HandleMouseClick()
    {
        if (cam == null)
        {
            AssignCamera(Camera.main);
        }

        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.Log("NavMeshAgent is not active or not on NavMesh.");
            return;
        }

        if (!agent.enabled)
        {
            Debug.LogError("NavMeshAgent is not enabled.");
            return;
        }

        if (!IsStarted)
        {
            return;
        }

        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = cam.ScreenToWorldPoint(mousePosition);

        if (IsPositionValid(worldPosition))
        {
            int targetX = (int)(worldPosition.x * 100 / GameManager.spriteSize);
            int targetY = (int)(worldPosition.y * 100 / GameManager.spriteSize);

            Vector3 destination = new Vector3((targetX + 0.5f) * GameManager.spriteSize / 100f,
                                              (targetY + 0.5f) * GameManager.spriteSize / 100f, 0);

            MoveToDestinationServerRpc(destination);
        }

    }

    [ServerRpc]
    public void MoveToDestinationServerRpc(Vector3 destination, ServerRpcParams serverRpcParams = default)
    {
        if (agent == null || !agent.isOnNavMesh)
        {
            InitializeServerAgent();
        }

        if (serverRpcParams.Receive.SenderClientId != OwnerClientId)
        {
            Debug.LogError($"Unauthorized movement request from client {serverRpcParams.Receive.SenderClientId} for {gameObject.name}.");
            return;
        }

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);

            Vector2 direction = (destination - transform.position).normalized;
            UpdateAnimationClientRpc(direction.x, direction.y, true);

            StartCoroutine(UpdateAnimationWhileMoving());
        }
    }
    private IEnumerator UpdateAnimationWhileMoving()
    {
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            Vector2 movement = new Vector2(agent.velocity.x, agent.velocity.y).normalized;
            bool isMoving = movement.magnitude > 0.02f;

            UpdateAnimationClientRpc(movement.x, movement.y, isMoving);

            yield return null;
        }

        UpdateAnimationClientRpc(0, 0, false);
    }



    private bool IsPositionValid(Vector3 position)
    {
        return position.x > 0 && position.y > 0 &&
               position.x * 100 < MapScript.sprites.GetLength(1) * GameManager.spriteSize &&
               position.y * 100 < MapScript.sprites.GetLength(0) * GameManager.spriteSize;
    }
    [ServerRpc]
    public void RequestAnimationUpdateServerRpc(float horizontal, float vertical, bool isMoving, ServerRpcParams rpcParams = default)
    {
        if (!IsOwner) return;

        UpdateAnimationClientRpc(horizontal, vertical, isMoving);
    }

    [ClientRpc]
    public void UpdateAnimationClientRpc(float horizontal, float vertical, bool isMoving)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[Client] Animator is NULL! Failed to update animation.");
                return;
            }
        }

        if (isMoving)
        {
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);
        }

        animator.SetBool("IsMoving", isMoving);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;

        PlayerHandlerScript enemy = collision.GetComponent<PlayerHandlerScript>();
        if (enemy != null && enemy != this)
        {
            RequestBattleServerRpc(enemy.OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestBattleServerRpc(ulong enemyId)
    {
        GameManager.Instance.StartBattleServerRpc(OwnerClientId, enemyId);
    }
    public void Die()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartRespawnTimerServerRpc(playerId, respawnTime);
    }

    public void Respawn()
    {
        Vector3 respawnPosition = this.SpawnPoint;
        this.transform.position = respawnPosition;
        this.IsAllowedToMove = true;
        gameObject.SetActive(true);
    }

}
