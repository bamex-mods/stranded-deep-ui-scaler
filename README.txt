Stranded Deep UI Scaler — canonical standalone workspace

Plugin GUID: com.bamex.strandeddeep.uiscaler
Version: 0.3.0
Known-good: 0.3.0 (build/deploy/game/local split-screen tested)

Standalone ownership:
- StrandedDeepUIScaler.cs owns Scale and runtime CanvasScaler behavior.
- SDK/ModSettingsClient.cs is a vendored optional integration bridge.
- Mod Settings is SoftDependency only; no compile reference to StrandedDeepModSettings.dll.
- Without Mod Settings, Scale remains controlled by BepInEx config.

Build:
  powershell -NoProfile -ExecutionPolicy Bypass -File .\build.ps1
Deploy (backup should be made by canonicalization orchestrator first):
  powershell -NoProfile -ExecutionPolicy Bypass -File .\deploy.ps1

Runtime:
  F:\SteamLibrary\steamapps\common\Stranded Deep\BepInEx\plugins\StrandedDeepUIScaler\StrandedDeepUIScaler.dll
Config:
  F:\SteamLibrary\steamapps\common\Stranded Deep\BepInEx\config\com.bamex.strandeddeep.uiscaler.cfg
