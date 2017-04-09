using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicClap : MonoBehaviour {

	public MicMonitor micMonitor;

	public delegate void MicClapEvent ();
	public MicClapEvent OnMicClapEvent;

	private float lowerBounds;
	private float upperBounds;

	public void SetActive (bool isActive)
	{
		if (isActive)
		{
			micMonitor.processNewMicrophoneRMS += HandleNewMicrophoneLoudness;
		}
		else
		{
			micMonitor.processNewMicrophoneRMS -= HandleNewMicrophoneLoudness;
		}
	}

	public void SetLowerBounds (float lowerBounds)
	{
	}

	public void SetUpperBounds (float upperBounds)
	{
	}

	private void HandleNewMicrophoneLoudness (float loudness)
	{
		bool detectedClap = false;

		// process

		// Broadcast to delegates
		if (detectedClap && OnMicClapEvent != null)
		{
			OnMicClapEvent();
		}
	}
}
