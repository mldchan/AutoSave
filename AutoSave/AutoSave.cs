using System;
using System.IO;
using System.Xml.Serialization;
using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace AutoSave
{
   public class AutoSave : Mod
   {
      private static float _coolDownSetting = 60;

      private static readonly bool _settingsLoaded = false;

      private float _cooldown;

      private Settings _intervalSettings;

      private Keybind _setupMenu;
      public override string ID => "AutoSave";
      public override string Name => "AutoSave";
      public override string Author => "MLDKYT";
      public override string Version => "1.3";

      public override bool UseAssetsFolder => false;

      public override void OnNewGame()
      {
      }

      public override void OnLoad()
      {
         _setupMenu = new Keybind("menu", "Open Menu", KeyCode.A, KeyCode.LeftAlt);
         if (File.Exists(Path.Combine(ModLoader.GetModConfigFolder(this), "autoconfig.xml")))
         {
            XmlSerializer serializer = new XmlSerializer(typeof(AutoSaveClass));
            FileStream stream = new FileStream(Path.Combine(ModLoader.GetModConfigFolder(this), "autoconfig.xml"),
               FileMode.Open);
            AutoSaveClass saveClass = serializer.Deserialize(stream) as AutoSaveClass;
            _coolDownSetting = saveClass.interval;
            stream.Close();
            _intervalSettings.Value = _coolDownSetting;
         }
      }

      public override void ModSettings()
      {
         Settings.AddHeader(this, "Settings", Color.black, Color.white);
         _intervalSettings = new Settings("interval", "Interval", SettingsChanged);
         Settings.AddText(this, "Interval is measured in seconds.");
         Settings.AddSlider(this, _intervalSettings, 30, 7200);
      }

      private void SettingsChanged()
      {
         _coolDownSetting = (float) _intervalSettings.GetValue();
      }

      public override void OnMenuLoad()
      {
         if (!_settingsLoaded) ModConsole.Print("Set up settings.");
      }

      public override void OnSave()
      {
         XmlSerializer serializer = new XmlSerializer(typeof(AutoSaveClass));
         if (File.Exists(Path.Combine(Application.persistentDataPath, "autoconfig.xml")))
            File.Delete(Path.Combine(Application.persistentDataPath, "autoconfig.xml"));
         FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, "autoconfig.xml"),
            FileMode.Create);
         AutoSaveClass saveClass = new AutoSaveClass {interval = _coolDownSetting};
         serializer.Serialize(stream, saveClass);
         stream.Close();
      }

      private bool _showWarning;

      public override void OnGUI()
      {
         if (_showWarning)
         {
            GUI.Box(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 10, 150, 20),
               "Auto saving in 5 seconds, your game will lag!");
         }
      }

      public override void Update()
      {
         if (FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle").Value == "")
            _cooldown += Time.deltaTime;

         _showWarning = _cooldown > _coolDownSetting - 5;

         if (_cooldown > _coolDownSetting &&
             FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle").Value == "")
         {
            _cooldown = 0;
            PlayMakerFSM.BroadcastEvent("SAVEGAME");
            Application.LoadLevel(1);
            Application.LoadLevel(3);
         }
      }
   }

   [Serializable]
   public class AutoSaveClass
   {
      public float interval;
   }
}