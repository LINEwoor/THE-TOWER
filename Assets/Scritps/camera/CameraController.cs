using System;
using UnityEngine;

namespace camera
{
    public class CameraController : MonoBehaviour
    {
        public float followSpeed = 5f;
        public float maxOffset = 5f;
        
        public float zoomSpeed = 10f;
        public float minFOV = 20f;
        public float maxFOV = 80f;
        public float smoothTime = 5f;
    
        private Camera cam;
        private float targetFOV;
        private Vector3 startPosition;
    
        void Start()
        {
            startPosition = transform.position;
            cam = GetComponent<Camera>();
            targetFOV = cam.fieldOfView;
        }
    
        void Update()
        {
            HandleZoom();
            HandleFollow();
        }

        void HandleFollow()
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            
            Vector3 mouseViewportPos = cam.ScreenToViewportPoint(mouseScreenPos);
            
            Vector3 offsetFromCenter = new Vector3(
                mouseViewportPos.x - 0.5f,
                0f,
                mouseViewportPos.y - 0.5f
            );
            
            offsetFromCenter *= followSpeed;
            
            offsetFromCenter.x = Mathf.Clamp(offsetFromCenter.x, -maxOffset, maxOffset);
            offsetFromCenter.z = Mathf.Clamp(offsetFromCenter.z, -maxOffset, maxOffset);
            
            Vector3 targetPosition = startPosition + new Vector3(offsetFromCenter.x, 0f, offsetFromCenter.z);
            
            transform.position = Vector3.Lerp(
                transform.position,
                new Vector3(targetPosition.x, transform.position.y, targetPosition.z),
                smoothTime * Time.deltaTime
            );
        }
        
        void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                targetFOV -= scroll * zoomSpeed;
                targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
            }
        
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, smoothTime * Time.deltaTime);
        }
    }
}