using BepInEx;
using BepInEx.Configuration;
using BepInEx.NET.Common;
using BepInExResoniteShim;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using Renderite.Shared;

namespace Context_Menu_Funnies;

[BepInPlugin(GUID, Name, Version)]
public class Patch : BaseResonitePlugin
{
    public const string GUID = "dev.lecloutpanda.contextmenufunnies";
    public const string Name = "Context Menu Funnies";
    public const string Version = "1.2.2";
    public override string Author => "LeCloutPanda";
    public override string Link => "https://github.com/LeCloutPanda/ContextMenuFunnies";

    private static ConfigEntry<bool> MASTER_ENABLED;
    // Left
    private static ConfigEntry<float> LEFT_SEPERATION;
    private static ConfigEntry<float> LEFT_RADIUS_RATIO;
    private static ConfigEntry<float> LEFT_ARCLAYOUT_ARC;
    private static ConfigEntry<float> LEFT_ARCLAYOUT_OFFSET;
    private static ConfigEntry<ArcLayout.Direction> LEFT_ITEM_DIRECTION;
    //private ConfigEntry<bool> LEFT_ICON_ENABLED;
    private static ConfigEntry<bool> LEFT_INNER_CIRCLE_ENABLED;
    private static ConfigEntry<float> LEFT_ROUNDED_CORNER_RADIUS;
    private static ConfigEntry<float> LEFT_FILL_COLOR_ALPHA;
    private static ConfigEntry<CurvePreset> LEFT_ANIMATION_CURVE;
    private static ConfigEntry<float> LEFT_ANIMATION_TIME;
    // Right
    private static ConfigEntry<float> RIGHT_SEPERATION;
    private static ConfigEntry<float> RIGHT_RADIUS_RATIO;
    private static ConfigEntry<float> RIGHT_ARCLAYOUT_ARC;
    private static ConfigEntry<float> RIGHT_ARCLAYOUT_OFFSET;
    private static ConfigEntry<ArcLayout.Direction> RIGHT_ITEM_DIRECTION;
    //private ConfigEntry<bool> RIGHT_ICON_ENABLED;
    private static ConfigEntry<bool> RIGHT_INNER_CIRCLE_ENABLED;
    private static ConfigEntry<float> RIGHT_ROUNDED_CORNER_RADIUS;
    private static ConfigEntry<float> RIGHT_FILL_COLOR_ALPHA;
    private static ConfigEntry<CurvePreset> RIGHT_ANIMATION_CURVE;
    private static ConfigEntry<float> RIGHT_ANIMATION_TIME;

    public override void Load()
    {
        MASTER_ENABLED = Config.Bind("General", "Enabled", true, "Enable/disable the mod");
        // Left
        LEFT_SEPERATION = Config.Bind("Left", "ItemSeparation", 6f, "Left menu item separation");
        LEFT_RADIUS_RATIO = Config.Bind("Left", "RadiusRatio", 0.5f, "Left menu distance from center");
        LEFT_ARCLAYOUT_ARC = Config.Bind("Left", "ArcAmount", 360f, "Left menu arc amount");
        LEFT_ARCLAYOUT_OFFSET = Config.Bind("Left", "ArcOffset", 0f, "Left menu rotation offset");
        LEFT_ITEM_DIRECTION = Config.Bind("Left", "ItemDirection", ArcLayout.Direction.Clockwise, "Left menu layout direction");
        //LEFT_ICON_ENABLED = Config.Bind("Left", "IconEnabled", true, "Left menu center icon enabled");
        LEFT_INNER_CIRCLE_ENABLED = Config.Bind("Left", "InnerCircleEnabled", true, "Left menu inner circle enabled");
        LEFT_ROUNDED_CORNER_RADIUS = Config.Bind("Left", "RoundedCornerRadius", 16f, "Left Rounded Corner Radius");
        LEFT_FILL_COLOR_ALPHA = Config.Bind("Left", "FillColorAlpha", 1f, "Left Fill Color Alpha");
        LEFT_ANIMATION_CURVE = Config.Bind("Left", "AnimationCurve", CurvePreset.Linear, "Left Animation Curve");
        LEFT_ANIMATION_TIME = Config.Bind("Left", "AnimationTime", 0.0f, "Left Animation Time (Disabled when 0.0)");
        // Right
        RIGHT_SEPERATION = Config.Bind("Right", "ItemSeparation", 6f, "Right menu item separation");
        RIGHT_RADIUS_RATIO = Config.Bind("Right", "RadiusRatio", 0.5f, "Right menu distance from center");
        RIGHT_ARCLAYOUT_ARC = Config.Bind("Right", "ArcAmount", 360f, "Right menu arc amount");
        RIGHT_ARCLAYOUT_OFFSET = Config.Bind("Right", "ArcOffset", 0f, "Right menu rotation offset");
        RIGHT_ITEM_DIRECTION = Config.Bind("Right", "ItemDirection", ArcLayout.Direction.Clockwise, "Right menu layout direction");
        //RIGHT_ICON_ENABLED = Config.Bind("Right", "IconEnabled", true, "Right menu center icon enabled");
        RIGHT_INNER_CIRCLE_ENABLED = Config.Bind("Right", "InnerCircleEnabled", true, "Right menu inner circle enabled");
        RIGHT_ROUNDED_CORNER_RADIUS = Config.Bind("Right", "RoundedCornerRadius", 16f, "Right Rounded Corner Radius");
        RIGHT_FILL_COLOR_ALPHA = Config.Bind("Right", "FillColorAlpha", 1f, "Right Fill Color Alpha");
        RIGHT_ANIMATION_CURVE = Config.Bind("Right", "AnimationCurve", CurvePreset.Linear, "Right Animation Curve");
        RIGHT_ANIMATION_TIME = Config.Bind("Right", "AnimationTime", 0.0f, "Right Animation Time (Disabled when 0.0)");

        HarmonyInstance.PatchAll();
    }

