using UnityEngine;

public class AnimationUtil
{
    public static bool IsPlayingAnimation(GameObject obj, string name)
    {
        Animation anim = obj.GetComponent<Animation>();
        return anim != null && anim.IsPlaying(name);
    }

    public static bool AnimationEnds(GameObject obj, string name)
	{
		if (obj.GetComponent<Animation>()[name].time >= obj.GetComponent<Animation>()[name].clip.length || obj.GetComponent<Animation>()[name].wrapMode == WrapMode.Loop)
		{
			return true;
		}
		return false;
	}

    public static bool IsAnimationPlayedPercentage(GameObject obj, string aniName, float percentage)
    {
        Animation anim = obj.GetComponent<Animation>();
        if (anim != null && anim[aniName] != null)
        {
            if (anim[aniName].time >= anim[aniName].clip.length * percentage)
            {
                return true;
            }
        }
        return false;
    }

    public static void PlayAnimate(GameObject obj, string animationName, WrapMode wrapMode)
	{
		if (obj.GetComponent<Animation>()[animationName] != null)
		{
			obj.GetComponent<Animation>()[animationName].wrapMode = wrapMode;
			obj.GetComponent<Animation>().Play(animationName);
		}
	}

	public static void CrossAnimate(GameObject obj, string animationName, WrapMode wrapMode)
	{
		if (obj.GetComponent<Animation>()[animationName] != null)
		{
			obj.GetComponent<Animation>()[animationName].wrapMode = wrapMode;
			obj.GetComponent<Animation>().CrossFade(animationName);
		}
	}

	public static void CrossAnimate(GameObject obj, string animationName, WrapMode wrapMode, float len)
	{
		if (obj.GetComponent<Animation>()[animationName] != null)
		{
			obj.GetComponent<Animation>()[animationName].wrapMode = wrapMode;
			obj.GetComponent<Animation>().CrossFade(animationName, len);
		}
	}

	public static void BlendAnimate(GameObject obj, string animationName, WrapMode wrapMode, float len = 0.3f)
	{
		if (obj.GetComponent<Animation>()[animationName] != null)
		{
			obj.GetComponent<Animation>()[animationName].wrapMode = wrapMode;
			obj.GetComponent<Animation>().Blend(animationName, 1f, len);
		}
	}

	public static void Stop(GameObject obj)
	{
		if (obj.GetComponent<Animation>() != null)
		{
			obj.GetComponent<Animation>().Stop();
		}
	}

	public static void Stop(GameObject obj, string animationName)
	{
		if (obj.GetComponent<Animation>() != null)
		{
			obj.GetComponent<Animation>().Stop(animationName);
		}
	}

	public static void SetAnimationSpeedWithLength(GameObject obj, string animationName, float length)
	{
		if (obj.GetComponent<Animation>()[animationName] != null)
		{
			obj.GetComponent<Animation>()[animationName].speed = obj.GetComponent<Animation>()[animationName].clip.length / length;
		}
	}
}
