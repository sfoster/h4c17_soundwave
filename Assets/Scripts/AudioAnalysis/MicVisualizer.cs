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

		MicMonitor.Instance.processNewMicrophoneFFT += PlotFrequency;
	}

	void PlotFrequency (float[] buffer) {
		float highest = buffer[0];
		int highIdx = 0;
		for (int i = 0; i < buffer.Length; i++) {
			if (buffer [i] > highest) {
				highest = buffer [i];
				highIdx = i;
			}
		}
		float pcent = (float) highIdx / (float) buffer.Length;
		lineRenderer.positionCount = 2;
		lineRenderer.SetPosition (0, beginPosition);
		lineRenderer.SetPosition (1, beginPosition + deltaPosition*pcent);
		Debug.Log(pcent);
	}

	void PlotAllFrequencies (float[] buffer)
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
			lineRenderer.SetPosition(i, beginPosition + deltaPosition * pct + (Vector3.up * h));
		}
	}
}
