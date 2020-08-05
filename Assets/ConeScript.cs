// Solution for Lab 2, Q5 (challenge)
// Script to procedurally generate a cone mesh
// Created July 2017 by Alex Zable for COMP30019
// Last modified July 2020 by Martin Reinoso for COMP30019 (OnValidate to have intertive change)

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ConeScript : MonoBehaviour {

	[Range(3, 100)] public int numBaseVertices = 20; //Range shows a slider in the inspector for the public variable
	[Range(1.5f, 100)] public float radius = 1.5f;
	[Range(3.0f, 100)] public float height = 3.0f;
	public bool shareVertices = true;
	[SerializeField] private int numberOfVertices; //SerializeField Makes private variables visible in the inspector (but not editable)
	[SerializeField] private int numberOfTriangles;

	//Variables to have references to the componets MeshFilter and MeshRender
	private MeshFilter coneMesh;
	private MeshRenderer renderer;

	// Use this for initialization
	void Start()
	{
		// Add a MeshFilter component to this entity. This essentially comprises of a
		// mesh definition, which in this example is a collection of vertices, colours 
		// and triangles (groups of three vertices). 
		coneMesh = gameObject.AddComponent<MeshFilter>();
		coneMesh.mesh = CreateConeMesh();

		// Add a MeshRenderer component. This component actually renders the mesh that
		// is defined by the MeshFilter component.
		renderer = this.gameObject.AddComponent<MeshRenderer>();
		renderer.material.shader = Shader.Find("Unlit/VertexColorShader");
	}

	//	This function is called when the script is loaded or a value is changed in the 
	//  inspector (Called in the editor only).
	void OnValidate()
     {
        if(coneMesh == null){return;}
        coneMesh.mesh = CreateConeMesh();

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
		// Two approaches: They are both right but which one is more efficient?
		for (int i = 0; i < numBaseVertices; i++) 
		{
			if (shareVertices) // Each triangle uses the same base and tip vertices
			{
				float fraction = (float)i / numBaseVertices;
				float angle = fraction * Mathf.PI * 2.0f;
				vertices.Add(new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle)) * radius);
				colors.Add(Color.HSVToRGB(fraction, 1.0f, 1.0f)); // Vary hue around edge to create rainbow effect
			}
            else // Multiplied vertices (base and tip vertices are added for each triangle)
			{
				// Slightly different approach, we calculate the size of the PI slice according to the number of vertices
				// We then calculate the cos/sin and add it to the vertex list
				float angle1 = ((2 * Mathf.PI) / numBaseVertices) * ((float)i);
				float angle2 = ((2 * Mathf.PI) / numBaseVertices) * ((float)(i + 1));

				// Makre sure it's in clock-wise order
				vertices.Add(new Vector3(Mathf.Sin(angle1) * radius, 0.0f, Mathf.Cos(angle1) * radius)); // v1
				vertices.Add(new Vector3(0.0f, 0.0f, 0.0f)); // Base vertex
				vertices.Add(new Vector3(Mathf.Sin(angle2) * radius, 0.0f, Mathf.Cos(angle2) * radius)); // v2
				colors.Add(Color.HSVToRGB(((float)i) / numBaseVertices, 1.0f, 1.0f));
				colors.Add(Color.black);
				colors.Add(Color.HSVToRGB(((float)i+1) / numBaseVertices, 1.0f, 1.0f));

				// Makre sure it's in clock-wise order
				vertices.Add(new Vector3(0.0f, height, 0.0f)); // Tip vertex
				vertices.Add(new Vector3(Mathf.Sin(angle1) * radius, 0.0f, Mathf.Cos(angle1) * radius)); // v1
				vertices.Add(new Vector3(Mathf.Sin(angle2) * radius, 0.0f, Mathf.Cos(angle2) * radius)); // v2
				colors.Add(Color.black);
				colors.Add(Color.HSVToRGB(((float)i) / numBaseVertices, 1.0f, 1.0f));
				colors.Add(Color.HSVToRGB(((float)i+1) / numBaseVertices, 1.0f, 1.0f));
			}
		}

		// Define the triangles -
		if (shareVertices)
		{
			// Base-center and tip vertices
			vertices.Add(new Vector3(0.0f, 0.0f, 0.0f));
			vertices.Add(new Vector3(0.0f, height, 0.0f));
			colors.Add(Color.black);
			colors.Add(Color.black);

			// If we are sharing vertices, we need to be mindful in the way we add the vertices to the triangle list
			// We can't simply set the triangles array to 0,1,2...|V|-1
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
		}
		else
		{
			// If we are not sharing vertices, we can refer to the cube/pyramid scrip and add each vertex index to the list
			// We had already added them in order
			for (int i = 0; i < vertices.Count; i++)
				triangles.Add(i);
		}

		m.vertices = vertices.ToArray();
		m.colors = colors.ToArray();
		m.triangles = triangles.ToArray();

		numberOfTriangles = m.triangles.Length/3;
		numberOfVertices = m.vertices.Length;
		return m;
	}
}
