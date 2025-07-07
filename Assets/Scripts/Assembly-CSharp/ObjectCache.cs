using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCache : MonoBehaviour
{
	//public CrystalMono crystal;
	
	public static ObjectCache Instance;

	private void Awake()
	{
		Instance = this;
	}
}
