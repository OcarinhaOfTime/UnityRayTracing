using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputeShaderTest : MonoBehaviour {
    public ComputeShader computeShader;
    private RawImage rawImage;

    private void Start() {
        rawImage = GetComponent<RawImage>();
        RunShader();
    }

    private void RunShader() {
        int kernelID = computeShader.FindKernel("CSMain");
        RenderTexture rt = new RenderTexture(256, 256, 24);
        rt.enableRandomWrite = true;
        rt.Create();
        computeShader.SetTexture(kernelID, "Result", rt);
        computeShader.Dispatch(kernelID, 256 / 8, 256 / 8, 1);
        rawImage.texture = rt;
    }
}
