using UnityEngine;

public class StartGameButtonOnClick : MonoBehaviour
{
    public SceneHandler sceneHandler;

    public void OnClick()
    {
        sceneHandler.ChangeState(Constants.GAME_SETTINGS_SCENE_INDEX);
    }
}
