using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TeleportationController : UdonSharpBehaviour
{
    [Header("Teleportation Settings")]
    [SerializeField] private float teleportCooldown = 0.5f;
    [SerializeField] private float raycastDistance = 100f;
    [SerializeField] private LayerMask validTeleportLayers;
    
    [Header("Movement & Input")]
    [SerializeField] private bool allowMovementDuringCooldown = true;
    [SerializeField] private KeyCode teleportInputKey = KeyCode.F;
    [SerializeField] private VRCPlayerApi.TrackingDataType trackingType = VRCPlayerApi.TrackingDataType.Head;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject teleportEffectPrefab;
    [SerializeField] private GameObject targetIndicatorPrefab;
    [SerializeField] private LineRenderer trajectoryLineRenderer;
    
    // Private state
    private float lastTeleportTime = -999f;
    private Vector3 targetPosition;
    private bool hasValidTarget = false;
    private VRCPlayerApi localPlayer;
    private GameObject targetIndicator;

    private void Start()
    {
        // Get reference to local player
        localPlayer = Networking.LocalPlayer;
        
        if (localPlayer == null)
        {
            Debug.LogError("[Teleportation] Failed to get local player reference");
            enabled = false;
            return;
        }

        // Initialize target indicator if prefab is assigned
        if (targetIndicatorPrefab != null)
        {
            targetIndicator = Instantiate(targetIndicatorPrefab);
            targetIndicator.SetActive(false);
        }

        // Disable trajectory line initially
        if (trajectoryLineRenderer != null)
        {
            trajectoryLineRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (localPlayer == null || !Networking.IsOwner(gameObject))
            return;

        // Check for teleport input
        if (Input.GetKeyDown(teleportInputKey))
        {
            OnTeleportRequested();
        }

        // Update target visualization every frame
        UpdateTargetVisualization();
    }

    /// <summary>
    /// Performs raycasting from the tracking point to find a valid teleport destination.
    /// </summary>
    private bool CalculateTargetPosition(out Vector3 calculatedPosition)
    {
        calculatedPosition = Vector3.zero;

        if (localPlayer == null)
            return false;

        // Get the tracking point (Head, LeftHand, or RightHand)
        Vector3 trackingPosition = GetTrackingPosition();
        Vector3 trackingForward = GetTrackingForward();

        // Perform raycast from tracking position
        Ray ray = new Ray(trackingPosition, trackingForward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, validTeleportLayers))
        {
            // Valid hit found
            calculatedPosition = hit.point;
            
            // Offset slightly above the surface to prevent clipping
            calculatedPosition.y += 0.1f;
            
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the position of the tracking point based on the specified TrackingDataType.
    /// </summary>
    private Vector3 GetTrackingPosition()
    {
        switch (trackingType)
        {
            case VRCPlayerApi.TrackingDataType.Head:
                return localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            case VRCPlayerApi.TrackingDataType.LeftHand:
                return localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
            case VRCPlayerApi.TrackingDataType.RightHand:
                return localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
            default:
                return localPlayer.GetPosition();
        }
    }

    /// <summary>
    /// Gets the forward direction of the tracking point.
    /// </summary>
    private Vector3 GetTrackingForward()
    {
        switch (trackingType)
        {
            case VRCPlayerApi.TrackingDataType.Head:
                return localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation * Vector3.forward;
            case VRCPlayerApi.TrackingDataType.LeftHand:
                return localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).rotation * Vector3.forward;
            case VRCPlayerApi.TrackingDataType.RightHand:
                return localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).rotation * Vector3.forward;
            default:
                return localPlayer.GetRotation() * Vector3.forward;
        }
    }

    /// <summary>
    /// Main teleportation logic. Called when user initiates a teleport.
    /// </summary>
    private void OnTeleportRequested()
    {
        // Check cooldown
        if (!IsCooldownElapsed())
        {
            Debug.LogWarning("[Teleportation] Teleport is on cooldown");
            return;
        }

        // Calculate target position via raycast
        if (!CalculateTargetPosition(out Vector3 destination))
        {
            Debug.LogWarning("[Teleportation] No valid teleport target found");
            return;
        }

        // Execute teleportation
        ExecuteTeleport(destination);

        // Apply cooldown
        ApplyCooldown();
    }

    /// <summary>
    /// Executes the actual teleportation and spawns effects.
    /// </summary>
    private void ExecuteTeleport(Vector3 destination)
    {
        if (localPlayer == null)
            return;

        // Store origin for effect spawning
        Vector3 origin = localPlayer.GetPosition();

        // Teleport the player
        localPlayer.TeleportTo(destination, Quaternion.identity);

        // Spawn teleport effect at origin
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, origin, Quaternion.identity);
        }

        // Spawn teleport effect at destination
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, destination, Quaternion.identity);
        }

        Debug.Log($"[Teleportation] Teleported to {destination}");
    }

    /// <summary>
    /// Updates the visual indicator showing where the player will land.
    /// </summary>
    private void UpdateTargetVisualization()
    {
        hasValidTarget = CalculateTargetPosition(out targetPosition);

        // Update target indicator
        if (targetIndicator != null)
        {
            targetIndicator.SetActive(hasValidTarget);
            if (hasValidTarget)
            {
                targetIndicator.transform.position = targetPosition;
            }
        }

        // Update trajectory line
        if (trajectoryLineRenderer != null)
        {
            trajectoryLineRenderer.enabled = true;
            
            Vector3 trackingPosition = GetTrackingPosition();
            Vector3 trackingForward = GetTrackingForward();

            trajectoryLineRenderer.SetPosition(0, trackingPosition);
            
            if (hasValidTarget)
            {
                trajectoryLineRenderer.SetPosition(1, targetPosition);
            }
            else
            {
                trajectoryLineRenderer.SetPosition(1, trackingPosition + (trackingForward * raycastDistance));
            }
        }
    }

    /// <summary>
    /// Checks if the cooldown period has elapsed.
    /// </summary>
    private bool IsCooldownElapsed()
    {
        return (Time.time - lastTeleportTime) >= teleportCooldown;
    }

    /// <summary>
    /// Applies the teleport cooldown timer.
    /// </summary>
    private void ApplyCooldown()
    {
        lastTeleportTime = Time.time;
    }

    /// <summary>
    /// Public method to check if player can currently teleport.
    /// </summary>
    public bool CanTeleport()
    {
        return IsCooldownElapsed() && hasValidTarget;
    }

    /// <summary>
    /// Public method to get remaining cooldown time.
    /// </summary>
    public float GetCooldownRemaining()
    {
        float remaining = teleportCooldown - (Time.time - lastTeleportTime);
        return Mathf.Max(0, remaining);
    }
}
