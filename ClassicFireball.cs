using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PLAYERTWO.PlatformerProject;


namespace PLAYERTWO.PlatformerProject
{

	public class ClassicFireball : MonoBehaviour
	{
		private Player player;
		PLAYERTWO.PlatformerProject.PlayerInputManager playerInput;

		public GameObject fireball;
		public Transform fireball_spawn_loc;
		public Vector3 fireballVel;

		private Animator anim;
		private Animator camAnim;

		void Start()
		{
			player = (Player)GameObject.FindObjectOfType(typeof(Player));
			playerInput = player.GetComponent<PLAYERTWO.PlatformerProject.PlayerInputManager>();
			Debug.Log(playerInput);
		}

		private void Update()
		{
			if (playerInput.actions["Fire"].WasPressedThisFrame())
			{
				StartCoroutine(Shoot_Fireball());
			}
		}

		IEnumerator Shoot_Fireball()
		{
			//if (playerInput.actions["Fire"].WasPressedThisFrame())
				//{
				yield return new WaitForSeconds(0.1f);
				GameObject Clone = Instantiate(fireball, fireball_spawn_loc.position, fireball.transform.rotation);
				Clone.GetComponent<Fireball>().enabled = true;
				Clone.transform.GetChild(0).gameObject.SetActive(true);
				Clone.GetComponent<Rigidbody>().velocity = transform.TransformDirection(fireballVel.x, fireballVel.y, fireballVel.z);//initial speed
			//}

		}


	}
}