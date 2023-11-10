using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PopUpMenus : MonoBehaviour
{
    public GameObject popupMenu; // ссылка на всплывающее меню
    [SerializeField] private new Camera camera;
    void Update()
    {
        //// Проверяем, была ли нажата левая кнопка мыши
        //if (Input.GetMouseButtonDown(0))
        //{
        //    // Создаем луч из позиции мыши
        //    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    // Проверяем, попал ли луч в наш объект
        //    if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        //    {
        //        // Если да, отображаем всплывающее меню
        //        ShowPopupMenu();
        //    }
        //    else
        //    {
        //        // Если луч не попал в объект, скрываем меню
        //        HidePopupMenu();
        //    }
        //}

    }


    void ShowPopupMenu()
    {
        // Показываем всплывающее меню
        popupMenu.SetActive(true);
    }

    void HidePopupMenu()
    {
        // Скрываем всплывающее меню
        popupMenu.SetActive(false);
    }

    void Start()
    {
    }
}