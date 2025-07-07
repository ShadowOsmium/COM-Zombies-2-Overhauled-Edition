using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
	public static EnemyController AddEnemyComponent(GameObject enemy, string typeName)
	{
		return enemy.AddComponent(System.Type.GetType(typeName)) as EnemyController;
	}

	public static WeaponController AddWeaponComponent(GameObject weapon, string typeName)
	{
		return weapon.AddComponent(System.Type.GetType(typeName)) as WeaponController;
	}

	public static NPCController AddNPCComponent(GameObject npc, string typeName)
	{
		return npc.AddComponent(System.Type.GetType(typeName)) as NPCController;
	}

	public static void AddComponent(GameObject target, string typeName)
	{
		target.AddComponent(System.Type.GetType(typeName));
	}
}
