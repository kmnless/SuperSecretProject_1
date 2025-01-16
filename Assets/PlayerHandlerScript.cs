using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class PlayerHandlerScript : MonoBehaviour
{
    [SerializeField] float moveAllowance;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private new Camera camera;
    [SerializeField] private GameObject bases;
    private FieldStates[,] field;
    private NavMeshAgent agent = null;
    private List<Vector3> path; 
    private GameObject player;
    private AStar aStar;
    private Vector3 previousStep;
    private float animationCounter = 0;
    private int counter = 1;
    private bool allowMove = false;
    public float animationSpeed = 0.1f;
    public PlayerProperty properties;
    public int id = GlobalVariableHandler.Instance.MyIndex;

    public void SpawnPlayer(Vector3 position)
    {
        GameObject playerObject = Instantiate(playerPrefab, position, Quaternion.identity);
        playerObject.name = $"Player_{id}";
        agent = playerObject.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }


    //public void createPlayer(Vector3 pos)
    //{
    //    player = Instantiate(playerPrefab,pos, Quaternion.identity);
    //    player.name = "Player";
    //    agent = player.GetComponent<NavMeshAgent>();
    //    agent.updateRotation = false;
    //    agent.updateUpAxis = false;
    //    //properties = GlobalVariableHandler.players[id]; kak tolko budet server budet ok!
    //}
    public void addXP(int XP)
    {
        properties.CurrentXP+=XP;
        if(properties.CurrentXP>=properties.NeededXP)
        {
            properties.Level++;
            properties.CurrentXP-=properties.NeededXP;
            properties.NeededXP = (int)(properties.NeededXP*properties.MultiplierXP);
            properties.StrengthMultiplier *=properties.StrengthMultiplierGain;
        }
    }

    public void addMoney(int money)
    {
        properties.Money+=money;
    }
    public void Awake()
    {

    }
    public void Start()
    {
        Vector3 pos;
        field = new FieldStates[GlobalVariableHandler.Instance.FieldSizeY, GlobalVariableHandler.Instance.FieldSizeX];
        //MapScript.sprites[0,1].SetActive(false);
        for(int y = 0; y < GlobalVariableHandler.Instance.FieldSizeY; y++)
        {
            for(int x = 0; x < GlobalVariableHandler.Instance.FieldSizeX; x++)
            {
               // Debug.Log($"{GlobalVariableHandler.terrainField[y, x]} on x={x} on y={y}");
                if(GlobalVariableHandler.Instance.TerrainField[y, x]>=-moveAllowance && GlobalVariableHandler.Instance.TerrainField[y, x]<=moveAllowance)
                {
                    field[y, x] = FieldStates.Empty;
                }
                else
                {
                    field[y, x] = FieldStates.Wall;
                }
            }
        }

        if(bases.transform.childCount>GlobalVariableHandler.Instance.MyIndex)
        {
            pos = bases.transform.GetChild(GlobalVariableHandler.Instance.MyIndex).position;
        }
        else
        {
            pos = new Vector3(GlobalVariableHandler.Instance.CellSize /200f,GlobalVariableHandler.Instance.CellSize /200f,0);
        }
        SpawnPlayer(pos);
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
    private void findPath(Vector3 worldPosition)
    {
        allowMove = false;
        counter = 1;
        animationCounter = 0;
        this.path = new List<Vector3>();
        if(worldPosition.x > 0 && worldPosition.y > 0 && worldPosition.x*100 < MapScript.sprites.GetLength(1)*GameLoaderScript.spriteSize && worldPosition.y*100 < MapScript.sprites.GetLength(0)*GameLoaderScript.spriteSize)
        {
            FieldStates playerPos = new FieldStates();
            FieldStates playerTarget = new FieldStates();
            List<Coordinate> cordPath = new List<Coordinate>{};
            int playerX = (int)(player.transform.position.x*100/GameLoaderScript.spriteSize);
            int playerY = (int)(player.transform.position.y*100/GameLoaderScript.spriteSize);
            int targetX = (int)(worldPosition.x*100/GameLoaderScript.spriteSize);
            int targetY = (int)(worldPosition.y*100/GameLoaderScript.spriteSize);
            playerPos = field[playerY, playerX];
            if(playerPos!=FieldStates.Wall)
                field[playerY, playerX] = FieldStates.Start;
            playerTarget = field[targetY, targetX];
            if(playerTarget != FieldStates.Wall)
                field[targetY, targetX] = FieldStates.Finish;
            try
            {
            aStar = new AStar(field);
            cordPath = aStar.solve();
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message);
            }
            for(int i = 0; i < cordPath.Count; i++)
            {
                this.path.Add(new Vector3((cordPath[cordPath.Count-1-i].y+0.5f) * GameLoaderScript.spriteSize/100, (cordPath[cordPath.Count-1-i].x+0.5f) * GameLoaderScript.spriteSize/100, 0.0f));
            }

            field[playerY, playerX] = playerPos;
            field[targetY, targetX] = playerTarget;
        }
        if (path.Count > 0)
        {
            previousStep = path.First();
            allowMove = true;
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Получаем позицию клика мышью в пикселях
            Vector3 mousePosition = Input.mousePosition;

            // Преобразуем позицию из пикселей в мировые координаты
            Vector3 worldPosition = camera.ScreenToWorldPoint(mousePosition);

            if (worldPosition.x > 0 && worldPosition.y > 0 && worldPosition.x * 100 < MapScript.sprites.GetLength(1) * GameLoaderScript.spriteSize && worldPosition.y * 100 < MapScript.sprites.GetLength(0) * GameLoaderScript.spriteSize)
            {
                // Выводим координаты места клика в консоль
                Debug.Log("Mouse Clicked at: " + worldPosition);
                if (MapScript.sprites != null)
                {
                    int targetX = (int)(worldPosition.x * 100 / GameLoaderScript.spriteSize);
                    int targetY = (int)(worldPosition.y * 100 / GameLoaderScript.spriteSize);
                    if (GlobalVariableHandler.Instance.BuildingsField[targetY, targetX] != Convert.ToInt32(Constants.Buildings.None))
                    {
                        Vector3 pos = new Vector3((targetX+0.5f) * GameLoaderScript.spriteSize / 100f, (targetY+0.5f) * GameLoaderScript.spriteSize / 100f,10f);
                        Debug.Log("Destination: " + pos );
                        agent.SetDestination(pos);
                    }
                }
            }
        }
    }
}
