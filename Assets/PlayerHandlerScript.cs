using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandlerScript : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private new Camera camera;
    private GameObject player;
    public void createPlayer(Vector3 pos)
    {
        player = Instantiate(playerPrefab, pos + new Vector3(0,0,10), Quaternion.identity);
        player.name = "Player";
    }

    public void Awake()
    {
        createPlayer(new(0,0,-10));
    }
    public void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Получаем позицию клика мышью в пикселях
            Vector3 mousePosition = Input.mousePosition;

            // Преобразуем позицию из пикселей в мировые координаты
            Vector3 worldPosition = camera.ScreenToWorldPoint(mousePosition);

            // Выводим координаты места клика в консоль
            Debug.Log("Mouse Clicked at: " + worldPosition);
            if(MapScript.sprites != null)
            {
                if(worldPosition.x > 0 && worldPosition.y > 0 && worldPosition.x*100 < MapScript.sprites.GetLength(1)*GameLoaderScript.spriteSize && worldPosition.y*100 < MapScript.sprites.GetLength(0)*GameLoaderScript.spriteSize)
                {
                    MapScript.sprites[(int)(worldPosition.y*100/GameLoaderScript.spriteSize),(int)(worldPosition.x*100/GameLoaderScript.spriteSize)].SetActive(false);
                }
            }
        }
    }
}
