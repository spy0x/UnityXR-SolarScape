using UnityEngine;
using UnityEngine.UIElements;

public class SmallSolarPanel : MonoBehaviour
{
    [SerializeField] private Vector3 rotateAxis = Vector3.up; // Axis to rotate around
    [SerializeField] private float rotationSpeed = 10f; // Speed of rotation in degrees per second

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
        if (other.CompareTag("Hammer"))
        {
            Destroy(gameObject);
        }
    }
}
