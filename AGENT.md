# Agent conventions for this project

## C# naming
- Do NOT use `_` prefix for private fields. Use plain camelCase: `selectedShip`, not `_selectedShip`.

---

# Project overview

## Origin story

This project was started by Raciborski, who inherited the skeleton of a previous ship simulator from someone else. The original codebase was scrapped entirely — the team decided to switch to Unity and rewrite everything from scratch. A small group of contributors was invited to work on it together.

## What this is

A **maritime ship dynamics simulator**. The core idea: you input parameters of a real ship — weight, dimensions, propeller/engine characteristics, rudder limits — and the simulator shows you how that ship would actually behave during nautical maneuvers.

The goal is not a game. It's an engineering/educational tool for visualizing ship handling behavior in a physically realistic way.

## How the physics works

The simulator uses **Nomoto manoeuvring models** — standard equations used in real maritime engineering:

- **1st order Nomoto**: `r_dot = (K*delta - r) / T` — simple yaw rate model
- **2nd order Nomoto**: adds yaw acceleration term for more realistic transient response
- **Test model**: simplified empirical model for development/debugging

All models share the same surge (speed) dynamics: thrust vs. quadratic hydrodynamic drag, with rudder-induced drag penalty.

Ship position uses **WGS84 geodetic coordinates** (real GPS system) with a floating origin to avoid floating-point precision loss at large distances.

## Configurable ship parameters

- Identification: name, MMSI, call sign
- Geometry: length, breadth, draft, block coefficient
- Mass: displacement (tonnes)
- Performance: max speed (Vmax), max rudder angle, rudder deflection rate
- Physics tuning: Nomoto K/T1/T2/T3 constants (currently hardcoded per ship type, planned to load from JSON)

## Currently implemented ship types

- `cargo_general` — 172m general cargo vessel (Fast Cargo Mariner class)
- `tanker_lng` — 290m LNG carrier (based on 'Galea' vessel)

Planned but not yet implemented: bulk carrier, container ship, reefer, RoRo, oil tanker, cruise ship, ferry.

## What's already working

- Full ship physics with 3 switchable movement models
- WGS84 coordinate system with floating origin
- Engine and rudder controls (sliders + manual input)
- Telemetry display: speed, heading, COG, SOG, ROT, lat/lon
- Simulation control: play/pause/stop/restart
- Time scaling: 1–50× speed multiplier
- Dynamic weather system (clear, fog, rain, thunderstorm presets)
- Time of day with dynamic lighting
- Trajectory trace visualization
- Orbit camera + top-down view
- Obstacle placement system (spawning/rendering works)

## What's not yet implemented / in progress

- **Maneuvers** — the main planned feature. Ships should be able to execute defined nautical maneuvers, e.g. passing another vessel, collision avoidance, docking approach. None of this exists yet.
- Autopilot — enums defined, no logic implemented
- Collision response — detection infrastructure exists but physics response is missing
- Wind/wave effects on ship dynamics — weather values tracked but not yet fed into physics
- More ship types — only 2 of 9 defined types have Nomoto parameters
- JSON-based ship parameter loading — currently hardcoded
- Save/load scenarios

## Scenes

- `MainMenu` — scenario selection screen
- `Simulation` — the actual simulator with all systems active

---

# MainMenu scene — architecture

The MainMenu scene has a single Canvas with a `Scenarios` panel. The panel is split into a left list and a right detail area.

## Scene hierarchy (inside Canvas)

```
Canvas
└── Scenarios
    ├── LeftPanel          — scenario list buttons (ScenarioListUI)
    └── RightPanel
        ├── Header         — "Scenarios" title + icon
        ├── ScrollView     — scrollable parameter area
        │   └── Content    — dynamic parameter panels (ScenarioDynamicUI)
        └── Footer         — "Back to Main Menu" + "Start Simulation" buttons
```

## Scripts

### `ScenarioData.cs`
Pure data classes deserialized from JSON.

