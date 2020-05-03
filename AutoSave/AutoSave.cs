using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using System;
using UnityEngine.UI;

namespace AutoSave
{
    public class AutoSave : Mod
    {
        public override string ID => "AutoSave"; //Your mod ID (unique)
        public override string Name => "AutoSave"; //You mod name
        public override string Author => "MLDKYT"; //Your Username
        public override string Version => "1.3"; //Version

        public Keybind setupMenu;

        public static float coolDownSetting = 60;

        private static bool settingsLoaded = false;

        // Set this to true if you will be load custom assets from Assets folder.
        // This will create subfolder in Assets folder for your mod.
        public override bool UseAssetsFolder => false;

        public override void OnNewGame()
        {
            // Called once, when starting a New Game, you can reset your saves here
        }

        public override void OnLoad()
        {
            setupMenu = new Keybind("menu", "Open Menu", KeyCode.A, KeyCode.LeftAlt);
        }

        public override void ModSettings()
        {
            Settings.AddHeader(this, "Settings", Color.black, Color.white);
            Settings.AddSlider(this, new Settings("interval", "Interval", new Action(settingsChanged)), 30, 7200);
        }

        public static void settingsChanged()
        {
            ModConsole.Print(new Settings("interval", "Interval", new Action(settingsChanged)).GetValue());
        }

        public override void OnMenuLoad()
        {
            if (!settingsLoaded)
            {
                ModConsole.Print("Set up settings.");
            }
        }

        public override void OnSave()
        {
            // Called once, when save and quit
            // Serialize your save file here.
        }

        public override void OnGUI()
        {
            // Draw unity OnGUI() here
        }

        private float cooldown = 0;

        public override void Update()
        {
            // Update is called once per frame
            if (FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle").Value == "")
            {
                cooldown += Time.deltaTime;
            }

            if (cooldown > coolDownSetting && FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle").Value == "")
            {
                cooldown = 0;
                PlayMakerFSM.BroadcastEvent("SAVEGAME");
                Application.LoadLevelAsync(1);
                Application.LoadLevelAsync(3);
            }
        }
    }
}
