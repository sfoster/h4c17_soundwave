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

	// linePoints is a list 256 of Vector3s we want to plot
	// each callback we check the frequencies we're passed. 
	// and create a point that represents frequency (index) and magnitude
	// push each point on the list. Shift off the oldest. 
	// then render the points, 

	void Start () 
	{
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

		MicMonitor.Instance.processNewMicrophoneFFT += RenderBuffer;

	}

	void RenderBuffer (float[] buffer)
	{
		RenderDominantFrequency (buffer);
	}

	void RenderAllFrequencies (float[] buffer)
	{
		Debug.Log(buffer.Length);
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
			lineRenderer.SetPosition(i, beginPosition + deltaPosition * pct + Vector3.up * h);
		}
	}

	void RenderDominantFrequency (float[] buffer)
	{
		lineRenderer.positionCount = buffer.Length;

		float highest = buffer[0];
		float lowest = buffer[0];
		int highIdx = 0;
		int lowIdx = 0;
		for (int i = 0; i < buffer.Length; i++) {
			if (buffer [i] > highest) {
				highest = buffer [i];
				highIdx = i;
			}
			if (buffer [i] < highest) {
				lowest = buffer [i];
				lowIdx = i;
			}
		}
		Debug.Log ("Highest: " + highest + ", Idx: " + highIdx);
		Debug.Log ("Lowest: " + lowest + ", LowIdx: " + lowIdx);

		float pct = (float)highest / buffer.Length;
		float h = highest * maxHeight * heightScalar;
		lineRenderer.SetPosition(0, beginPosition + deltaPosition * pct + Vector3.up * h);
	}
	 
}
