using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicPitch : MonoBehaviour {

	public MicMonitor micMonitor;

	private float normalizedPitch;
	private float lowerBounds;
	private float upperBounds;

	// returns a value of zero to one
	public float GetNormalizedPitch ()
	{
		return normalizedPitch;
	}

	public void SetActive (bool isActive)
	{
		if (isActive)
		{
			micMonitor.processNewMicrophoneFFT += HandleNewMicrophoneFFT;
		}
		else
		{
			micMonitor.processNewMicrophoneFFT -= HandleNewMicrophoneFFT;
		}
	}

	public void SetLowerBounds (float lowerBounds)
	{
	}

	public void SetUpperBounds (float upperBounds)
	{
	}

	private void HandleNewMicrophoneFFT (float[] buffer)
	{
		// determine what the loudest frequency is
		float loudestFrequency = 0;

		normalizedPitch = Mathf.Clamp01((upperBounds - loudestFrequency) / (upperBounds - lowerBounds));
	}
}
