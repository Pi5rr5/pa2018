using System.Runtime.InteropServices;
using UnityEngine;

public class MainScript : MonoBehaviour
{

    [DllImport("Unity_dll")]
    private static extern int mult(int a, int b);

    private void Start()
    {
        Debug.Log(mult(8, 10));
    }
}