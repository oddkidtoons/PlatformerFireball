using UnityEngine;
using System.Collections;
using PLAYERTWO.PlatformerProject;
using UnityEngine.Events;

namespace PLAYERTWO.PlatformerProject{
[RequireComponent(typeof(AudioSource))]
    public class PPFireballHeat : MonoBehaviour
    {
        
AudioSource audioSource;
      [Header("Attack Settings")]
		public bool breakObjects;
		public int breakableDamage = 1;

		protected Collider m_collider;

        private Rigidbody rb;

        private Vector3 velocity;

        public Transform FireballParent;
        public GameObject BulletVFX;
        public AudioClip bulletAudio;
        public GameObject MuzzleVFX;

        public int enemyDamageAmount = 1;

          public GameObject ExplosionVFX;
          public bool timeDestroy;
           public float TimeToDestroy = 1f;
           float counter;
        public AudioClip explosionAudio;
       
      
public UnityEvent onExplode;

   

        void Update()
    {
        counter += Time.deltaTime;

        if (!timeDestroy) return;

        if (counter >= TimeToDestroy)
        {
           Explode();
        }
    }


        // Use this for initialization
        void Start()
        {
                 InitializeCollider();

            rb = GetComponent<Rigidbody>();
            velocity = rb.linearVelocity;

             audioSource = gameObject.GetComponent<AudioSource>();
            if (bulletAudio != null){ 
           
            audioSource.PlayOneShot(bulletAudio);}

BulletVFX = Instantiate(BulletVFX, transform.position, Quaternion.FromToRotation(Vector3.up, transform.up)) as GameObject;
            BulletVFX.transform.parent = transform;
            if (MuzzleVFX)
            {
                MuzzleVFX = Instantiate(MuzzleVFX, transform.position, Quaternion.FromToRotation(Vector3.up, transform.up)) as GameObject;
                Destroy(MuzzleVFX, 1.5f); // 2nd parameter is lifetime of effect in seconds
            }

        }


		protected virtual void InitializeCollider()
		{
			m_collider = GetComponent<Collider>();
			m_collider.isTrigger = true;
		}
        protected virtual void HandleCollision(Collider other)
		{
		

			if (other.TryGetComponent(out Breakable breakable))
			{
				HandleBreakableObject(breakable);
			}
		}
        protected virtual void HandleBreakableObject(Breakable breakable)
		{
			if (!breakObjects) return;

			breakable.ApplyDamage(breakableDamage);

		}

        // Update is called once per frame
        void FixedUpdate()
        {
  

            Vector3 gravity = 120 * Vector3.down; //cant simulate fireball bounces with normal realworld gravity, so i ad a downwards force that i can change from script, simulating gravity for fireball only
            rb.AddForce(gravity, ForceMode.Acceleration);

            if (rb.linearVelocity.y < velocity.y) //to avoid arcs formed when mario initially shoots fireball
            {
                rb.linearVelocity = velocity;
            }


        }
        
        
        protected virtual void HandleCustomCollision(Collider other) { }

      private void OnTriggerEnter(Collider other)
{    

     
    if (other.TryGetComponent<Enemy>(out var enemy))
    {
        enemy.ApplyDamage(enemyDamageAmount, transform.position);
      
        HandleCustomCollision(other);
        Explode();
    Destroy(this.gameObject);
        
      
    }
    
    HandleCollision(other);
    
}


        void OnCollisionEnter(Collision col)
        {

            
            if (col.contacts[0].normal.y > 0.4 && col.contacts[0].normal.y < 1.6)
            {
                rb.linearVelocity = new Vector3(velocity.x, -velocity.y, velocity.z);
            }

            //reflect
            if (col.contacts[0].normal.x > 0.3 || col.contacts[0].normal.z > 0.3f || col.contacts[0].normal.x < -0.3f || col.contacts[0].normal.z < -0.3f)
            {
                Vector3 oldVel = velocity;
                oldVel = oldVel.normalized;
                oldVel *= 1900 * Time.deltaTime;

                Vector3 newvel = Vector3.Reflect(oldVel, col.contacts[0].normal);

                velocity = new Vector3(newvel.x, oldVel.y, newvel.z);
            
          
              
     Explode();           
 Destroy(this.gameObject);


        }

        }   

private void Explode(){

      if (explosionAudio != null){ 
         AudioSource.PlayClipAtPoint(explosionAudio, transform.position, 1f);}

               // transform.position = hit.point + (hit.normal * collideOffset); // Move projectile to point of collision
 if (ExplosionVFX != null){ 
                GameObject impactP = Instantiate(ExplosionVFX, transform.position, Quaternion.FromToRotation(Vector3.up, transform.up)) as GameObject; // Spawns impact effect

                ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>(); // Gets a list of particle systems, as we need to detach the trails
                //Component at [0] is that of the parent i.e. this object (if there is any)
                for (int i = 1; i < trails.Length; i++) // Loop to cycle through found particle systems
                {
                    ParticleSystem trail = trails[i];

                    if (trail.gameObject.name.Contains("Trail"))
                    {
                        trail.transform.SetParent(null); // Detaches the trail from the projectile
                        Destroy(trail.gameObject, 2f); // Removes the trail after seconds
                    }
                }
                   Destroy(impactP, 3.5f); // Removes impact effect after delay
                }
onExplode?.Invoke();
                Destroy(BulletVFX, 3f); // Removes particle effect after delay
             
                Destroy(gameObject); // Removes the projectile
              
}



       }}