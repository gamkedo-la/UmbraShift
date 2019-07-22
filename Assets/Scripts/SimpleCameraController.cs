using UnityEngine;

namespace UnityTemplateProjects
{
    public class SimpleCameraController : MonoBehaviour
    {
        class CameraState
        {
            public float yaw;
            public float pitch;
            public float roll;
            public float x;
            public float y;
            public float z;

            public void SetFromTransform(Transform t)
            {
                pitch = t.eulerAngles.x;
                yaw = t.eulerAngles.y;
                roll = t.eulerAngles.z;
                x = t.parent.position.x;
                y = t.parent.position.y;
                z = t.parent.position.z;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;
                Vector3 flatTranslation = rotatedTranslation;
                flatTranslation.y = 0;
                flatTranslation.Normalize();
                x += flatTranslation.x;
//                y += flatTranslation.y; // should be 0 anyway
                z += flatTranslation.z;
            }

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
                roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);
                
                x = Mathf.Lerp(x, target.x, positionLerpPct);
                y = Mathf.Lerp(y, target.y, positionLerpPct);
                z = Mathf.Lerp(z, target.z, positionLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(pitch, yaw, roll);
                t.parent.position = new Vector3(x, y, z); // moving pivot, not camera directly
            }
        }

        private bool manualControlled = false;  // true if the below variable is above 0
        private float timeoutRemainingBeforeAutoCam = 0.0f;
        
        CameraState m_TargetCameraState = new CameraState();
        CameraState m_InterpolatingCameraState = new CameraState();

        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        public float boost = 3.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        public float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        public bool invertY = false;

        void OnEnable()
        {
            m_TargetCameraState.SetFromTransform(transform);
            m_InterpolatingCameraState.SetFromTransform(transform);
        }

        Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = new Vector3();
            bool cameraInput = false;
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
                cameraInput = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
                cameraInput = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
                cameraInput = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
                cameraInput = true;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.down;
                cameraInput = true;
            }
            if (Input.GetKey(KeyCode.E))
            {
                direction += Vector3.up;
                cameraInput = true;
            }

            if (cameraInput)
            {
                timeoutRemainingBeforeAutoCam = 3.0f; // needs to be constant
            }
            return direction;
        }
        
        void Update()
        {
            // Exit Sample  
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
				#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false; 
				#endif
            }

            // Hide and lock cursor when right mouse button pressed
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            // Unlock and show cursor when right mouse button released
            if (Input.GetMouseButtonUp(1))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            /*
            // Rotation - does bad things to the camera for this game
            if (Input.GetMouseButton(1))
            {
                var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));
                
                var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            }*/
            var translation = GetInputTranslationDirection() * Time.deltaTime;
            manualControlled = timeoutRemainingBeforeAutoCam > 0.0f;
            if (manualControlled)
            {
                timeoutRemainingBeforeAutoCam -= Time.deltaTime;
                // Translation
                

                // Speed up movement when shift key held
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    translation *= 10.0f;
                }
            
                // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
                boost += Input.mouseScrollDelta.y * 0.2f;
                translation *= Mathf.Pow(2.0f, boost);

                m_TargetCameraState.Translate(translation);
                
                // Framerate-independent interpolation
                // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
                var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
                var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
                m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

                m_InterpolatingCameraState.UpdateTransform(transform);
            }
            else
            {
                if (TurnManager.instance.ActiveCharacter != null)
                {
                    transform.parent.transform.position = TurnManager.instance.ActiveCharacter.transform.position;
                }
            }

        }
    }

}