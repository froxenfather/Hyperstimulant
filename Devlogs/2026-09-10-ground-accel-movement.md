# 2026-09-10 — Ground movement: snap → acceleration-based

## Goal

Grounded movement used to be instant: `rb.linearVelocity = moveDir * moveSpeed`. You hit
full speed on frame one and stopped dead on release. The air movement (accelerate toward a
direction, momentum carries) felt much better, so the goal was to give the ground that same
"ease toward a target" feel — while making sure holding a key can never push you past the
walk/sprint speed, and that letting go stops you almost immediately.

This is Pass 1 of a bigger plan (air-model rework, slide, wall jump, universal speed
ceilings, and eventually moving off Rigidbody to a hand-rolled controller). Only the
grounded branch of `Move()` changed here.

## What changed

**File:** `Assets/Scripts/Player Scripts/BaseMovement.cs`

- Removed `moveSpeed`. Added:
  - `walkSpeed = 5.5`
  - `sprintSpeed = 7.5`
  - `groundAccel = 55` — rate we ramp toward the target speed when below it
  - `groundOverspeedDecel = 12` — gentler rate we ease *down* to the target when we're
    already above it (arrived hot from a launch pad / air landing)
  - `groundFriction = 120` — very high, so releasing all keys stops you near-instantly
- Added `sprinting` bool, read in `ReadInput()` from `leftShiftKey.isPressed` (hold to sprint).
- Grounded branch of `Move()` rewritten:

  ```csharp
  Vector3 relVel = rb.linearVelocity - supportVelocity;      // support-relative
  Vector3 horiz  = new Vector3(relVel.x, 0f, relVel.z);
  float targetSpeed = sprinting ? sprintSpeed : walkSpeed;

  if (hasInput)
  {
      float rate = horiz.magnitude > targetSpeed ? groundOverspeedDecel : groundAccel;
      horiz = Vector3.MoveTowards(horiz, moveDirection * targetSpeed, rate * dt);
  }
  else
  {
      horiz = Vector3.MoveTowards(horiz, Vector3.zero, groundFriction * dt);
  }

  rb.linearVelocity = horiz + supportVelocity  (Y left untouched);
  ```

## Reasoning / decisions

- **Why `Vector3.MoveTowards` toward `moveDir * targetSpeed`.** It's self-limiting by
  construction: it moves the current horizontal velocity toward the target vector at a fixed
  units/sec rate and never overshoots. So:
  - Below target → accelerates up to it, then holds exactly at it. Holding W forever settles
    at 5.5 and never creeps higher — no cap check needed.
  - Above target (came in from a boost) → the same call decelerates you toward it, at the
    gentler `groundOverspeedDecel` rate so it feels like a skid, not a wall.
  - Turning the camera → the target vector rotates, and MoveTowards redirects velocity
    toward it. Natural steering.
- **Two rates (`groundAccel` vs `groundOverspeedDecel`).** If we used one rate, landing a
  launch-pad boost while holding forward would brake hard and feel bad. Splitting them lets
  acceleration-from-slow be punchy (55) while over-speed bleed-off is soft (12, ~1s from 20
  down to 7.5).
- **`groundFriction = 120` (near-instant stop).** Deliberately not smooth. Requested feel:
  the moment you let go of the keys you're planted. Still routed through MoveTowards +
  `Time.fixedDeltaTime` so it's frame-rate independent rather than a hard `= 0`.
- **Support-relative velocity.** Kept the existing moving-platform handling: subtract
  `supportVelocity` before doing the math, add it back after, so the accel logic works in the
  platform's frame and you don't fight its motion.
- **Air branch untouched.** `airControl`, `maxAirSpeed`, `LimitAirSpeed()` all still there.
  The air-model rework (directional soft cap, no hard clamp on total speed) is Pass 2.

### Air-cap model — decisions logged for later (not implemented yet)

Went back and forth on how air should cap:

1. **Hard clamp on total horizontal speed** (what's there now, `maxAirSpeed = 8`) — rejected:
   feels like hitting glass, kills boosts instantly.
2. **Quake-style tiny wish-speed (~2.5)** — rejected: that's exactly what enables the
   "hold strafe + swing the mouse to fly forward forever" exploit. Each tick adds a small
   perpendicular sliver that lengthens velocity without redirecting it.
3. **Directional soft cap (chosen for Pass 2):** project current velocity onto the
   camera-relative wish direction; if that projected speed is `< airSoftCap` (~8), ease
   toward it; once at the cap, add nothing (never decelerate — momentum above the cap from
   boosts/slides is preserved). Total speed is not clamped. Diagonals cap along the diagonal
   for free because `moveDirection` is normalized. Look-only does nothing because no
   movement key means `moveDirection` is the zero vector.

## How it felt / tested

TODO — fill in after Play-mode testing:
- Tap forward → short ramp to ~5.5, not instant?
- Hold forward → settles at 5.5, no creep?
- LeftShift + forward → eases to 7.5?
- Release keys mid-run → near-instant stop, feels planted?
- Launch pad → land holding forward → keep speed briefly, skid down to 7.5/5.5 over ~1s?
- Moving platform → still carried, no jitter?

Note: the scene (`SampleScene.unity`) had the old `moveSpeed: 8.5` serialized on the
player. That field is gone, so the new fields load at their code defaults (5.5 / 7.5 / 55 /
12 / 120). Tune in the Inspector on the PlayerMovement component.

## Fix: landing killed all momentum

First test surfaced a regression: jump while running, land without holding a movement key,
and `groundFriction = 120` zeroed horizontal speed in ~2 physics frames — couldn't chain
jumps, landings felt like hitting a wall.

Added a **landing grace window** (`landingGrace = 0.15`): track `wasGrounded` + `landTime`,
and while `Time.time - landTime < landingGrace` skip the no-input friction branch entirely,
so horizontal momentum carries through the landing. Normal near-instant stop resumes after
the window. The `hasInput` path is unchanged (still eases toward walk/sprint at
`groundOverspeedDecel` if you land hot while steering).

Follow-up option if landings still bleed too much while steering: also skip the
`groundOverspeedDecel` path during grace for full momentum preservation.

## Next

Pass 2: replace the air branch with the directional soft-cap model above and delete
`LimitAirSpeed()` + `maxAirSpeed`. Then Pass 3 (sprint-jump boost, slide, wall jump).
