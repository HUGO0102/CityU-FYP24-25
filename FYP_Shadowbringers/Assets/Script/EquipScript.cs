using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipScript : MonoBehaviour
{
    public GameObject unequippedWeapon;
    public GameObject guitar;
    public Camera Camera;
    public float range = 2f;
    public GameObject interactionUI;
    public GameObject Player;
    public GameObject LabDoorTrigger;

    public bool isEquipped = false;

    private bool isWithinRange = false;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the interaction UI is initially inactive
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is within range of the target
        DetectTargetDistance();

        // Use raycast to check if the player is looking at the target and allow picking it up
        CheckRaycastForPickup();
    }

    void DetectTargetDistance()
    {
        // Ensure the target is assigned
        if (unequippedWeapon != null)
        {
            // Calculate the distance between the player and the target
            float distance = Vector3.Distance(Player.transform.position, unequippedWeapon.transform.position);

            // If within range, show the interaction UI
            if (distance <= range)
            {
                if (!isWithinRange && interactionUI != null)
                {
                    interactionUI.SetActive(true); // Show the interaction UI
                }
                isWithinRange = true;
            }
            else
            {
                // If out of range, hide the interaction UI
                if (isWithinRange && interactionUI != null)
                {
                    interactionUI.SetActive(false); // Hide the interaction UI
                }
                isWithinRange = false;
            }
        }
    }

    void CheckRaycastForPickup()
    {
        // Perform raycast only if the player is within range
        if (isWithinRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, range))
            {
                // Check if the raycast hit the target object
                if (hit.transform.gameObject == unequippedWeapon)
                {
                    // If the player presses "F", pick the object
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        PickObject();
                    }
                }
            }
        }
    }

    void PickObject()
    {
        // Attempt to get the Target component from the target object
        Target targetComponent = unequippedWeapon.GetComponent<Target>();

        if (targetComponent != null)
        {
            // Call the target's picked logic
            targetComponent.Picked();
        }

        // Activate the guitar and deactivate the unequipped weapon
        guitar.SetActive(true);
        LabDoorTrigger.SetActive(true);
        isEquipped = true;
        if (unequippedWeapon != null)
        {
            unequippedWeapon.SetActive(false);
        }

        // Destroy the picked object
        Destroy(unequippedWeapon);

        // Hide the interaction UI
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        // Reset proximity state
        isWithinRange = false;
        unequippedWeapon = null;
    }
}