```csharp
ScenarioDefinition { scenarioId, title, ScenarioParameter[] parameters }
ScenarioParameter  { type, id, label, options[], min, max, defaultValue, defaultText, ScenarioRowChild[] children }
ScenarioRowChild   { type, id, label, options[], min, max, defaultValue, defaultText }
```

**Important**: `ScenarioRowChild` is a separate non-recursive class. `ScenarioParameter` originally held `ScenarioParameter[] children` (self-referential), but `JsonUtility` silently drops self-referential types — deserialization always returns null. The fix: `ScenarioRowChild` has the same leaf fields but no `children` property.

Supported parameter types:
- `"header"` — section title
- `"slider"` — float range slider with value label
- `"dropdown"` — TMP_Dropdown
- `"input"` — TMP_InputField
- `"row"` — horizontal container with exactly two child parameters side-by-side

### `ScenarioLoader.cs`
Static helper. Loads all `TextAsset` files from `Assets/Resources/Scenarios/`, deserializes each with `JsonUtility.FromJson<ScenarioDefinition>()`, sorts by filename.

```csharp
ScenarioLoader.LoadAll()   // returns ScenarioDefinition[]
ScenarioLoader.Load(id)    // returns single ScenarioDefinition
```

### `ScenarioListUI.cs`
`[ExecuteAlways]`. Mounted on `LeftPanel`.

`OnEnable()` flow:
1. `ScenarioLoader.LoadAll()` → `dynamicUI.BuildAll(scenarios)` → `SpawnButtons()` → `SelectScenario(0)`

`SelectScenario(index)` calls `dynamicUI.Show(index)` and updates button colors.

