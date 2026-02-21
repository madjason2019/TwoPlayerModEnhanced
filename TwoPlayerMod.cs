using System;
using GTA;
using GTA.Math;
using GTA.Native;

namespace TwoPlayerModEnhanced
{
    public class TwoPlayerMod : Script
    {
        private Ped player1;
        private Ped player2;

        private bool modEnabled = false;
        private bool player2Spawned = false;

        private SplitScreen splitScreen;

        public TwoPlayerMod()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
            Interval = 0;

            splitScreen = new SplitScreen();
        }

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Toggle mod with F11
            if (e.KeyCode == System.Windows.Forms.Keys.F11)
            {
                modEnabled = !modEnabled;

                if (modEnabled)
                {
                    EnableMod();
                }
                else
                {
                    DisableMod();
                }
            }
        }

        private void EnableMod()
        {
            player1 = Game.Player.Character;

            if (!player2Spawned)
            {
                SpawnPlayer2();
            }

            if (player1 != null && player2 != null)
            {
                splitScreen.Enable(player1, player2);
            }

            UI.Notify("TwoPlayerMod Enabled (Split-Screen Active)");
        }

        private void DisableMod()
        {
            splitScreen.Disable();
            CleanupPlayer2();
            modEnabled = false;

            UI.Notify("TwoPlayerMod Disabled");
        }

        private void SpawnPlayer2()
        {
            if (player2 != null && player2.Exists())
                return;

            player1 = Game.Player.Character;

            Vector3 spawnPos = player1.Position + player1.ForwardVector * 2.0f;
            player2 = World.CreatePed(PedHash.Michael, spawnPos);

            if (player2 != null)
            {
                player2.Health = 200;
                player2.Armor = 100;
                player2.CanSwitchWeapons = true;
                player2.Task.ClearAllImmediately();
                player2Spawned = true;
            }
        }

        private void CleanupPlayer2()
        {
            if (player2 != null && player2.Exists())
            {
                player2.MarkAsNoLongerNeeded();
                player2.Delete();
            }

            player2Spawned = false;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (!modEnabled)
                return;

            if (player1 == null || !player1.Exists())
            {
                player1 = Game.Player.Character;
            }

            if (player2 == null || !player2.Exists())
            {
                player2Spawned = false;
                SpawnPlayer2();

                if (player1 != null && player2 != null)
                {
                    splitScreen.Enable(player1, player2);
                }
            }

            // Update split-screen cameras
            splitScreen.Update();

            // Basic placeholder for Player 2 movement
            BasicPlayer2Follow();
        }

        private void BasicPlayer2Follow()
        {
            if (player1 == null || player2 == null)
                return;

            float distance = player1.Position.DistanceTo(player2.Position);

            if (distance > 10f)
            {
                player2.Task.RunTo(player1.Position);
            }
        }
    }

    // -------------------------
    // Split-Screen System
    // -------------------------

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

            if (player1 != null && player1.Exists())
            {
                cam1.Position = player1.Position + new Vector3(0, -4, 1.5f);
                cam1.PointAt(player1);
            }

            if (player2 != null && player2.Exists())
            {
                cam2.Position = player2.Position + new Vector3(0, -4, 1.5f);
                cam2.PointAt(player2);
            }
        }

        public void Disable()
        {
            enabled = false;

            Function.Call(Hash.SET_SPLIT_SCREEN, false);

            if (cam1 != null && cam1.Exists()) cam1.Delete();
            if (cam2 != null && cam2.Exists()) cam2.Delete();

            World.RenderingCamera = null;
        }
    }
}
