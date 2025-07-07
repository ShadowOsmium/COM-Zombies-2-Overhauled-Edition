using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
	public enum PathShowType
	{
		TDPosition,
		SphereCap
	}

	public string pathName = string.Empty;

	public Color pathColor = Color.cyan;

	public List<Vector3> nodes = new List<Vector3>
	{
		Vector3.zero,
		Vector3.zero
	};

	public int nodeCount;

	public InterpAlgorithm.InterpType type;

	public bool initialized;

	public string initialName = string.Empty;

	public PathShowType showtype;

	public Vector3 UnLocalModify(Vector3 node)
	{
		return node - base.transform.position;
	}

	public Vector3 LocalModify(Vector3 node)
	{
		return node + base.transform.position;
	}

	public List<Vector3> LocalModify(List<Vector3> nodes)
	{
		List<Vector3> list = new List<Vector3>();
		foreach (Vector3 node in nodes)
		{
			list.Add(node + base.transform.position);
		}
		return list;
	}

	private void OnDrawGizmosSelected()
	{
		if (base.enabled && nodes.Count > 0)
		{
			PathMaker.DrawPath(type, LocalModify(nodes).ToArray(), pathColor);
		}
	}

	public Vector3 GetStart()
	{
		return LocalModify(nodes[0]);
	}

	public Vector3 GetEnd()
	{
		return LocalModify(nodes[nodes.Count - 1]);
	}
}
