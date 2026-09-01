# Independent Study Syllabus

## Physics-Based First-Person Movement and Parkour in Unity

## Course Description

This independent study focuses on learning the Unity game engine and applying software engineering, 3D mathematics, and physics concepts to the development of a first-person parkour movement system.

The first portion of the course will focus on learning Unity through smaller programming, physics, and movement projects. Development will then transition into an iterative first-person movement system, beginning with basic movement and progressively introducing acceleration, momentum, air control, slopes, sliding, slide jumping, wall jumping, and wall riding.

The course is intentionally front-loaded, with the primary development of the movement controller occurring during the first eight weeks. The later portion of the semester will focus on wall mechanics, system integration, testing, environment design, and documentation.

The central focus of the project will be the preservation and redirection of player momentum across different movement states and environmental interactions.

If the required movement system is completed ahead of schedule, automatic bar traversal and rail grinding will be explored as stretch goals.

## Learning Objectives

By the end of this independent study, I will be able to:

- Navigate Unity and understand its scene, object, component, prefab, physics, and scripting systems.
- Develop gameplay systems using C# and Unity's scripting API.
- Apply vectors, acceleration, gravity, collision detection, raycasts, and surface normals to 3D movement.
- Design and implement a modular first-person character controller.
- Implement acceleration, deceleration, friction, gravity, air control, and slope-based movement.
- Preserve and redirect player momentum between different movement states.
- Implement momentum-based sliding and slide jumping.
- Implement wall jumping and sustained wall riding.
- Design a 3D environment around the capabilities and limitations of a movement system.
- Analyze and tune movement mechanics based on responsiveness, consistency, and player feedback.
- Document the architecture and mathematical behavior of a complete movement system.
- Explore additional traversal mechanics as time permits.

## Tools and Technologies

- Unity
- C#
- Unity Input System
- Unity Physics
- Blender or other 3D modeling software
- Git / GitHub
- Unity Profiler and debugging tools

---

# Section I: Unity Fundamentals and Core Movement

## Week 1: Unity Fundamentals and Scene Construction

### Goal

Become comfortable working inside Unity and understand the basic structure of a Unity project.

### Tasks

- Learn the Unity Editor, Scene View, Game View, Hierarchy, Inspector, Project window, and Console.
- Understand GameObjects, Components, Transforms, Scenes, Materials, Colliders, and Prefabs.
- Learn Unity's coordinate system and basic 3D transformations.
- Create and manipulate simple 3D objects.
- Import basic custom 3D models into Unity.
- Create a small 3D environment containing platforms, ramps, obstacles, lighting, and collision.
- Establish a Git repository and project structure.

### Deliverable

A small explorable 3D scene built from Unity primitives and imported assets.

---

## Week 2: C# and Unity Gameplay Programming

### Goal

Learn how C# scripts interact with objects, components, input, and events inside Unity.

### Tasks

- Learn `MonoBehaviour`, `Start`, `Update`, and `FixedUpdate`.
- Understand Unity's component-based scripting model.
- Read player input using Unity's Input System.
- Manipulate GameObjects and Components through scripts.
- Use serialized fields to expose configurable parameters.
- Work with collisions and trigger volumes.
- Create small scripted interactions such as moving platforms, launch pads, switches, and hazards.
- Practice communication between scripts and GameObjects.

### Deliverable

A small interactive environment containing multiple independently scripted objects and physics interactions.

---

## Week 3: 3D Physics and Vector Mathematics

### Goal

Develop an understanding of the physics and mathematics required for the final movement system.

### Tasks

- Experiment with Unity's physics systems.
- Apply forces, impulses, acceleration, drag, and gravity.
- Work with `Vector3` position, direction, and velocity.
- Use vector magnitude and normalization.
- Use dot products and cross products.
- Project vectors onto surfaces and directions.
- Calculate movement relative to surface normals.
- Use raycasts and sphere casts to detect environmental geometry.
- Experiment with ramps and angled surfaces.
- Visualize velocity vectors, collision normals, and raycasts for debugging.

### Deliverable

A physics demonstration scene showing forces, velocity, raycasts, surface normals, and interactions with differently angled surfaces.

---

