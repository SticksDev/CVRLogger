using MelonLoader;
using System;
using UnityEngine;
using ABI_RC.Helpers;
using HarmonyLib;

namespace sticksdev
{
    public class CVRLogger : MelonMod
    {

        public static HarmonyLib.Harmony MyHarmony = new HarmonyLib.Harmony("dev.sticks.CVRLogger");

        public override void OnApplicationStart()
        {
            LoggerInstance.Msg("CVRLogger Starting at " + DateTime.Now.ToString());
            LoggerInstance.Msg("Hooking To UnityGame (main) Logger...");

            // Hook to unityEngines Loggers
            Application.logMessageReceived += LogMessageReceived;
            LoggerInstance.Msg("Hooked To UnityGame (main) Logger! Patching Debug.Log...");

            // Patch Debug.Log to use our logger
            LoggerInstance.Msg("Applying Harmony Patches...");
            MyHarmony.PatchAll();

            // Finsh up and tell them the mod is ready
            LoggerInstance.Msg("Patches Applied!");
            LoggerInstance.Msg("All hooks applied!");
            LoggerInstance.Warning("[NOTICE] Some logs may be duplicated. This is due to the fact that UnityGame (main) Logger is hooked to the same log function as Debug.Log in some places. This is not a problem, but it may be annoying.");
            LoggerInstance.Msg("CVRLogger Started at " + DateTime.Now.ToString());
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            LoggerInstance.Msg("[" + type.ToString() + "]" + " from " + condition + ": " + stackTrace);
        }

        public override void OnApplicationQuit()
        {
            // Unhook from unityEngines Logger;
            LoggerInstance.Msg("Got OnApplicationQuit, unhooking from main Logger...");
            Application.logMessageReceived -= LogMessageReceived;

            LoggerInstance.Msg("Unhooked from main Logger! - calling unpatchAll");
            MyHarmony.UnpatchAll();

            LoggerInstance.Msg("All Unhooked - Shutting down...");
            LoggerInstance.Msg("CVRLogger Quitting at " + DateTime.Now.ToString());
        }

        // Patch for Debug.log
        [HarmonyPatch(typeof(Debug), "Log", new Type[] { typeof(object) })]
        public static class Debug_Log_Patch
        {
            public static void Postfix(string message)
            {
                // Reroute to info
                MelonLogger.Msg("[DEBUG] " + message);
            }
        }
    }
}
