using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    private string Name;
    private int Id;
    public float Delay;

    public void ChangeState(string sceneName)
    {
        Name = sceneName;
        Invoke("RoutineLoadScene", Delay);
    }
    public void ChangeState(int id)
    {
        Id=id;
        Invoke("RoutineLoadSceneId", Delay);
    }
    private void RoutineLoadScene()
    {
        SceneManager.LoadScene(Name);
    }
    private void RoutineLoadSceneId()
    {
        SceneManager.LoadScene(Id);
    }
}
