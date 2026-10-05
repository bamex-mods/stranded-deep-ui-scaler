Stranded Deep UI Scaler — canonical standalone workspace

Plugin GUID: com.bamex.strandeddeep.uiscaler
Version: 0.3.1
Known-good: 0.3.1 (RU/EN Mod Settings, build/deploy/game/local split-screen tested)

Standalone ownership:
- StrandedDeepUIScaler.cs owns Scale and runtime CanvasScaler behavior.
- SDK/ModSettingsClient.cs is a vendored optional integration bridge.
- Mod Settings is SoftDependency only; no compile reference to StrandedDeepModSettings.dll.
- Without Mod Settings, Scale remains controlled by BepInEx config.

Build:
  powershell -NoProfile -ExecutionPolicy Bypass -File .\build.ps1 -GameRoot "C:\Path\To\Stranded Deep"
Deploy (backup should be made by canonicalization orchestrator first):
  powershell -NoProfile -ExecutionPolicy Bypass -File .\deploy.ps1 -GameRoot "C:\Path\To\Stranded Deep"

Runtime:
  <GameRoot>\BepInEx\plugins\StrandedDeepUIScaler\StrandedDeepUIScaler.dll
Config:
  <GameRoot>\BepInEx\config\com.bamex.strandeddeep.uiscaler.cfg

GameRoot may also be supplied through STRANDED_DEEP_GAME_ROOT.