## Week 4: Basic First-Person Movement

### Goal

Develop the first functional version of the first-person character controller.

### Tasks

- Implement mouse-controlled first-person camera movement.
- Implement directional WASD movement.
- Implement basic player acceleration and movement speed.
- Implement gravity.
- Implement grounded detection.
- Implement jumping.
- Handle collision with environmental geometry.
- Handle basic slopes and stairs.
- Track player velocity and current speed.
- Expose important movement parameters for testing.

### Deliverable

A functional first-person controller capable of walking, running, looking, jumping, and navigating a simple obstacle course.

---

## Week 5: Smooth Movement and Controller Architecture

### Goal

Transform the basic controller into a smooth, configurable movement system capable of supporting more advanced mechanics.

### Tasks

- Replace immediate velocity changes with acceleration-based movement.
- Implement smooth deceleration.
- Implement ground friction.
- Improve directional changes while moving.
- Tune maximum ground speed.
- Refine camera and mouse movement.
- Separate player input from movement calculations where appropriate.
- Design a movement state architecture.
- Establish initial movement states including:
  - Grounded
  - Airborne
  - Sliding
  - Wall Riding
- Create debugging tools for displaying:
  - Player speed
  - Velocity
  - Grounded state
  - Current movement state
  - Detected surface information
- Begin construction of a gray-box parkour testing environment.

### Deliverable

Version 2.0 of the first-person controller featuring smooth acceleration, deceleration, friction, configurable movement parameters, and an architecture designed for additional traversal mechanics.

---

# Section II: Momentum-Based First-Person Parkour

## Week 6: Air Movement, Momentum, and Slopes

### Goal

Expand the movement system to preserve momentum across jumps and respond naturally to terrain.

### Tasks

- Implement separate ground and air acceleration.
- Implement controllable air movement.
- Preserve horizontal momentum when leaving the ground.
- Improve transitions between grounded and airborne states.
- Improve slope detection using surface normals.
- Project movement appropriately across angled surfaces.
- Allow downhill movement to increase player speed.
- Reduce player velocity appropriately when traveling uphill.
- Prevent unintended acceleration from invalid surfaces.
- Tune gravity and jumping around the completed momentum model.
- Expand the testing environment with ramps and slope-based movement challenges.

### Deliverable

A momentum-based movement system that supports smooth ground movement, air control, jumping, and slope-based acceleration.

---

## Week 7: Sliding and Terrain Interaction

### Goal

Implement sliding as the first major momentum-based parkour mechanic.

### Tasks

- Implement crouching and sliding.
- Require appropriate player velocity to initiate a slide.
- Reduce friction while sliding.
- Preserve momentum when entering a slide.
- Gradually reduce momentum while sliding across flat terrain.
- Accelerate the player while sliding downhill.
- Decelerate the player while sliding uphill.
- Handle transitions between running, sliding, and airborne movement.
- Prevent sliding from producing unintended infinite acceleration.
- Expand the testing environment with downhill, uphill, and flat sliding sections.

### Deliverable

A functional momentum-based sliding system that responds to player speed, terrain angle, and existing velocity.

---

## Week 8: Slide Jumping, High-Speed Movement, and Core Integration

### Goal

Complete the primary ground movement system and integrate running, jumping, slopes, sliding, and slide jumping into a consistent momentum model.

### Tasks

- Implement jumping directly from a slide.
- Preserve forward momentum during a slide jump.
- Tune the relationship between slide velocity and jump velocity.
- Determine whether controlled momentum boosts should occur during successful slide jumps.
- Improve airborne control following a slide jump.
- Add speed-responsive field-of-view changes.
- Add visual feedback for high-speed movement.
- Test repeated slide-jump sequences for unintended acceleration.
- Test transitions between all existing movement states.
- Tune running, jumping, slope traversal, sliding, and slide jumping as a complete system.

### Deliverable

A complete core locomotion system supporting smooth movement, momentum, slopes, air control, sliding, slide jumping, and high-speed movement feedback.

---

## Week 9: Wall Detection and Wall Jumping

### Goal

Allow environmental surfaces to redirect the player's airborne momentum.

### Tasks

