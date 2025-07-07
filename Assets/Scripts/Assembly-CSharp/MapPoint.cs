using UnityEngine;

public class MapPoint : MonoBehaviour
{
	public Color Color;

	public int Size;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color;
		Gizmos.DrawSphere(base.transform.position, Size);
	}
}
