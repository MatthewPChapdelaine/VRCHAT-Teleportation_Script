# VRChat Teleportation System - Implementation Guide

## Quick Start

### Prerequisites
- Unity 2022.3.6f1 (the only correct version for current VRChat SDK)
- VRChat SDK3 for Avatars or Worlds (latest)
- UdonSharp installed in your project

### 30-Second Setup
1. Add the `TeleportationController.cs` script to a GameObject
2. Set up a LineRenderer component on the same GameObject
3. Create Layer masks for valid teleport surfaces
4. Assign layer to platform/ground objects
5. Configure "Valid Teleport Layers" in Inspector
6. Test with F key (default)

---

## Detailed Configuration

### GameObject Setup
```
TeleportationManager (GameObject)
├── TeleportationController (Script Component)
├── LineRenderer (Component)
├── Particle System (Teleport Effect - Optional)
└── Indicator Visual (GameObject - Optional)
```

### Inspector Configuration

#### Required
- **Valid Teleport Layers**: Check the layers that players can teleport to
  - Recommendation: Create "TeleportSurface" layer for easy management

#### Optional but Recommended
- **Teleport Effect Prefab**: Assign a particle system or shader effect
  - Example: VRC Avatar Dynamics effect, custom particle burst, light flash
  - Recommended duration: 0.5-2 seconds
  
- **Target Indicator Prefab**: Visual marker showing landing spot
  - Example: Sphere, quad with material, ring effect
  - Keep it subtle to avoid visual clutter
  
- **Trajectory Line Renderer**: Shows aiming line
  - Set to a different layer or parent to world space if needed

#### Tuning Parameters
- **Teleport Cooldown**: 
  - 0.2s: Fast spam (not recommended)
  - 0.5s: Balanced (default)
  - 1.0s: Deliberate, slower gameplay
  
- **Raycast Distance**: 
  - 50m: Close-range (tight spaces)
  - 100m: Standard (default)
  - 200m: Long-range (large worlds)
  
- **Tracking Type**:
  - Head: Aim from where you're looking
  - LeftHand: Aim from left controller
  - RightHand: Aim from right controller

- **Allow Movement During Cooldown**: 
  - True: Player moves freely between teleports (recommended)
  - False: Player locked in place during cooldown

---

## Visual Effects Best Practices

### Teleport Origin Effect
Show the "departure" visual at where player started
- Recommended: Explosion/burst effect
- Duration: 0.3-0.5 seconds
- Color: Blue/cyan or player's color

### Teleport Destination Effect
Show the "arrival" visual where player landed
- Recommended: Impact/ring effect
- Duration: 0.3-0.5 seconds
- Color: Bright/white or accent color

### Trajectory Line
- **Material**: Standard Unity LineRenderer material works fine
- **Color**: Semi-transparent blue/green
- **Width**: 0.05-0.15 units
- **Position**: Line should originate from tracking point and extend to target

### Target Indicator
- **Visual Style**: Ring, disc, or marker
- **Size**: 0.5-1.5 units diameter
- **Color**: Matches trajectory line
- **Glow**: Optional but improves visibility
- **Fade**: Use alpha over distance for visual polish

---

## Layer Setup Example

### Recommended Layer Organization
```
Layer 0: Default
Layer 1: TransparentFX
Layer 2: Ignore Raycast
Layer 3: Water
Layer 4: UI
...
Layer 8: TeleportSurface (CREATE THIS)
Layer 9: TeleportIndicator (CREATE THIS)
```

### Assigning Objects to Layers
```csharp
// In scene:
// Platforms → TeleportSurface layer
// Ground → TeleportSurface layer
// Walkways → TeleportSurface layer
// Invisible collision → TeleportSurface layer

// NOT:
// Avatar parts → Don't include
// Dynamic objects → Be careful with moving platforms
// Physics objects → Avoid if they fall away
```

### Valid Teleport Layers Configuration
In Inspector, set the LayerMask to include only "TeleportSurface":
```
Valid Teleport Layers: ☑ TeleportSurface
```

---

## Common Configurations

### Tight Indoor Space
```
Teleport Cooldown: 0.5s
Raycast Distance: 30m
Tracking Type: Head (easier aiming in tight spaces)
Allow Movement: True
Effects: Subtle particle system
```

### Open Outdoor World
```
Teleport Cooldown: 0.3s (faster paced)
Raycast Distance: 150m (longer sight lines)
Tracking Type: Head or RightHand (preference)
Allow Movement: True
Effects: Bright, visible effects
```

