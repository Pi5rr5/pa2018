using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MainScript : MonoBehaviour
{

    [DllImport("Unity_dll")]
    private static extern IntPtr returnArray();
    
    [DllImport("Unity_dll")]
    private static extern double returnArraySum(double[] array, int size);
    
    [DllImport("Unity_dll")]
    private static extern void releaseArray(IntPtr array);

    private void Start()
    {
        double[] managedArray = { 10,10,10,10 };
        Debug.Log(returnArraySum(managedArray,4));
    }
}