using UnityEngine;

// URP14 only honors additional camera data for Game cameras. This marker routes
// an owned offscreen capture through the actor pass without world presentation.
[DisallowMultipleComponent]
public sealed class DungeonCaptureCamera : MonoBehaviour { }