    [HarmonyPatch(typeof(ContextMenu))]
    class ContextMenuPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ContextMenu), "OpenMenu")]
        static void PrefixOpenMenu(ContextMenu __instance, Sync<float> ___Separation, Sync<float> ___RadiusRatio, SyncRef<ArcLayout> ____arcLayout, SyncRef<OutlinedArc> ____innerCircle, SyncRef<Image> ____iconImage, SyncRef ____currentSummoner)
        {
            if (__instance.World.IsUserspace()) return;

            if (MASTER_ENABLED.Value)
            {
                __instance.RunInUpdates(1, () =>
                {
                    if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;

                    Chirality side = __instance.Pointer.Target.GetComponent<InteractionLaser>().Side;

                    float seperation = side == Chirality.Right ? RIGHT_SEPERATION.Value : LEFT_SEPERATION.Value;
                    float radiusRatio = side == Chirality.Right ? RIGHT_RADIUS_RATIO.Value : LEFT_RADIUS_RATIO.Value;
                    float arc = side == Chirality.Right ? RIGHT_ARCLAYOUT_ARC.Value : LEFT_ARCLAYOUT_ARC.Value;
                    float offset = side == Chirality.Right ? RIGHT_ARCLAYOUT_OFFSET.Value : LEFT_ARCLAYOUT_OFFSET.Value;
                    ArcLayout.Direction direction = side == Chirality.Right ? RIGHT_ITEM_DIRECTION.Value : LEFT_ITEM_DIRECTION.Value;
                    bool innerCircleEnabled = side == Chirality.Right ? RIGHT_INNER_CIRCLE_ENABLED.Value : LEFT_INNER_CIRCLE_ENABLED.Value;
                    //bool iconEnabled = side == Chirality.Right ? RIGHT_ICON_ENABLED.Value : LEFT_ICON_ENABLED.Value;
                    CurvePreset animationCurve = side == Chirality.Right ? RIGHT_ANIMATION_CURVE.Value : LEFT_ANIMATION_CURVE.Value;
                    float animationTime = side == Chirality.Right ? RIGHT_ANIMATION_TIME.Value : LEFT_ANIMATION_TIME.Value;

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

            if (MASTER_ENABLED.Value)
            {
                if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;
                ___RadiusRatio.Value = __instance.Pointer.Target.GetComponent<InteractionLaser>().Side == Chirality.Right ? RIGHT_RADIUS_RATIO.Value : LEFT_RADIUS_RATIO.Value;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ContextMenu), "Close")]
        static void PrefixClose(ContextMenu __instance, SyncRef<ArcLayout> ____arcLayout)
        {
            if (__instance.World.IsUserspace()) return;

            if (MASTER_ENABLED.Value)
            {
                __instance.RunInUpdates(3, () =>
                {
                    if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;

                    Chirality side = __instance.Pointer.Target.GetComponent<InteractionLaser>().Side;
                    float arc = side == Chirality.Right ? RIGHT_ARCLAYOUT_ARC.Value : LEFT_ARCLAYOUT_ARC.Value;
                    CurvePreset animationCurve = side == Chirality.Right ? RIGHT_ANIMATION_CURVE.Value : LEFT_ANIMATION_CURVE.Value;
                    float animationTime = side == Chirality.Right ? RIGHT_ANIMATION_TIME.Value : LEFT_ANIMATION_TIME.Value;

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

                if (!MASTER_ENABLED.Value || __instance == null || __instance.Slot == null || arc == null) return;

                User activeUser = __instance.Slot.ActiveUserRoot?.ActiveUser;
                if (activeUser == null || activeUser != __instance.LocalUser) return;

                ContextMenu menu = __instance.Slot?.GetComponentInParents<ContextMenu>();
                Chirality? side = menu?.Pointer?.Target?.GetComponent<InteractionLaser>()?.Side;
                if (menu == null || side == null || menu.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;

                arc.RoundedCornerRadius.Value = side == Chirality.Right ? RIGHT_ROUNDED_CORNER_RADIUS.Value : LEFT_ROUNDED_CORNER_RADIUS.Value;
            }

            [HarmonyPostfix]
            [HarmonyPatch("UpdateColor", new Type[] { })]
            public static void UpdateColorPostfix(ContextMenuItem __instance, SyncRef<Button> ____button)
            {
                if (__instance.World.IsUserspace()) return;
                if (!MASTER_ENABLED.Value || __instance == null || __instance.Slot == null || ____button == null) return;
                User activeUser = __instance.Slot.ActiveUserRoot?.ActiveUser;
                if (activeUser == null || activeUser != __instance.LocalUser) return;
                ContextMenu menu = __instance.Slot?.GetComponentInParents<ContextMenu>();
                Chirality? side = menu?.Pointer?.Target?.GetComponent<InteractionLaser>()?.Side;
                if (menu == null || side == null || menu.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;
                var colorDrive = ____button?.Target?.ColorDrivers[0];
                if (colorDrive == null) return;
                var alpha = side == Chirality.Right ? RIGHT_FILL_COLOR_ALPHA.Value : LEFT_FILL_COLOR_ALPHA.Value;
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