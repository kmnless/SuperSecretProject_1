using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    private string Name;

    public float Delay;

    public void ChangeState(string sceneName)
    {
        Name = sceneName;
        Invoke("RoutineLoadScene", Delay);
    }

    private void RoutineLoadScene()
    {
        SceneManager.LoadScene(Name);
    }
}
