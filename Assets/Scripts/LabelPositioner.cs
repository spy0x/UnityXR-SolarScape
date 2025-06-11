using UnityEngine;

public class LabelPositioner : MonoBehaviour
{
    [SerializeField] Transform[] labelPositions; // Array of positions for labels
    [SerializeField] Transform playerHead; // Reference to the player's head for positioning

    private void Start()
    {
        playerHead = GameObject.FindWithTag("MainCamera").transform;
    }

    void Update()
    {
        UpdatePosition();
        LookTowardsPlayer();
    }

    private void LookTowardsPlayer()
    {
        if (playerHead == null) return;

        // Calculate the direction to the player head
        Vector3 directionToPlayer = playerHead.position - transform.position;
        directionToPlayer.y = 0; // Keep the label horizontal

        // Set the rotation to look at the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    // Position the label in the highest Y direction
    private void UpdatePosition()
    {
        if (labelPositions == null || labelPositions.Length == 0) return;

        // Find the position with the highest Y value
        Transform highestPosition = labelPositions[0];
        foreach (Transform position in labelPositions)
        {
            if (position.position.y > highestPosition.position.y)
            {
                highestPosition = position;
            }
        }

        // Set the position of this GameObject to the highest position
        transform.position = highestPosition.position;
    }
}
