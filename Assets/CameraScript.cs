using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float cameraSpeed = 5.0f; // Скорость передвижения камеры
    public float screenEdgeBorder = 50.0f; // Ширина "границы" от края экрана
    public float minZoom = 2.0f; // Минимальное увеличение
    public float maxZoom = 10.0f; // Максимальное увеличение
    public float zoomSpeed = 1.0f; // Скорость приближения/отдаления

    private float mapSizeX = (float)GlobalVariableHandler.fieldSizeX;
    private float mapSizeY = (float)GlobalVariableHandler.fieldSizeY;

    private Transform target;
    private bool isFollowing = false;
    void Start()
    {
        target = GameObject.Find("Player").transform;
        transform.position = new Vector3(target.position.x, target.position.y, -10);
    }
    
    void unBind()
    {
        isFollowing = false;
    }
    void bind()
    {
        isFollowing = true;
    }

    void moveCamera()
    {
        Vector3 mousePosition = Input.mousePosition;

        // Проверяем положение мыши относительно края экрана
        if (mousePosition.x <= screenEdgeBorder)
        {
            // Двигаем камеру влево
            transform.Translate(Vector3.left * cameraSpeed * Time.deltaTime);
        }
        else if (mousePosition.x >= Screen.width - screenEdgeBorder)
        {
            // Двигаем камеру вправо
            transform.Translate(Vector3.right * cameraSpeed * Time.deltaTime);
        }

        if (mousePosition.y <= screenEdgeBorder)
        {
            // Двигаем камеру вниз
            transform.Translate(Vector3.down * cameraSpeed * Time.deltaTime);
        }
        else if (mousePosition.y >= Screen.height - screenEdgeBorder)
        {
            // Двигаем камеру вверх
            transform.Translate(Vector3.up * cameraSpeed * Time.deltaTime);
        }

        // Ограничение движения камеры
        Vector3 currentPosition = transform.position;
        currentPosition.x = Mathf.Clamp(currentPosition.x, 0, mapSizeX);
        currentPosition.y = Mathf.Clamp(currentPosition.y, 0, mapSizeY);
        transform.position = currentPosition;
    }

    void zoomCamera()
    {
        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        Camera mainCamera = GetComponent<Camera>();

        if (mainCamera != null)
        {
            float newZoom = mainCamera.orthographicSize - zoom;
            mainCamera.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            bind();
        }
        else if(Input.GetKey(KeyCode.Z))
        {
            unBind();
        }
        zoomCamera();
        if (isFollowing)
        {
            transform.position = target.position + new Vector3(0, 0, -10);
            //Debug.Log(transform.position);
        }
        else
        {
            moveCamera();
        }
    }
}
