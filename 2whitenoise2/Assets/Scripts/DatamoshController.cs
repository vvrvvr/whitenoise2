using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class DatamoshController : MonoBehaviour
{
	public UniversalRendererData Renderer;

	private Datamosh _datamosh;
	private bool isDatamoshEnabled = false;

	void Start()
	{
		_datamosh = (Datamosh) Renderer.rendererFeatures.Find(x => x is Datamosh);
		
		
	}

	private void Update()
	{
		_datamosh.Enabled = isDatamoshEnabled;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			isDatamoshEnabled = !isDatamoshEnabled;
		}
		
	}

	
}