- Detect walls adjacent to the player.
- Determine valid wall surfaces using surface normals and angles.
- Determine which side of the player contains the detected wall.
- Implement directional wall jumping.
- Calculate wall-jump direction using player velocity and wall normals.
- Preserve appropriate forward momentum when leaving a wall.
- Redirect momentum away from the contacted surface.
- Prevent repeated exploitation of the same wall.
- Tune wall-jump timing and player control.
- Add wall-jump testing sections to the parkour environment.

### Deliverable

A reliable wall-jumping system capable of redirecting player momentum using environmental geometry.

---

## Week 10: Wall Riding and Wall Running

### Goal

Extend wall interaction into sustained momentum-based traversal.

### Tasks

- Implement a wall-riding movement state.
- Determine the player's direction of travel relative to a wall.
- Project player velocity along the wall surface.
- Modify gravity while wall riding.
- Preserve momentum when entering a wall ride.
- Establish minimum velocity requirements for entering and maintaining a wall ride.
- Implement wall-ride termination conditions.
- Transition between wall riding, wall jumping, airborne movement, and ground movement.
- Add camera feedback during wall traversal.
- Expand the environment with wall-riding sections.

### Deliverable

A functional wall-riding system capable of preserving player momentum along valid wall surfaces.

---

## Week 11: Wall Integration and Movement Chaining

### Goal

Complete the required movement feature set by integrating wall mechanics with the existing momentum system.

### Tasks

- Support transitions between multiple wall surfaces.
- Improve transitions between wall riding and wall jumping.
- Preserve velocity across appropriate movement-state changes.
- Redirect momentum rather than resetting player velocity.
- Test transitions between walls, ground movement, slides, and airborne movement.
- Eliminate unexpected momentum loss during transitions.
- Prevent unintended or infinite acceleration.
- Improve player control at high speeds.
- Test complex movement sequences.
- Tune wall mechanics alongside the existing movement controller.

Example traversal sequence:

`Run -> Downhill Ramp -> Slide -> Slide Jump -> Wall Ride -> Wall Jump -> Landing`

### Deliverable

A feature-complete first-person parkour controller supporting smooth movement, momentum, slopes, sliding, slide jumping, wall jumping, and wall riding.

---

## Week 12: Movement System Integration and Stability

### Goal

Improve the reliability and consistency of the completed movement system without introducing additional required mechanics.

### Tasks

- Conduct systematic testing of all movement states.
- Identify conflicts between movement mechanics.
- Fix movement-state transition bugs.
- Test unusual collision and terrain conditions.
- Test movement at unusually high and low velocities.
- Fix camera clipping or movement feedback issues.
- Refine ground and wall detection.
- Address physics exploits and unintended acceleration.
- Tune acceleration, friction, gravity, air control, and jump forces.
- Refactor or simplify movement code where necessary.

### Deliverable

A stable and integrated version of the required movement system suitable for use in the final parkour environment.

---

## Week 13: Parkour Environment and Playtesting

### Goal

Create a complete 3D environment specifically designed to demonstrate and evaluate the movement system.

### Tasks

- Expand the existing gray-box testing environment into a deliberate parkour course.
- Design sections involving:
  - Ramps
  - Slides
  - Walls
  - Gaps
  - Vertical traversal
  - High-speed movement
- Establish obstacle spacing based on player speed and jump distance.
- Create routes requiring multiple movement mechanics to be chained together.
- Create alternate routes or shortcuts that reward maintaining momentum.
- Import or create simple environmental assets where appropriate.
- Conduct playtesting with the completed movement system.
- Adjust the environment and controller based on testing results.

### Deliverable

A complete 3D parkour demonstration environment designed around the required movement mechanics.

---

## Week 14: Polish and Optional Advanced Traversal

### Goal

Polish the completed project and explore an additional traversal mechanic if the required movement system is complete and stable.

### Core Tasks

- Fix remaining movement bugs and edge cases.
- Improve transitions between movement states.
- Refine speed-responsive camera effects.
- Improve sliding and wall-riding camera feedback.
- Add audio or visual feedback where appropriate.
- Continue movement tuning based on playtesting.
- Profile the project and address significant performance issues.

