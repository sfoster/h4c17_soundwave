using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicVisualizer : MonoBehaviour {

	public float heightScalar = 1;

	float maxHeight;
	private LineRenderer lineRenderer;
    Vector3 beginPosition;
    Vector3 deltaPosition;
    Vector3 endPosition;

	int MAX_FREQUENCY = 2147483647;
	int upperFrequency;
	int lowerFrequency;

	bool captureHighFrequency = false;
	bool captureLowFrequency = false;

	void Start () 
	{
		upperFrequency = 0; 
		lowerFrequency = MAX_FREQUENCY;

		lineRenderer = GetComponent<LineRenderer>();
		MeshRenderer r = GetComponent<MeshRenderer>();
        Bounds b = r.bounds;

        maxHeight = b.extents.y;
        beginPosition = transform.position - transform.right * b.extents.x;
        beginPosition -= transform.up * maxHeight;
        endPosition = transform.position + transform.right * b.extents.x;
        endPosition -= transform.up * maxHeight;
        deltaPosition = endPosition - beginPosition;

		r.enabled = false;
        if (heightScalar <= 0) heightScalar = 1;

		MicMonitor.Instance.processNewMicrophoneFFT += PlotFrequency;
	}

	void Update ()
	{
		// reset calibration on arrow key down
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			upperFrequency = 0;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			lowerFrequency = MAX_FREQUENCY;
		}
		// update frequency calibration on next mic input
		if (Input.GetKey(KeyCode.UpArrow)) {
			Debug.Log ("Up key");
			captureHighFrequency = true;
		}
		if (Input.GetKey(KeyCode.DownArrow)) {
			Debug.Log ("Down key");
			captureLowFrequency = true;
		}
	}

	int getDominantFrequencyIndex(float[] buffer) 
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

	void PlotFrequency (float[] buffer) {
		if (captureHighFrequency) {
			int upperIndex = getDominantFrequencyIndex (buffer);
			if (upperIndex > upperFrequency) {
				upperFrequency = upperIndex;
				Debug.Log ("new upperFrequency: " + upperFrequency);
			}
			captureHighFrequency = false;
		}
		if (captureLowFrequency) {
			int lowerIndex = getDominantFrequencyIndex (buffer);
			if (lowerIndex < lowerFrequency) {
				lowerFrequency = lowerIndex;
				Debug.Log ("new lowerFrequency: " + lowerFrequency);
			}
			captureLowFrequency = false;
		}

		int freqIndex = getDominantFrequencyIndex(buffer);
		int freqRange = Math.Min(buffer.Length, upperFrequency - lowerFrequency);
		freqIndex = Math.Max(lowerFrequency, Math.Min(freqIndex, upperFrequency));
		// the frequency as a float within the calibrated upper/lower range
		float pcent = (float)(freqIndex - lowerFrequency) / freqRange;
		Debug.Log ("pcent: " + pcent);
		lineRenderer.positionCount = 2;
		lineRenderer.SetPosition (0, beginPosition);
		lineRenderer.SetPosition (1, beginPosition + deltaPosition*pcent);

		// Debug.Log(pcent);
	}

	void PlotAllFrequencies (float[] buffer)
	{
		// Debug.Log(buffer.Length);
		lineRenderer.positionCount = buffer.Length;
		float smoothing = 5.0f;

		float val = buffer[0];
		for (int i=0; i < buffer.Length; i++)
		{
			// basic smoothing
			float currentValue = buffer[i];
			val += (currentValue - val) / smoothing;

			float pct = (float)i / buffer.Length;
			float h = val * maxHeight * heightScalar;
			lineRenderer.SetPosition(i, beginPosition + deltaPosition * pct + (Vector3.up * h));
		}
	}
}
