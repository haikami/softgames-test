# Unity Developer Assignment — Hayk Papoyán

Project built in **Unity 6000.4.91f** using the **Built-in Render Pipeline**.

## Running the project

Open the **Preload** scene located in `Scenes` and press **Play**.

> **Emoji attribution**
>
> Emojis used in **Magic Words** come from:
> https://github.com/jdecked/twemoji

---

# Project Architecture

I approached this assignment as if it were a small production game rather than three unrelated demos, since that felt closer to how this would actually be evaluated in a studio environment.

Each task is treated as a **feature**.

A feature is defined by a configuration asset that contains:

- Display name (shown in the menu)
- Scene to load
- Any feature specific configuration

The **Preload** scene initializes all shared services once and then loads the main menu. The menu reads a catalog of feature configurations and automatically creates a button for each one.

Adding a new feature later would simply require creating:

- a new configuration asset
- a new scene

No menu code would need to change.

---

## Top Bar

A persistent top bar exists throughout the application.

Each feature configures it when loaded:

- which buttons are visible
- what actions they perform

To keep the UI simple, I exposed a single **Cheat** button that every feature configures for its own debugging action instead of creating separate buttons per task.

---

## Code Organization

The project is split into Assembly Definitions:

- **Core**
  - Shared services
  - Main menu
  - Generic infrastructure
- **One assembly per task**
  - Keeps every feature self contained
  - Prevents accidental dependencies between tasks

I also included a small unit test suite covering two Core systems:

- Object Pool
- Network Service

---

## Aspect ratio

The UI was built to behave reasonably across different aspect ratios (both portrait and landscape) rather than being tuned for a single target resolution.

---

## Core

The Core assembly contains shared systems such as:

- Main menu
- Scene loading
- Object pooling
- Networking
- Other reusable logic and assets

---

## Feature Structure

Each feature follows the same overall structure:

- A main `TaskController` (named after the feature)
    - Initializes subsystems
    - Validates setup
    - Orchestrates the feature flow
- Separate folders for:
    - Data
    - Logic
    - Interfaces
- A `Configs` folder containing the feature's ScriptableObjects
- Proper cleanup logic:
    - Return pooled objects
    - Remove event subscriptions
    - Cancel in-flight network requests
    - Kill active tweens

---

## Design Note

After finishing all three tasks I noticed they share a fair amount of structure (reset flow, cleanup pattern, config resolution during startup).

I intentionally chose **not** to abstract those into a shared base class to avoid complexity without providing much value.

---

# Task 1 — Ace of Shadows

The task is configurable through a ScriptableObject, including:

- Number of cards
- Interval between moves
- Animation timings

Two stacks are created:

- one starts full
- one starts empty

A dedicated class moves the top card between stacks, randomly selecting from a small set of animation styles each time.

## Performance

Cards are obtained from an object pool rather than repeatedly instantiated and destroyed.

Each stack owns its own Canvas so that animating or dragging one stack does not force Unity to rebuild the layout of unrelated UI elements.

## Extras

- Drag stacks around while the shuffle animation is still running.
- Cheat button that runs the entire demo at **3× speed** by adjusting the game's time scale.

---

# Task 2 — Magic Words

A loading screen is displayed while dialogue data is downloaded.

Once received:

1. The raw response is mapped into an internal model.
2. A dedicated controller plays the conversation.
3. Each line appears together with its avatar.

Avatar loading is handled up front:

- All avatars begin downloading together.
- They are given a short grace period.
- After that:
    - avatars still loading display a loading state
    - failed or missing avatars fall back to a default icon

I interpreted the `"left"` and `"right"` values from the response as determining which side of the conversation each character should appear on.

---

## Networking

Networking is handled by a reusable service in **Core**.

It wraps an `IWebRequester` implementation and provides:

- retries
- timeouts
- configurable behavior

This allows the transport implementation to be swapped while keeping retry logic independently testable.

---

## Response Edge Cases

Handled:

- Duplicate avatar names → keep the first occurrence.
- Avatars never referenced by dialogue → discarded, sprite not fetched.
- Missing, failed, or timed-out avatars → default icon.

---

## Edge Cases Considered

I thought about implementing these but chose not to for this assignment:

- Ensuring there is always at least one character on both the left and right sides.
    - In the provided sample data, Sheldon appears with both left and right entries, and because he is the only character marked as left, he ends up being selected for that side.
- If multiple avatar entries exist for a character, downloading all of them and randomly choosing among the successfully loaded ones.

---

## Emoji Handling

Emojis were mapped into a TextMeshPro sprite sheet using the same identifiers returned by the endpoint.

This makes displaying them as simple as replacing:

```text
{emojiID}
```

with:

```text
<sprite name="emojiID">
```

No additional parsing logic is required.

For this assignment I included only the emojis referenced by the provided endpoint. In a production project I would include whatever subset the game's content requires.

The emoji sprites are displayed approximately **25% larger** than surrounding text so they stand out naturally within dialogue.

---

## Extras

A cheat button toggles between:

- the real endpoint
- local mock data

Some of the edge cases listed above don't actually exist in the supplied endpoint data given the way I handled errors, so I created a small local dialogue that demonstrates the different avatar states. It also allows development without depending on the mock server being available.

---

## Future Improvements

Ideas I considered but didn't implement:

- Tap to skip to the next dialogue line.
- Animate avatars, dialogue bubbles, and text as they appear.
- Smoother scrolling while new dialogue lines are added.

---

# Task 3 — Phoenix Flame

The flame is implemented as a reusable prefab.

It consists of:

- a parent object
    - Animator
    - Animation Clips
- three child particle systems layered together to create the final flame effect

Each animation clip only changes one property:

- the material color of each particle system.

Transitions between flame colors are performed using `Animator.CrossFade()`, allowing smooth blending without manually authoring transitions between every possible color pair.

The prefab exposes a small public API:

- Play
- Advance to next color
- Reset

It has no knowledge of menus, configs, or the rest of the project, allowing it to be dropped into any scene independently.

The task controller simply interacts with this API whenever the user presses the color cheat button.

---

# Closing

Thank you for taking the time to review my submission, and for the opportunity.