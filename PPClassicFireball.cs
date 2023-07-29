using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;





public class PPClassicFireball : MonoBehaviour
	{
	
		
		public Fireball_Input playerControls;
	private InputAction fire;

	public GameObject fireball;
		public Transform fireball_spawn_loc;
		public Vector3 fireballVel;

		private Animator anim;
		private Animator camAnim;

	private void Awake()
	{
		playerControls = new Fireball_Input();
	}

	private void OnEnable()
	{
		fire = playerControls.Player.Fire;
		fire.Enable();
		fire.performed += shootFireBall;

			}

	private void OnDisable()
	{
		fire.Disable();
		
	}

	private void shootFireBall(InputAction.CallbackContext Context) 
	
	{
				StartCoroutine(Shoot_Fireball());
		
		}

		IEnumerator Shoot_Fireball()
		{
			
				yield return new WaitForSeconds(0.1f);
				GameObject Clone = Instantiate(fireball, fireball_spawn_loc.position, fireball.transform.rotation);
				Clone.GetComponent<PPFireball>().enabled = true;
				Clone.transform.GetChild(0).gameObject.SetActive(true);
				Clone.GetComponent<Rigidbody>().velocity = transform.TransformDirection(fireballVel.x, fireballVel.y, fireballVel.z);//initial speed
		

		}


	}
