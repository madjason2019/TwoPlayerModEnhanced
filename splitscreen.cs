using UnityEngine;

namespace TwoPlayerModEnhanced
{
    public class SplitScreen : MonoBehaviour
    {
        [Header("Player Objects")]
        public GameObject player1;
        public GameObject player2;

        [Header("Camera Prefab")]
        public Camera cameraPrefab;

        private Camera cam1;
        private Camera cam2;

        [Header("Settings")]
        public bool verticalSplit = true;   // true = left/right, false = top/bottom
        public float gutterSize = 0.002f;   // small gap between screens

        void Start()
        {
            if (player1 == null || player2 == null)
            {
                Debug.LogError("[SplitScreen] Players not assigned.");
                return;
            }

            SetupCameras();
            PositionCameras();
        }

        private void SetupCameras()
        {
            // Create camera 1
            cam1 = Instantiate(cameraPrefab);
            cam1.name = "Player1Camera";
            cam1.transform.SetParent(transform);

            // Create camera 2
            cam2 = Instantiate(cameraPrefab);
            cam2.name = "Player2Camera";
            cam2.transform.SetParent(transform);

            UpdateViewports();
        }

        private void UpdateViewports()
        {
            if (verticalSplit)
            {
                // Left camera
                cam1.rect = new Rect(
                    0f,
                    0f,
                    0.5f - gutterSize,
                    1f
                );

                // Right camera
                cam2.rect = new Rect(
                    0.5f + gutterSize,
                    0f,
                    0.5f - gutterSize,
                    1f
                );
            }
            else
            {
                // Top camera
                cam1.rect = new Rect(
                    0f,
                    0.5f + gutterSize,
                    1f,
                    0.5f - gutterSize
                );

                // Bottom camera
                cam2.rect = new Rect(
                    0f,
                    0f,
                    1f,
                    0.5f - gutterSize
                );
            }
        }

        private void PositionCameras()
        {
            cam1.transform.position = player1.transform.position + new Vector3(0, 1.5f, -3f);
            cam1.transform.LookAt(player1.transform);

            cam2.transform.position = player2.transform.position + new Vector3(0, 1.5f, -3f);
            cam2.transform.LookAt(player2.transform);
        }

        void LateUpdate()
        {
            if (player1 != null)
            {
                cam1.transform.position = player1.transform.position + new Vector3(0, 1.5f, -3f);
                cam1.transform.LookAt(player1.transform);
            }

            if (player2 != null)
            {
                cam2.transform.position = player2.transform.position + new Vector3(0, 1.5f, -3f);
                cam2.transform.LookAt(player2.transform);
            }
        }
    }
}
