﻿using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using ResoniteModLoader;

namespace Context_Menu_Funnies
{
    public class Patch : ResoniteMod
    {
        public override string Author => "LeCloutPanda";
        public override string Name => "Context Menu Funnies";
        public override string Version => "1.0.1";
        public override string Link => "https://github.com/LeCloutPanda/ContextMenuFunnies/";

        public static ModConfiguration config;
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> MASTER_ENABLED = new ModConfigurationKey<bool>("Enabled", "", () => true);
        // Left
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_SEPERATION = new ModConfigurationKey<float>("Left menu item seperation", "", () => 6f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_RADIUS_RATIO = new ModConfigurationKey<float>("Left menu distance from center", "", () => 0.5f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_ARCLAYOUT_ARC = new ModConfigurationKey<float>("Left menu arc amount", "", () => 360f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_ARCLAYOUT_OFFSET = new ModConfigurationKey<float>("Left menu rotation offset", "", () => 0f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<ArcLayout.Direction> LEFT_ITEM_DIRECTION = new ModConfigurationKey<ArcLayout.Direction>("Left menu layout direction", "", () => ArcLayout.Direction.Clockwise);
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> LEFT_ICON_ENABLED = new ModConfigurationKey<bool>("Left menu center icon enabled", "", () => true);
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> LEFT_INNER_CIRCLE_ENABLED = new ModConfigurationKey<bool>("Left menu inner circle enabled", "", () => true);

        [AutoRegisterConfigKey] private static ModConfigurationKey<string> SPACER_A = new ModConfigurationKey<string>("", "", () => "");
        // Right
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_SEPERATION = new ModConfigurationKey<float>("Right menu item seperation", "", () => 6f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_RADIUS_RATIO = new ModConfigurationKey<float>("Right menu distance from center", "", () => 0.5f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_ARCLAYOUT_ARC = new ModConfigurationKey<float>("Right menu arc amount", "", () => 360f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_ARCLAYOUT_OFFSET = new ModConfigurationKey<float>("Right menu rotation offset", "", () => 0f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<ArcLayout.Direction> RIGHT_ITEM_DIRECTION = new ModConfigurationKey<ArcLayout.Direction>("Right menu layout direction", "", () => ArcLayout.Direction.Clockwise);
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> RIGHT_ICON_ENABLED = new ModConfigurationKey<bool>("Right menu center icon enabled", "", () => true);
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> RIGHT_INNER_CIRCLE_ENABLED = new ModConfigurationKey<bool>("Right menu inner circle enabled", "", () => true);
        public override void OnEngineInit()
        {
            config = GetConfiguration();
            config.Save();

            Harmony harmony = new Harmony($"dev.lecloutpanda.contextmenufunnies");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(ContextMenu))]
        class ContextMenuPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(ContextMenu), "OpenMenu")]
            static void Postfix(ContextMenu __instance, Sync<float> ___Separation, Sync<float> ___RadiusRatio, SyncRef<ArcLayout> ____arcLayout, SyncRef<OutlinedArc> ____innerCircle, SyncRef<Image> ____iconImage, SyncRef ____currentSummoner)
            {
                if (config.GetValue(MASTER_ENABLED))
                {
                    __instance.RunInUpdates(3, () =>
                    {
                        if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;

                        Chirality side = __instance.Pointer.Target.GetComponent<InteractionLaser>().Side;

                        float seperation = side == Chirality.Right ? config.GetValue(RIGHT_SEPERATION) : config.GetValue(LEFT_SEPERATION);
                        float radiusRatio = side == Chirality.Right ? config.GetValue(RIGHT_RADIUS_RATIO) : config.GetValue(LEFT_RADIUS_RATIO);
                        float arc = side == Chirality.Right ? config.GetValue(RIGHT_ARCLAYOUT_ARC) : config.GetValue(LEFT_ARCLAYOUT_ARC);
                        float offset = side == Chirality.Right ? config.GetValue(RIGHT_ARCLAYOUT_OFFSET) : config.GetValue(LEFT_ARCLAYOUT_OFFSET);
                        ArcLayout.Direction direction = side == Chirality.Right ? config.GetValue(RIGHT_ITEM_DIRECTION) : config.GetValue(LEFT_ITEM_DIRECTION);
                        bool innerCircleEnabled = side == Chirality.Right ? config.GetValue(RIGHT_INNER_CIRCLE_ENABLED) : config.GetValue(LEFT_INNER_CIRCLE_ENABLED);
                        bool iconEnabled = side == Chirality.Right ? config.GetValue(RIGHT_ICON_ENABLED) : config.GetValue(LEFT_ICON_ENABLED);

                        ___Separation.Value = seperation;
                        ___RadiusRatio.Value = radiusRatio;
                        ____arcLayout.Target.Arc.Value = arc;
                        ____arcLayout.Target.ItemDirection.Value = direction;
                        ____arcLayout.Target.Offset.Value = offset;
                        ____innerCircle.Target.Enabled = innerCircleEnabled;
                        ____iconImage.Target.Enabled = iconEnabled;
                    });
                }
            }
        }
    }
}

