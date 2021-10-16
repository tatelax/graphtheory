using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Graph
{
	private class Node
	{
		public readonly GameObject VertexView;
		public EdgeView EdgeView;
		
		public Node(Vector3 position)
		{
			VertexView = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			VertexView.transform.position = position;
		}

		public void Destroy()
		{
			Object.Destroy(EdgeView.gameObject);
			Object.Destroy(VertexView);
		}
	}

	private readonly Dictionary<int, Node> vertexViews;
	private readonly Dictionary<int, LinkedList<int>> vertices; // key = value of the vertex, value = connected vertices

	private readonly GameObject edgePrefab;
	
	public Graph(GameObject _edgePrefab)
	{
		edgePrefab = _edgePrefab;
		vertices = new Dictionary<int, LinkedList<int>>();
		vertexViews = new Dictionary<int, Node>();
	}

	public void AddVertex(int value, Vector3 position = new Vector3())
	{
		if (vertices.ContainsKey(value))
		{
			Debug.LogWarning($"Vertex {value} already exists.");
			return;
		}
		
		vertices.Add(value, new LinkedList<int>());
		vertexViews.Add(value, new Node(position));
	}

	public void AddEdge(int u, int v)
	{
		vertices[u].AddLast(v);
		vertices[v].AddLast(u);
		
		CreateEdgeView(u, v);
	}

	public void RemoveVertex(int vertex)
	{
		if (!vertices.ContainsKey(vertex))
		{
			Debug.LogWarning($"Tried to remove {vertex}, but it doesn't exist.");
			return;
		}
		
		// Remove vertex from all of the things that vertex is connected to
		foreach (int i in vertices[vertex])
		{
			vertices[i].Remove(vertex);
		}
		
		// Remove the vertex itself
		vertices.Remove(vertex);
		
		// Destroy the vertex gameObject and its edgeview if it has one
		vertexViews[vertex].Destroy();
		vertexViews.Remove(vertex);
		
		Print();
	}

	private void CreateEdgeView(int start, int end)
	{
		EdgeView newEdge = Object.Instantiate(edgePrefab).GetComponent<EdgeView>();

		Vector3 startVertexPos = vertexViews[start].VertexView.transform.position;
		Vector3 endVertexPos = vertexViews[end].VertexView.transform.position;

		vertexViews[start].EdgeView = newEdge;
		vertexViews[end].EdgeView = newEdge;
		
		// Sets the start and end vertices of the line renderer so we can see it
		newEdge.Init(startVertexPos, endVertexPos);
	}

	public void Print()
	{
		foreach (KeyValuePair<int,LinkedList<int>> vertex in vertices)
		{
			StringBuilder output = new StringBuilder();

			output.Append($"[{vertex.Key}]: ");
			
			foreach (int i in vertex.Value)
			{
				output.Append($"-> {i} ");
			}

			Debug.Log(output);
		}
		
		Debug.Log("========== END OUTPUT ==========");
	}
}
