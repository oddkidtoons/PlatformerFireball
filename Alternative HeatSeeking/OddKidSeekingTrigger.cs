using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class OddKidSeekingTrigger : MonoBehaviour
{
    public OddKid_Input playerControls;
    private InputAction fire;

    public GameObject firePoint;        // The fire point from where the projectile will be fired
    public GameObject effectToSpawn;    // The projectile prefab to be spawned

    int selectedPrefab = 0;
    private float timeToFire = 0;       // Controls fire rate timing
    private Text prefabName;            // UI text to show the current selected prefab

    private void Awake()
    {
        playerControls = new OddKid_Input();
    }

    private void OnEnable()
    {
        fire = playerControls.Player.Fire;
        fire.Enable();
        fire.performed += ShootFireBall;
    }

    private void OnDisable()
    {
        fire.Disable();
    }

    void Start()
    {
        // Add any initialization code if needed
    }

    private void ShootFireBall(InputAction.CallbackContext context)
    {
        // Time-based firing logic
        if (Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / effectToSpawn.GetComponent<OddKidSeekingProjectile>().fireRate;
            SpawnEffects();
        }
    }

    void SpawnEffects()
    {
        GameObject clone;

        // Check if firePoint is assigned
        if (firePoint != null)
        {
            // Instantiate the projectile at firePoint position with firePoint's rotation
            clone = Instantiate(effectToSpawn, firePoint.transform.position, firePoint.transform.rotation);

            // Make sure the projectile moves in the correct direction by using firePoint's forward vector
            Rigidbody rb = clone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.transform.forward * effectToSpawn.GetComponent<OddKidSeekingProjectile>().speed;
            }
        }
        else
        {
            Debug.Log("No Fire Point assigned!");
        }
    }
}
