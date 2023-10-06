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
        public override string Version => "0.0.1";

        public static ModConfiguration config;
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> MASTER_ENABLED = new ModConfigurationKey<bool>("Enabled", "", () => true);
        // Left
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_SEPERATION = new ModConfigurationKey<float>("LEFT_SEPERATION", "", () => 6f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_RADIUS_RATIO = new ModConfigurationKey<float>("LEFT_RADIUS_RATIO", "", () => 0.5f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_ARCLAYOUT_ARC = new ModConfigurationKey<float>("LEFT_ARCLAYOUT_ARC", "", () => 360f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> LEFT_ARCLAYOUT_OFFSET = new ModConfigurationKey<float>("LEFT_ARCLAYOUT_OFFSET", "", () => 0f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<ArcLayout.Direction> LEFT_ITEM_DIRECTION = new ModConfigurationKey<ArcLayout.Direction>("LEFT_ITEM_DIRECTION", "", () => ArcLayout.Direction.Clockwise);
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> LEFT_ICON_ENABLED = new ModConfigurationKey<bool>("LEFT_ICON_ENABLED", "", () => true);
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> LEFT_INNER_CIRCLE_ENABLED = new ModConfigurationKey<bool>("LEFT_INNER_CIRCLE_ENABLED", "", () => true);

        [AutoRegisterConfigKey] private static ModConfigurationKey<string> SPACER_A = new ModConfigurationKey<string>("", "", () => "");
        // Right
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_SEPERATION = new ModConfigurationKey<float>("RIGHT_SEPERATION", "", () => 6f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_RADIUS_RATIO = new ModConfigurationKey<float>("RIGHT_RADIUS_RATIO", "", () => 0.5f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_ARCLAYOUT_ARC = new ModConfigurationKey<float>("RIGHT_ARCLAYOUT_ARC", "", () => 360f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<float> RIGHT_ARCLAYOUT_OFFSET = new ModConfigurationKey<float>("RIGHT_ARCLAYOUT_OFFSET", "", () => 0f);
        [AutoRegisterConfigKey] private static ModConfigurationKey<ArcLayout.Direction> RIGHT_ITEM_DIRECTION = new ModConfigurationKey<ArcLayout.Direction>("RIGHT_ITEM_DIRECTION", "", () => ArcLayout.Direction.Clockwise);
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> RIGHT_ICON_ENABLED = new ModConfigurationKey<bool>("RIGHT_ICON_ENABLED", "", () => true);
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> RIGHT_INNER_CIRCLE_ENABLED = new ModConfigurationKey<bool>("RIGHT_INNER_CIRCLE_ENABLED", "", () => true);
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
            [HarmonyPatch(typeof(ContextMenu), "OnChanges")]
            static void OnChanges(ContextMenu __instance, Sync<float> ___Separation, Sync<float> ___RadiusRatio, SyncRef<ArcLayout> ____arcLayout, SyncRef<OutlinedArc> ____innerCircle, SyncRef<Image> ____iconImage, SyncRef ____currentSummoner)
            {
                if (config.GetValue(MASTER_ENABLED))
                {
                    __instance.RunInUpdates(3, () =>
                    {
                        if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;

                        InteractionHandler tool = (InteractionHandler)____currentSummoner.Target;
                        if (tool == null) return;
                        Chirality side = tool.Side;

                        float seperation = side == Chirality.Right ? config.GetValue(RIGHT_SEPERATION) : config.GetValue(LEFT_SEPERATION);
                        float radiusRatio = side == Chirality.Right ? config.GetValue(RIGHT_RADIUS_RATIO) : config.GetValue(LEFT_RADIUS_RATIO);
                        float arc = side == Chirality.Right ? config.GetValue(RIGHT_ARCLAYOUT_ARC) : config.GetValue(LEFT_ARCLAYOUT_ARC);
                        float offset = side == Chirality.Right ? config.GetValue(RIGHT_ARCLAYOUT_OFFSET) : config.GetValue(LEFT_ARCLAYOUT_OFFSET);
                        ArcLayout.Direction direction = side == Chirality.Right ? config.GetValue(RIGHT_ITEM_DIRECTION) : config.GetValue(LEFT_ITEM_DIRECTION);
                        bool innerCircleEnabled = side == Chirality.Right ? config.GetValue(RIGHT_INNER_CIRCLE_ENABLED) : config.GetValue(LEFT_INNER_CIRCLE_ENABLED);
                        bool iconEnabled = side == Chirality.Right ? config.GetValue(RIGHT_ICON_ENABLED) : config.GetValue(LEFT_ICON_ENABLED);

                        if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;
                        ___Separation.Value = seperation;
                        ___RadiusRatio.Value = radiusRatio;
                        ____arcLayout.Target.Arc.Value = arc;
                        ____arcLayout.Target.ItemDirection.Value = direction;
                        ____arcLayout.Target.Offset.Value = offset;
                        ____innerCircle.Target.Enabled = innerCircleEnabled;
                        ____iconImage.Target.Enabled = iconEnabled;

                        //try
                        //{
                        //    for (int i = 0; i < ____arcLayout.Slot.GetAllChildren().Count; i++)
                        //    {
                        //        Slot item = ____arcLayout.Slot[i];
                        //        item.GetComponent<OutlinedArc>().RoundedCornerRadius.Value = 0;
                        //    }
                        //}
                        //catch (Exception e)
                        //{
                        //    
                        //    Error("Error occured in Context Menu Funny\n" + e);
                        //}
                    });
                }
            }
        }
    }
}

