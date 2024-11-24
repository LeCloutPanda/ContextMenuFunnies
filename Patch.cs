using System;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using ResoniteModLoader;

namespace ContextMenuFunnies;
public class ContextMenuFunnies : ResoniteMod
{
	public override string Author => "LeCloutPanda";
	public override string Name => "Context Menu Funnies";
	public override string Version => "1.0.2";
	public override string Link => "https://github.com/LeCloutPanda/ContextMenuFunnies/";

	[AutoRegisterConfigKey] private static ModConfigurationKey<bool> MASTER_ENABLED = new ModConfigurationKey<bool>("Enabled", "", () => true);

	[AutoRegisterConfigKey] private static ModConfigurationKey<float> SEPERATION = new ModConfigurationKey<float>("Item seperation", "", () => 6f);
	[AutoRegisterConfigKey] private static ModConfigurationKey<float> RADIUS_RATIO = new ModConfigurationKey<float>("Radius Ratio", "", () => 0.5f);
	[AutoRegisterConfigKey] private static ModConfigurationKey<float> ARCLAYOUT_ARC = new ModConfigurationKey<float>("Arc Amount", "", () => 360f);
	[AutoRegisterConfigKey] private static ModConfigurationKey<float> ARCLAYOUT_OFFSET = new ModConfigurationKey<float>("Rotation offset", "", () => 0f);
	[AutoRegisterConfigKey] private static ModConfigurationKey<ArcLayout.Direction> ITEM_DIRECTION = new ModConfigurationKey<ArcLayout.Direction>("Layout direction", "", () => ArcLayout.Direction.Clockwise);
	[AutoRegisterConfigKey] private static ModConfigurationKey<bool> ICON_ENABLED = new ModConfigurationKey<bool>("Center Icon Enabled", "", () => true);
	[AutoRegisterConfigKey] private static ModConfigurationKey<bool> INNER_CIRCLE_ENABLED = new ModConfigurationKey<bool>("Inner Circle Enabled", "", () => true);
	[AutoRegisterConfigKey] private static ModConfigurationKey<float> ROUNDED_CORNER_RADIUS = new ModConfigurationKey<float>("Rounded Corner Radius", "", () => 16f);
	[AutoRegisterConfigKey] private static ModConfigurationKey<float> FILL_COLOR_ALPHA = new ModConfigurationKey<float>("Fill Color Alpha", "", () => 1f);
	
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
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ContextMenu), "OpenMenu")]
		static void Postfix(ContextMenu __instance, Sync<float> ___Separation, Sync<float> ___RadiusRatio, SyncRef<ArcLayout> ____arcLayout, SyncRef<OutlinedArc> ____innerCircle, SyncRef<Image> ____iconImage, SyncRef ____currentSummoner)
		{
			if (config.GetValue(MASTER_ENABLED))
			{
				__instance.RunInUpdates(3, () =>
				{
					if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;

					float seperation = config.GetValue(SEPERATION);
					float radiusRatio = config.GetValue(RADIUS_RATIO);
					float arc = config.GetValue(ARCLAYOUT_ARC);
					float offset = config.GetValue(ARCLAYOUT_OFFSET);
					ArcLayout.Direction direction = config.GetValue(ITEM_DIRECTION);
					bool innerCircleEnabled = config.GetValue(INNER_CIRCLE_ENABLED);
					bool iconEnabled = config.GetValue(ICON_ENABLED);

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
	
	[HarmonyPatch(typeof(ContextMenuItem))]
	class ContextMenuItemPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch("Initialize")]
		public static void Postfix(ContextMenuItem __instance, OutlinedArc arc)
		{
			if (!config.GetValue(MASTER_ENABLED) || __instance == null || arc == null) return;
			
			arc.RoundedCornerRadius.Value = config.GetValue(ROUNDED_CORNER_RADIUS);
		}
		
		[HarmonyPostfix]
		[HarmonyPatch("UpdateColor", new Type[] { })]
		public static void Postfix(ContextMenuItem __instance, SyncRef<Button> ____button)
		{
			if (!config.GetValue(MASTER_ENABLED) || __instance == null || ____button == null) return;
			
			var colorDrive = ____button.Target?.ColorDrivers[0];
			if (colorDrive == null) return;
			
			var alpha = config.GetValue(FILL_COLOR_ALPHA);
			
			var oldNormalColor = colorDrive.NormalColor.Value;
			var oldHighlightColor = colorDrive.HighlightColor.Value;
			var oldPressColor = colorDrive.PressColor.Value;
			colorDrive.NormalColor.Value = new colorX(oldNormalColor.r, oldNormalColor.g, oldNormalColor.b, alpha);
			colorDrive.HighlightColor.Value = new colorX(oldHighlightColor.r, oldHighlightColor.g, oldHighlightColor.b, alpha);
			colorDrive.PressColor.Value = new colorX(oldPressColor.r, oldPressColor.g, oldPressColor.b, alpha);
		}
	}
}
