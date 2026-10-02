using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using Bamex.StrandedDeep.ModSettings;
using UnityEngine;
using UnityEngine.UI;

namespace StrandedDeepUIScaler
{
    [BepInPlugin(
        "com.bamex.strandeddeep.uiscaler",
        "Stranded Deep UI Scaler",
        "0.3.0")]
    [BepInDependency(
        "com.bamex.strandeddeep.modsettings",
        BepInDependency.DependencyFlags.SoftDependency)]
    public class UiScalerPlugin : BaseUnityPlugin
    {
        private ConfigEntry<float> _scale;

        private readonly Dictionary<int, Vector2> _originalResolutions =
            new Dictionary<int, Vector2>();

        private float _nextScanTime = 0f;
        private float _nextMenuRegistrationTime = 0f;

        private float _lastLoggedScale = -1f;
        private int _lastCanvasCount = -1;

        private bool _menuRegistered;

        private void Awake()
        {
            _scale = Config.Bind(
                "General",
                "Scale",
                1.5f,
                "Player UI scale. 1.0 = original, 1.5 = 150%, 2.0 = 200%.");

            ClampScale();

            Logger.LogInfo(
                "Stranded Deep UI Scaler v0.3.0 loaded.");

            Logger.LogInfo(
                "UI scale is configured through Settings -> MODS when Stranded Deep Mod Settings is installed.");

            Logger.LogInfo(
                "Initial scale: " +
                Math.Round(_scale.Value * 100f) +
                "%");

            TryRegisterMenu();
        }

        private void Update()
        {
            if (
                !_menuRegistered &&
                Time.unscaledTime >=
                _nextMenuRegistrationTime
            )
            {
                _nextMenuRegistrationTime =
                    Time.unscaledTime + 1.0f;

                TryRegisterMenu();
            }

            if (Time.unscaledTime >= _nextScanTime)
            {
                _nextScanTime =
                    Time.unscaledTime + 0.5f;

                ApplyScale();
            }
        }

        private void TryRegisterMenu()
        {
            if (_menuRegistered)
                return;

            if (!ModSettingsClient.IsAvailable())
                return;

            bool modOk =
                ModSettingsClient.RegisterMod(
                    "uiscaler",
                    "Интерфейс",
                    100);

            bool sliderOk =
                ModSettingsClient.AddSlider(
                    "uiscaler",
                    "scale",
                    "Масштаб интерфейса",
                    100,
                    1.0f,
                    2.0f,
                    0.1f,
                    100.0f,
                    "%",
                    0,
                    GetScale,
                    SetScaleFromMenu);

            _menuRegistered =
                modOk && sliderOk;

            if (_menuRegistered)
            {
                Logger.LogInfo(
                    "Registered UI scale in shared MODS menu API.");
            }
        }

        private float GetScale()
        {
            return _scale.Value;
        }

        private void SetScaleFromMenu(
            float value)
        {
            _scale.Value =
                Mathf.Clamp(
                    value,
                    1.0f,
                    2.0f);

            Logger.LogInfo(
                "UI scale changed to " +
                Math.Round(_scale.Value * 100f) +
                "% via MODS menu.");

            _nextScanTime = 0f;
        }

        private void ClampScale()
        {
            _scale.Value =
                Mathf.Clamp(
                    _scale.Value,
                    1.0f,
                    2.0f);
        }

        private void ApplyScale()
        {
            Canvas[] canvases =
                Resources.FindObjectsOfTypeAll<Canvas>();

            int playerCanvasCount = 0;

            foreach (Canvas canvas in canvases)
            {
                if (canvas == null)
                    continue;

                GameObject go =
                    canvas.gameObject;

                if (!go.scene.IsValid())
                    continue;

                if (go.name != "Canvas - Game")
                    continue;

                Transform parent =
                    canvas.transform.parent;

                if (parent == null)
                    continue;

                if (
                    parent.name !=
                    "PlayerUI(Clone)"
                )
                {
                    continue;
                }

                CanvasScaler scaler =
                    go.GetComponent<CanvasScaler>();

                if (scaler == null)
                    continue;

                if (
                    scaler.uiScaleMode !=
                    CanvasScaler.ScaleMode.ScaleWithScreenSize
                )
                {
                    continue;
                }

                int id =
                    scaler.GetInstanceID();

                Vector2 originalResolution;

                if (
                    !_originalResolutions.TryGetValue(
                        id,
                        out originalResolution)
                )
                {
                    originalResolution =
                        scaler.referenceResolution;

                    _originalResolutions.Add(
                        id,
                        originalResolution);

                    Logger.LogInfo(
                        "Player canvas found. Camera rect=" +
                        CameraRect(canvas) +
                        ", original reference=" +
                        originalResolution);
                }

                Vector2 targetResolution =
                    originalResolution /
                    _scale.Value;

                if (
                    Math.Abs(
                        scaler.referenceResolution.x -
                        targetResolution.x) > 0.1f
                    ||
                    Math.Abs(
                        scaler.referenceResolution.y -
                        targetResolution.y) > 0.1f
                )
                {
                    scaler.referenceResolution =
                        targetResolution;
                }

                playerCanvasCount++;
            }

            if (
                playerCanvasCount !=
                _lastCanvasCount
                ||
                Math.Abs(
                    _lastLoggedScale -
                    _scale.Value) > 0.001f
            )
            {
                _lastCanvasCount =
                    playerCanvasCount;

                _lastLoggedScale =
                    _scale.Value;

                Logger.LogInfo(
                    "Applied " +
                    Math.Round(_scale.Value * 100f) +
                    "% UI scale to " +
                    playerCanvasCount +
                    " player canvas(es).");
            }
        }

        private string CameraRect(
            Canvas canvas)
        {
            if (
                canvas.worldCamera == null
            )
            {
                return "<null>";
            }

            Rect r =
                canvas.worldCamera.rect;

            return String.Format(
                "({0:0.##},{1:0.##},{2:0.##},{3:0.##})",
                r.x,
                r.y,
                r.width,
                r.height);
        }
    }
}
