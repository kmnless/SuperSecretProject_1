using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine.AI;
using UnityEngine;

public class PlayerHandlerScript : NetworkBehaviour
{
    [SerializeField] private float moveAllowance = 0.1f;
    private Camera cam;
    private FieldStates[,] field;
    public NavMeshAgent agent { get; private set; }
    public string playerName { get; private set; }
    private List<Vector3> path;
    private Vector3 previousStep;
    private float animationCounter = 0;
    private int counter = 1;
    private bool allowMove = false;
    public float animationSpeed = 0.1f;

    public void SetPlayerName(string name)
    {
        playerName = name;
        gameObject.name = name;
    }

    private void Awake()
    {
        cam = Camera.main;

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

    public override void OnNetworkSpawn()
    {
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
            Debug.LogError("NavMeshAgent is missing on this object.");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.Log($"Agent is not on NavMesh at {transform.position}. Adjusting position.");
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.Warp(hit.position);
            }
            else
            {
                Debug.Log($"Could not find a valid position on NavMesh.");
            }
        }

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (string.IsNullOrEmpty(playerName))
        {
            SetPlayerName($"Player{OwnerClientId}");
        }

        Debug.Log($"Local player initialized: {gameObject.name}");
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

        Debug.Log($"Remote player initialized: {gameObject.name}");
    }

    private void Update()
    {
        if (!IsOwner || agent == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        if (cam == null)
        {
            Debug.LogError("Camera is not initialized.");
            return;
        }

        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.Log("NavMeshAgent is not active or not on NavMesh.");
            //return;
        }

        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = cam.ScreenToWorldPoint(mousePosition);

        if (IsPositionValid(worldPosition))
        {
            int targetX = (int)(worldPosition.x * 100 / GameLoaderScript.spriteSize);
            int targetY = (int)(worldPosition.y * 100 / GameLoaderScript.spriteSize);

            Vector3 destination = new Vector3((targetX + 0.5f) * GameLoaderScript.spriteSize / 100f,
                                              (targetY + 0.5f) * GameLoaderScript.spriteSize / 100f, 0);

            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                Debug.Log($"Moving to: {hit.position}");
            }
            else
            {
                Debug.LogError($"Target destination {destination} is not on NavMesh.");
            }
        }
    }

    private bool IsPositionValid(Vector3 position)
    {
        return position.x > 0 && position.y > 0 &&
               position.x * 100 < MapScript.sprites.GetLength(1) * GameLoaderScript.spriteSize &&
               position.y * 100 < MapScript.sprites.GetLength(0) * GameLoaderScript.spriteSize;
    }

}


/*public class PlayerHandlerScript : MonoBehaviour
{
    [SerializeField] float moveAllowance;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private new Camera camera;
    private FieldStates[,] field;
    public static NavMeshAgent agent = null;
    private List<Vector3> path;
    public static GameObject player;
    private Vector3 previousStep;
    private float animationCounter = 0;
    private int counter = 1;
    private bool allowMove = false;
    public float animationSpeed = 0.1f;
    public PlayerProperty properties;
    public int id;

    public void addXP(int XP)
    {
        properties.CurrentXP += XP;
        if (properties.CurrentXP >= properties.NeededXP)
        {
            properties.Level++;
            properties.CurrentXP -= properties.NeededXP;
            properties.NeededXP = (int)(properties.NeededXP * properties.MultiplierXP);
            properties.StrengthMultiplier *= properties.StrengthMultiplierGain;
        }
    }

    public void addMoney(int money)
    {
        properties.Money += money;
    }
    public void Awake()
    {
        id = GlobalVariableHandler.Instance.MyIndex.Value;
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
    public void Start()
    {
        if (player == null)
            player = GameObject.Find($"Player{id}");
        agent = player.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void FixedUpdate()
    {
        Vector3 intermediate = new Vector3();


        if (path != null && path.Count > counter && allowMove)
        {
            intermediate = path[counter] - previousStep;

            player.transform.position = previousStep + intermediate * animationCounter;
            animationCounter += animationSpeed;
            if (animationCounter > 1f)
            {
                previousStep = path[counter];
                animationCounter = 0;
                counter++;
            }

        }
        else
        {
            counter = 1;
            animationCounter = 0;
            path = null;
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Input.mousePosition;

            Vector3 worldPosition = camera.ScreenToWorldPoint(mousePosition);

            if (worldPosition.x > 0 && worldPosition.y > 0 && worldPosition.x * 100 < MapScript.sprites.GetLength(1) * GameLoaderScript.spriteSize && worldPosition.y * 100 < MapScript.sprites.GetLength(0) * GameLoaderScript.spriteSize)
            {
                //Debug.Log("Mouse Clicked at: " + worldPosition);
                if (MapScript.sprites != null)
                {
                    int targetX = (int)(worldPosition.x * 100 / GameLoaderScript.spriteSize);
                    int targetY = (int)(worldPosition.y * 100 / GameLoaderScript.spriteSize);
                    if (GlobalVariableHandler.Instance.BuildingsField[targetY, targetX] != Convert.ToInt32(Constants.Buildings.None))
                    {
                        Vector3 pos = new Vector3((targetX + 0.5f) * GameLoaderScript.spriteSize / 100f, (targetY + 0.5f) * GameLoaderScript.spriteSize / 100f, 10f);
                        //Debug.Log("Destination: " + pos );
                        agent.SetDestination(pos);
                    }
                }
            }
        }
    }
}*/
