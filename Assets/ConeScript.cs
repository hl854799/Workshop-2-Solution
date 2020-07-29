// Solution for Lab 2, Q5 (challenge)
// Script to procedurally generate a cone mesh
// Created July 2017 by Alex Zable for COMP30019
// Last modified July 2020 by Martin Reinoso for COMP30019 (OnValidate to have intertive change)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeScript : MonoBehaviour {

	public int numBaseVertices = 20;
	public float radius = 1.5f;
	public float height = 3.0f;

	//Variables to have references to the componets MeshFilter and MeshRender
	private MeshFilter cubeMesh;
	private MeshRenderer renderer;

	// Use this for initialization
	void Start()
	{
		// Add a MeshFilter component to this entity. This essentially comprises of a
		// mesh definition, which in this example is a collection of vertices, colours 
		// and triangles (groups of three vertices). 
		cubeMesh = gameObject.AddComponent<MeshFilter>();
		cubeMesh.mesh = CreateConeMesh();

		// Add a MeshRenderer component. This component actually renders the mesh that
		// is defined by the MeshFilter component.
		renderer = this.gameObject.AddComponent<MeshRenderer>();
		renderer.material.shader = Shader.Find("Unlit/VertexColorShader");
	}

	//	This function is called when the script is loaded or a value is changed in the 
	//  inspector (Called in the editor only).
	void OnValidate()
     {
         if(cubeMesh == null){return;}
         cubeMesh.mesh = CreateConeMesh();
     }

    // Method to procedurally generate the cone.
    // Note that there is more than one way to do this - i.e. the base-center
    // vertex defined below is not required but is used for simplicity's sake.
	Mesh CreateConeMesh()
	{
		Mesh m = new Mesh();
		m.name = "Cone";

		List<Vector3> vertices = new List<Vector3> ();
		List<Color> colors = new List<Color> ();
		List<int> triangles = new List<int> ();

		// Define the circular edge vertices
		for (int i = 0; i < numBaseVertices; i++) 
		{
            float fraction = (float)i / numBaseVertices;
			float angle = fraction * Mathf.PI * 2.0f;
			vertices.Add(new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle)) * radius);
			colors.Add(Color.HSVToRGB(fraction, 1.0f, 1.0f)); // Vary hue around edge to create rainbow effect
		}

		// Base-center and tip vertices
		vertices.Add(new Vector3 (0.0f, 0.0f, 0.0f));
		vertices.Add(new Vector3 (0.0f, height, 0.0f));
		colors.Add(Color.black);
		colors.Add(Color.black);

		// Define the triangles -
        // Unlike in the cube/pyramid scripts, trangles here are sharing vertices,
        // so we aren't simply setting the triangles array to 0,1,2...|V|-1
		int vBaseCenter = vertices.Count - 2;
		int vTip = vertices.Count - 1;
		for (int i = 0; i < numBaseVertices; i++) 
		{
			int v1 = i;
			int v2 = (i + 1) % numBaseVertices;

			// Base triangle
			triangles.Add(vBaseCenter);
			triangles.Add(v2);
			triangles.Add(v1);

			// Side triangle
			triangles.Add(vTip);
			triangles.Add(v1); 
			triangles.Add(v2);
		}

		m.vertices = vertices.ToArray();
		m.colors = colors.ToArray();
		m.triangles = triangles.ToArray();

		return m;
	}
}
