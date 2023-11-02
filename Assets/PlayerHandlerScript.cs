using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandlerScript : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private new Camera camera;
    private FieldStates[,] field;
    private List<Vector3> path; 
    private GameObject player;
    private AStar aStar;
    private float counter=0;
    public void createPlayer(Vector3 pos)
    {
        player = Instantiate(playerPrefab, pos + new Vector3(0 + GameLoaderScript.spriteSize/200,0 + GameLoaderScript.spriteSize/200,10), Quaternion.identity);
        player.name = "Player";
    }

    public void Awake()
    {
        createPlayer(new(0,0,-10));
    }
    public void Start()
    {
        field = new FieldStates[GlobalVariableHandler.fieldSizeY, GlobalVariableHandler.fieldSizeX];
        //MapScript.sprites[0,1].SetActive(false);
        for(int y = 0; y < GlobalVariableHandler.fieldSizeY; y++)
        {
            for(int x = 0; x < GlobalVariableHandler.fieldSizeX; x++)
            {
                Debug.Log($"{GlobalVariableHandler.terrainField[y, x]} on x={x} on y={y}");
                if(GlobalVariableHandler.terrainField[y, x]>=-0.1f && GlobalVariableHandler.terrainField[y, x]<=0.1f)
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
    public void FixedUpdate()
    {
        if(path!=null && path.Count > 0){
            player.transform.position = path[(int)counter];
            if(counter<path.Count-1)
            {
                counter+=0.01f;
            }
            else{ counter=0f; }
        }
    }
    private void findPath(Vector3 worldPosition)
    {
        this.path = new List<Vector3>();
        if(worldPosition.x > 0 && worldPosition.y > 0 && worldPosition.x*100 < MapScript.sprites.GetLength(1)*GameLoaderScript.spriteSize && worldPosition.y*100 < MapScript.sprites.GetLength(0)*GameLoaderScript.spriteSize)
        {
            int playerX = (int)(player.transform.position.x*100/GameLoaderScript.spriteSize);
            int playerY = (int)(player.transform.position.y*100/GameLoaderScript.spriteSize);
            int targetX = (int)(worldPosition.x*100/GameLoaderScript.spriteSize);
            int targetY = (int)(worldPosition.y*100/GameLoaderScript.spriteSize);
            field[playerY, playerX] = FieldStates.Start;
            field[targetY, targetX] = FieldStates.Finish;
            aStar = new AStar(field);
            List<Coordinate> cordPath = aStar.solve();
            for(int i = 0; i < cordPath.Count; i++)
            {
                this.path.Add(new Vector3((cordPath[cordPath.Count-1-i].y+0.5f) * GameLoaderScript.spriteSize/100, (cordPath[cordPath.Count-1-i].x+0.5f) * GameLoaderScript.spriteSize/100, 0.0f));
            }
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

            // Выводим координаты места клика в консоль
            Debug.Log("Mouse Clicked at: " + worldPosition);
            if(MapScript.sprites != null)
            {
                findPath(worldPosition);
            }
        }
    }
}
