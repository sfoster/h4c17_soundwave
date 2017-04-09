using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_SlowLightning : FiniteState 
{
	public AudioLightningController lightning;
	public ScriptedMotion cameraMotion;
	public float moveDuration = 1;
	public float timeScale = 0.1f;

	protected override void OnEnter()
	{
		cameraMotion.MoveToEnd(moveDuration);
		lightning.Activate(moveDuration);
		Time.timeScale = timeScale;
	}

	protected override void OnProcess ()
	{
		moveDuration -= Time.deltaTime * Time.timeScale;
		if (moveDuration <= 0)
		{
			lightning.Deactivate();
			ScreenFader.instance.FadeInFromColor(Color.white, 1f);
			Time.timeScale = 1;
			finiteStateController.GoToNextState();
		}
	}

	protected override void OnExit ()
	{
	}
}
