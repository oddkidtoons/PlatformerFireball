using System.Collections;
using UnityEngine;

public class OddKidSeekingProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;  // Speed of the projectile
    public float fireRate = 1f; // Rate at which projectiles can be fired

    [Header("VFX")]
    public GameObject muzzlePrefab;  // Effect to spawn when the projectile is fired
    public GameObject hitPrefab;  // Effect to spawn when the projectile hits something
    public GameObject projectileVFX;  // Projectile VFX (to be assigned in the Inspector)

    // Scale adjustments for each VFX
    public Vector3 muzzleScale = Vector3.one;  // Scale factor for muzzle VFX
    public Vector3 hitScale = Vector3.one;  // Scale factor for hit VFX
    public Vector3 projectileScale = Vector3.one;  // Scale factor for projectile VFX

    [Header("Seeking Settings")]
    public string targetTag = "Enemy";  // Tag of the target to seek
    public float startSeekDelay = 0.2f;
    public float seekRange = 20f;  // The range within which the missile will search for a target
    public float baseRotationSpeed = 5f;  // Base speed of rotation for turning
    public float closeDistanceThreshold = 2f; // Distance at which the missile makes sharper turns
    public float hitEventDelay = 1f;  // Delay before the hit event is invoked

    [Header("Hit")]
    public UnityEngine.Events.UnityEvent onHitEvent;  // Event to trigger when the projectile hits the target

    private Transform target;  // The current target's transform
    private Rigidbody rb;  // Rigidbody for smooth movement and physics handling
    private bool isSeeking = false;  // Flag to indicate whether the missile is seeking a target
    private bool hasHit = false;  // Flag to check if the projectile has already hit something
    private GameObject instantiatedProjectileVFX;  // Reference to the instantiated projectile VFX

[Header("Ignored Layers")]
    public LayerMask ignoredLayers;  // Layer mask to specify which layers to ignore


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Instantiate muzzle effect with scale
        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = transform.forward;
            muzzleVFX.transform.localScale = muzzleScale;  // Apply the scale to the muzzle VFX
            var psMuzzle = muzzleVFX.GetComponent<ParticleSystem>();
            if (psMuzzle != null)
            {
                Destroy(muzzleVFX, psMuzzle.main.duration);
            }
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }

        // Instantiate projectile VFX and make it follow the projectile with scale
        if (projectileVFX != null)
        {
            instantiatedProjectileVFX = Instantiate(projectileVFX, transform.position, transform.rotation);
            instantiatedProjectileVFX.transform.SetParent(transform); // Make the VFX follow the projectile
            instantiatedProjectileVFX.transform.localScale = projectileScale;  // Apply the scale to the projectile VFX
        }
        else
        {
            Debug.LogWarning("projectileVFX is null. Ensure it is assigned in the Inspector.");
        }

        // Start seeking after a delay
        StartCoroutine(SeekTargetAfterDelay(startSeekDelay));  // Start seeking after a delay
    }

    void Update()
    {
        if (isSeeking && target != null)
        {
            Vector3 directionToTarget = GetTargetCenter(target) - transform.position;
            float distanceToTarget = directionToTarget.magnitude;
            directionToTarget.Normalize();

            float rotationSpeed = baseRotationSpeed;

            if (distanceToTarget <= closeDistanceThreshold)
            {
                rotationSpeed *= 3f; // Increase the turn rate when very close
            }
            else
            {
                rotationSpeed = Mathf.Lerp(baseRotationSpeed, baseRotationSpeed * 2, 1f - (distanceToTarget / seekRange));
            }

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            rb.linearVelocity = transform.forward * speed;
        }
        else
        {
            rb.linearVelocity = transform.forward * speed;
        }
    }

    IEnumerator SeekTargetAfterDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        isSeeking = true;
        FindClosestTarget();
    }

    void FindClosestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, seekRange);
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag(targetTag))
            {
                float distanceToTarget = Vector3.Distance(transform.position, col.bounds.center);
                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    closestTarget = col.transform;
                }
            }
        }

        if (closestTarget != null)
        {
            target = closestTarget;
        }
    }

    Vector3 GetTargetCenter(Transform targetTransform)
    {
        Collider targetCollider = targetTransform.GetComponent<Collider>();
        if (targetCollider != null)
        {
            return targetCollider.bounds.center;
        }
        else
        {
            return targetTransform.position;
        }
    }

     void OnCollisionEnter(Collision other)
    {
        if (hasHit) return;

        // Check if the object is on one of the ignored layers
        if ((ignoredLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            return;  // Ignore collision if the object is on an ignored layer
        }

        speed = 0;

        ContactPoint contact = other.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot);
            var psHit = hitVFX.GetComponent<ParticleSystem>();
            hitVFX.transform.localScale = hitScale;
            if (psHit != null)
            {
                Destroy(hitVFX, psHit.main.duration);
            }
            else
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
        }

        Destroy(gameObject);
    }



    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // Check if the target is the correct tag (EmeraldAI enemies)
        if (other.CompareTag(targetTag))
        {
            hasHit = true;
            rb.linearVelocity = Vector3.zero;

            // Destroy VFX if instantiated
            if (instantiatedProjectileVFX != null)
            {
                Destroy(instantiatedProjectileVFX);
            }

            // Spawn the hit effect at the correct position with scale
            if (hitPrefab != null)
            {
                InstantiateHitEffect(other);
            }

            // Invoke the event and destroy the projectile
            StartCoroutine(InvokeEventWithDelay(hitEventDelay));
            Destroy(gameObject, 3f);
        }
    }

    // Helper function to handle hit effect instantiation with scale
    private void InstantiateHitEffect(Collider other)
    {
        Vector3 hitPosition = other.ClosestPoint(transform.position); // Use the exact collision point
        Quaternion rot = Quaternion.LookRotation(other.transform.position - transform.position); // Look at the target

        var hitVFX = Instantiate(hitPrefab, hitPosition, rot);
        hitVFX.transform.SetParent(other.transform); // Make the hit effect follow the target
        hitVFX.transform.localScale = hitScale;  // Apply the scale to the hit VFX

        var psHit = hitVFX.GetComponent<ParticleSystem>();
        if (psHit != null)
        {
            Destroy(hitVFX, psHit.main.duration);
        }
        else
        {
            var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
            Destroy(psChild.gameObject, psChild.main.duration);
        }
    }

    IEnumerator InvokeEventWithDelay(float delay)
    {
        Debug.Log("Hit detected, starting event coroutine.");
        yield return new WaitForSeconds(delay);

        if (onHitEvent != null)
        {
            onHitEvent.Invoke();
            Debug.Log("Event invoked successfully.");
        }
        else
        {
            Debug.LogWarning("onHitEvent is not assigned.");
        }
    }
}
