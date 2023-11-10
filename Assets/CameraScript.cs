using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float cameraSpeed = 7.5f; // �������� ������������ ������
    public float screenEdgeBorder = 50.0f; // ������ "�������" �� ���� ������
    public float minZoom = 1.5f; // ����������� ����������
    public float maxZoom = 10.0f; // ������������ ����������
    public float zoomSpeed = 1.0f; // �������� �����������/���������

    private float mapSizeX = (float)GlobalVariableHandler.fieldSizeX*GlobalVariableHandler.cellSize/100f;
    private float mapSizeY = (float)GlobalVariableHandler.fieldSizeY*GlobalVariableHandler.cellSize/100f;

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

        // ��������� ��������� ���� ������������ ���� ������
        if (mousePosition.x <= screenEdgeBorder)
        {
            // ������� ������ �����
            transform.Translate(Vector3.left * cameraSpeed * Time.deltaTime);
        }
        else if (mousePosition.x >= Screen.width - screenEdgeBorder)
        {
            // ������� ������ ������
            transform.Translate(Vector3.right * cameraSpeed * Time.deltaTime);
        }

        if (mousePosition.y <= screenEdgeBorder)
        {
            // ������� ������ ����
            transform.Translate(Vector3.down * cameraSpeed * Time.deltaTime);
        }
        else if (mousePosition.y >= Screen.height - screenEdgeBorder)
        {
            // ������� ������ �����
            transform.Translate(Vector3.up * cameraSpeed * Time.deltaTime);
        }

        // ����������� �������� ������
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
