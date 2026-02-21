using GTA;
using GTA.Math;
using GTA.Native;

namespace TwoPlayerModEnhanced
{
    public class SplitScreen
    {
        private Camera cam1;
        private Camera cam2;

        private Ped player1;
        private Ped player2;

        private bool enabled = false;

        public SplitScreen()
        {
        }

        public void Enable(Ped p1, Ped p2)
        {
            player1 = p1;
            player2 = p2;

            if (player1 == null || player2 == null)
                return;

            CreateCameras();
            SetupViewports();

            enabled = true;
        }

        private void CreateCameras()
        {
            cam1 = World.CreateCamera(player1.Position + new Vector3(0, -4, 1.5f), Vector3.Zero, 60f);
            cam2 = World.CreateCamera(player2.Position + new Vector3(0, -4, 1.5f), Vector3.Zero, 60f);

            cam1.IsActive = true;
            cam2.IsActive = true;

            World.RenderingCamera = cam1;
        }

        private void SetupViewports()
        {
            Function.Call(Hash.SET_SPLIT_SCREEN, true);

            // Left side (Player 1)
            Function.Call(Hash.SET_CAM_SPLIT_SCREEN_MULTIPLIER,
                cam1.Handle, 0.0f, 0.0f, 0.5f, 1.0f);

            // Right side (Player 2)
            Function.Call(Hash.SET_CAM_SPLIT_SCREEN_MULTIPLIER,
                cam2.Handle, 0.5f, 0.0f, 1.0f, 1.0f);
        }

        public void Update()
        {
            if (!enabled)
                return;

            if (player1 != null)
            {
                cam1.Position = player1.Position + new Vector3(0, -4, 1.5f);
                cam1.PointAt(player1);
            }

            if (player2 != null)
            {
                cam2.Position = player2.Position + new Vector3(0, -4, 1.5f);
                cam2.PointAt(player2);
            }
        }

        public void Disable()
        {
            enabled = false;

            Function.Call(Hash.SET_SPLIT_SCREEN, false);

            if (cam1 != null) cam1.Delete();
            if (cam2 != null) cam2.Delete();

            World.RenderingCamera = null;
        }
    }
}
