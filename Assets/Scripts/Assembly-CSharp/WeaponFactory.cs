using UnityEngine;

public class WeaponFactory : MonoBehaviour
{
	public static WeaponController CreateWeapon(string weapon_name)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/" + weapon_name)) as GameObject;
		GameObject gameObject2 = Object.Instantiate(gameObject.GetComponent<SinglePrefabReference>().Instance) as GameObject;
		WeaponController weaponController = Utility.AddWeaponComponent(gameObject2, GetWeaponTypeControllerName(GameData.Instance.WeaponData_Set[weapon_name].weapon_type));
		weaponController.ori_pos = gameObject2.transform.position;
		weaponController.SetWeaponData(GameData.Instance.WeaponData_Set[weapon_name]);
		weaponController.Accessory = gameObject.GetComponent<SinglePrefabReference>().Accessory;
		Object.Destroy(gameObject);
		return weaponController;
	}

	public static WeaponController CreateWeaponCoop(string weapon_name)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/" + weapon_name)) as GameObject;
		GameObject gameObject2 = Object.Instantiate(gameObject.GetComponent<SinglePrefabReference>().Instance) as GameObject;
		WeaponController weaponController = Utility.AddWeaponComponent(gameObject2, GetCoopWeaponTypeControllerName(GameData.Instance.WeaponData_Set[weapon_name].weapon_type));
		weaponController.ori_pos = gameObject2.transform.position;
		weaponController.SetWeaponData(GameData.Instance.WeaponData_Set[weapon_name]);
		weaponController.Accessory = gameObject.GetComponent<SinglePrefabReference>().Accessory;
		Object.Destroy(gameObject);
		return weaponController;
	}

	public static string GetWeaponTypeControllerName(WeaponType type)
	{
		string result = "WeaponController";
		switch (type)
		{
		case WeaponType.AssaultRifle:
			result = "AssaultRifleController";
			break;
		case WeaponType.Saw:
			result = "ChaisawController";
			break;
		case WeaponType.Baseball:
			result = "BaseballController";
			break;
		case WeaponType.ShotGun:
			result = "ShotGunController";
			break;
		case WeaponType.RocketLauncher:
			result = "RPGController";
			break;
		case WeaponType.PGM:
			result = "PGMController";
			break;
		case WeaponType.Gatling:
			result = "GatlingController";
			break;
		case WeaponType.Pistol:
			result = "PistolController";
			break;
		case WeaponType.Flame:
			result = "FlameController";
			break;
		case WeaponType.Mines:
			result = "MinesController";
			break;
		case WeaponType.Medicine:
			result = "MedicineController";
			break;
		case WeaponType.Shield:
			result = "ShieldController";
			break;
		case WeaponType.Laser:
			result = "LasergunController";
			break;
		case WeaponType.IonCannon:
			result = "IonCannonController";
			break;
		case WeaponType.M32:
			result = "M32Controller";
			break;
		case WeaponType.IceGun:
			result = "IceGunController";
			break;
		}
		return result;
	}

	public static string GetCoopWeaponTypeControllerName(WeaponType type)
	{
		string result = "WeaponCoopController";
		switch (type)
		{
		case WeaponType.AssaultRifle:
			result = "AssaultRifleCoopController";
			break;
		case WeaponType.ShotGun:
			result = "ShotGunCoopController";
			break;
		case WeaponType.RocketLauncher:
			result = "RPGCoopController";
			break;
		case WeaponType.PGM:
			result = "PGMCoopController";
			break;
		case WeaponType.Gatling:
			result = "GatlingCoopController";
			break;
		case WeaponType.Laser:
			result = "LasergunCoopController";
			break;
		case WeaponType.IonCannon:
			result = "IonCannonCoopController";
			break;
		case WeaponType.M32:
			result = "M32CoopController";
			break;
		case WeaponType.IceGun:
			result = "IceGunCoopController";
			break;
		}
		return result;
	}
}
