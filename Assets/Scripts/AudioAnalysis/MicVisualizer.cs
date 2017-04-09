﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicVisualizer : MonoBehaviour {

	public float heightScalar = 1;

	float maxHeight;
	private LineRenderer frequencyLineRenderer;
	private LineRenderer loudnessLineRenderer;
	Vector3 beginPosition;
	Vector3 deltaPosition;
	Vector3 endPosition;

	private bool captureHighFrequency = false;
	private bool captureLowFrequency = false;

	private float? maxLoudness = 1.0f;
	private float? upperLoudness;
	private float? lowerLoudness;
	private float loudnessRange = 0.0f;

	private bool captureHighLoudness = false;
	private bool captureLowLoudness = false;

	private int loudnessChangeDirection = 0;
	private float? loudnessAtEdge;
	const float peakThreshold = 0.5f; // %age of the range considered a "peak"

	private LinkedList<float> loudnessSamples = new LinkedList<float>();

	void Start ()
	{
		frequencyLineRenderer = GetComponent<LineRenderer>();
		loudnessLineRenderer = GetComponent<LineRenderer>();
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

		// MicMonitor.Instance.processNewMicrophoneFFT += PlotFrequency;
		MicMonitor.Instance.processNewMicrophoneRMS += PlotLoudness;
	}

	float clamp(float value, float lbound, float ubound) {
		return Math.Max((float)lbound, Math.Min(value, (float)ubound));
	}

	int clamp(int value, int lbound, int ubound) {
		return Math.Max((int)lbound, Mathf.Min(value, (int)ubound));
	}

	float meanAverage(LinkedList<float> floatList)
	{
		float sum = 0;
		foreach (float value in floatList)
		{
			sum += value;
		}
		return sum / floatList.Count;
	}

	private float getStandardDeviation(LinkedList<float> floatList)
	{
		float average = meanAverage(floatList);
		float sumOfDerivation = 0;
		foreach (float value in floatList)
		{
			sumOfDerivation += (value) * (value);
		}
		float sumOfDerivationAverage = sumOfDerivation / (float)(floatList.Count - 1);
		return (float)Math.Sqrt(sumOfDerivationAverage - (average*average));
	}

	float smooth(float value, LinkedList<float> list, float smoothing) {
		float previous;
		if (list.Count > 0) {
			previous = list.Last.Value;
		} else {
			previous = value;
		}
		return (value - previous) / smoothing;
	}


	void UpdateLoudnessSamples(float loudness) {
		if (loudnessSamples.Count >= 16) {
			loudnessSamples.RemoveFirst();
		}
		// could average the last n samples
		// use some change threshold? or rely on the smoothing we already did
		float previous = loudnessSamples.Count > 0 ? loudnessSamples.Last.Value : loudness;
		float delta = loudness - previous;
		float significantChange = 0.05f;
		int previousDirection = loudnessChangeDirection;

		loudnessSamples.AddLast(loudness);

		// detect rising edge, falling edge.
		if (Math.Abs (delta) >= significantChange) {
			if (loudness > previous) {
				if (loudnessChangeDirection <= 0) {
					// was falling/static, now rising
					Debug.Log ("UpdateLoudnessSamples: Rising loudness: previous " + previous + ", current: " + loudness);
					loudnessChangeDirection = 1;
				}
			}
			else if (loudness < previous) {
				if (loudnessChangeDirection >= 0) {
					Debug.Log("UpdateLoudnessSamples: Falling loudness: previous " + previous + ", current: " + loudness);
					loudnessChangeDirection = -1;
				}
			}
		} else {
			Debug.Log ("Loudness is stable: " + loudness + ", delta: " + delta);
		}
		if (Input.GetKey(KeyCode.S)) {
			Debug.Log("UpdateLoudnessSamples: " +
				", loudness: " + loudness +
				", previous: " + previous +
				", delta: " + delta +
				", loudnessChangeDirection: " + loudnessChangeDirection +
				", Count: " + loudnessSamples.Count
			);
		}
		if (previousDirection != loudnessChangeDirection) {
			if (loudnessChangeDirection < 0 && loudnessAtEdge.HasValue) {
				// falling edge, was this a significant peak?
				float peakMagnitude = loudness - loudnessAtEdge.Value;
				float deviation = getStandardDeviation (loudnessSamples);
				if (peakMagnitude >= peakThreshold) {
					Debug.Log ("UpdateLoudnessSamples, detected a peak: " + peakMagnitude +
					", deviation: " + deviation);
				} else {
					Debug.Log ("UpdateLoudnessSamples, not a peak: " + peakMagnitude +
						", deviation: " + deviation);
				}
			}
			loudnessAtEdge = loudness;
		}

		#if false
		DebugLog("UpdateLoudnesSamples" +
			", loudness: " + loudness +
			", Count: " + loudnessSamples.Count +
			", deviation: " + deviation);
		#endif
	}

	void PlotLoudness (float meanLoudness) {
		if (captureHighLoudness) {
			if (meanLoudness > upperLoudness) {
				upperLoudness = meanLoudness;
				Debug.Log ("new upperLoudness: " + upperLoudness);
			}
			captureHighLoudness = false;
		}
		if (captureLowLoudness) {
			if (meanLoudness < lowerLoudness) {
				lowerLoudness = meanLoudness;
				Debug.Log ("new lowerLoudness: " + lowerLoudness);
			}
			captureLowLoudness = false;
		}

		if (!upperLoudness.HasValue) {
			upperLoudness = 1;
		}
		if (!lowerLoudness.HasValue) {
			lowerLoudness = 0;
		}
		loudnessRange = Math.Max(0.01f, (float)upperLoudness - (float)lowerLoudness);

		float loudness = clamp(meanLoudness, (float)lowerLoudness, (float)upperLoudness);
		float loudnessPcent = (loudness - lowerLoudness.Value) / loudnessRange;
		// the frequency as a float within the calibrated upper/lower range
		float smoothedLoudness = smooth (loudnessPcent, loudnessSamples, 1.5f);
		#if false
		Debug.Log ("Check: " +
			", meanLoudness: " + meanLoudness +
			", loudness: " + loudness +
			", loudnessPcent: " + loudnessPcent +
			", smoothedLoudness: " + smoothedLoudness +
			", loudnessRange: " + loudnessRange +
			", lowerLoudness: " + (float)lowerLoudness +
			", upperLoudness: " + (float)upperLoudness
		);
		#endif
		// Debug.Log ("meanLoudness: " + meanLoudness + ", loudness: " + loudness + ", pcent: " + pcent);
		loudnessLineRenderer.positionCount = 2;
		loudnessLineRenderer.SetPosition (0, beginPosition);
		Debug.Log ("beginPosition: " + beginPosition.ToString ());
		// Debug.Log ("end position: " + (beginPosition + deltaPosition*pcent).ToString());
		loudnessLineRenderer.SetPosition (1, beginPosition + deltaPosition*smoothedLoudness);

		UpdateLoudnessSamples(smoothedLoudness);

	}

	void PlotFrequency (float[] buffer) {
		if (!maxFrequency.HasValue) {
			maxFrequency = buffer.Length;
		}
		if (captureHighFrequency) {
			int upperIndex = getDominantFrequencyIndex (buffer);
			if (upperIndex > (int)upperFrequency) {
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

		if (!upperFrequency.HasValue) {
			upperFrequency = buffer.Length;
		}
		if (!lowerFrequency.HasValue) {
			lowerFrequency = 0;
		}

		int freqIndex = getDominantFrequencyIndex(buffer);
		int freqRange = clamp((int)upperFrequency - (int)lowerFrequency, 1, buffer.Length);
		//Debug.Log ("freqIndex: " + freqIndex + ", freqRange: " + freqRange + ", lowerFrequenct: " + (int)lowerFrequency + ", upperFrequency: " + (int)upperFrequency);
		freqIndex = clamp(freqIndex, (int)lowerFrequency, (int)upperFrequency);

		// the frequency as a float within the calibrated upper/lower range
		float pcent = (float)(freqIndex - (int)lowerFrequency) / freqRange;
		//Debug.Log ("frequency pcent: " + pcent);
		frequencyLineRenderer.positionCount = 2;
		frequencyLineRenderer.SetPosition (0, beginPosition);
		frequencyLineRenderer.SetPosition (1, beginPosition + deltaPosition*pcent);

		// Debug.Log(pcent);
	}

	void PlotAllFrequencies (float[] buffer)
	{
		// Debug.Log(buffer.Length);
		frequencyLineRenderer.positionCount = buffer.Length;
		float smoothing = 5.0f;

		float val = buffer[0];
		for (int i=0; i < buffer.Length; i++)
		{
			// basic smoothing
			float currentValue = buffer[i];
			val += (currentValue - val) / smoothing;

			float pct = (float)i / buffer.Length;
			float h = val * maxHeight * heightScalar;
			frequencyLineRenderer.SetPosition(i, beginPosition + deltaPosition * pct + (Vector3.up * h));
		}
	}
}
