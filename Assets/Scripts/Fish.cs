using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    // Speed and rotation speed of the fish
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f; 

    // Variables used for safe area
    private Camera mainCamera;
    private float camVerticalExtent;
    private float camHorizontalExtent;

    // to keep track of other neighbor fish
    private List<GameObject> fishInsideTrigger = new List<GameObject>();

    // Will be used to prevent fish from attempting to follow others (when they are out of bounds and need to return to safe area)
    private bool canFollow = true; 

    void Start()
    {
        //Getting boundaries
        mainCamera = Camera.main;
        camVerticalExtent = mainCamera.orthographicSize + 15;
        camHorizontalExtent = (camVerticalExtent * Screen.width / Screen.height) + 15;

        // Generate a random starting rotation angle for the fish
        float randomRotation = Random.Range(0, 360);
        transform.Rotate(new Vector3(0f, 0f, randomRotation));


    }

    void Update()
    {
        // If fish is allowed to follow its neighbors turn towards them (average position of them)
        if (canFollow)
        {
            RotateTowardsTarget();
        }

        // Check to see if we need to move fish back to the safe area
        OutOfBoundsCheck();

        // Move the fish forward
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }

    // Checks and corrects fish going out of bounds
    private void OutOfBoundsCheck()
    {
        // Check if the fish's position is beyond the camera's view
        float camTop = mainCamera.transform.position.y + camVerticalExtent;
        float camBottom = mainCamera.transform.position.y - camVerticalExtent;
        float camRight = mainCamera.transform.position.x + camHorizontalExtent;
        float camLeft = mainCamera.transform.position.x - camHorizontalExtent;

        if ((transform.position.y > camTop || transform.position.y < camBottom || transform.position.x > camRight || transform.position.x < camLeft) && canFollow)
        {
            // Move the player to the new spawn position and temporarily disable follow
            transform.position = -transform.position;
            StartCoroutine(TimeOut(2f));
        }
    }

    // Prevents fish from following others temporarily
    IEnumerator TimeOut(float delay)
    {
        canFollow = false;

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // After the delay, set the boolean to false
        canFollow = true;
    }

    // Turns towards the average position of the fish neighbors
    private void RotateTowardsTarget()
    {
        // If list of neighbors is empty don't rotate the fish.
        if (fishInsideTrigger.Count == 0)
        {
            return;
        }

        // Get average position and rotation needed
        Vector2 directionToTarget = GetAveragePosition(fishInsideTrigger);
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //rotate the fish
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    }

    // When neighbor triggers circle collider around this fish
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (fishInsideTrigger.Count < 5 && !fishInsideTrigger.Contains(collision.gameObject))
        {
            fishInsideTrigger.Add(collision.gameObject);
            Debug.Log("Fish entered trigger");
        }
    }

    // When neighbor leaves circle collider around this fish
    private void OnTriggerExit2D(Collider2D other)
    {
        if (fishInsideTrigger.Contains(other.gameObject))
        {
            fishInsideTrigger.Remove(other.gameObject);
            Debug.Log("Fish exited trigger");
        }
    }

    public static Vector3 GetAveragePosition(List<GameObject> objects)
    {
        // If list is empty return 0 vector
        if (objects == null || objects.Count == 0)
        {
            Debug.Log("List is empty");
            return Vector3.zero;
        }

        //calculate average position by adding all positions and dividing by the count
        Vector3 averagePosition = Vector3.zero;
        foreach (GameObject obj in objects)
        {
            averagePosition += obj.transform.position;
        }

        averagePosition /= objects.Count;
        return averagePosition;
    }
}
