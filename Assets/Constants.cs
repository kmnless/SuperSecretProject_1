using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
public class Constants : MonoBehaviour
{
    public enum Buildings 
    {
        None=0,
        Base=1,
        Flag=2,
        Outpost=3,
        Road=4
    
    }
    public const int MENU_SCENE_INDEX = 0;
    public const int GAME_SETTINGS_SCENE_INDEX = 1;
    public const int GAME_SCENE_INDEX = 2;
}
