using UnityEngine;

public class UILotterSeat : MonoBehaviour
{
	public TUIMeshSprite seat_light;

	public int award_level;

	public int seat_lottery_weight;

	public GameAwardItem lottery_award;

	public TUIMeshSprite award_sprite;

	public TUIMeshSprite award_sprite_bk;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetLightState(bool state)
	{
		seat_light.gameObject.SetActive(state);
	}

    public void SetLotteryAward(GameAwardItem award)
    {
        lottery_award = award;
        if (award_sprite == null)
        {
            Debug.LogWarning("award_sprite is null in UILotterSeat: " + gameObject.name);
            return;
        }
        award_sprite.texture = lottery_award.award_name;

        if (lottery_award.award_type == GameAwardItem.AwardType.WeaponFragment)
        {
            award_sprite_bk.gameObject.SetActive(true);
        }
        else
        {
            award_sprite_bk.gameObject.SetActive(false);
        }
    }
}
