using UnityEngine;
using UnityEngine.UI;

public class ShopHandler : MonoBehaviour
{
    public Text coins;
    public Animator shopScreenAnimator;

    public void OnShopButtonClick()
    {
        shopScreenAnimator.ResetTrigger("closePanel");
        shopScreenAnimator.SetTrigger("openPanel");
    }

    public void OnCloseButtonClick()
    {
        shopScreenAnimator.ResetTrigger("openPanel");
        shopScreenAnimator.SetTrigger("closePanel");
    }

    public void OnBuyCoinsPackageComplete(int coinsBought)
    {
        int numberOfCoins = PlayerPrefs.GetInt("coins");
        numberOfCoins += coinsBought;
        PlayerPrefs.SetInt("coins", numberOfCoins);
        coins.text = PlayerPrefs.GetInt("coins").ToString();
    }
}