### Stretch Goal: Automatic Bar Traversal

If the required movement systems are complete and stable:

- Create environmental traversal bars.
- Detect valid bar interactions automatically.
- Determine whether player direction and speed permit interaction.
- Automatically attach the player without requiring additional input.
- Measure incoming player velocity.
- Convert incoming horizontal velocity into rotational movement around the bar.
- Automatically determine an appropriate release point.
- Automatically release the player.
- Redirect momentum primarily into vertical and forward launch velocity.
- Scale the resulting launch based on incoming speed.
- Integrate bar traversal with existing airborne movement.

### Deliverable

A polished version of the required movement system. Automatic bar traversal may be included if development time permits.

---

## Week 15: Final Testing, Documentation, and Optional Rail Grinding

### Goal

Complete testing and documentation of the project while using any remaining development time for final polish or additional experimentation.

### Core Tasks

- Conduct end-to-end testing of all required traversal mechanics.
- Fix remaining high-priority bugs.
- Record final movement parameters.
- Document movement architecture and state transitions.
- Document relevant vector mathematics and physics calculations.
- Explain how momentum is preserved or redirected by each major traversal mechanic.
- Compare the initial first-person controller with the completed movement system.
- Record a final demonstration of the parkour environment.
- Complete a written reflection on technical challenges, design decisions, and results.

### Stretch Goal: Rail Grinding

If the required project, documentation, and automatic bar traversal are complete:

- Create grindable rail objects.
- Detect valid rail contact.
- Automatically attach the player to a rail.
- Determine travel direction using incoming velocity.
- Constrain movement along the rail.
- Preserve momentum when entering and leaving the rail.
- Accelerate or decelerate based on rail slope.
- Allow the player to jump from the rail.
- Transfer rail velocity back into normal airborne movement.

### Final Deliverables

- Playable first-person parkour demonstration.
- Complete Unity project and C# source code.
- 3D parkour demonstration environment.
- Documented movement architecture.
- Movement-state diagram.
- Technical explanation of physics and momentum calculations.
- Final demonstration video.
- Written reflection on development and results.
- Any completed stretch-goal traversal mechanics.

---

# Required Project Scope

Successful completion of the independent study will require implementation and integration of the following systems:

1. Basic first-person movement
2. Smooth acceleration and deceleration
3. Ground friction and momentum
4. Air control and jumping
5. Slope-based movement and downhill acceleration
6. Sliding
7. Slide jumping
8. Wall detection
9. Wall jumping
10. Wall riding
11. Movement chaining and momentum preservation
12. A 3D parkour environment demonstrating the completed movement system

---

# Stretch Goals

The following mechanics are outside the required scope of the independent study and will only be pursued after the required movement system is complete and stable.

## Automatic Bar Traversal

The player will automatically interact with horizontal traversal bars when approaching from a valid direction and speed.

The system will use the player's incoming velocity to automatically attach the player to the bar and convert horizontal momentum into rotational movement. The player will then automatically release from the bar, redirecting the incoming momentum primarily into vertical and forward velocity.

This mechanic will explore the conversion and redirection of player momentum without requiring explicit grab or release input.

## Rail Grinding

The player will automatically attach to valid rails and travel along their path while preserving momentum.

Rail slope may influence acceleration and deceleration, and momentum will be transferred back into normal movement when the player leaves or jumps from the rail.

Rail grinding will be treated as a secondary stretch goal and will only be pursued if the required project and other higher-priority development tasks are complete.

---

# Evaluation

Progress will be evaluated based on:

- Completion of weekly milestones.
- Functionality and stability of the first-person movement controller.
- Quality and responsiveness of basic and smooth movement.
- Successful implementation of momentum-based sliding and slide jumping.
- Successful implementation of wall jumping and wall riding.
- Effective application of 3D mathematics and physics concepts.
- Preservation and redirection of momentum between movement states.
- Quality and maintainability of the C# codebase.
- Integration and consistency between movement mechanics.
- Quality of the final parkour demonstration environment.
- Clarity and completeness of technical documentation.
- Analysis of design decisions, technical challenges, and results.

Completion of automatic bar traversal and rail grinding is not required for successful completion of the independent study.