### Puzzle/Parkour Level
```
Teleport Cooldown: 0.2s (fast skill-based gameplay)
Raycast Distance: 50m (controlled precision)
Tracking Type: Head (precision aiming)
Allow Movement: False (commit to actions)
Effects: Feedback effects (required)
```

### Avatar Preview Room
```
Teleport Cooldown: 1.0s (casual, slow)
Raycast Distance: 20m (small contained space)
Tracking Type: Head
Allow Movement: True
Effects: Minimal/none
```

---

## Troubleshooting

### Problem: Teleport Not Working
**Solutions:**
1. Check that you're pressing F key (or configured key)
2. Verify "Valid Teleport Layers" includes target surface layer
3. Ensure target surface is not obstructed
4. Check Console for error messages

### Problem: Teleporting Into Objects
**Solutions:**
1. Verify surface is not sloped steep
2. Check that surface has collider
3. Adjust raycast point - try different tracking type
4. Increase the 0.1f offset in ExecuteTeleport if needed

### Problem: Raycast Misses Valid Surfaces
**Solutions:**
1. Increase "Raycast Distance" parameter
2. Check that tracking type is correct
3. Verify collider is enabled on surface
4. Check layer is included in "Valid Teleport Layers"

### Problem: Effects Not Showing
**Solutions:**
1. Assign valid prefabs to effect slots
2. Ensure effects are on correct layers (not culled)
3. Check world performance budget
4. Verify Instantiate is being called (check Console)

### Problem: Script Crashes
**Solutions:**
1. Check Console for null reference errors
2. Verify localPlayer was successfully obtained
3. Ensure you have Network access (not guest)
4. Disable script and re-enable to reset state

---

## Performance Considerations

### CPU Impact
- Raycast per frame: ~0.1-0.3ms
- Line Renderer update: ~0.05ms
- Total overhead: Very minimal

### Optimization Tips
1. Disable LineRenderer if not needed
2. Use simple indicator meshes (avoid complex geometry)
3. Reuse effect prefabs with pooling if teleporting frequently
4. Avoid raycasting physics layers with many rigidbodies

### VRChat Quality Settings
- Effects budget: 100-500 particles
- Raycast layers: Keep to 2-3 layers maximum
- LineRenderer: Enable only when needed

---

## Advanced Usage

### Disabling Teleportation Dynamically
```csharp
// In another script:
teleportController.enabled = false; // Disable teleports
teleportController.enabled = true;  // Re-enable
```

### Checking Teleport Availability
```csharp
// In UI or feedback system:
if (teleportController.CanTeleport())
{
    // Show "Ready" feedback
}

float remaining = teleportController.GetCooldownRemaining();
if (remaining > 0)
{
    // Show cooldown timer
    Debug.Log($"Teleport available in {remaining:F1}s");
}
```

### Custom Tracking Conditions
You can modify `GetTrackingPosition()` to add conditions like:
- Only allow teleport if looking down
- Require both hands pointing same direction
- VR hand gesture recognition

---

## Networking Notes

### Ownership Check
The script checks `Networking.IsOwner()` to prevent conflicts. This ensures:
- Only the local player executes their own teleports
- No sync issues with multiple players
- Safe for multiplayer worlds

### Syncing Teleports Across Network
If you need to show other players teleporting:
1. Override OnTeleportRequested to call Networking.RPC
2. Create an Udon callback to sync positions
3. VRChat will automatically sync player position

---

## VRChat SDK Versions

This script is compatible with:
- ✅ VRChat SDK3 Worlds (2023.1 and later)
- ✅ VRChat SDK3 Avatars (for avatar-world interaction)
- ✅ UdonSharp 1.0+

Tested with Unity 2022.3.6f1 (required for VRChat)

---

## Best Practices Summary

✅ DO:
- Use layer masks for surface validation
- Provide visual feedback to players
- Test cooldown timings for your world type
- Use Networking.IsOwner() for safety
- Comment your customization choices

❌ DON'T:
- Teleport players without their input
- Set cooldown to 0 (spam prevention)
- Forget to assign Valid Teleport Layers
- Use Debug.Log in production (impacts performance)
- Try to modify localPlayer during teleport

---

## Support

For issues with:
- **Script**: Check TeleportationController.cs comments
- **VRChat SDK**: See official documentation at docs.vrchat.com
- **UdonSharp**: Check UdonSharp GitHub repository
- **Unity**: Use Unity documentation (2022.3.6f1 version)
