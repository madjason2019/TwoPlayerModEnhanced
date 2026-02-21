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

        // NEW: Split-screen system
        private SplitScreen splitScreen;

        public TwoPlayerMod()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
            Interval = 0;

            // NEW: Initialize split-screen
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

            // NEW: Activate split-screen
            if (player1 != null && player2 != null)
            {
                splitScreen.Enable(player1, player2);
            }

            UI.Notify("TwoPlayerMod Enabled (Split-Screen Active)");
        }

        private void DisableMod()
        {
            // NEW: Disable split-screen
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

                // NEW: Re-enable split-screen after respawn
                if (player1 != null && player2 != null)
                {
                    splitScreen.Enable(player1, player2);
                }
            }

            // NEW: Update split-screen cameras every frame
            splitScreen.Update();

            // Legacy Player 2 control system remains untouched
            UpdatePlayer2Controls();
        }

        // This calls your existing legacy control files (movement, weapons, vehicles)
        private void UpdatePlayer2Controls()
        {
            if (player2 == null || !player2.Exists())
                return;

            // If Player 2 is in a vehicle, use vehicle controller
            if (player2.IsInVehicle())
            {
                // Legacy vehicle control logic
                Function.Call(Hash.TASK_VEHICLE_TEMP_ACTION, player2, player2.CurrentVehicle, 1, 100);
            }
            else
            {
                // Legacy on-foot movement logic
                float lx = Function.Call<float>(Hash.GET_CONTROL_NORMAL, 0, 218); // Left stick X
                float ly = Function.Call<float>(Hash.GET_CONTROL_NORMAL, 0, 219); // Left stick Y

                Vector3 move = (player2.ForwardVector * -ly) + (player2.RightVector * lx);
                player2.Task.RunTo(player2.Position + move);
            }
        }
    }
}
