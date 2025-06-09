using UnityEngine;
using UnityEngine.UIElements;

public class SmallSolarPanel : MonoBehaviour
{
    [SerializeField] private Vector3 rotateAxis = Vector3.up; // Axis to rotate around
    [SerializeField] private float rotationSpeed = 10f; // Speed of rotation in degrees per second
    [SerializeField] private GameObject panelPrefab; // Prefab to spawn when hit by a hammer
    [SerializeField] private float resetHitTime = 1f; // Time in seconds to reset the hit state
    [SerializeField] private float spawnDistance = 2f; // Distance to spawn the panel in front of the camera

    private bool hasBeenHit = false; // Flag to check if the panel has been hit

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        // Rotate the panel around the specified axis at the specified speed
        transform.Rotate(rotateAxis, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenHit) return; // Prevent multiple spawns
        if (other.CompareTag("Hammer"))
        {
            hasBeenHit = true; // Set the flag to true to prevent further spawns
            SpawnPanel();
            Invoke(nameof(EnableHasBeenHit), resetHitTime); // Reset the flag after 1 second
        }
    }

    private void SpawnPanel()
    {
        // spawn in front of the player MainCamera
        if (Camera.main)
        {
            Vector3 cameraRotation = Camera.main.transform.rotation.eulerAngles;
            Quaternion eularRotation = Quaternion.Euler(cameraRotation.x - 45, cameraRotation.y + 90, 0);
            Vector3 cameraPositionForward =
                Camera.main.transform.position + Camera.main.transform.forward * spawnDistance;
            Instantiate(panelPrefab, cameraPositionForward, eularRotation);
        }
    }

    private void EnableHasBeenHit()
    {
        hasBeenHit = false; // Reset the flag to allow future hits
    }
}