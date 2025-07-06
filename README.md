# README: Singularity

This document provides an overview of the "Singularity" project, its features, and its technical architecture. It was developed as a vertical slice prototype in under 7 hours to demonstrate a "juicy" core gameplay loop and a scalable architecture.

---

## Story

The game, to me, represents, abstractly the cosmic life cycle of a black hole. The player begins as an insignificant seed of gravity in a vast, empty universe, having little effect on the world.

Through interaction (clicking) and consumption (absorbing orbiting bodies), the singularity grows in power and influence. Each level represents a new stage of its evolution, where its effect on the environment becomes more profound, warping space, changing the ambient color and light, and pulling in matter from further away.

The final level culminates in the singularity becoming so massive that it consumes the fabric of reality itself, collapsing into a new universe where a new, insignificant singularity is born, repeating the cycle endlessly.

---

## Basic Controls

* **Click** the central circle (the "Singularity") to interact with it.
* Press the **Escape** key to quit the game.

---

## Main Features

* **Evolving Core Gameplay:** The central "singularity" grows larger as it absorbs orbiting objects. The game progresses through multiple levels, with each level changing the visuals, audio, and gameplay parameters.

* **Thematic Audio-Visual Arc:** The game's 11 levels take the player on a complete sensory journey. The audio evolves from a meditative click and slow, melodic ambient music in the early stages to deep, bass-heavy clicks and fast, chaotic electronic soundtracks in the final levels. This is mirrored by the post-processing effects, which start at nearly negligible values and grow to intensely warp the screen, reflecting the singularity's growing power.

* **Data-Driven Levels:** The entire game flow, from the progression goal to the color palette, camera zoom, and the 11 unique audio tracks and sound effects, is controlled by **Scriptable Objects**.

* **Dynamic Physics-Based Environment:** Orbiting bodies are controlled by a custom physics script, applying gravitational forces to create emergent, dynamic movement.

* **Procedural "Juice" Effects:** Each click is accentuated by a suite of effects including procedural shockwaves, camera shake, and a post-processing lens distortion pulse, which are also handled through **Scriptable Objects**.

---

## Project Structure & Architecture

The project is structured in a decoupled, three-layer architecture to ensure scalability and maintainability.

* **1. Data Layer (Scriptable Objects):**
    * **`LevelData.cs`:** Defines the state of each level (progression goal, visuals, audio, object spawning).
    * **`ClickData.cs`:** Defines the immediate feedback of a single click. This separation makes the system highly modular.

* **2. Controller/Manager Layer:**
    * **`SingularityController.cs`:** The central "brain" of the game. It manages state, reads from the SOs, and directs all other systems.
    * **`ObjectSpawner.cs`:** Manages the lifecycle of the orbiting bodies.

* **3. Component Layer:**
    * **`PhysicsOrbitalMovement.cs`:** A self-contained script on each orbiting body that handles its movement, attraction, and absorption.
    * Helper scripts like `CameraShaker` and `ProceduralShockwave` handle specific effects.

### Use of Prefabs

**Prefabs** are used extensively for all dynamic game objects to allow for easy modification and variation.
* **Absorbable Objects:** Each type of orbiting body is a prefab. This allows for creating many visual variations and assigning them to the `LevelData`'s spawn list. Modifying one of these prefabs will instantly update every instance of it spawned by the game.
* **Visual Effects:** The `Shockwave` and `Absorption VFX` are also prefabs. This makes it trivial to swap out effects or tweak their appearance in one place without changing any code.

### Architectural Justification
* **Iterability:** When the main appeal of a game is more about game "feel" rather than skill, competition or engaging gameplay, It becomes very important that one can try out many different configuration to find the best game "feel" or "juice".
* **Decoupled Data (Scriptable Objects):** This was the core architectural choice. A designer can create, modify, and balance an infinite number of levels by editing `.asset` files in the editor, with no code changes required. This makes the project extremely flexible and scalable.
* **Event-Based Communication:** A `static event` (`OnSingularityClicked`) allows the many orbiting objects to react to a click without the `SingularityController` needing to know about them. This is a clean and efficient broadcasting system.

---

## Development Context & Future Enhancements

This prototype was developed in **under 7 hours** with the primary goal of creating a polished and satisfying core interaction, supported by a robust and scalable architecture. The current build represents a complete "vertical slice" of the core gameplay loop.

With additional time, the focus would shift from the core loop to building out the meta-game and player-facing systems to make it a more complete and dynamic experience.

### 1. Player-Facing Systems (Main Menu & Settings)

* **Home Screen:** A dedicated main menu scene would be the first thing the player sees, featuring options to "Start New Cycle," access "Settings," and "Quit." This screen could also display stats from previous cycles.
* **Settings Menu:** A comprehensive settings menu would be implemented to give players control over their experience, including audio sliders (Master, Music, SFX) and graphics toggles for performance.

### 2. Data-Driven Live Updates (JSON / Server)

While the project currently uses Scriptable Objects, the next professional step would be to read this data from external `.json` files at startup. This would allow for balancing, hotfixing, or adding new levels without requiring the player to download a new build of the game.

### 3. Enhanced Player Agency (Upgrade System)

To make the game "more of a game," player choice would be introduced.
* **Mass as Currency:** Instead of automatically progressing, absorbing objects would grant "Mass."
* **Upgrade Tree:** This Mass could be spent on a simple UI panel to purchase permanent or temporary upgrades, such as increased gravity or a stronger click-attraction pulse. This would give players a reason to complete the cycle multiple times, trying different "builds."

---

## Optimization Notes

* **Efficient Object Tracking:** The ObjectSpawner needs to know how many orbiting bodies exist to respect the per-level maximum. Instead of using an expensive, frame-by-frame FindObjectsByType() call, we use a simple static int counter on the PhysicsOrbitalMovement script. This counter is incremented when an object is enabled and decremented when it's disabled, providing an extremely fast and efficient way to track the population.
* **Audio Management:** Sound effects and music are handled by separate `AudioSource` components. Click SFX are set to interrupt the currently playing sound, preventing audio channel overload from rapid spam-clicking.
* **Physics:** The central singularity uses a `Static` Rigidbody2D, which is optimal as it participates in collisions without requiring expensive physics calculations.
* **Draw Call Batching:** To render many sprites efficiently, they must be batched into as few draw calls as possible. This was achieved by: Ensuring all orbiting objects of a similar type can share the same Material. Unity's URP Sprite Batcher can automatically group objects that use the same texture and material, significantly improving performance when many objects are on screen.


## Developed by:
Omar Zaki
