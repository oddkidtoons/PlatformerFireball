using UnityEngine;
using System.Collections;
using PLAYERTWO.PlatformerProject;

namespace PLAYERTWO.PlatformerProject{

    public class PPFireball : MonoBehaviour
    {
      [Header("Attack Settings")]
		public bool breakObjects;
		public int breakableDamage = 1;

		protected Collider m_collider;
       

        private Rigidbody rb;

        private Vector3 velocity;

        public float TimeToDestroy = 1f;

        public Transform FireballParent;
        public GameObject FireVFX;

        public int enemyDamageAmount = 1;

        float counter;

          public GameObject ExplosionVFX;
        private GameObject effectClone;
        public float delayExplode;


        private void Awake()
        {
            StartCoroutine(timedDeath());
        }


        // Use this for initialization
        void Start()
        {
            	ExplosionVFX.SetActive(false);
			InitializeCollider();

            rb = GetComponent<Rigidbody>();
            velocity = rb.linearVelocity;
            //Assigns the transform of the first child of the Game Object this script is attached to.
            // FireballObject = FireballObject.gameObject.transform.GetChild(0);
            //Assigns the first child of the first child of the Game Object this script is attached to.
            FireVFX = FireballParent.gameObject.transform.GetChild(0).gameObject;
            FireVFX.SetActive(true);


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
        Invoke("Explode", delayExplode);
        Destroy(this.gameObject);
        HandleCustomCollision(other);
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
            //rb.velocity = rb.velocity;
          
                Invoke("Explode", delayExplode);
                Destroy(this.gameObject);
          



        }

        }

      


        void Explode()
        {
            ExplosionVFX.SetActive(true);
            effectClone = ExplosionVFX;
            Instantiate(effectClone, gameObject.transform.position, gameObject.transform.rotation);
            
        }


        public IEnumerator Destroy()
        {
            transform.GetComponent<MeshRenderer>().enabled = false;
            transform.GetComponent<SphereCollider>().enabled = false;
            transform.GetComponent<ParticleSystem>().Stop();
            GameObject Dissolve = transform.GetChild(0).gameObject;
            GameObject Clone = Instantiate(Dissolve, transform.position, Dissolve.transform.rotation);
            Clone.GetComponent<ParticleSystem>().Play();
            Destroy(Clone, 3);
            yield return new WaitForSeconds(1);
            Destroy(gameObject);

        }

        IEnumerator timedDeath()
        {
            yield return new WaitForSeconds(TimeToDestroy);
            Object.Destroy(this.gameObject);
        }
    } }