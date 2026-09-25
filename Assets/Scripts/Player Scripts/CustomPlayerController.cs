using System.Collections.Generic;
using IObjects;
using UnityEngine;
using UnityEngine.InputSystem;

// Setup:
// Kinematic Rigidbody + CapsuleCollider on Player root.
// Feet are assumed to sit at the object's pivot.
//
// Controller philosophy:
// - Rigidbody does NOT drive movement.
// - We manually integrate velocity.
// - Capsule sweeps detect collisions before movement.
// - Collisions are resolved using "collide and slide".
// - Kinematic Rigidbody mainly exists for:
//      1. Trigger interactions
//      2. Rigidbody interpolation
//
// This is essentially a custom kinetic character controller.

// Ladies and Gentlemen
// the FRAGNUM OPUS!
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class CustomPlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private bool allowWASD = true;

    [Header("Ground Movement")]
    [SerializeField] private float walkSpeed = 12f;
    [SerializeField] private float sprintSpeed = 20f;

    // How quickly velocity approaches target movement speed.
    [SerializeField] private float groundAccel = 90.9f;

    // Deceleration when already moving faster than our desired speed.
    [SerializeField] private float groundOverspeedDecel = 52.1f;

    // Deceleration when no movement input is held.
    [SerializeField] private float groundFriction = 68.6f;

    // Brief friction-free window after landing.
    // Lets momentum survive landings / allows bunny hopping.
    [SerializeField] private float landingGrace = 0.04f;

    [Header("Air Movement")]
    [SerializeField] private float jumpVelocity = 14.68f;

    // Downward acceleration in m/s^2.
    [SerializeField] private float gravity = 30.01f;

    // Horizontal acceleration while airborne.
    [SerializeField] private float airControl = 15.22f;

    // Prevent infinite horizontal air acceleration.
    [SerializeField] private float maxAirSpeed = 20f;

    // How fast horizontal speed bleeds off when above maxAirSpeed.
    // High = acts like a hard clamp, low = momentum carries through the air.
    [SerializeField] private float airOverspeedDecel = 52.1f;

    [Header("Slide")]
    // Friction while sliding. Way lower than groundFriction so momentum carries.
    [SerializeField] private float slidingGroundFriction = 4f;

    // Speed added the moment a slide starts...
    [SerializeField] private float slideBoost = 5f;

    // ...but only if we're going slower than this. So the most a boost can ever give is about this + slideBoost.
    [Range(0f, 60f)]
    [SerializeField] private float slideBoostMaxSpeed = 25f;

    // Can't start a slide slower than this, and a slide dies on the ground below it.
    [SerializeField] private float slideMinSpeed = 5f;

    // Multiplier on gravity pulling us along a slope while sliding. 1 = plain g * sin(angle).
    [SerializeField] private float slideSlopeGravityScale = 1f;

    // One shared timer, counted from the start of a slide:
    // no new slide can start before it runs out, and a jump inside it is a slide jump.
    [SerializeField] private float slideGrace = 0.5f;

    // Extra horizontal speed a slide jump gives.
    [SerializeField] private float slideJumpBoost = 8f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionMask = ~0;

    // Small gap maintained between capsule and surfaces.
    // Helps avoid floating point collision jitter.
    [SerializeField] private float skinWidth = 0.03f;

    [SerializeField] private float maxSlopeAngle = 60f;
    [SerializeField] private float stepHeight = 0.4f;

    // How far we try to "stick" downward while already grounded.
    [SerializeField] private float groundSnapDistance = 0.3f;

    // Small landing detection distance while airborne.
    [SerializeField] private float groundProbeDistance = 0.05f;

    // Uphill slopes above this angle can launch us when we leave them.
    [SerializeField] private float launchSlopeAngle = 10f;

    // Maximum collision-and-slide attempts in one FixedUpdate.
    [SerializeField] private int maxBumps = 4;

    [Header("Pushing Rigidbodies")]
    [SerializeField] private float playerMass = 10f;
    [SerializeField] private float pushStrength = 1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugHUD = true;
    [SerializeField] private bool drawDebugRays = true;

    // Anything shorter than this counts as "not moving".
    private const float MinMoveDistance = 0.0001f;

    // If the Rigidbody is further than this from where we put it, something else moved us.
    // Squared distance, so 0.0004 means 0.02 meters.
    private const float ExternalMoveThresholdSqr = 0.0004f;

    // Upward speed (m/s) above which leaving a steep slope launches us.
    private const float MinLaunchUpSpeed = 0.05f;

    // Speed (m/s) away from a surface above which we count as leaving it.
    private const float MovingAwaySpeed = 0.1f;

    // Step-up tuning.
    private const float MinStepRaiseHeight = 0.02f;     // less headroom than this: can't step
    private const float StepForwardNudge = 0.05f;       // always try to move at least this far forward
    private const float MinStepForwardDistance = 0.01f; // no room even when raised: it's a real wall
    private const float MinStepHeightGained = 0.01f;    // shorter than this isn't a step
    private const float StepHeightTolerance = 0.01f;    // slack on stepHeight

    // Not worth moving us down for less than this when snapping to the ground.
    private const float MinSnapDistance = 0.002f;

    // How upright a platform must be (dot with world up) to turn us with it.
    private const float PlatformUprightDot = 0.7f;

    // ResolveGroundNormal starts a short ray this far above the contact and casts this far down.
    private const float NormalProbeHeight = 0.1f;
    private const float NormalProbeDistance = 0.2f;

    private Rigidbody rb;
    private CapsuleCollider capsule;

    // Custom authoritative player position.
    // This is the position our controller actually reasons about.
    private Vector3 position;

    // Absolute world-space velocity.
    private Vector3 velocity;

    // Last position we told the Rigidbody to move toward.
    // Used to detect external movement / teleports.
    private Vector3 lastTarget;

    // Velocity inherited from moving platforms.
    private Vector3 currentPlatformVelocity;

    private Vector2 moveInput;
    private bool sprinting;
    private bool jumpQueued;

    // Flat speed we had when we left the ground. Air control can't push us past it.
    private float airSpeedCap;

    // Slide state.
    private bool sliding;
    private bool slideQueued;
    private bool slideHeld;
    private float slideStartTime = -999f;

    private bool grounded;
    private Vector3 groundNormal = Vector3.up;
    private Collider groundCollider;
    private float slopeAngle;
    private float landTime = -999f;

    // Minimum Y component a surface normal needs to count as walkable.
    private float minGroundNormalY;

    // Moving platform tracking.
    private Transform platform;
    private Vector3 platformLocalPos;
    private Quaternion platformLastRot;

    // Reused buffers avoid allocating straight garbage every physics frame.
    private readonly RaycastHit[] hitBuffer = new RaycastHit[16];
    private readonly Collider[] overlapBuffer = new Collider[16];

    // Track collision contacts so events fire once on contact.
    private readonly HashSet<Collider> contactsThisStep = new HashSet<Collider>();
    private readonly HashSet<Collider> contactsLastStep = new HashSet<Collider>();

    public Vector3 Velocity => velocity;
    public bool IsGrounded => grounded;
    public bool IsSliding => sliding;

    // Flat speed relative to whatever we're standing on, so riding a fast platform doesn't count as running.
    public float HorizontalSpeed => Flatten(velocity - currentPlatformVelocity).magnitude;

    // ============================================================
    // UNITY MESSAGES
    // ============================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        SetupRigidbody();

        position = transform.position;
        lastTarget = position;

        CalculateMinGroundNormal();
    }

    private void OnValidate()
    {
        // Recalculate immediately when changing slope angle in Inspector.
        CalculateMinGroundNormal();
    }

    private void Update()
    {
        ReadInput();
    }

    // Everything the controller does each physics step, in order.
    // Read this function top to bottom to see the whole controller.
    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        AcceptExternalMovement();

        // First inherit motion from whatever platform we stand on.
        currentPlatformVelocity = ApplyPlatformCarry(dt);

        // Fix any overlap caused by moving platforms, teleports, etc.
        Depenetrate();

        // Movement logic should happen RELATIVE to the platform.
        //
        // playerVelocity = relativeVelocity + platformVelocity
        //
        // Therefore:
        //
        // relativeVelocity = playerVelocity - platformVelocity
        Vector3 calculatedMovement = velocity - currentPlatformVelocity;

        bool startedGrounded = grounded;

        // Remember our takeoff speed every step we spend on the ground.
        // Whenever we leave it (jump, ledge, launch), the last value stays as the air cap.
        // Walk speed is the floor so a standing jump can still reach walking speed.
        if (startedGrounded)
            airSpeedCap = Mathf.Max(walkSpeed, Flatten(calculatedMovement).magnitude);

        // 1. Change velocity.
        // Leaving the ground can happen two ways: we jump, or the ground launches us.
        bool leftGround =
            Jump(ref calculatedMovement) ||
            LaunchOffMovingGround(calculatedMovement, dt);

        Slide(ref calculatedMovement, dt);
        calculatedMovement = Move(calculatedMovement, dt);

        // 2. Change position.
        //
        // displacement = velocity * time
        //
        // x_new = x_old + v * dt
        MoveAndSlide(calculatedMovement * dt, ref calculatedMovement, startedGrounded && !leftGround);

        // 3. Work out where we ended up.
        UpdateGrounded(startedGrounded, leftGround, calculatedMovement);

        // Convert relative velocity back into world velocity.
        velocity = calculatedMovement + currentPlatformVelocity;

        // 4. Tell the world about it, and remember things for next step.
        DispatchContacts();
        RecordPlatformAnchor();
        CommitPosition();
        DrawDebugRays();
    }

    private void OnGUI()
    {
        DrawDebugHUD();
    }

    // ============================================================
    // SETUP HELPERS
    // ============================================================

    private void SetupRigidbody()
    {
        // Rigidbody exists as a Unity physics shell.
        // Our code remains responsible for actual movement.

        // better to set these here so we dont untick them ingame yknow?
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    private void CalculateMinGroundNormal()
    {
        // Math Time!
        // Surface is walkable when:
        //
        // normal.y >= cos(maxSlopeAngle)
        //
        // Example:
        // maxSlopeAngle = 60 degrees
        // cos(60) = 0.5
        //
        // So any surface normal with y >= 0.5 is <= 60 degrees steep.
        minGroundNormalY = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
    }

    private void AcceptExternalMovement()
    {
        // If another system moved the Rigidbody unexpectedly,
        // accept that new position instead of snapping back.
        // Helpful trigger for doors shoving you or platforms moving you!
        if ((rb.position - lastTarget).sqrMagnitude > ExternalMoveThresholdSqr)
            position = rb.position;
    }

    private void CommitPosition()
    {
        lastTarget = position;

        // Let Rigidbody interpolation visually smooth the custom position.
        rb.MovePosition(position);
    }

    // ============================================================
    // INPUT
    // ============================================================

    private void ReadInput()
    {
        // Basic Movement
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        Vector2 input = Vector2.zero;

        if (keyboard.upArrowKey.isPressed || (allowWASD && keyboard.wKey.isPressed))
            input.y += 1f;

        if (keyboard.downArrowKey.isPressed || (allowWASD && keyboard.sKey.isPressed))
            input.y -= 1f;

        if (keyboard.rightArrowKey.isPressed || (allowWASD && keyboard.dKey.isPressed))
            input.x += 1f;

        if (keyboard.leftArrowKey.isPressed || (allowWASD && keyboard.aKey.isPressed))
            input.x -= 1f;

        // So there was a funny bug at the start where diagonal movement was reasonably twice as fast as usual movement
        // Prevent diagonal input from being faster than straight input.
        //
        // Without normalize:
        // (1,1) magnitude = sqrt(2) ~= 1.414
        //
        // With normalize:
        // magnitude = 1
        moveInput = input.normalized;
        // Normalizing the direction keeps speed precise!
        sprinting = keyboard.leftShiftKey.isPressed;

        // Queue jump here so FixedUpdate cannot miss a single-frame key press.
        if (keyboard.spaceKey.wasPressedThisFrame)
            jumpQueued = true;

        // Slide is on either thumb button of the mouse. Same queue trick as jump for the press,
        // and a plain "is it down" for holding the slide.
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        slideHeld = mouse.backButton.isPressed || mouse.forwardButton.isPressed;

        if (mouse.backButton.wasPressedThisFrame || mouse.forwardButton.wasPressedThisFrame)
            slideQueued = true;
    }

    // ============================================================
    // VELOCITY STEPS: JUMP / SLIDE / MOVE
    // ============================================================

    // Returns true if we jumped (which means we left the ground this step).
    private bool Jump(ref Vector3 calculatedMovement)
    {
        bool jumpPressed = jumpQueued;

        // The queued press is used up either way, so pressing Space
        // in mid-air does not secretly "buffer" a jump for later.
        jumpQueued = false;

        if (jumpPressed && grounded)
        {
            // Jump is simply an immediate upward velocity assignment.
            calculatedMovement.y = jumpVelocity;

            grounded = false;
            return true;
        }

        return false;
    }

    // A platform that stops (or slows down) faster than gravity can pull us
    // back down leaves us flying with the speed it had.
    // Same idea as the old Rigidbody version launching you at the apex.
    //
    // Returns true if we launched (which means we left the ground this step).
    private bool LaunchOffMovingGround(Vector3 calculatedMovement, float dt)
    {
        if (!grounded)
            return false;

        // How fast we are moving AWAY from the ground we stand on.
        //
        // When a platform stops, its old speed shows up in
        // calculatedMovement (velocity - platformVelocity), pointing away from it.
        float speedAwayFromGround = Vector3.Dot(calculatedMovement, groundNormal);

        // Gravity can only pull us back by gravity * dt each step.
        // Moving away faster than that means the ground let go of us.
        if (speedAwayFromGround > gravity * dt)
        {
            grounded = false;
            return true;
        }

        return false;
    }

    // Decides whether we are sliding this step. (The actual sliding physics comes in later stages.)
    //
    // Plan:
    // - Start a slide when the button is pressed, we are grounded, and we are fast enough.
    // - Lower friction while sliding, but keep the momentum we came in with.
    // - Speed up going downhill, slow down going uphill.
    // - Let the player jump out of the slide (slide jump).
    private void Slide(ref Vector3 calculatedMovement, float dt)
    {
        bool slidePressed = slideQueued;

        // Used up either way, same as the jump press.
        slideQueued = false;

        float flatSpeed = Flatten(calculatedMovement).magnitude;

        if (!sliding)
        {
            // TODO 1 Start Slide
            //
            // Pseudocode:
            // - A slide starts only when ALL of these are true:
            //     * the slide button was just pressed
            //     * we are on the ground
            //     * we are moving fast enough (slideMinSpeed)
            //     * enough time has passed since the LAST slide started (slideGrace)
            // - When it starts, remember it: turn sliding on, and store the time it began (slideStartTime).
            //
            // Hint: same shape as the multi-line if in TryStepUp. Time.time is the clock.
        }
        else
        {
            // TODO 2 End Slide
            //
            // Pseudocode:
            // - The slide ends if EITHER of these is true:
            //     * the button is no longer held (slideHeld)
            //     * we are on the ground AND slower than slideMinSpeed
            // - Ending it is just turning sliding back off.
            //
            // Question to think about: why "on the ground AND slower"? What would happen if we
            // ended the slide for being slow while still in the air?
        }
    }

    // Choose movement model based on grounded state.
    private Vector3 Move(Vector3 calculatedMovement, float dt)
    {
        if (grounded)
            return GroundMove(calculatedMovement, dt);

        return AirMove(calculatedMovement, dt);
    }

    // ============================================================
    // The Big MOVEMENT MODELS
    // ============================================================

    private Vector3 WishDirection()
    {
        // Movement relative to where the player is facing.
        Vector3 forward = Flatten(transform.forward).normalized; //gotta do this shit every time man
        Vector3 right = Flatten(transform.right).normalized;

        // WASD becomes a world-space direction.
        return forward * moveInput.y + right * moveInput.x;
    }

    // Same vector with the height removed.
    private static Vector3 Flatten(Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }

    private Vector3 GroundMove(Vector3 calculatedMovement, float dt)
    {
        Vector3 wish = WishDirection();

        bool hasInput = wish.sqrMagnitude > 0.0001f;
        bool inLandingGrace = Time.time - landTime < landingGrace;

        // target speed depends on if im sprinting or not
        float targetSpeed = walkSpeed;

        if (sprinting)
            targetSpeed = sprintSpeed;

        // Remove velocity pointing directly into/out of the ground.

        // Projection onto a plane:
        // v_plane = v - n * dot(v, n)
        // n = ground surface normal
        // This leaves only velocity parallel to the ground.
        Vector3 velocityAlongGround = Vector3.ProjectOnPlane(calculatedMovement, groundNormal); //Take a look at week 3 exp 9! Super helpful dot product funtion

        if (hasInput)
        {
            //Arguably the coolest and smoothest model I have here!
            // Project desired movement onto the slope.
            //
            // This means "forward" becomes "forward along the ramp."
            Vector3 directionAlongGround = Vector3.ProjectOnPlane(wish, groundNormal).normalized;

            Vector3 targetVelocity = directionAlongGround * targetSpeed;

            // If above target speed, slow gently.
            // Otherwise accelerate aggressively.
            float acceleration = groundAccel;

            if (velocityAlongGround.magnitude > targetSpeed)
                acceleration = groundOverspeedDecel;

            // MoveTowards is effectively bounded acceleration:
            // Its a unity function as well.
            // deltaVelocity <= acceleration * dt
            //
            // v_new approaches targetVelocity gradually.
            velocityAlongGround = Vector3.MoveTowards(velocityAlongGround, targetVelocity, acceleration * dt);
        }
        else if (!inLandingGrace)
        {
            // Friction is implemented as constant deceleration toward zero.
            //
            // v_new = MoveTowards(v, 0, friction * dt)
            velocityAlongGround = Vector3.MoveTowards(velocityAlongGround, Vector3.zero, groundFriction * dt);
        }

        return velocityAlongGround;
    }

    private Vector3 AirMove(Vector3 calculatedMovement, float dt)
    {
        // Air acceleration:
        //
        // deltaVelocity = acceleration * dt
        //
        // v_new = v_old + a * dt
        float speedBeforeAccel = Flatten(calculatedMovement).magnitude;

        calculatedMovement += WishDirection() * airControl * dt;

        Vector3 horizontal = Flatten(calculatedMovement);

        // Being midair should never speed us up.
        // Air control can turn and brake us, but not push us past the speed we took off with.
        // (If we are already faster than that, say from a launch pad, we just can't get any faster.)
        float airSpeedLimit = Mathf.Max(speedBeforeAccel, airSpeedCap);

        if (horizontal.magnitude > airSpeedLimit)
        {
            horizontal = horizontal.normalized * airSpeedLimit;

            calculatedMovement.x = horizontal.x;
            calculatedMovement.z = horizontal.z;
        }

        // Over the limit: slow down gradually toward maxAirSpeed, keeping direction.
        // MoveTowards never goes past maxAirSpeed, so this can't undershoot.
        if (horizontal.magnitude > maxAirSpeed)
        {
            float slowedSpeed = Mathf.MoveTowards(horizontal.magnitude, maxAirSpeed, airOverspeedDecel * dt);

            horizontal = horizontal.normalized * slowedSpeed;

            calculatedMovement.x = horizontal.x;
            calculatedMovement.z = horizontal.z;
        }

        // Gravity:
        // Lets go back to phys 1 lol
        // v = v0 + g * dt
        //
        // Downward direction makes acceleration negative in Y.
        calculatedMovement += Vector3.down * gravity * dt;
        // Gotta be careful to not clamp gravity here!

        return calculatedMovement;
    }

    // ============================================================
    // COLLISION QUERIES
    // ============================================================

    private void GetCapsulePoints(Vector3 pos, out Vector3 p0, out Vector3 p1, out float radius)
    {
        Vector3 scale = transform.lossyScale;

        // Capsule radius must account for object scaling.
        radius = capsule.radius * Mathf.Max(scale.x, scale.z);

        float height = Mathf.Max(capsule.height * scale.y, radius * 2f);

        Vector3 center = pos + Vector3.Scale(capsule.center, scale);

        // Distance from capsule center to each sphere center.
        float half = height * 0.5f - radius;

        p0 = center + Vector3.down * half;
        p1 = center + Vector3.up * half;
    }

    private bool IsSelf(Collider otherCollider)
    {
        // Ignore our own collider and child colliders.
        return otherCollider == null ||
               otherCollider == capsule ||
               otherCollider.transform.IsChildOf(transform);
    }

    // A surface is walkable when it is tilted less than maxSlopeAngle from straight up.
    private bool IsWalkable(Vector3 surfaceNormal)
    {
        return surfaceNormal.y >= minGroundNormalY;
    }

    private bool SweepCapsule(Vector3 pos, Vector3 direction, float distance, out RaycastHit bestHit)
    {
        bestHit = default;

        if (distance <= 0f)
            return false;

        GetCapsulePoints(pos, out Vector3 p0, out Vector3 p1, out float radius);

        // Sweep our capsule through space.
        // Think "what would we hit if the player moved this direction?"
        int hitCount = Physics.CapsuleCastNonAlloc(
            p0,
            p1,
            radius,
            direction,
            hitBuffer,
            distance,
            collisionMask,
            QueryTriggerInteraction.Ignore
        );

        bool foundHit = false;
        float closestDistance = float.MaxValue;

        // CapsuleCastNonAlloc is not guaranteed to return sorted hits,
        // so manually find the nearest collision.
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit candidate = hitBuffer[i];

            if (IsSelf(candidate.collider))
                continue;

            // Weird zero-distance hits usually mean we already overlap.
            // Depenetrate() handles those separately.
            if (candidate.distance <= 0f && candidate.point == Vector3.zero)
                continue;

            if (candidate.distance < closestDistance)
            {
                bestHit = candidate;
                closestDistance = candidate.distance;
                foundHit = true;
            }
        }

        return foundHit;
    }

    // Capsule edges can produce misleading normals near corners.
    // This attempts to retrieve the actual face normal instead.
    private Vector3 ResolveGroundNormal(RaycastHit hit)
    {
        // Already walkable: trust the capsule result.
        if (IsWalkable(hit.normal))
            return hit.normal;

        // Ray straight downward from slightly above the contact.
        Vector3 rayOrigin = hit.point + Vector3.up * NormalProbeHeight;

        if (Physics.Raycast(
                rayOrigin,
                Vector3.down,
                out RaycastHit rayHit,
                NormalProbeDistance,
                collisionMask,
                QueryTriggerInteraction.Ignore)
            && rayHit.collider == hit.collider)
        {
            return rayHit.normal;
        }

        return hit.normal;
    }

    private void Depenetrate()
    {
        // Repeat a few times because resolving one overlap
        // can sometimes push us into another surface.
        for (int iteration = 0; iteration < 3; iteration++)
        {
            GetCapsulePoints(position, out Vector3 p0, out Vector3 p1, out float radius);

            int overlapCount = Physics.OverlapCapsuleNonAlloc(
                p0,
                p1,
                radius,
                overlapBuffer,
                collisionMask,
                QueryTriggerInteraction.Ignore
            );

            bool moved = false;

            for (int i = 0; i < overlapCount; i++)
            {
                Collider otherCollider = overlapBuffer[i];

                if (IsSelf(otherCollider))
                    continue;

                // ComputePenetration gives:
                //
                // direction = shortest escape direction
                // distance  = how far we must move to exit overlap
                if (Physics.ComputePenetration(
                        capsule,
                        position,
                        transform.rotation,
                        otherCollider,
                        otherCollider.transform.position,
                        otherCollider.transform.rotation,
                        out Vector3 escapeDirection,
                        out float escapeDistance))
                {
                    // Push ourselves barely outside the collider.
                    position += escapeDirection * (escapeDistance + 0.001f);

                    moved = true;
                }
            }

            if (!moved)
                break;
        }
    }

    // ============================================================
    // COLLIDE AND SLIDE
    // ============================================================

    // Core collide-and-slide algorithm.
    //
    // Basic idea:
    //
    // 1. Try moving.
    // 2. Find first surface hit.
    // 3. Move right up to it.
    // 4. Remove movement INTO the wall.
    // 5. Continue using the remaining sideways motion.
    //
    // Repeat several times for corners / multiple surfaces.
    private void MoveAndSlide(Vector3 motion, ref Vector3 calculatedMovement, bool allowStep)
    {
        Vector3 remainingMotion = motion;

        // null until we have hit a first surface.
        Vector3? previousNormal = null;

        for (int bump = 0; bump < maxBumps; bump++)
        {
            float distance = remainingMotion.magnitude;

            if (distance < MinMoveDistance)
                break;

            Vector3 direction = remainingMotion / distance;

            // Nothing ahead: perform entire remaining movement.
            if (!SweepCapsule(position, direction, distance + skinWidth, out RaycastHit hit))
            {
                position += remainingMotion;
                break;
            }

            contactsThisStep.Add(hit.collider);

            // Move as far as safely possible, keep whatever motion remains.
            remainingMotion = MoveUpToHit(hit, direction, distance);

            Rigidbody hitBody = hit.collider.attachedRigidbody;

            bool dynamicBody =
                hitBody != null &&
                !hitBody.isKinematic;

            if (dynamicBody)
                PushBody(hitBody, hit, direction, calculatedMovement);

            Vector3 surfaceNormal = hit.normal;

            bool walkable = IsWalkable(surfaceNormal);

            // Try stepping over low obstacles instead of sliding into them.
            if (allowStep &&
                !walkable &&
                surfaceNormal.y > -0.1f &&
                !dynamicBody &&
                TryStepUp(ref remainingMotion))
            {
                previousNormal = null;
                continue;
            }

            SlideAlongSurface(surfaceNormal, previousNormal, ref remainingMotion, ref calculatedMovement);

            previousNormal = surfaceNormal;
        }
    }

    // Moves us right up to the surface, leaving a small gap.
    // Returns whatever motion is still left over.
    private Vector3 MoveUpToHit(RaycastHit hit, Vector3 direction, float distance)
    {
        // Move slightly short of the collision. More math time
        //
        // dot(direction, normal) tells us how directly we're approaching the surface.
        //
        // Head-on collision:
        // dot ~= -1
        //
        // Grazing collision:
        // dot ~= 0
        //
        // More grazing -> slightly larger backoff needed.
        float backOffDistance = Mathf.Min(skinWidth / Mathf.Max(0.05f, -Vector3.Dot(direction, hit.normal)), skinWidth * 4f);

        float safeDistance = Mathf.Clamp(hit.distance - backOffDistance, 0f, distance);

        // Move as far as safely possible.
        position += direction * safeDistance;

        // Keep whatever motion remains.
        return direction * (distance - safeDistance);
    }

    // Removes the part of our motion and velocity that points INTO the surface.
    private void SlideAlongSurface(
        Vector3 surfaceNormal,
        Vector3? previousNormal,
        ref Vector3 remainingMotion,
        ref Vector3 calculatedMovement)
    {
        // Surface faces upward but is too steep to walk on.
        bool steepUpward =
            !IsWalkable(surfaceNormal) &&
            surfaceNormal.y > 0f;

        // Slide equation:
        //
        // remaining_parallel =
        // remaining - normal * dot(remaining, normal)
        //
        // The perpendicular "into wall" component disappears.
        Vector3 slidMotion = Vector3.ProjectOnPlane(remainingMotion, surfaceNormal);

        // Never magically climb walls that are too steep.
        if (steepUpward && slidMotion.y > 0f)
            slidMotion.y = 0f;

        if (previousNormal.HasValue &&
            Vector3.Dot(slidMotion, previousNormal.Value) < -0.0001f)
        {
            // We hit two surfaces and are effectively wedged.
            SlideAlongCrease(previousNormal.Value, surfaceNormal, remainingMotion, ref slidMotion, ref calculatedMovement);
        }
        else if (Vector3.Dot(calculatedMovement, surfaceNormal) < 0f)
        {
            // If velocity points INTO the wall,
            // remove its wall-normal component.
            calculatedMovement = Vector3.ProjectOnPlane(calculatedMovement, surfaceNormal);

            if (steepUpward && calculatedMovement.y > 0f)
                calculatedMovement.y = 0f;
        }

        remainingMotion = slidMotion;
    }

    // Wedged between two surfaces: only keep motion along the line where they meet.
    //
    // Their cross product creates a vector pointing
    // along the line where the two planes intersect.
    //
    // crease = n1 x n2
    private void SlideAlongCrease(
        Vector3 previousNormal,
        Vector3 surfaceNormal,
        Vector3 remainingMotion,
        ref Vector3 slidMotion,
        ref Vector3 calculatedMovement)
    {
        Vector3 crease = Vector3.Cross(previousNormal, surfaceNormal);

        if (crease.sqrMagnitude > 0.000001f)
        {
            crease.Normalize();

            // Only retain motion along the crease.
            slidMotion = crease * Vector3.Dot(remainingMotion, crease);

            calculatedMovement = crease * Vector3.Dot(calculatedMovement, crease);
        }
        else
        {
            // Parallel/opposing planes:
            // nowhere meaningful left to move.
            slidMotion = Vector3.zero;
            calculatedMovement = Vector3.zero;
        }
    }

    private bool TryStepUp(ref Vector3 remainingMotion)
    {
        Vector3 horizontalMotion = Flatten(remainingMotion);

        float horizontalDistance = horizontalMotion.magnitude;

        if (horizontalDistance < MinMoveDistance)
            return false;

        Vector3 horizontalDirection = horizontalMotion / horizontalDistance;

        // Step test is basically:
        //
        // 1. Can I move UP?
        // 2. Can I move FORWARD?
        // 3. Can I move DOWN onto walkable ground?

        // --------------------
        // 1. MOVE UP
        // --------------------

        float raiseHeight = stepHeight;

        if (SweepCapsule(position, Vector3.up, raiseHeight + skinWidth, out RaycastHit ceilingHit))
            raiseHeight = Mathf.Max(ceilingHit.distance - skinWidth, 0f);

        if (raiseHeight < MinStepRaiseHeight)
            return false;

        Vector3 raisedPosition = position + Vector3.up * raiseHeight;

        // --------------------
        // 2. MOVE FORWARD
        // --------------------

        float forwardDistance = Mathf.Max(horizontalDistance, StepForwardNudge);

        if (SweepCapsule(raisedPosition, horizontalDirection, forwardDistance + skinWidth, out RaycastHit wallHit))
            forwardDistance = Mathf.Max(wallHit.distance - skinWidth, 0f);

        if (forwardDistance < MinStepForwardDistance)
            return false;

        Vector3 forwardPosition = raisedPosition + horizontalDirection * forwardDistance;

        // --------------------
        // 3. DROP DOWN
        // --------------------

        if (!SweepCapsule(forwardPosition, Vector3.down, raiseHeight + skinWidth * 3f, out RaycastHit floorHit))
            return false;

        // Must land on a walkable surface.
        if (!IsWalkable(ResolveGroundNormal(floorHit)))
            return false;

        float droppedDistance = Mathf.Max(floorHit.distance - skinWidth, 0f);

        // Actual height of the step:
        //
        // gainedHeight = amountRaised - amountDropped
        float heightGained = raiseHeight - droppedDistance;

        if (heightGained < MinStepHeightGained ||
            heightGained > stepHeight + StepHeightTolerance)
        {
            return false;
        }

        position = forwardPosition + Vector3.down * droppedDistance;

        // Preserve whatever forward motion remains.
        remainingMotion = horizontalDirection * Mathf.Max(horizontalDistance - forwardDistance, 0f);

        return true;
    }

    // ============================================================
    // GROUND DETECTION- RAYCASDTING
    // ============================================================

    private void UpdateGrounded(bool startedGrounded, bool leftGround, Vector3 calculatedMovement)
    {
        bool wasGrounded =
            startedGrounded &&
            !leftGround;

        float previousSlope = slopeAngle;

        // Assume airborne until proven grounded.
        ClearGroundState();

        // Jump explicitly disables grounding this frame.
        if (leftGround)
            return;

        // Ramp launching:
        //
        // If our slope-projected velocity points upward and
        // the old slope was sufficiently steep, we should
        // continue through the air instead of being snapped downward.
        bool launching =
            wasGrounded &&
            calculatedMovement.y > MinLaunchUpSpeed &&
            previousSlope >= launchSlopeAngle;

        bool snapping =
            wasGrounded &&
            !launching;

        // Since the capsule touches angled ground at an offset,
        // skinWidth / normal.y roughly converts the desired
        // perpendicular skin gap into the needed vertical clearance.
        float probeDistance = skinWidth / minGroundNormalY + groundProbeDistance;

        // Only reach far downward when we intend to stick to the ground.
        if (snapping)
            probeDistance += groundSnapDistance;

        if (!SweepCapsule(position, Vector3.down, probeDistance, out RaycastHit groundHit))
            return;

        Vector3 surfaceNormal = ResolveGroundNormal(groundHit);

        // Too steep to stand on.
        if (!IsWalkable(surfaceNormal))
            return;

        // dot(calculatedMovement, surfaceNormal) > 0 means velocity is moving AWAY
        // from the surface along its normal.
        //
        // So don't instantly reground while launching.
        if (!snapping &&
            Vector3.Dot(calculatedMovement, surfaceNormal) > MovingAwaySpeed)
        {
            return;
        }

        // Move downward until maintaining our skin gap.
        float snapDistance = groundHit.distance - skinWidth / Mathf.Max(surfaceNormal.y, 0.5f);

        if (snapDistance > MinSnapDistance)
            position += Vector3.down * snapDistance;

        grounded = true;
        groundNormal = surfaceNormal;
        groundCollider = groundHit.collider;

        // Angle between surface normal and straight up.
        //
        // Flat floor:
        // angle(up, up) = 0 degrees
        //
        // Vertical wall:
        // angle(sideways, up) = 90 degrees
        slopeAngle = Vector3.Angle(surfaceNormal, Vector3.up);

        contactsThisStep.Add(groundHit.collider);

        // Record exact landing time for bunny-hop friction grace.
        if (!wasGrounded)
            landTime = Time.time;
    }

    private void ClearGroundState()
    {
        grounded = false;
        groundCollider = null;
        groundNormal = Vector3.up;
        slopeAngle = 0f;
    }

    // ============================================================
    // PUSHING
    // ============================================================

    private void PushBody(Rigidbody body, RaycastHit hit, Vector3 moveDirection, Vector3 calculatedMovement)
    {
        // Only push horizontally.
        Vector3 pushDirection = Flatten(moveDirection);

        if (pushDirection.sqrMagnitude < 0.000001f)
            return;

        pushDirection.Normalize();

        Vector3 playerVelocity = calculatedMovement + currentPlatformVelocity;

        // Relative closing velocity:
        //
        // closingSpeed =
        // dot(playerVelocity - objectVelocity, pushDirection)
        //
        // Positive means we're moving INTO the object.
        float closingSpeed = Vector3.Dot(playerVelocity - body.GetPointVelocity(hit.point), pushDirection);

        if (closingSpeed <= 0f)
            return;

        // Reduced mass:
        //
        // μ = (m1 * m2) / (m1 + m2)
        //
        // This gives realistic collision response between
        // two objects of different mass.
        float reducedMass = (playerMass * body.mass) / (playerMass + body.mass);

        // Impulse:
        //
        // J = μ * relativeVelocity
        //
        // Impulse changes momentum immediately:
        //
        // J = Δp = m * Δv
        body.AddForceAtPosition(pushDirection * (reducedMass * closingSpeed * pushStrength), hit.point, ForceMode.Impulse);
    }

    // ============================================================
    // MOVING PLATFORMS
    // ============================================================

    private Vector3 ApplyPlatformCarry(float dt)
    {
        if (platform == null ||
            !platform.gameObject.activeInHierarchy)
        {
            platform = null;
            return Vector3.zero;
        }

        RotateWithPlatform();

        // platformLocalPos stores our anchor relative to the platform.
        //
        // TransformPoint converts that old local anchor into
        // its NEW world position.
        //
        // Therefore:
        //
        // platformDelta =
        // newAnchorWorldPosition - playerPosition
        Vector3 platformDelta = platform.TransformPoint(platformLocalPos) - position;

        if (platformDelta.sqrMagnitude < 0.0000000001f)
            return Vector3.zero;

        Vector3 discardedMovement = Vector3.zero;

        // Carry us with the platform while still respecting collision.
        MoveAndSlide(platformDelta, ref discardedMovement, false);

        // Velocity equation:
        //
        // velocity = displacement / time
        //
        // v = Δx / Δt
        return platformDelta / dt;
    }

    private void RotateWithPlatform()
    {
        // Only inherit yaw from mostly upright platforms.
        if (Vector3.Dot(platform.up, Vector3.up) > PlatformUprightDot)
        {
            // Rotation since last frame:
            //
            // deltaRotation =
            // currentRotation * inverse(previousRotation)
            Quaternion rotationDelta = platform.rotation * Quaternion.Inverse(platformLastRot);

            float yawDelta = Mathf.DeltaAngle(0f, rotationDelta.eulerAngles.y);

            if (Mathf.Abs(yawDelta) > 0.0001f)
                transform.Rotate(0f, yawDelta, 0f, Space.World);
        }
    }

    private void RecordPlatformAnchor()
    {
        // Only Rigidbody-backed ground can carry us.
        Rigidbody groundBody = null;

        if (grounded && groundCollider != null)
            groundBody = groundCollider.attachedRigidbody;

        if (groundBody == null)
        {
            platform = null;
            return;
        }

        platform = groundBody.transform;

        // Convert player world position into platform-local coordinates.
        //
        // This anchor will move naturally when the
        // platform translates or rotates.
        platformLocalPos = platform.InverseTransformPoint(position);

        platformLastRot = platform.rotation;
    }

    // ============================================================
    // CONTACTS / EXTERNAL CONTROL
    // ============================================================

    private void DispatchContacts()
    {
        foreach (Collider contact in contactsThisStep)
        {
            if (contact == null ||
                contactsLastStep.Contains(contact))
            {
                continue;
            }

            // Only fire contact when first touching the object.
            if (contact.TryGetComponent(out IPlayerContact receiver))
                receiver.OnPlayerContact(this);
        }

        // Current contacts become next frame's previous contacts.
        contactsLastStep.Clear();

        foreach (Collider contact in contactsThisStep)
            contactsLastStep.Add(contact);

        contactsThisStep.Clear();
    }

    public void SetVelocity(Vector3 worldVelocity)
    {
        // External systems like launch pads can directly set velocity.
        velocity = worldVelocity;

        grounded = false;
        platform = null;
    }

    public void Teleport(Vector3 worldPosition)
    {
        // Synchronize every representation of position
        // so the controller doesn't snap back afterward.
        position = worldPosition;
        lastTarget = worldPosition;

        velocity = Vector3.zero;

        grounded = false;
        platform = null;

        rb.position = worldPosition;
        transform.position = worldPosition;
    }

    // ============================================================
    // DEBUG
    // ============================================================

    private void DrawDebugRays()
    {
        if (!drawDebugRays)
            return;

        // Green = ground normal.
        Debug.DrawRay(position + Vector3.up * 0.1f, groundNormal * 1.5f, Color.green);

        // Cyan = velocity.
        Debug.DrawRay(position + Vector3.up, velocity * 0.15f, Color.cyan);
    }

    private void DrawDebugHUD()
    {
        if (!showDebugHUD)
            return;

        Vector3 horizontalVelocity = Flatten(velocity);

        string platformName = "none";

        if (platform != null)
            platformName = platform.name;

        GUI.Label(
            new Rect(10f, 10f, 420f, 160f),
            $"speed: {velocity.magnitude:F1}   horizontal: {horizontalVelocity.magnitude:F1}\n" +
            $"velocity: {velocity}\n" +
            $"grounded: {grounded}   slope: {slopeAngle:F0} deg\n" +
            $"sliding: {sliding}\n" +
            $"platform: {platformName}"
        );
    }
}
