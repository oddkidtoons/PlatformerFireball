using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace PLAYERTWO.PlatformerProject
{
    public class PPClassicFireballHeat : MonoBehaviour
    {
      
        public Fireball_Input playerControls;
        private InputAction fire;
  [Header("-------- Classic Fireball ------------------------")]
        public GameObject fireball;
        public Transform fireball_spawn_loc;
        public Vector3 fireballVel;
    [Header("-------- Shots to Overheat ------------------------")]
        public int maxShotsBeforeOverheat = 5;
          [Header("-------- Overheat Cooldown Delay ------------------------")]
        public float cooldownTime = 5f; // Overheat cooldown time
        private int shotsFired = 0;
        private bool isOverheated = false;
 [Header("-------- UI SLider / Fill ------------------------")]
        [SerializeField] private Image overheatingFill;
        public Color normalColor = Color.green;
        public Color overheatedColor = Color.red;
          [Header("-------- Overheat SoundFX ------------------------")]
   public AudioSource audioSource;
        public AudioClip overheatSound;

     [Header("-------- Overheat Events ------------------------")]
        public UnityEvent onOverheated;
        public UnityEvent onCooldownComplete;



        private bool isShooting = false;
        private bool isCooldownActive = false;
        private float stopShootingTimer = 0f;
          [Header("-------- Stop Shooting Cooldown Delay ------------------------")]
        public float stopShootingDelay = 1f;

     

        private float currentFillAmount = 0f;
        private Coroutine stopShootingDelayCoroutine;  // Reference to the delay coroutine

         [Header("--------For Testing Only ------------------------")]
        [SerializeField] private TextMeshProUGUI shotCountText;

        private void Awake()
        {
            playerControls = new Fireball_Input();
            shotsFired = 0;
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
                overheatingFill.fillAmount = 0f;
                overheatingFill.color = normalColor;
            }
            UpdateShotCountUI();
        }

        private void Update()
        {
             if (overheatingFill != null)
            {
            if (isCooldownActive)
            {
                stopShootingTimer += Time.deltaTime;
                float cooldownProgress;

                if (!isOverheated)
                {
                    // Cooldown after stop shooting
                    cooldownProgress = stopShootingTimer / stopShootingDelay;
                    currentFillAmount = Mathf.Lerp((float)shotsFired / maxShotsBeforeOverheat, 0f, cooldownProgress);

                    // Update the shot count based on the cooldown
                    int shotsToRemove = Mathf.FloorToInt(shotsFired * cooldownProgress);
                    shotsFired -= shotsToRemove;

                    // Update the UI with the current percentage
                    overheatingFill.fillAmount = Mathf.Clamp01(currentFillAmount);

                    if (cooldownProgress >= 1f)
                    {
                        ResetShots();
                        isCooldownActive = false;
                    }
                }
                else
                {
                    // Cooldown after overheat
                    cooldownProgress = stopShootingTimer / cooldownTime;
                    currentFillAmount = Mathf.Lerp(1f, 0f, cooldownProgress);

                    // Update the shot count based on the cooldown
                    int shotsToRemove = Mathf.FloorToInt(shotsFired * cooldownProgress);
                    shotsFired -= shotsToRemove;

                    overheatingFill.fillAmount = Mathf.Clamp01(currentFillAmount);

                    if (cooldownProgress >= 1f)
                    {
                        ResetShots();
                        isCooldownActive = false;
                        onCooldownComplete?.Invoke();
                    }
                }
            }}

            UpdateShotCountUI();
        }

        private void ShootFireBall(InputAction.CallbackContext context)
        {
            if (isOverheated)
            {
                Debug.Log("Weapon is overheated! Wait for cooldown.");
                return;
            }

            if (!isOverheated)
            {
                StartCoroutine(Shoot_Fireball());
                shotsFired++;

                if (overheatingFill != null)
                {
                    currentFillAmount = (float)shotsFired / maxShotsBeforeOverheat;
                    overheatingFill.fillAmount = currentFillAmount;
                }

                UpdateShotCountUI();
            }

            if (shotsFired >= maxShotsBeforeOverheat)
            {
                OverheatWeapon();
            }

            // If shooting again during cooldown, reset the delay timer and restart it
            if (isCooldownActive)
            {
                ResetCooldownState();
            }
        }

        private void StopShooting(InputAction.CallbackContext context)
        {
            if (!isOverheated && shotsFired > 0)
            {
                if (stopShootingDelayCoroutine != null)
                {
                    // Cancel the previous stop shooting delay coroutine
                    StopCoroutine(stopShootingDelayCoroutine);
                }
                stopShootingDelayCoroutine = StartCoroutine(StartCooldownAfterDelay());
                Debug.Log("Stopping fire, cooldown starting now.");
            }
        }

        private IEnumerator StartCooldownAfterDelay()
        {
            // Delay before starting the stop shooting cooldown
            yield return new WaitForSeconds(stopShootingDelay);

            // Set the current fill amount based on shots fired, for a smooth transition
            if (overheatingFill != null)
            {
                currentFillAmount = (float)shotsFired / maxShotsBeforeOverheat;
                overheatingFill.fillAmount = currentFillAmount;
            }

            // Start cooldown after the delay
            isCooldownActive = true;
            stopShootingTimer = 0f;
            Debug.Log("Stop shooting cooldown has started.");
        }

        private void ResetCooldownState()
        {
            // Reset the cooldown state and UI when shooting resumes
            isCooldownActive = false;
            stopShootingTimer = 0f;

            if (overheatingFill != null)
            {
                currentFillAmount = (float)shotsFired / maxShotsBeforeOverheat;
                overheatingFill.fillAmount = currentFillAmount;
            }

            Debug.Log("Cooldown reset due to new shot.");
        }

        private void ResetShots()
        {
            shotsFired = 0;
            isOverheated = false;
            stopShootingTimer = 0f;

            if (overheatingFill != null)
            {
                overheatingFill.fillAmount = 0f;
                overheatingFill.color = normalColor;
            }

            Debug.Log("Cooldown complete, shots reset to 0.");
            UpdateShotCountUI();
        }

        private void OverheatWeapon()
        {
            isOverheated = true;

            if (audioSource != null && overheatSound != null)
            {
                audioSource.PlayOneShot(overheatSound);
            }

            Debug.Log("Weapon overheated! Cooling down...");

            if (overheatingFill != null)
            {
                overheatingFill.color = overheatedColor;
            }
            onOverheated?.Invoke();

            // Trigger cooldown after overheating
            isCooldownActive = true;
            stopShootingTimer = 0f;
        }

        private IEnumerator Shoot_Fireball()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject clone = Instantiate(fireball, fireball_spawn_loc.position, fireball.transform.rotation);
            clone.GetComponent<Rigidbody>().linearVelocity = transform.TransformDirection(fireballVel);
        }

        private void UpdateShotCountUI()
        {
            if (shotCountText != null)
            {
                shotCountText.text = $"{shotsFired}";
            }
        }
    }
}
