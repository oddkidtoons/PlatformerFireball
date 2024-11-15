using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;  // <-- Add this for accessing Unity UI Image component
using TMPro;  // For TextMeshPro

namespace PLAYERTWO.PlatformerProject
{
    public class PPClassicFireball : MonoBehaviour
    {
        public Fireball_Input playerControls;
        private InputAction fire;

        public GameObject fireball;
        public Transform fireball_spawn_loc;
        public Vector3 fireballVel;

        public int maxShotsBeforeOverheat = 5; // Max shots before overheating
        public float cooldownTime = 5f; // Time to fully cool down (in seconds)
        private int shotsFired = 0; // Number of shots fired
        private bool isOverheated = false; // Flag for overheating state

        [SerializeField] private Image overheatingFill; // UI Image fill for overheating status
        public Color normalColor = Color.green; // Bar color when not overheated
        public Color overheatedColor = Color.red; // Bar color when overheated
        private bool isShooting = false; // To track if the player is shooting
        private bool isCooldownActive = false; // To check if cooldown is active
        private float stopShootingTimer = 0f; // Timer to delay cooldown after stopping shooting
        public float stopShootingDelay = 1f; // Delay before shots reset after stopping shooting (configurable by the user)

        public AudioSource audioSource; // Reference to the AudioSource
        public AudioClip overheatSound; // Sound when the weapon overheats

        // Reference to TextMeshPro for showing the shot count
        [SerializeField] private TextMeshProUGUI shotCountText; // For TextMeshProUGUI

        private void Awake()
        {
            playerControls = new Fireball_Input();
            shotsFired = 0; // Reset shots fired
        }

        private void OnEnable()
        {
            fire = playerControls.Player.Fire;
            fire.Enable();
            fire.performed += ShootFireBall;
            fire.canceled += StopShooting;
        }

        private void OnDisable()
        {
            fire.Disable();
        }

        private void Start()
        {
            if (overheatingFill != null)
            {
                overheatingFill.fillAmount = 0f; // Initialize at 0 (no overheating)
                overheatingFill.color = normalColor;
            }

            // Initialize the shot count UI
            UpdateShotCountUI();
        }

        private void Update()
        {
            // If cooldown is active, handle the cooldown process
            if (isCooldownActive)
            {
                stopShootingTimer += Time.deltaTime;

                // Wait for stopShootingDelay before resetting shots
                if (stopShootingTimer >= stopShootingDelay)
                {
                    ResetShots();
                    isCooldownActive = false;
                }
            }

            // If overheating, handle cooling down process
            if (isOverheated)
            {
                stopShootingTimer += Time.deltaTime;
                // Calculate cooldown progress
                float cooldownProgress = stopShootingTimer / cooldownTime;

                // Update overheating UI fill
                if (overheatingFill != null)
                {
                    overheatingFill.fillAmount = 1f - cooldownProgress; // Cooldown decreases the fill
                }

                // Once cooldown is complete, reset overheating and shots
                if (cooldownProgress >= 1f)
                {
                    ResetShots();
                }
            }

            // Update shot count UI with current value
            UpdateShotCountUI();
        }

        private void ShootFireBall(InputAction.CallbackContext context)
        {
            if (isOverheated)
            {
                Debug.Log("Weapon is overheated! Wait for cooldown.");
                return;
            }

            if (shotsFired < maxShotsBeforeOverheat)
            {
                StartCoroutine(Shoot_Fireball());
                shotsFired++; // Increase shots fired count

                // Update the overheating fill bar based on shots fired
                if (overheatingFill != null)
                {
                    overheatingFill.fillAmount = (float)shotsFired / maxShotsBeforeOverheat;
                }

                // Update shot count UI
                UpdateShotCountUI();

                if (shotsFired >= maxShotsBeforeOverheat)
                {
                    OverheatWeapon();
                }
            }
        }

        private void StopShooting(InputAction.CallbackContext context)
        {
            if (isShooting)
            {
                stopShootingTimer = 0f; // Reset stop shooting timer if shooting resumes
                isCooldownActive = false; // Stop cooldown if shooting is resumed
            }

            // Start cooldown after the delay when shooting stops
            if (!isOverheated && shotsFired > 0)
            {
                Debug.Log("Stopping fire, cooldown will start after delay...");
                stopShootingTimer += Time.deltaTime;
                if (stopShootingTimer >= 2f)
                {
                    isCooldownActive = true;
                }
            }
        }

        // Reset shots fired and handle overheating state
        private void ResetShots()
        {
            shotsFired = 0;
            isOverheated = false; // Reset overheating state
            stopShootingTimer = 0f; // Reset cooldown timer

            if (overheatingFill != null)
            {
                overheatingFill.fillAmount = 0f;
                overheatingFill.color = normalColor; // Reset color to normal
            }

            Debug.Log("Cooldown complete, shots reset to 0.");
            UpdateShotCountUI(); // Update the shot count display
        }

        // Overheat logic
        private void OverheatWeapon()
        {
            isOverheated = true;

            if (audioSource != null && overheatSound != null)
            {
                audioSource.PlayOneShot(overheatSound); // Play the overheat sound
            }

            Debug.Log("Weapon overheated! Cooling down...");

            if (overheatingFill != null)
            {
                overheatingFill.color = overheatedColor; // Change the bar color to red when overheated
            }
        }

        private IEnumerator Shoot_Fireball()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject clone = Instantiate(fireball, fireball_spawn_loc.position, fireball.transform.rotation);
            clone.GetComponent<Rigidbody>().linearVelocity = transform.TransformDirection(fireballVel);
        }

        // Update the shot count UI text (TextMeshPro)
        private void UpdateShotCountUI()
        {
            if (shotCountText != null)
            {
                shotCountText.text = $"{shotsFired}";
            }
        }
    }
}