Uses `DestroyImmediate` in edit mode (required — `Destroy` is deferred and doesn't work in `OnEnable`).

Size fields are **const** (not `[SerializeField]`) — see sizing note below.

### `ScenarioDynamicUI.cs`
`[ExecuteAlways]`. Mounted on `Content` inside `ScrollView`.

Builds all panels at once in `BuildAll()`, then shows/hides using `CanvasGroup` (alpha + interactable + blocksRaycasts). Never destroys and recreates panels on scenario switch.

Key methods:
- `BuildAll(ScenarioDefinition[])` — clears children, creates one hidden panel per scenario
- `Show(int index)` — sets active panel's CanvasGroup alpha=1, others=0
- `CollectValues()` — returns `Dictionary<string,string>` of current values from the visible panel

Builder methods: `BuildHeader`, `BuildDropdown`, `BuildSlider`, `BuildInput`, `BuildRow`

`BuildRow` creates a `HorizontalLayoutGroup` with one column per child, then calls the appropriate child builder using `RowChildToParam()` to convert `ScenarioRowChild` → `ScenarioParameter`.

#### Slider construction order (critical)
The Unity `Slider` component's `fillRect` and `handleRect` must be assigned **before** calling `SetValueWithoutNotify()`. If you assign the value first, the fill position is never initialized. The fill rect anchor must be `anchorMax = new Vector2(0, 1)` — not `Vector2.one`. With `Vector2.one` the fill covers the entire parent (visible as a large colored rectangle).

#### Sizing — why const instead of [SerializeField]
The size/font fields (`fontSizeLabel`, `heightSliderRow`, etc.) are `const` rather than `[SerializeField]`. This is intentional: serialized field defaults only apply when a component is *first added* to a scene. Once the scene is saved, Unity stores the serialized values and ignores any C# default changes. Because these values were changed multiple times across sessions without the scene being resaved, the serialized values in the scene were always stale (e.g. `fontSizeLabel=16` while code had `38f`). Making them `const` ensures Unity always uses the code value.

Current sizes:
```
fontSizeLabel   = 38f
fontSizeHeader  = 52f
fontSizeInput   = 38f
heightDropdown  = 100f
heightInput     = 100f
heightSliderRow = 120f
heightHandle    = 44f
buttonHeight    = 136f   (ScenarioListUI)
fontSize        = 44f    (ScenarioListUI button labels)
```

### `ScenarioConfigAsset.cs`
ScriptableObject (`[CreateAssetMenu]`) wrapping a single JSON `TextAsset`. Used by the custom editor for Inspector preview. Not used at runtime — runtime loading goes through `ScenarioLoader`.

### `MainMenu.cs`
Wires buttons to scene transitions. Holds `[SerializeField]` references to `ScenarioDynamicUI` and `ScenarioListUI`.

```csharp
StartSimulation()  // CollectValues() → SceneManager.LoadScene("Simulation")
GoFreeMode()       // SceneManager.LoadScene("Simulation") with no config
ExitApp()          // Application.Quit()
```

## Scenario JSON format

Files live in `Assets/Resources/Scenarios/`. Filename (without `.json`) becomes the sort key. Each file:

```json
{
  "scenarioId": "open_water_free_roam",
  "title": "Open Water Free Roam",
  "parameters": [
    { "type": "header", "label": "Own Vessel" },
    { "type": "dropdown", "id": "shipClass", "label": "Ship Class",
      "options": ["cargo_general", "tanker_lng"] },
    {
      "type": "row", "id": "vesselKinematics",
      "children": [
        { "type": "slider", "id": "initialSpeed", "label": "Speed (knots)", "min": 0, "max": 25, "defaultValue": 10 },
        { "type": "input",  "id": "initialHeading", "label": "Heading (degrees)", "defaultText": "000.0" }
      ]
    },
    { "type": "header", "label": "Environment" },
    {
      "type": "row", "id": "envConditions",
      "children": [
        { "type": "slider",   "id": "timeOfDay",      "label": "Time of Day (hr)", "min": 0, "max": 23, "defaultValue": 7 },
        { "type": "dropdown", "id": "weatherPreset",  "label": "Weather Preset",
          "options": ["Clear", "Fog", "Rain", "Thunderstorm"] }
      ]
    }
  ]
}
```

## Current scenarios (5 total)

| File | Title | Key parameters |
|------|-------|----------------|
| `open_water_free_roam.json` | Open Water Free Roam | shipClass, speed+heading (row), timeOfDay+weather (row) |
| `target_ship_passing.json` | Target Ship Passing | + obstacleType, obstacleSpeed+heading (row) |
| `manoeuvring_trials.json` | Manoeuvring Trials | + trialType, zigZagAngle |
| `harbour_approach.json` | Harbour Approach | + harbourLayout, tugAssist, windSpeed+currentSpeed (row) |
| `rough_weather_navigation.json` | Rough Weather Navigation | + windSpeed+waveHeight (row), windDirection, seaState |

## Editor tooling

- `Assets/Editor/ScenariosUISetup.cs` — `[MenuItem("Ship Simulator/Setup Scenarios UI")]` — builds the static structural GameObjects (panels, scroll view, header, footer). Run once to set up the scene; dynamic content is generated at runtime by ScenarioListUI/ScenarioDynamicUI.
- `Assets/Editor/ScenarioConfigAssetEditor.cs` — `[CustomEditor(typeof(ScenarioConfigAsset))]` — shows color-coded parameter rows in the Inspector.

## Known pitfalls / lessons learned

1. **JsonUtility recursive types** — `ScenarioParameter[] children` inside `ScenarioParameter` always deserializes as null. Solution: separate `ScenarioRowChild` class.
2. **Slider Fill anchor** — must be `anchorMax = new Vector2(0, 1)`, NOT `Vector2.one`. `Vector2.one` fills the entire parent.
3. **Slider value before rects** — assign `fillRect`/`handleRect` before `SetValueWithoutNotify`, otherwise initial fill position is wrong.
4. **[SerializeField] default drift** — changing C# defaults doesn't update existing scene instances. Use `const` for values that must always match the code, or reset the component in the Inspector.
5. **DestroyImmediate in edit mode** — `Destroy` is deferred and will not work in `OnEnable` when `[ExecuteAlways]` is active. Always use `DestroyImmediate` there.
6. **FindObjectOfType obsolete** — Unity 6 deprecates `FindObjectOfType<T>()`. Use `FindAnyObjectByType<T>()`.
7. **Scene save blocked in Play Mode** — `MCP save_scene` fails during Play Mode. Component changes made via MCP while in Play Mode are not persisted. Exit Play Mode before saving.
