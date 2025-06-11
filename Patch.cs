using System;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using ResoniteModLoader;

namespace Context_Menu_Funnies
{
    public class Patch : ResoniteMod
    {
        public override string Author => "LeCloutPanda";
        public override string Name => "Context Menu Funnies";
        public override string Version => "1.2.1";
        public override string Link => "https://github.com/LeCloutPanda/ContextMenuFunnies/";

        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> MASTER_ENABLED = new ModConfigurationKey<bool>("Enabled", "", () => true);
        [AutoRegisterConfigKey] private static ModConfigurationKey<dummy> SPACER_A = new ModConfigurationKey<dummy>("", "");
        // Left
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_SEPERATION = new ModConfigurationKey<float>("Left menu item seperation", "", () => 6f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_RADIUS_RATIO = new ModConfigurationKey<float>("Left menu distance from center", "", () => 0.5f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_ARCLAYOUT_ARC = new ModConfigurationKey<float>("Left menu arc amount", "", () => 360f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_ARCLAYOUT_OFFSET = new ModConfigurationKey<float>("Left menu rotation offset", "", () => 0f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<ArcLayout.Direction> LEFT_ITEM_DIRECTION = new ModConfigurationKey<ArcLayout.Direction>("Left menu layout direction", "", () => ArcLayout.Direction.Clockwise);
        //[AutoRegisterConfigKey] private static ModConfigurationKey<bool> LEFT_ICON_ENABLED = new ModConfigurationKey<bool>("Left menu center icon enabled", "", () => true);
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> LEFT_INNER_CIRCLE_ENABLED = new ModConfigurationKey<bool>("Left menu inner circle enabled", "", () => true);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_ROUNDED_CORNER_RADIUS = new ModConfigurationKey<float>("Left Rounded Corner Radius", "", () => 16f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_FILL_COLOR_ALPHA = new ModConfigurationKey<float>("Left Fill Color Alpha", "", () => 1f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<CurvePreset> LEFT_ANIMATION_CURVE = new ModConfigurationKey<CurvePreset>("Left Animation Curve", "", () => CurvePreset.Linear);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_ANIMATION_TIME = new ModConfigurationKey<float>("Left Animation Time (Disabled when 0.0)", "", () => 0.0f);

        [AutoRegisterConfigKey] private static ModConfigurationKey<dummy> SPACER_B = new ModConfigurationKey<dummy>(" ", "");
        // Right
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_SEPERATION = new ModConfigurationKey<float>("Right menu item seperation", "", () => 6f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_RADIUS_RATIO = new ModConfigurationKey<float>("Right menu distance from center", "", () => 0.5f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_ARCLAYOUT_ARC = new ModConfigurationKey<float>("Right menu arc amount", "", () => 360f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_ARCLAYOUT_OFFSET = new ModConfigurationKey<float>("Right menu rotation offset", "", () => 0f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<ArcLayout.Direction> RIGHT_ITEM_DIRECTION = new ModConfigurationKey<ArcLayout.Direction>("Right menu layout direction", "", () => ArcLayout.Direction.Clockwise);
        //[AutoRegisterConfigKey] private static ModConfigurationKey<bool> RIGHT_ICON_ENABLED = new ModConfigurationKey<bool>("Right menu center icon enabled", "", () => true);
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> RIGHT_INNER_CIRCLE_ENABLED = new ModConfigurationKey<bool>("Right menu inner circle enabled", "", () => true);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_ROUNDED_CORNER_RADIUS = new ModConfigurationKey<float>("Right Rounded Corner Radius", "", () => 16f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_FILL_COLOR_ALPHA = new ModConfigurationKey<float>("Right Fill Color Alpha", "", () => 1f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<CurvePreset> RIGHT_ANIMATION_CURVE = new ModConfigurationKey<CurvePreset>("Right Animation Curve", "", () => CurvePreset.Linear);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_ANIMATION_TIME = new ModConfigurationKey<float>("Right Animation Time (Disabled when 0.0)", "", () => 0.0f); 

        private static ModConfiguration config;

        public override void OnEngineInit()
        {
            config = GetConfiguration();
            config.Save(true);

            Harmony harmony = new Harmony("dev.lecloutpanda.contextmenufunnies");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(ContextMenu))]
        class ContextMenuPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(ContextMenu), "OpenMenu")]
            static void PrefixOpenMenu(ContextMenu __instance, Sync<float> ___Separation, Sync<float> ___RadiusRatio, SyncRef<ArcLayout> ____arcLayout, SyncRef<OutlinedArc> ____innerCircle, SyncRef<Image> ____iconImage, SyncRef ____currentSummoner)
            {
                if (__instance.World.IsUserspace()) return;

                if (config.GetValue(MASTER_ENABLED))
                {
                    __instance.RunInUpdates(1, () =>
                    {
                        if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;

                        Chirality side = __instance.Pointer.Target.GetComponent<InteractionLaser>().Side;

                        float seperation = side == Chirality.Right ? config.GetValue(RIGHT_SEPERATION) : config.GetValue(LEFT_SEPERATION);
                        float radiusRatio = side == Chirality.Right ? config.GetValue(RIGHT_RADIUS_RATIO) : config.GetValue(LEFT_RADIUS_RATIO);
                        float arc = side == Chirality.Right ? config.GetValue(RIGHT_ARCLAYOUT_ARC) : config.GetValue(LEFT_ARCLAYOUT_ARC);
                        float offset = side == Chirality.Right ? config.GetValue(RIGHT_ARCLAYOUT_OFFSET) : config.GetValue(LEFT_ARCLAYOUT_OFFSET);
                        ArcLayout.Direction direction = side == Chirality.Right ? config.GetValue(RIGHT_ITEM_DIRECTION) : config.GetValue(LEFT_ITEM_DIRECTION);
                        bool innerCircleEnabled = side == Chirality.Right ? config.GetValue(RIGHT_INNER_CIRCLE_ENABLED) : config.GetValue(LEFT_INNER_CIRCLE_ENABLED);
                        //bool iconEnabled = side == Chirality.Right ? config.GetValue(RIGHT_ICON_ENABLED) : config.GetValue(LEFT_ICON_ENABLED);
                        CurvePreset animationCurve = side == Chirality.Right ? config.GetValue(RIGHT_ANIMATION_CURVE) : config.GetValue(LEFT_ANIMATION_CURVE);
                        float animationTime = side == Chirality.Right ? config.GetValue(RIGHT_ANIMATION_TIME) : config.GetValue(LEFT_ANIMATION_TIME);

                        ___Separation.Value = seperation;
                        ___RadiusRatio.Value = radiusRatio;
                        ____arcLayout.Target.ItemDirection.Value = direction;
                        ____arcLayout.Target.Offset.Value = offset;
                        ____innerCircle.Target.Enabled = innerCircleEnabled;
                        //____iconImage.Target.Enabled = iconEnabled;

                        if (animationTime > 0.0f) ____arcLayout.Target.Arc.TweenFromTo(0.0f, arc, animationTime, animationCurve);
                        else ____arcLayout.Target.Arc.Value = arc;
                    });
                }
            }

            // Hack to make the ratio work correctly and not be wrong requiring a reopen to fix
            [HarmonyPostfix]
            [HarmonyPatch(typeof(ContextMenu), "OpenMenu")]
            static void PostfixOpenMenu(ContextMenu __instance, Sync<float> ___RadiusRatio)
            {
                if (__instance.World.IsUserspace()) return;

                if (config.GetValue(MASTER_ENABLED))
                {
                    if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;
                    ___RadiusRatio.Value = __instance.Pointer.Target.GetComponent<InteractionLaser>().Side == Chirality.Right ? config.GetValue(RIGHT_RADIUS_RATIO) : config.GetValue(LEFT_RADIUS_RATIO);
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(ContextMenu), "Close")]
            static void PrefixClose(ContextMenu __instance, SyncRef<ArcLayout> ____arcLayout)
            {
                if (__instance.World.IsUserspace()) return;

                if (config.GetValue(MASTER_ENABLED))
                {
                    __instance.RunInUpdates(3, () =>
                    {
                        if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;

                        Chirality side = __instance.Pointer.Target.GetComponent<InteractionLaser>().Side;
                        float arc = side == Chirality.Right ? config.GetValue(RIGHT_ARCLAYOUT_ARC) : config.GetValue(LEFT_ARCLAYOUT_ARC);
                        CurvePreset animationCurve = side == Chirality.Right ? config.GetValue(RIGHT_ANIMATION_CURVE) : config.GetValue(LEFT_ANIMATION_CURVE);
                        float animationTime = side == Chirality.Right ? config.GetValue(RIGHT_ANIMATION_TIME) : config.GetValue(LEFT_ANIMATION_TIME);

                        if (animationTime > 0.0f) ____arcLayout.Target.Arc.TweenFromTo(____arcLayout.Target.Arc.Value, 0.0f, animationTime, animationCurve);
                        else ____arcLayout.Target.Arc.Value = arc;
                    });
                }
            }

            [HarmonyPatch(typeof(ContextMenuItem))]
            class ContextMenuItemPatch
            {
                [HarmonyPostfix]
                [HarmonyPatch("Initialize")]
                public static void InitializePostfix(ContextMenuItem __instance, OutlinedArc arc)
                {
                    if (__instance.World.IsUserspace()) return;

                    if (!config.GetValue(MASTER_ENABLED) || __instance == null || __instance.Slot == null || arc == null) return;

                    User activeUser = __instance.Slot.ActiveUserRoot?.ActiveUser;
                    if (activeUser == null || activeUser != __instance.LocalUser) return;

                    ContextMenu menu = __instance.Slot?.GetComponentInParents<ContextMenu>();
                    Chirality? side = menu?.Pointer?.Target?.GetComponent<InteractionLaser>()?.Side;
                    if (menu == null || side == null || menu.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;

                    arc.RoundedCornerRadius.Value = side == Chirality.Right ? config.GetValue(RIGHT_ROUNDED_CORNER_RADIUS) : config.GetValue(LEFT_ROUNDED_CORNER_RADIUS);
                }

                [HarmonyPostfix]
                [HarmonyPatch("UpdateColor", new Type[] { })]
                public static void UpdateColorPostfix(ContextMenuItem __instance, SyncRef<Button> ____button)
                {
                    if (__instance.World.IsUserspace()) return;
                   
                    if (!config.GetValue(MASTER_ENABLED) || __instance == null || __instance.Slot == null || ____button == null) return;

                    User activeUser = __instance.Slot.ActiveUserRoot?.ActiveUser;
                    if (activeUser == null || activeUser != __instance.LocalUser) return;

                    ContextMenu menu = __instance.Slot?.GetComponentInParents<ContextMenu>();
                    Chirality? side = menu?.Pointer?.Target?.GetComponent<InteractionLaser>()?.Side;
                    if (menu == null || side == null || menu.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;

                    var colorDrive = ____button?.Target?.ColorDrivers[0];
                    if (colorDrive == null) return;

                    var alpha = side == Chirality.Right ? config.GetValue(RIGHT_FILL_COLOR_ALPHA) : config.GetValue(LEFT_FILL_COLOR_ALPHA);

                    var oldNormalColor = colorDrive.NormalColor.Value;
                    var oldHighlightColor = colorDrive.HighlightColor.Value;
                    var oldPressColor = colorDrive.PressColor.Value;
                    colorDrive.NormalColor.Value = new colorX(oldNormalColor.r, oldNormalColor.g, oldNormalColor.b, alpha);
                    colorDrive.HighlightColor.Value = new colorX(oldHighlightColor.r, oldHighlightColor.g, oldHighlightColor.b, alpha);
                    colorDrive.PressColor.Value = new colorX(oldPressColor.r, oldPressColor.g, oldPressColor.b, alpha);
                }
            }
        }
    }
}

