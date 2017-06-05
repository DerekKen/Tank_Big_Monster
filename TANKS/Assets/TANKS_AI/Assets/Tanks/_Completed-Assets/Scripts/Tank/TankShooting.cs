﻿using UnityEngine;
using UnityEngine.UI;
using CnControls;
namespace Complete
{
	public class TankShooting : MonoBehaviour
	{
		public int m_PlayerNumber = 1;              // Used to identify the different players.
		public Rigidbody m_Shell;                   // Prefab of the shell.
		public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
		public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
		public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
		public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
		public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
		public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
		public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
		public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.


		private string m_FireButton;                // The input axis that is used for launching shells.
		private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
		private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
		private bool m_Fired;                       // Whether or not the shell has been launched with this button press.
		private float nextFireTime;           


		private void OnEnable()
		{
			m_CurrentLaunchForce = m_MinLaunchForce;
			m_AimSlider.value = m_MinLaunchForce;
		}


		private void Start()
		{
			m_FireButton = "Fire" + m_PlayerNumber;
            
			m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
		}

		private void Update()
		{
			// Track the current state of the fire button and make decisions based on the current launch force.

			m_AimSlider.value = m_MinLaunchForce;

			//at max charge, not yet fired
			if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) {
				m_CurrentLaunchForce = m_MaxLaunchForce;
				Fire ();
			}

			// have we pressed the fire button for the first time
			else if (CnInputManager.GetButtonDown (m_FireButton)) {
				m_Fired = false;
				m_CurrentLaunchForce = m_MinLaunchForce;

				m_ShootingAudio.clip = m_ChargingClip;
				m_ShootingAudio.Play ();

			}

			//Holding the fire button, not yet fired
			else if (CnInputManager.GetButton (m_FireButton) && !m_Fired) {
				m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

				m_AimSlider.value = m_CurrentLaunchForce;
			}

			//We released the fire button, having not fired yet
			else if (CnInputManager.GetButtonUp (m_FireButton) && !m_Fired) 
			{
				Fire ();
			}

		}


		public void Fire()
		{
			// Instantiate and launch the shell.
			m_Fired = true;

			Rigidbody shellInstance = Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

			shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

			m_ShootingAudio.clip = m_FireClip;
			m_ShootingAudio.Play ();

			m_CurrentLaunchForce = m_MinLaunchForce;

		}


		public void Fire (float launchForce, float fireRate)
		{
			if (Time.time > nextFireTime) 
			{
				nextFireTime = Time.time + fireRate;
				// Set the fired flag so only Fire is only called once.
				m_Fired = true;

				// Create an instance of the shell and store a reference to it's rigidbody.
				Rigidbody shellInstance =
					Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

				// Set the shell's velocity to the launch force in the fire position's forward direction.
				shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; 

				// Change the clip to the firing clip and play it.
				m_ShootingAudio.clip = m_FireClip;
				m_ShootingAudio.Play ();

				// Reset the launch force.  This is a precaution in case of missing button events.
				m_CurrentLaunchForce = m_MinLaunchForce;
			}

		}
	}
}