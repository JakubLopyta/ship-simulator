# Unity Ship Simulation UI

A Unity-based interface for controlling and monitoring a simulated ship's behavior. The interface supports multiple ships and separates the UI logic from the simulation logic using the `ShipUIController` script.

## 📦 Features

- Real-time ship control via UI:
  - Engine power (0–100%)
  - Rudder angle
- Real-time data display:
  - Speed (m/s)
  - Rotation rate (°/s)
  - Course Over Ground (COG)
  - Heading (HDG)
  - Speed Over Ground (SOG)
- Play, Pause, Stop, and Return buttons to manage simulation state
- Easily extendable for multiple ships

## 🧩 Project Structure

- `Ship.cs` – MonoBehaviour attached to the ship GameObject. Handles simulation logic and ship state.
- `ShipUIController.cs` – UI logic and controls for interacting with a single `Ship` reference.
- `Canvas` – UI elements like sliders, input fields, and buttons.
- `EventSystem` – Required Unity component for handling UI events.

## 🚀 How to Use

1. Add the `Ship.cs` component to your ship GameObject.
2. Create a UI Canvas with sliders, input fields, and text fields.
3. Attach the `ShipUIController` script to a UI GameObject (e.g., an empty `UIManager`).
4. Assign all UI references (buttons, sliders, text fields) to the `ShipUIController` in the Inspector.
5. Assign the `Ship` reference to `ShipUIController.shipReference` by dragging your ship GameObject.

## 🛠 Requirements

- Unity 2020+ (tested on 2021.3+)
- TextMeshPro package