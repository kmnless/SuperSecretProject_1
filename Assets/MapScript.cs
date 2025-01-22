using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static Constants;

public class MapScript : MonoBehaviour
{

    static public GameObject[,] sprites;
    private static int GetRoadType(int x, int y, int[,] buildings)
    {
        bool up = y > 0 && buildings[y - 1, x] == (int)Constants.Buildings.Road;
        bool down = y < buildings.GetLength(0) - 1 && buildings[y + 1, x] == (int)Constants.Buildings.Road;
        bool left = x > 0 && buildings[y, x - 1] == (int)Constants.Buildings.Road;
        bool right = x < buildings.GetLength(1) - 1 && buildings[y, x + 1] == (int)Constants.Buildings.Road;

        // Горизонтальная дорога
        if (left && right && !up && !down) return 1;

        // Вертикальная дорога
        if (up && down && !left && !right) return 2;

        // Повороты
        if (left && down && !right && !up) return 3; // Нижний-левый
        if (right && down && !left && !up) return 4; // Нижний-правый
        if (left && up && !right && !down) return 5; // Верхний-левый
        if (right && up && !left && !down) return 6; // Верхний-правый

        // Т-образные перекрестки
        if (up && left && right && !down) return 7; // Верх
        if (down && left && right && !up) return 8; // Низ
        if (left && up && down && !right) return 9; // Лево
        if (right && up && down && !left) return 10; // Право

        // Перекресток
        if (up && down && left && right) return 11;

        return 0; // Одиночная клетка дороги (по умолчанию)
    }
    private static void texturize(double perlinHeight, int buildingType, GameObject obj, float spriteSize, bool near, int x, int y, int[,] buldings)
    {
        obj.AddComponent<SpriteRenderer>();
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        obj.AddComponent<NavMeshPlus.Components.NavMeshModifier>();
        var walk = obj.GetComponent<NavMeshPlus.Components.NavMeshModifier>();
        walk.overrideArea = true;

        if (buildingType == (int)Constants.Buildings.Road)
        {
            int roadType = GetRoadType(x, y, buldings);
            walk.area = 0;
            switch (roadType)
            {
                case 1: // Горизонтальная дорога
                    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadHorizontalTexture, new Rect(0, 0, GlobalVariableHandler.Instance.RoadHorizontalTexture.width, GlobalVariableHandler.Instance.RoadHorizontalTexture.height), new Vector2(0.5f, 0.5f));
                    break;
                case 2: // Вертикальная дорога
                    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadVerticalTexture, new Rect(0, 0, GlobalVariableHandler.Instance.RoadVerticalTexture.width, GlobalVariableHandler.Instance.RoadVerticalTexture.height), new Vector2(0.5f, 0.5f));
                    break;
                case 3: // Поворот: нижний-левый
                    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadCornerBLTexture, new Rect(0, 0, GlobalVariableHandler.Instance.RoadCornerBLTexture.width, GlobalVariableHandler.Instance.RoadCornerBLTexture.height), new Vector2(0.5f, 0.5f));
                    break;
                case 4: // Поворот: нижний-правый
                    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadCornerBRTexture, new Rect(0, 0, GlobalVariableHandler.Instance.RoadCornerBRTexture.width, GlobalVariableHandler.Instance.RoadCornerBRTexture.height), new Vector2(0.5f, 0.5f));
                    break;
                case 5: // Поворот: верхний-левый
                    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadCornerTLTexture, new Rect(0, 0, GlobalVariableHandler.Instance.RoadCornerTLTexture.width, GlobalVariableHandler.Instance.RoadCornerTLTexture.height), new Vector2(0.5f, 0.5f));
                    break;
                case 6: // Поворот: верхний-правый
                    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadCornerTRTexture, new Rect(0, 0, GlobalVariableHandler.Instance.RoadCornerTRTexture.width, GlobalVariableHandler.Instance.RoadCornerTRTexture.height), new Vector2(0.5f, 0.5f));
                    break;
                case 7: // Т-образный перекресток: верх
                    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadTUpTexture, new Rect(0, 0, GlobalVariableHandler.Instance.RoadTUpTexture.width, GlobalVariableHandler.Instance.RoadTUpTexture.height), new Vector2(0.5f, 0.5f));
                    break;
                case 8: // Т-образный перекресток: низ
                    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadTDownTexture, new Rect(0, 0, GlobalVariableHandler.Instance.RoadTDownTexture.width, GlobalVariableHandler.Instance.RoadTDownTexture.height), new Vector2(0.5f, 0.5f));
                    break;
                case 9: // Т-образный перекресток: лево
                    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadTLeftTexture, new Rect(0, 0, GlobalVariableHandler.Instance.RoadTLeftTexture.width, GlobalVariableHandler.Instance.RoadTLeftTexture.height), new Vector2(0.5f, 0.5f));
                    break;
                case 10: // Т-образный перекресток: право
                    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadTRightTexture, new Rect(0, 0, GlobalVariableHandler.Instance.RoadTRightTexture.width, GlobalVariableHandler.Instance.RoadTRightTexture.height), new Vector2(0.5f, 0.5f));
                    break;
                case 11: // Перекресток
                    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadCrossTexture, new Rect(0, 0, GlobalVariableHandler.Instance.RoadCrossTexture.width, GlobalVariableHandler.Instance.RoadCrossTexture.height), new Vector2(0.5f, 0.5f));
                    break;
                default: // Одиночная клетка дороги
                    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadTexture, new Rect(0, 0, GlobalVariableHandler.Instance.RoadTexture.width, GlobalVariableHandler.Instance.RoadTexture.height), new Vector2(0.5f, 0.5f));
                    break;
            }
        }
        //if (buildingType != Convert.ToInt32(Constants.Buildings.None)) 
        //{
        //    spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.RoadTexture, new(0, 0, GlobalVariableHandler.Instance.RoadTexture.width, GlobalVariableHandler.Instance.RoadTexture.height), new(0.0f, 0.0f));
        //    walk.area = 0;
        //}
        else
        {
            int height = Convert.ToInt32(255 * (perlinHeight + 1.0) / 2.0);
            if (height <= GlobalVariableHandler.Instance.Waterline)
            {
                spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.WaterTexture, new(0, 0, GlobalVariableHandler.Instance.WaterTexture.width, GlobalVariableHandler.Instance.WaterTexture.height), new(0.0f, 0.0f));
                walk.area = 1;
                return;
            }
            else if (height >= 255 - GlobalVariableHandler.Instance.MountainLine)
            {
                spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.MountainTexture, new(0, 0, GlobalVariableHandler.Instance.MountainTexture.width, GlobalVariableHandler.Instance.MountainTexture.height), new(0.0f, 0.0f));
                walk.area = 1;
                return;

            }
            int range = 255 - GlobalVariableHandler.Instance.Waterline - GlobalVariableHandler.Instance.MountainLine;
            
            // ??? ETO CHE VOOBSHE, MI POHODU POD NARCOTICAMI PISALI ETO
            //int singleInterval = range / (textures.Count() - 2);
            //spriteRenderer.sprite = Sprite.Create(textures[(height - GlobalVariableHandler.Instance.Waterline) / singleInterval + 1], new(0, 0, textures[(height - GlobalVariableHandler.Instance.Waterline) / singleInterval + 1].width, textures[(height - GlobalVariableHandler.Instance.Waterline) / singleInterval + 1].height), new(0.0f, 0.0f));
            
            spriteRenderer.sprite = Sprite.Create(GlobalVariableHandler.Instance.GrassTexture, new(0, 0, GlobalVariableHandler.Instance.GrassTexture.width, GlobalVariableHandler.Instance.GrassTexture.height), new(0.0f, 0.0f));
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
    public static void CreateSpriteMap(int sizeX, int sizeY, double[,] terrain, int[,] buldings, float spriteSize, GameObject map)
    {
        sprites = new GameObject[sizeY, sizeX];
        //GameObject walkable = new GameObject();
        //walkable.AddComponent<SpriteRenderer>();

        //var sprite = walkable.GetComponent<SpriteRenderer>();
        //sprite.sprite = Sprite.Create(GlobalVariableHandler.roadTexture, new(0, 0, GlobalVariableHandler.roadTexture.width, GlobalVariableHandler.roadTexture.height), new(0.0f, 0.0f));
        //walkable.transform.localScale = new Vector3(sizeX,sizeY,1.0f);
        //walkable.AddComponent<NavMeshPlus.Components.NavMeshModifier>();
        //walkable.name = "Walkable";
        //walkable.transform.parent = map.transform;
        for (int i = 0; i < sizeY; ++i)
        {
            for (int j = 0; j < sizeX; ++j)
            {
                sprites[i, j] = new GameObject();
                texturize(terrain[i, j], buldings[i, j], sprites[i, j], spriteSize, NearRoad(j, i, buldings), j, i, buldings);
                sprites[i, j].transform.position = new Vector3(j * spriteSize / 100.0f, i * spriteSize / 100.0f, 10.0f);
                sprites[i, j].transform.parent = map.transform;
            }
        }
    }
    public static void CreateEntities(int sizeX, int sizeY, int[,] buldings, float spriteSize, GameObject bases, GameObject flags, GameObject outposts)
    {
        int baseIndex = 0;
        int flagIndex = 0;
        int outpostIndex = 0;
        GameObject buffer;
        for (int i = 0; i < sizeY; ++i)
        {
            for (int j = 0; j < sizeX; ++j)
            {
                switch (buldings[i, j])
                {
                    case (int)Constants.Buildings.None:
                        break;
                    case (int)Constants.Buildings.Base:
                        var pos = new Vector3((j + 0.5f) * spriteSize / 100.0f, (i + 0.5f) * spriteSize / 100.0f, -1.0f);
                        buffer = Instantiate(GlobalVariableHandler.Instance.BasePrefab, pos, Quaternion.identity);
                        buffer.transform.parent = bases.transform;
                        buffer.name = "Base " + GlobalVariableHandler.Instance.Players[baseIndex];
                        GameManager.basePositions.Add(pos);
                        BaseHandler baseProp = buffer.GetComponent<BaseHandler>();
                        // tut krch nado podumat, id navernoe prosto sdelat ++, a name budet name playera, kotoriy vladeet bazoy. ili hranit gde to otdelno kto kem vladeet.
                        baseProp.Id = baseIndex++;
                        //prop.Name = GlobalVariableHandler.Instance.Players[baseIndex++].Name.ToString(); // id mojet ne sovpadat, hz krch, nado testit (ya zabudu)
                        break;
                    case (int)Constants.Buildings.Flag:
                        buffer = Instantiate(GlobalVariableHandler.Instance.FlagPrefab, new Vector3((j + 0.5f) * spriteSize / 100.0f, (i + 0.5f) * spriteSize / 100.0f, -1.0f), Quaternion.identity);
                        buffer.transform.parent = flags.transform;
                        buffer.name = "Flag" + flagIndex.ToString();
                        FlagHandler flagProp = buffer.GetComponent<FlagHandler>();
                        flagProp.flagId = flagIndex++;
                        break;
                    case (int)Constants.Buildings.Outpost:
                        buffer = Instantiate(GlobalVariableHandler.Instance.OutpostPrefab, new Vector3((j + 0.5f) * spriteSize / 100.0f, (i + 0.5f) * spriteSize / 100.0f, -1.0f), Quaternion.identity);
                        buffer.transform.parent = outposts.transform;
                        buffer.name = "Outpost" + outpostIndex.ToString();
                        OutpostHandler outpostProp = buffer.GetComponent<OutpostHandler>();
                        outpostProp.outpostId = outpostIndex++;
                        break;
                }
            }
        }
    }
}
