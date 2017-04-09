﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicPitch : MonoBehaviour {

	public MicMonitor micMonitor;

	private float normalizedPitch;
	private int? lowerBounds;
	private int? upperBounds;
	private int? maxFrequency; 

	// returns a value of zero to one
	public float GetNormalizedPitch ()
	{
		return normalizedPitch;
	}
	public bool HasNormalizedPitch = false;

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

	public void SetLowerBounds (int lBounds)
	{
		lowerBounds = lBounds;
	}


	public void SetUpperBounds (int uBounds)
	{
		upperBounds = uBounds;
	}

	private void HandleNewMicrophoneFFT (float[] buffer)
	{
		if (!maxFrequency.HasValue) {
			maxFrequency = buffer.Length;
		}
		if (lowerBounds.HasValue && upperBounds.HasValue) {
			// determine what the loudest frequency is
			int loudestFrequency = getDominantFrequencyIndex(buffer);
			normalizedPitch = Mathf.Clamp01((upperBounds.Value - loudestFrequency.Value) / (upperBounds.Value - lowerBounds.Value));
			HasNormalizedPitch = true;
		}
	}

	private int getDominantFrequencyIndex(float[] buffer)
	{
		float max = buffer[0];
		int index = 0;
		for (int i = 0; i < buffer.Length; i++) {
			if (buffer [i] > max) {
				max = buffer [i];
				index = i;
			}
		}
		return index;
	}

}
