Udon Teleportation Script.

Using the VRCHAT SDK and API with Full Documentation of UdonScript, write an UdonScript Asset that completely encompasses the Teleportation Game Mechanic in all aspects, which are all customizable in the Inspector. Note that there is only one correct version of Unity for VRCHAT development, to avoid accidentally mistaking one framework for another.

The UdonScript Asset for a customizable VRChat teleportation mechanic would be structured around the following components, allowing for full customization through the Inspector:

* **Public Variables for Customization:** These will be exposed in the VRChat Inspector to allow for easy modification of the mechanic without editing the script.  
  * `public float TeleportCooldown`: Time delay between teleports.  
  * `public bool AllowMovementDuringCooldown`: Toggles if the player can move while waiting for the next teleport.  
  * `public VRCPlayerApi.TrackingDataType TrackingType`: Specifies which part of the player (e.g., Head, LeftHand, RightHand) determines the target direction.  
  * `public LayerMask ValidTeleportLayers`: A filter to only allow teleportation onto specific types of surfaces (e.g., a "Ground" layer).  
  * `public GameObject TeleportEffectPrefab`: A visual effect (like a particle burst) to spawn at the origin or destination of the teleport.  
* **Udon Behaviors and Events:** The core logic will be executed through Udon's event system.  
  * **`_Start`:** Initializes the script, setting up the initial state and any necessary references (like a raycast origin point).  
  * **`_Update`:** This event runs every frame and is used for:  
    * **Input Detection:** Checking for the user's input (e.g., button press on a controller) to initiate the teleport process.  
    * **Raycasting:** Performing a raycast from the tracking type's position to find a valid teleport destination on a surface defined by `ValidTeleportLayers`.  
    * **Target Visualization:** Displaying a visual indicator (e.g., a curved line or a marker) to show the player where they will land.  
  * **`OnTeleportRequest`:** The custom logic that executes the teleport.  
    * Checks if the `TeleportCooldown` has elapsed.  
    * If the raycast is successful, it calls the VRChat API function (e.g., `Networking.LocalPlayer.TeleportTo`) with the calculated destination and rotation.  
    * Spawns the `TeleportEffectPrefab` at the teleport location.  
    * Resets the teleport timer.  
* **Helper Functions:** Internal methods to keep the main event logic clean.  
  * `CalculateTargetPosition()`: Contains the math for the raycast, potentially handling curved-line projection for smoother VR aiming.  
  * `ApplyCooldown()`: Manages the internal timer and state to enforce the `TeleportCooldown`.

The script's success relies on utilizing the correct VRChat SDK APIs (specifically `VRCPlayerApi` and `Networking`) within the UdonSharp framework, ensuring it's written for the single correct version of Unity required for VRChat development.  
Based on the document, the VRChat API functions and classes necessary to successfully execute the teleportation logic are:

* **`Networking.LocalPlayer.TeleportTo`**: This is the specific function used to move the local player to the calculated destination and rotation.  
* **`VRCPlayerApi`**: This class is utilized for accessing player data, such as the `VRCPlayerApi.TrackingDataType` to determine the raycast origin and direction.  
* **`Networking`**: This class, specifically used for local player operations, contains the `TeleportTo` function.

The three main Udon Behaviors and Events that form the core logic of the teleportation script, as described in the document, are:

1. **`_Start`**: Initializes the script, setting up the initial state and any necessary references (like a raycast origin point).  
2. **`_Update`**: Runs every frame and is used for Input Detection, Raycasting, and Target Visualization (displaying a visual indicator for the landing spot).  
3. **`OnTeleportRequest`**: The custom logic that executes the teleport, including checking the `TeleportCooldown`, calling the VRChat API function (`Networking.LocalPlayer.TeleportTo`), spawning the `TeleportEffectPrefab`, and resetting the timer.

The `CalculateTargetPosition()` and `ApplyCooldown()` methods are contained within the **Helper Functions** component of the UdonScript Asset. The document describes this component as containing "Internal methods to keep the main event logic clean."  
The three main actions performed during the **\`\`\`**  
**\_Update**  
**\`\`\`** event in the UdonScript, as detailed in the document, are:

1. **Input Detection:** Checking for the user's input (e.g., button press on a controller) to initiate the teleport process.  
2. **Raycasting:** Performing a raycast from the tracking type's position to find a valid teleport destination on a surface defined by  
   ValidTeleportLayers  
    .  
3. **Target Visualization:** Displaying a visual indicator (e.g., a curved line or a marker) to show the player where they will land.

The two VRChat SDK APIs that must be utilized in the UdonSharp framework for the script's success, according to the document, are:

1. **\`\`\`**  
    **VRCPlayerApi**  
    **\`\`\`**  
2. **\`\`\`**  
    **Networking**  
    **\`\`\`**

The specific VRChat API function called if the raycast is successful during the **\`\`\`**  
**OnTeleportRequest**  
**\`\`\`** event is **\`\`\`**  
**Networking.LocalPlayer.TeleportTo**  
**\`\`\`**.  
The public variable used to control the time delay between teleports is **\`\`\`TeleportCooldown\`\`\`**.

The public variable that filters the valid surfaces for teleportation is **\`\`\`ValidTeleportLayers\`\`\`**.  
The public variable that allows the user to determine if they can move while waiting for the next teleport is **\`\`\`AllowMovementDuringCooldown\`\`\`**.  
The public variable that allows the user to specify which part of the player determines the target direction for the teleport is **\`\`\`VRCPlayerApi.TrackingDataType TrackingType\`\`\`**.  
The purpose of the **\`\`\`TeleportEffectPrefab\`\`\`** public variable is to serve as a visual effect (like a particle burst) to spawn at the origin or destination of the teleport.  
The two tasks managed by the **\`\`\`ApplyCooldown()\`\`\`** helper function are:

1. Managing the internal timer.  
2. Managing the state to enforce the **\`\`\`TeleportCooldown\`\`\`**.

The primary role of the **Public Variables for Customization** component in the UdonScript Asset is to be **exposed in the VRChat Inspector to allow for easy modification of the mechanic without editing the script.**  
The **\`\`\`CalculateTargetPosition()\`\`\`** helper function contains the math for the raycast, potentially handling **curved-line projection** for smoother VR aiming.  
The primary condition that must be met during the **\`\`\`OnTeleportRequest\`\`\`** event before the VRChat API function (like **\`\`\`Networking.LocalPlayer.TeleportTo\`\`\`**) is called is that **the raycast is successful**.

The script also first **Checks if the TeleportCooldown has elapsed** before proceeding with the rest of the teleport logic.  
The overall goal for writing the UdonScript Asset is to **write an UdonScript Asset that completely encompasses the Teleportation Game Mechanic in all aspects, which are all customizable in the Inspector.**

