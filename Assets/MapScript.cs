using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class MapScript : MonoBehaviour
{

    static public GameObject[,] sprites;
    private static void texturize(double perlinHeight, int buildingType, GameObject obj, float spriteSize, bool near)
    {
        Texture2D[] textures = GlobalVariableHandler.Instance.Textures;
        obj.AddComponent<SpriteRenderer>();
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        obj.AddComponent<NavMeshPlus.Components.NavMeshModifier>();
        var walk = obj.GetComponent<NavMeshPlus.Components.NavMeshModifier>();
        walk.overrideArea = true;
        if (buildingType != Convert.ToInt32(Constants.Buildings.None)) 
        {
            spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadTexture, new(0, 0, GlobalVariableHandler.Instance.RoadTexture.width, GlobalVariableHandler.Instance.RoadTexture.height), new(0.0f, 0.0f));
            walk.area = 0;
        }
        else
        {
            int height = Convert.ToInt32(255 * (perlinHeight + 1.0) / 2.0);
            if (height <= GlobalVariableHandler.Instance.Waterline)
            {
                spriteRenderer.sprite = Sprite.Create(textures.First(), new(0, 0, textures.First().width, textures.First().height), new(0.0f, 0.0f));
                walk.area = 1;
                return;
            }
            else if (height >= 255 - GlobalVariableHandler.Instance.MountainLine)
            {
                spriteRenderer.sprite = Sprite.Create(textures.Last(), new(0, 0, textures.Last().width, textures.Last().height), new(0.0f, 0.0f));
                walk.area = 1;
                return;

            }
            int range = 255 - GlobalVariableHandler.Instance.Waterline - GlobalVariableHandler.Instance.MountainLine;
            int singleInterval = range / (textures.Count() - 2);
            spriteRenderer.sprite = Sprite.Create(textures[(height - GlobalVariableHandler.Instance.Waterline) / singleInterval + 1], new(0, 0, textures[(height - GlobalVariableHandler.Instance.Waterline) / singleInterval + 1].width, textures[(height - GlobalVariableHandler.Instance.Waterline) / singleInterval + 1].height), new(0.0f, 0.0f));
            if (near) 
            {
                walk.area = 0;
            }
            else 
            {
                walk.area = 1;
            }
        }
    }
    private static bool NearRoad(int x, int y, int[,] buldings) 
    {
        for(int i = y - 1; i < y + 2; i++) 
        {
            for(int j = x - 1; j < x + 2; j++) 
            {
                if (i >= 0 && i < buldings.GetLength(0) && j >= 0 && j < buldings.GetLength(1)) 
                {
                    if (buldings[i,j] != Convert.ToInt32(Constants.Buildings.None))
                    {
                        return true;
                    }
                }
            }
        }
        return false ;
    }
    public static void CreateSpriteMap(int sizeX, int sizeY, double[,] terrain, int[,] buldings,float spriteSize,GameObject map)
    {
        sprites=new GameObject[sizeY,sizeX];
        //GameObject walkable = new GameObject();
        //walkable.AddComponent<SpriteRenderer>();

        //var sprite = walkable.GetComponent<SpriteRenderer>();
        //sprite.sprite = Sprite.Create(GlobalVariableHandler.roadTexture, new(0, 0, GlobalVariableHandler.roadTexture.width, GlobalVariableHandler.roadTexture.height), new(0.0f, 0.0f));
        //walkable.transform.localScale = new Vector3(sizeX,sizeY,1.0f);
        //walkable.AddComponent<NavMeshPlus.Components.NavMeshModifier>();
        //walkable.name = "Walkable";
        //walkable.transform.parent = map.transform;
        for(int i = 0; i < sizeY; ++i)
        {
            for (int j = 0; j < sizeX; ++j) 
            {
                sprites[i, j] = new GameObject();
                texturize(terrain[i, j], buldings[i, j], sprites[i, j], spriteSize, NearRoad(j,i,buldings));
                sprites[i, j].transform.position= new Vector3(j*spriteSize/100.0f,i*spriteSize/100.0f, 10.0f);
                sprites[i, j].transform.parent = map.transform;
            }
        }
        
    }   
    public static void CreateEntities(int sizeX, int sizeY,int[,] buldings, float spriteSize, GameObject bases, GameObject flags, GameObject outposts)
    {
        int baseIndex = 0;
        int flagIndex = 0;
        int outpostIndex = 0;
        GameObject buffer;
        for(int i = 0; i < sizeY; ++i)
        {
            for (int j = 0; j < sizeX; ++j) 
            {
                switch(buldings[i,j])
                {
                    case (int)Constants.Buildings.None:
                        break;
                    case (int)Constants.Buildings.Base:
                        buffer = Instantiate(GlobalVariableHandler.Instance.BasePrefab, new Vector3((j+0.5f)*spriteSize/100.0f,(i+0.5f)*spriteSize/100.0f, -1.0f), Quaternion.identity);
                        buffer.transform.parent = bases.transform;
                        buffer.name = "Base" + baseIndex.ToString();
                        BaseHandler prop=buffer.GetComponent<BaseHandler>();
                        prop.setId(baseIndex);
                        //prop.setName(GlobalVariableHandler.players[baseIndex++].Name); todo dobavit names ot igrokov!!!
                        break;
                    case (int)Constants.Buildings.Flag:
                        buffer = Instantiate(GlobalVariableHandler.Instance.FlagPrefab, new Vector3((j+0.5f)*spriteSize/100.0f,(i+0.5f)*spriteSize/100.0f, -1.0f), Quaternion.identity);
                        buffer.transform.parent = flags.transform;
                        buffer.name = "Flag" + flagIndex++.ToString();
                        break;
                    case (int)Constants.Buildings.Outpost:
                        buffer = Instantiate(GlobalVariableHandler.Instance.OutpostPrefab, new Vector3((j+0.5f)*spriteSize/100.0f,(i+0.5f)*spriteSize/100.0f, -1.0f), Quaternion.identity);
                        buffer.transform.parent = outposts.transform;
                        buffer.name = "Outpost" + outpostIndex++.ToString();
                        break;
                }
            }
        }
    }
}
