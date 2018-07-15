using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class NaiveRbf : MonoBehaviour {
	
	[SerializeField]
	private Transform _dots;
	public GameObject BlueDot;
	public GameObject RedDot;
	public float Gamma;
	
	[DllImport("Unity_dll")]
	private static extern IntPtr trainNaiveRBF(float[] inputs, int inputsRows, int inputsCols, float[] outputs, int outputsRows, int outputsCols, float gamma);
	
	[DllImport("Unity_dll")]
	private static extern float naiveRBFRegression(float[] inputs, int inputsRows, int inputsCols, float[] predict, int predictRows, int predictCols, IntPtr weights, int weightsRows, int weightsCols, float gamma);

	private void Start () {
		var dotsInputs = new List<float>();
		var dotsTargets = new List<float>();
		
		for(var i = 0; i < _dots.childCount; i++)
		{
			
			dotsInputs.Add(item:  _dots.GetChild(i).localPosition.x);
			dotsInputs.Add(item:  _dots.GetChild(i).localPosition.z);

			dotsTargets.Add(item:  _dots.GetChild(i).CompareTag(tag: "red") ? 1.0f : -1.0f);
		}
		
		Debug.Log("dotsInputs shape = " + dotsInputs.Count);
		var str = "{ ";
		foreach (var v in dotsInputs)
		{
			str += v + "f, ";
		}
		str += "}";
		Debug.Log(str);
		str = "{ ";
		Debug.Log("dotsTargets shape = " + dotsTargets.Count);
		foreach (var v in dotsTargets)
		{
			str += v + "f, ";
		}
		str += "}";
		Debug.Log(str);
		
		
		//var phi = findPhi(dotsInputs.ToArray(), dotsInputs.Count / 2, 2, 0.1f);
		var weights = trainNaiveRBF(dotsInputs.ToArray(), dotsInputs.Count / 2, 2, dotsTargets.ToArray(), dotsTargets.Count, 1, Gamma);    
		
		//var pred = naiveRBFRegression(dotsInputs.ToArray(), dotsInputs.Count / 2, 2, predict, 1, 2, weights, dotsInputs.Count / 2, 2, 0.1f);
		
		for (var i = -10; i <= 10; i ++)
		{
			for (var j = -10; j <= 10; j ++)
			{
				var result = 0f;
				var valueToClassify = new float[] {i, j};
				result = naiveRBFRegression(dotsInputs.ToArray(), dotsInputs.Count / 2, 2, valueToClassify, 1, 2, weights, dotsInputs.Count / 2, 1, Gamma);	
				
				Instantiate(original: result > 0 ? RedDot : BlueDot, position: new Vector3(x: i, y: -1.0f, z: j), rotation: Quaternion.identity);
			}
		}
	}
}
