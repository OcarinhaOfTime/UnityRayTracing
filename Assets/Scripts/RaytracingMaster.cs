using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RaytracingMaster : MonoBehaviour {
    public float refreshRate = .1f;
    public ComputeShader rayTracer;
    public Texture skyboxTexture;
    public Light directionalLight;
    private RenderTexture target;
    private Camera cam;

    private Material antiAliasMat;
    private uint currentSample = 0;

    private int ResultID = Shader.PropertyToID("Result");
    private int _CameraToWorldID = Shader.PropertyToID("_CameraToWorld");
    private int _CameraInverseProjectionID = Shader.PropertyToID("_CameraInverseProjection");
    private int _SkyboxTextureID = Shader.PropertyToID("_SkyboxTexture");
    private int _SampleID = Shader.PropertyToID("_Sample");
    private int _PixelOffsetID = Shader.PropertyToID("_PixelOffset");
    private int _DirectionalLightID = Shader.PropertyToID("_DirectionalLight");
    private int _SpheresID = Shader.PropertyToID("_Spheres");

    private float lastIntensity;
    private float timer = 0;

    private void Update() {
        if (transform.hasChanged) {
            currentSample = 0;
            transform.hasChanged = false;
        }

        if(timer > refreshRate) {
            timer = 0;
            transform.hasChanged = true;
        }

        timer += Time.deltaTime;
    }

    private void SetSpheresBuffer() {
        var rt_spheres = FindObjectsOfType<RayTracingSphere>();
        var spheres = new RayTracingSphere.Sphere[rt_spheres.Length];
        for(int i=0; i< rt_spheres.Length; i++) {
            spheres[i] = rt_spheres[i].sphere;
        }

        ComputeBuffer buffer = new ComputeBuffer(spheres.Length, System.Runtime.InteropServices.Marshal.SizeOf<RayTracingSphere.Sphere>());
        buffer.SetData(spheres);
        rayTracer.SetBuffer(0, _SpheresID, buffer);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        cam = GetComponent<Camera>();
        InitRenderTexture();

        rayTracer.SetTexture(0, ResultID, target);
        rayTracer.SetTexture(0, _SkyboxTextureID, skyboxTexture);
        rayTracer.SetMatrix(_CameraToWorldID, cam.cameraToWorldMatrix);
        rayTracer.SetMatrix(_CameraInverseProjectionID, cam.projectionMatrix.inverse);
        rayTracer.SetVector(_PixelOffsetID, new Vector2(Random.value, Random.value));
        Vector3 f = directionalLight.transform.forward;
        rayTracer.SetVector(_DirectionalLightID, new Vector4(f.x, f.y, f.z, directionalLight.intensity));

        SetSpheresBuffer();

        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8);
        rayTracer.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        if (antiAliasMat == null)
            antiAliasMat = new Material(Shader.Find("Hidden/AddShader"));

        antiAliasMat.SetFloat(_SampleID, currentSample);
        if(Application.isPlaying)
            Graphics.Blit(target, destination, antiAliasMat);
        else
            Graphics.Blit(target, destination);

        currentSample++;
    }

    private void InitRenderTexture() {
        if (target != null)
            return;

        target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        target.enableRandomWrite = true;
        target.Create();
    }
}
