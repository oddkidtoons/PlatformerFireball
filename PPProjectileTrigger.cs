using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



namespace PLAYERTWO.PlatformerProject{

public class PPProjectileTrigger : MonoBehaviour
	{
	
		
		public OddKid_Input playerControls;
	private InputAction fire;

	public GameObject projectile;
		public Transform projectile_spawn_loc;
		public Vector3 projectileVel;

		private Animator anim;
		private Animator camAnim;

	private void Awake()
	{
		playerControls = new OddKid_Input();
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
				StartCoroutine(Shoot_Projectile());
		
		}

		IEnumerator Shoot_Projectile()
		{
			
				yield return new WaitForSeconds(0.1f);
				GameObject Clone = Instantiate(projectile, projectile_spawn_loc.position, projectile.transform.rotation);
				Clone.GetComponent<PPProjectile>().enabled = true;
				//Clone.transform.GetChild(0).gameObject.SetActive(true);
				Clone.GetComponent<Rigidbody>().velocity = transform.TransformDirection(projectileVel.x, projectileVel.y, projectileVel.z);//initial speed
		

		}


	}
}