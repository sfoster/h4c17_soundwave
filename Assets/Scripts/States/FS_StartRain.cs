using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_StartRain : FiniteState {

	public ParticleSystem rainParticles;
	public float startRainDuration = 5;
	public AnimationCurve bringRainCurve;

	private float baseRate;
	private float rainAcceleration;
	private float rainAmount;
	private ParticleSystem.EmissionModule rainEmitter;

	protected override void OnInitialize()
	{
		rainEmitter = rainParticles.emission;
		baseRate = rainEmitter.rateOverTimeMultiplier;
		rainAcceleration = baseRate / startRainDuration;
		rainEmitter.rateOverTimeMultiplier = 0;
		rainEmitter.enabled = false;
	}

	protected override void OnEnter ()
	{
		rainEmitter.enabled = true;
	}

	protected override void OnProcess ()
	{
		// if the audience mic loudness is over a certain level
		bool isAudienceLoud = Input.GetKey(KeyCode.Space);

		if (isAudienceLoud)
		{
			rainAmount += rainAcceleration * Time.deltaTime;
		}
		else
		{
			rainAmount -= rainAcceleration * Time.deltaTime;
		}

		rainAmount = Mathf.Clamp(rainAmount, 0, baseRate);

		float t = bringRainCurve.Evaluate(rainAmount / baseRate);

		RainAudio.instance.SetHighRainVolume(t);
		rainEmitter.rateOverTimeMultiplier = t * baseRate;

		if (rainAmount >= baseRate)
		{
			finiteStateController.GoToNextState();
		}
	}
}
