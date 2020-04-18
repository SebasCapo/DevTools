using System;
using EXILED;
using MEC;

namespace DevTools {
    public class Plugin : EXILED.Plugin {

        public EventHandlers EventHandlers;

        public bool needPermission;

        public override void OnEnable() {
            try {
                Log.Info("Loading up config!");
                ReloadConfig();
                Log.Info("Initializing EventHandlers...");
                EventHandlers = new EventHandlers(this);
                Events.RemoteAdminCommandEvent += EventHandlers.OnCommand;
                Log.Info("Plugin loaded correctly!");
            } catch ( Exception e ) {
                Log.Error("Problem loading plugin: " + e.StackTrace);
            }
        }

        public override void OnDisable() {
            Events.RemoteAdminCommandEvent -= EventHandlers.OnCommand;

            EventHandlers = null;
        }

        public override void OnReload() {
        } //Andrés Calamaro

        public void ReloadConfig() {
            needPermission = Config.GetBool("dt_permissionsneed", false);
        }

        public override string getName { get; } = "DevTools 1.0.0 - SebasCapo";
    }
}