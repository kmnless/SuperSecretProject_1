using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectManager : MonoBehaviour
{
    // Статическое поле для хранения ссылки на экземпляр синглтона
    private static ObjectManager _instance;

    // Ссылка на ваш объект
    public GameObject client;
    public GameObject server;

    // Метод для получения ссылки на экземпляр синглтона
    public static ObjectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Попробуйте найти существующий экземпляр
                _instance = FindObjectOfType<ObjectManager>();

                // Если не найден, создайте новый
                if (_instance == null)
                {
                    GameObject obj = new GameObject("ObjectManager");
                    _instance = obj.AddComponent<ObjectManager>();
                }
            }

            return _instance;
        }
    }

    // Метод для установки объекта
    public void setClient(GameObject obj)
    {
        client = obj;
    }
    public void setServer(GameObject obj)
    {
        server = obj;
    }
}
