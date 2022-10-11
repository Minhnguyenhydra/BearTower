using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float farClipPlane = 10f;
    [SerializeField] private float startOthorsize = 13f;
    [SerializeField] private Vector3 startCamPos = new Vector3(-14, 1, -10);
    private Camera _mainCamera;

    private Camera mainCamera
    {
        get
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            return _mainCamera;
        }
    }
    void LateUpdate()
    {
        Vector3 curCamPos = mainCamera.transform.position;
        Vector3 offSet = curCamPos - startCamPos;
        for (int i = 0; i < transform.childCount; i++)
        {
            //Position
            Transform layer = transform.GetChild(i);
            Vector3 newPos = offSet - (curCamPos - startCamPos) * (farClipPlane - layer.localPosition.z) / farClipPlane;
            newPos.z = layer.localPosition.z;
            layer.localPosition = newPos;
            //Scale
            layer.localScale = Vector3.one * (1 - (1 - mainCamera.orthographicSize / startOthorsize) * layer.localPosition.z / farClipPlane);
            newPos = curCamPos + (layer.position - curCamPos) * layer.localScale.x;
            newPos.z = layer.position.z;
            layer.position = newPos;
        }
    }
}