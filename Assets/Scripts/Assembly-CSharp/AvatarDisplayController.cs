using UnityEngine;

public class AvatarDisplayController : MonoBehaviour
{
    public UIShopPlayerController normalAvatar;
    public UIShopPlayerController strongAvatar;
    public UIShopPlayerController superAvatar;

    private UIShopPlayerController current; 

    public void ShowAvatar(AvatarData.AvatarState state)
    {
        // Disable all
        normalAvatar.gameObject.SetActive(false);
        strongAvatar.gameObject.SetActive(false);
        superAvatar.gameObject.SetActive(false);

        // Enable the chosen one
        switch (state)
        {
            case AvatarData.AvatarState.Normal:
                current = normalAvatar;
                break;
            case AvatarData.AvatarState.Strong:
                current = strongAvatar;
                break;
            case AvatarData.AvatarState.Super:
                current = superAvatar;
                break;
        }

        current.gameObject.SetActive(true);

        // update the shop’s pointer
        UIShopSceneController.Instance.CurrentAvatar = current;
    }

    public UIShopPlayerController GetCurrent()
    {
        return current;
    }
}
