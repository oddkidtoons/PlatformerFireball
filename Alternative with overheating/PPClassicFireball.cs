using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // For UI components

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
        public float cooldownTime = 5f; // Time needed to fully cool down
        private int currentShots = 0; // Current shot count
        private bool isOverheated = false; // Flag for overheating state

        [SerializeField] private Image overheatingFill; // UI Image fill for overheating status
        public Color normalColor = Color.green; // Bar color when not overheated
        public Color overheatedColor = Color.red; // Bar color when overheated
        private float cooldownTimer = 0f;

        public AudioSource audioSource; // Reference to the AudioSource
        public AudioClip overheatSound; // Sound when the weapon overheats

        private void Awake()
        {
            playerControls = new Fireball_Input();
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

        private void Start()
        {
            if (overheatingFill != null)
            {
                overheatingFill.fillAmount = 0f; // Initialize at 0 (no overheating)
                overheatingFill.color = normalColor;
            }
        }

        private void Update()
        {
            // Handle cooldown when not shooting
            if (isOverheated)
            {
                cooldownTimer += Time.deltaTime;
                if (overheatingFill != null)
{
    overheatingFill.fillAmount = Mathf.Lerp(1f, 0f, cooldownTimer / cooldownTime);
    Debug.Log("Fill Amount during cooldown: " + overheatingFill.fillAmount);
}

                if (cooldownTimer >= cooldownTime)
                {
                    isOverheated = false;
                    currentShots = 0;
                    cooldownTimer = 0f;
                    Debug.Log("Weapon cooled down and ready to fire!");

                    if (overheatingFill != null)
                    {
                        overheatingFill.color = normalColor;
                        overheatingFill.fillAmount = 0f; // Reset to empty
                    }
                }
            }
        }

        private void ShootFireBall(InputAction.CallbackContext context)
        {
            if (!isOverheated)
            {
                StartCoroutine(Shoot_Fireball());
                currentShots++;
                if (overheatingFill != null)
{
    overheatingFill.fillAmount = (float)currentShots / maxShotsBeforeOverheat;
    Debug.Log("Fill Amount after shot: " + overheatingFill.fillAmount);
}

                if (currentShots >= maxShotsBeforeOverheat)
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
                }
            }
            else
            {
                Debug.Log("Weapon is overheated! Wait for cooldown.");
            }
        }

        private IEnumerator Shoot_Fireball()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject clone = Instantiate(fireball, fireball_spawn_loc.position, fireball.transform.rotation);
            clone.GetComponent<Rigidbody>().linearVelocity = transform.TransformDirection(fireballVel);
        }
    }
}