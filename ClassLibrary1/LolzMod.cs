using BepInEx;
using HarmonyLib;
using System;
using GUIFramework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;



namespace ClassLibrary1
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInProcess("valheim.exe")]
    public class LolzMod : BaseUnityPlugin
    {
        private const string modGUID = "LolzDLL.LolzMod";
        private const string modName = "LolzMod";
        private const string modVersion = "0.0.0.1";
        private readonly Harmony harmony = new Harmony(modGUID);


        private void Awake()
        {
            harmony.PatchAll();
            Chat_Add.AddCommands();
        }


        //приколы с верстаком, не нужно
        //[HarmonyPatch(typeof(CraftingStation), "Start")]
        //public class Crafting_Patches
        //{
        //    [HarmonyPrefix]
        //    private static void SetCraftingPatch(ref bool ___m_craftRequireRoof, ref bool ___m_craftRequireFire)
        //    {
        //        ___m_craftRequireFire = false;
        //        ___m_craftRequireRoof = false;
        //    }
        //}


        //приколы с чатом
        [HarmonyPatch(typeof(Chat))]
        public class Chat_Add : Terminal
        {
            public static Chat m_instance;
            private static List<float> originalRunSpeed = new List<float>();
            private static List<float> originalJumpForce = new List<float>();
            private static List<float> originalSwimSpeed = new List<float>();


            protected override Terminal m_terminalInstance => throw new NotImplementedException();

            //выводит в чат 1 сообщение
            [HarmonyPatch(typeof(Chat), "Awake")]
            [HarmonyPostfix]
            private static void AddMessageToChatTest(ref Chat ___m_instance)
            {
                m_instance = ___m_instance;
                Debug.Log("-----------------------------------------------------------------------------------------");
            }

            private static void AddMessageToChat(string text)
            {
                m_instance.AddString(text);
            }

            public static void AddCommands()
            {
                new ConsoleCommand("updatestat", "[stat] [amount]", delegate (ConsoleEventArgs args)
                {
                    if (args.TryParameterInt(2, out var amount))
                    {
                        UpdateStats(args[1], amount);
                    }
                    else
                    {
                        args.Context.AddString("Syntax: updatestat [stat] [amount]");
                    }
                }, isCheat: false, isNetwork: false, onlyServer: false, isSecret: false, allowInDevBuild: false, delegate
                {
                    List<string> list8 = new List<string>
                    {
                        "runspeed",
                        "jumpforce",
                        "swimspeed"
                    };

                    return list8;
                });
                Debug.LogWarning("-------------------------------updatestat added-----------------------------------------------");
            }

            private static void UpdateStats(string stat, float amount)
            {
                originalRunSpeed.Add(Player.m_localPlayer.m_runSpeed);
                originalJumpForce.Add(Player.m_localPlayer.m_jumpForce);
                originalSwimSpeed.Add(Player.m_localPlayer.m_swimSpeed);

                switch (stat)
                {
                    case "runspeed":
                        Player.m_localPlayer.m_runSpeed = amount;
                        AddMessageToChat($"{stat} : {Player.m_localPlayer.m_runSpeed}");
                        break;
                    case "jumpforce":
                        Player.m_localPlayer.m_jumpForce = amount;
                        AddMessageToChat($"{stat} : {Player.m_localPlayer.m_jumpForce}");
                        break;
                    case "swimspeed":
                        Player.m_localPlayer.m_swimSpeed = amount;
                        break;
                    case "default":
                        Player.m_localPlayer.m_runSpeed = originalRunSpeed.FirstOrDefault();
                        Player.m_localPlayer.m_jumpForce = originalJumpForce.FirstOrDefault();
                        Player.m_localPlayer.m_swimSpeed = originalSwimSpeed.FirstOrDefault();
                        AddMessageToChat($"{stat} : {Player.m_localPlayer.m_runSpeed}");
                        AddMessageToChat($"{stat} : {Player.m_localPlayer.m_jumpForce}");
                        AddMessageToChat($"{stat} : {Player.m_localPlayer.m_swimSpeed}");
                        break;
                    default:
                        Debug.LogWarning($"Характеристика {stat} не найдена либо не обрабатывается <3");
                        break;
                }
            }

            //обрабатывает инпут в чате, оригинальный метод отключен
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Chat), "InputText")]
            public static bool RedefineOriginalInput(ref Chat ___m_instance)
            {
                string text = ((TMP_InputField)___m_instance.m_input).text;
                if (text.Length != 0)
                {
                    text = ((text[0] != '/') ? ("say " + text) : (text == "/done" ? "dance" : text.Substring(1)));
                   ___m_instance.TryRunCommand(text, ___m_instance);
                }
                return false;
            }

            //добавляет к каждому сообщению в чате прикол
            [HarmonyPatch(typeof(Chat), "SendText")]
            [HarmonyPrefix]
            public static void MakeRofl(ref string text)
            {
                text += " бллииин";
                Debug.Log(text);
            }
        }

        //приколы с кроватью, не нужно
        //[HarmonyPatch]
        //private class Bed_Patches
        //{
        //    [HarmonyPrefix]
        //    [HarmonyPatch(typeof(Bed), "CheckFire")]
        //    private static bool SetChechFire(ref bool __result)
        //    {   
        //        __result = true;
        //        return false;
        //    }

        //    [HarmonyPrefix]
        //    [HarmonyPatch(typeof(Bed), "CheckWet")]
        //    private static bool SetChechWet(ref bool __result)
        //    {
        //        __result = true;
        //        return false;
        //    }

        //    [HarmonyPrefix]
        //    [HarmonyPatch(typeof(Bed), "CheckExposure")]
        //    private static bool SetCheckExposure(ref bool __result)
        //    {
        //        __result = true;
        //        return false;
        //    }
        //}
    }
}
