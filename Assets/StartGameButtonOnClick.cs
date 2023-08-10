using UnityEngine;

public class StartGameButtonOnClick : MonoBehaviour
{
    public SceneHandler sceneHandler;

    public void OnClick()
    {
        sceneHandler.ChangeState("Game");
    }
}
