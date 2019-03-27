using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracingSphere : MonoBehaviour {
    public struct Sphere {
        public Vector3 position;
        public float radius;
        public Vector3 albedo;
        public Vector3 specular;
    }

    public Color albedo = Color.white;
    public Color specular = Color.gray;
    public float radius = 1;

    public Sphere sphere {
        get {
            Sphere s = new Sphere();
            s.position = transform.position;
            s.radius = radius;
            s.albedo = new Vector3(albedo.r, albedo.g, albedo.b);
            s.specular = new Vector3(specular.r, specular.g, specular.b);
            return s;
        }
    }
}
