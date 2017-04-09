using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicCalibrate : MonoBehaviour {

	private bool captureHighPitch = false;
	private bool captureLowPitch = false;

	private int? lowestPitch;
	private int? highestPitch;

	private bool captureHighLoudness = false;
	private bool captureLowLoudness = false;
	private int? lowestLoudness;
	private int? highestLoudness;

	void Update ()
	{
		// reset calibration on arrow key down
		if (Input.GetKeyDown(KeyCode.P)) {
			highestPitch = 0;
			lowestPitch = maxPitch;
			MicMonitor.Instance.processNewMicrophoneFFT += CapturePitch;
		}
		if (Input.GetKeyUp (KeyCode.P)) {
			MicMonitor.Instance.processNewMicrophoneFFT -= CapturePitch;
		}
		// update frequency calibration on next mic input
		if (Input.GetKey(KeyCode.P)) {
			Debug.Log ("Calibrate Pitch/Frequency");
			captureHighPitch = true;
			captureLowPitch = true;
		}

		// reset calibration on arrow key down
		if (Input.GetKeyDown(KeyCode.L)) {
			highestLoudness = 0;
			lowestLoudness = maxLoudness;
			MicMonitor.Instance.processNewMicrophoneRMS += PlotLoudness;
		}
		if (Input.GetKeyUp(KeyCode.L)) {
			MicMonitor.Instance.processNewMicrophoneRMS -= PlotLoudness;
		}
		// update loudness calibration on next mic input
		if (Input.GetKey(KeyCode.L)) {
			Debug.Log ("Calibrate Loudness");
			captureHighLoudness = true;
			captureLowLoudness = true;
		}
	}

	void CapturePitch (float[] buffer)
	{
		int pitch = getDominantFrequencyIndex(buffer);
		if (!highestPitch.HasValue || pitch > highestPitch) {
			highestPitch = pitch;
		}
		if (!lowestPitch.HasValue ||  pitch < lowestPitch) {
			lowestPitch = pitch;
		}
	}

}
