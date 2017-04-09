using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireController : MonoBehaviour {

	public static CampfireController instance;

	public Gradient fireGradient;
	public Gradient flashGradient;

	public ParticleSystem fireParticles;
	public Transform fireFlash;

	[System.Serializable]
	public class FireData 
	{
		public float minLifetime;
		public float maxLifetime;
		public float minSizeLow;
		public float maxSizeLow;
		public float minSizeHigh;
		public float maxSizeHigh;
		public float minSpeed;
		public float maxSpeed;

		public void SetNormalizedStrength (ParticleSystem particles, float value, Gradient colorGradient)
		{
			ParticleSystem.MainModule main = particles.main;

			float deltaLifetime = maxLifetime - minLifetime;
			main.startLifetime = minLifetime + deltaLifetime * value;

			float deltaMaxSize = maxSizeHigh - maxSizeLow;
			float maxCurve = maxSizeLow + deltaMaxSize * value;
			float deltaMinSize = minSizeHigh - minSizeLow;
			float minCurve = minSizeLow + deltaMinSize * value;
			main.startSize = new ParticleSystem.MinMaxCurve(minCurve, maxCurve); 

			float deltaSpeed = maxSpeed - minSpeed;
			main.startSpeed = minSpeed + deltaSpeed * value;

			main.startColor = colorGradient.Evaluate(value);
		}
	}

	public FireData lowFire;
	public FireData highFire;

	private ParticleSystem.EmissionModule fireEmitter;

	public void SetFireIsActive (bool isActive)
	{
		fireEmitter.enabled = isActive;
		SetFireIsLow(true);
	}

	public void SetFireIsLow (bool isLow)
	{
		if (isLow)
		{
			//lowFire.SetNormalizedStrength(fireParticles, 0);
		}
		else
		{
			//highFire.SetNormalizedStrength(fireParticles, 0);
		}
	}

	void Awake ()
	{
		instance = this;
		fireEmitter = fireParticles.emission;
		fireEmitter.enabled = false;
		fireFlash.gameObject.SetActive(false);
	}
}
