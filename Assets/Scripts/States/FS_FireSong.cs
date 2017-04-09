using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_FireSong : FiniteState 
{
	float rainFadeDuration = 5;
	public ParticleSystem rainParticles;
	public ParticleSystem smokeParticles;

	private float rainFadeTimer;
	private float baseRate;
	private ParticleSystem.EmissionModule rainEmitter;

	protected override void OnInitialize ()
	{
		ParticleSystem.EmissionModule smokeEmitter = smokeParticles.emission;
		smokeEmitter.enabled = false;
	}

	protected override void OnEnter()
	{
		ParticleSystem.EmissionModule smokeEmitter = smokeParticles.emission;
		smokeEmitter.enabled = true;

		rainEmitter = rainParticles.emission;
		baseRate = rainEmitter.rateOverTimeMultiplier;
	}

	protected override void OnProcess ()
	{
		rainFadeTimer += Time.deltaTime;
		float t = 1 - Mathf.Clamp01(rainFadeTimer / rainFadeDuration);
		rainEmitter.rateOverTimeMultiplier = baseRate * t;

		if (Input.GetKeyDown(KeyCode.Space)) finiteStateController.GoToNextState();
	}

	protected override void OnExit ()
	{
	}
}
