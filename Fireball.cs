using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour
{
    public string EnemyTag;
    private Rigidbody rb;

    private Vector3 velocity;

    public float TimeToDestroy = 1f;

    public Transform FireballObject;
    public GameObject FireChild;

    

    //float counter;
    private void Awake()
    {
        StartCoroutine(timedDeath());
    }
   

    // Use this for initialization
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        velocity = rb.velocity;
        //Assigns the transform of the first child of the Game Object this script is attached to.
        FireballObject = this.gameObject.transform.GetChild(0);
        //Assigns the first child of the first child of the Game Object this script is attached to.
	    FireChild = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;
	    FireChild.SetActive(true);

       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 gravity = 120 * Vector3.down; //cant simulate fireball bounces with normal realworld gravity, so i ad a downwards force that i can change from script, simulating gravity for fireball only
        rb.AddForce(gravity, ForceMode.Acceleration);

        if (rb.velocity.y < velocity.y) //to avoid arcs formed when mario initially shoots fireball
        {
            rb.velocity = velocity;
        }

    }


    void OnCollisionEnter(Collision col)
    {
           
        if (col.contacts[0].normal.y > 0.4 && col.contacts[0].normal.y < 1.6)
        {
            rb.velocity = new Vector3(velocity.x, -velocity.y, velocity.z);
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

            if (col.gameObject.tag == EnemyTag)
            {
                Invoke("Explode", delayExplode);
                Destroy(this.gameObject);
            }

        }

    }

    public GameObject effect;
    private GameObject effectClone;
    public float delayExplode;

    
    void Explode()
    {
        effectClone = effect;
        Instantiate(effectClone, gameObject.transform.position, gameObject.transform.rotation);
        //Destroy(effectClone);
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
}