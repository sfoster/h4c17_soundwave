using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_HighClouds : FiniteState 
{
	protected override void OnEnter()
	{
	}

	protected override void OnProcess ()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			finiteStateController.GoToNextState();
		}
	}

	protected override void OnExit ()
	{
	}
}
