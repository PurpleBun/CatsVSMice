using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 
 [RequireComponent(typeof(Camera))]
 public class ObserverCamera : MonoBehaviour
 {
    public List<Transform> targets;

    public Vector3 offset;
    public float smoothTime = .5f;

    public float maxZoom = 40f;
    public float minZoom = 10f;
    public float zoomLimiter = 50f;

    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (targets.Count == 0)
            return;

        Move();
        Zoom();
    }

    //Zoom in or out based on the distance between fartest away cats and/or mice
    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    //Smooth follow in center of fartest distance of cats and/or mice
    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    //Distance between fartest away cats and/or mice
    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }

    //Get Center of fartest distance of cats and/or mice
    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
 }