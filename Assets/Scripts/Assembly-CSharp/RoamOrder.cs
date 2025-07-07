using System;
using System.Collections.Generic;
using UnityEngine;

public class RoamOrder : MonoBehaviour
{
	[Serializable]
	public class RoamInfo
	{
		public CameraRoam cameraRoam;

		public float delaytime;

		public float lasttime;
	}

	public bool isAutoStart;

	public List<RoamInfo> roamlist;

	protected int count;

	protected bool isShow;

	protected CameraRoam curRoam;

	public void OnShow()
	{
		if (isShow)
		{
			OnEnd();
		}
		isShow = true;
		count = 0;
		Trigger();
		Next();
	}

	public void OnShow(Camera camera, bool isstart = false)
	{
		SetCamera(camera);
		if (isstart)
		{
			SetCameraStart(camera);
		}
		OnShow();
	}

	public void OnEnd()
	{
		if (curRoam != null)
		{
			curRoam.OnStop();
			curRoam = null;
		}
		CancelInvoke("Show");
		End();
	}

	private void Show()
	{
		RoamOrderEvent roamOrderEvent = roamlist[count].cameraRoam.gameObject.AddComponent<RoamOrderEvent>();
		roamOrderEvent.m_order = this;
		curRoam = roamlist[count].cameraRoam;
		roamlist[count].cameraRoam.OnRoam(roamlist[count].lasttime);
		count++;
	}

	public void Next()
	{
		curRoam = null;
		if (count < roamlist.Count)
		{
			Invoke("Show", roamlist[count].delaytime);
		}
		else
		{
			End();
		}
	}

	private void End()
	{
		isShow = false;
		count = 0;
		Stop();
	}

	private void Start()
	{
		if (isAutoStart)
		{
			OnShow();
		}
	}

	private void SetCamera(Camera camera)
	{
		foreach (RoamInfo item in roamlist)
		{
			item.cameraRoam.m_camera = camera;
		}
	}

	private void SetCameraStart(Camera camera)
	{
		CameraRoam cameraRoam = roamlist[0].cameraRoam;
		camera.transform.position = ((cameraRoam.cameraTrackPathType != 0) ? cameraRoam.cameraPath.GetEnd() : cameraRoam.cameraPath.GetStart());
	}

	protected void Trigger()
	{
		Component[] components = base.gameObject.GetComponents(typeof(IRoamEvent));
		Component[] array = components;
		foreach (Component component in array)
		{
			((IRoamEvent)component).OnRoamTrigger();
		}
	}

	protected void Stop()
	{
		Component[] components = base.gameObject.GetComponents(typeof(IRoamEvent));
		Component[] array = components;
		foreach (Component component in array)
		{
			((IRoamEvent)component).OnRoamStop();
		}
	}
}
