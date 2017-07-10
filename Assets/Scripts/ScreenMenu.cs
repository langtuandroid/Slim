﻿using DoozyUI;
using UnityEngine;

public class ScreenMenu : MonoBehaviour
{
    private bool _isBtnSettingsClicked;
    private bool _isButtonHiden;

    private bool _isShowBtnViveoAds;
    private bool _isWaitReward;
    private AudioClip _sndStart;
    public GameObject coin;

    public AudioClip sndBtnClick;
    public UIButton videoAdsButton;

    private void Start()
    {
        _sndStart = Resources.Load<AudioClip>("snd/start");
        Defs.PlaySound(_sndStart);

        showButtons();
    }

    private void OnEnable()
    {
        ScreenGame.OnShowMenu += ScreenGame_OnShowMenu;
        
        GlobalEvents<OnStartGame>.Happened += ScreenGame_OnHideMenu;
        GlobalEvents<OnGiveReward>.Happened += GetReward;
        GlobalEvents<OnRewardedAvailable>.Happened += IsRewardedVideoAvailable;
    }

    private void OnDisable()
    {
        ScreenGame.OnShowMenu -= ScreenGame_OnShowMenu;
        
        GlobalEvents<OnStartGame>.Happened += ScreenGame_OnHideMenu;
        GlobalEvents<OnGiveReward>.Happened -= GetReward;
        GlobalEvents<OnRewardedAvailable>.Happened -= IsRewardedVideoAvailable;
    }

    private void IsRewardedVideoAvailable(OnRewardedAvailable e)
    {
        _isShowBtnViveoAds = e.isAvailable;
        if (_isShowBtnViveoAds)
        {
            if (DefsGame.CurrentScreen == DefsGame.SCREEN_MENU)
            {
                UIManager.ShowUiElement("BtnVideoAds");
                FlurryEventsManager.SendEvent("RV_strawberries_impression", "start_screen");
            }
        }
        else
        {
            UIManager.HideUiElement("BtnVideoAds");
        }
    }

    private void ScreenGame_OnShowMenu()
    {
        showButtons();
    }

    private void ScreenGame_OnHideMenu(OnStartGame e)
    {
        hideButtons();
    }

    public void showButtons()
    {
        _isButtonHiden = false;

        FlurryEventsManager.SendStartEvent("start_screen_length");

        //UIManager.ShowUiElement ("MainMenu");
        UIManager.ShowUiElement("elementBestScore");
        UIManager.ShowUiElement("elementCoins");
        UIManager.ShowUiElement("BtnSkins");
        FlurryEventsManager.SendEvent("candy_shop_impression");
        if (_isShowBtnViveoAds)
        {
            UIManager.ShowUiElement("BtnVideoAds");
            FlurryEventsManager.SendEvent("RV_strawberries_impression", "start_screen");
        }
        UIManager.ShowUiElement("BtnMoreGames");
        UIManager.ShowUiElement("BtnSound");
        UIManager.ShowUiElement("BtnRate");
        FlurryEventsManager.SendEvent("rate_us_impression", "start_screen");
        UIManager.ShowUiElement("BtnLeaderboard");
        UIManager.ShowUiElement("BtnAchievements");
#if UNITY_ANDROID || UNITY_EDITOR
        UIManager.ShowUiElement("BtnGameServices");
#endif
        UIManager.ShowUiElement("BtnMoreGames");
        UIManager.ShowUiElement("BtnShare");
        UIManager.ShowUiElement("BtnPlus");
        FlurryEventsManager.SendEvent("iap_shop_impression");
        UIManager.HideUiElement("scrMenuWowSlider");

        if (DefsGame.ScreenSkins)
            if (DefsGame.ScreenSkins.CheckAvailableSkinBool()) UIManager.ShowUiElement("BtnHaveNewSkin");
            else
                UIManager.HideUiElement("BtnHaveNewSkin");

        _isBtnSettingsClicked = false;
    }

    public void hideButtons()
    {
        if (_isButtonHiden)
            return;

        _isButtonHiden = true;
        FlurryEventsManager.SendEndEvent("start_screen_length");
        //UIManager.HideUiElement ("MainMenu");
        UIManager.HideUiElement("elementBestScore");
        //UIManager.HideUiElement ("elementCoins");
        UIManager.HideUiElement("BtnSkins");
        UIManager.HideUiElement("BtnVideoAds");
        UIManager.HideUiElement("BtnAchievements");
        UIManager.HideUiElement("BtnMoreGames");
        UIManager.HideUiElement("BtnSound");
        UIManager.HideUiElement("BtnRate");
        UIManager.HideUiElement("BtnLeaderboard");
        UIManager.HideUiElement("BtnShare");
        UIManager.HideUiElement("BtnSound");
        UIManager.HideUiElement("BtnPlus");
        UIManager.HideUiElement("BtnGameServices");

        UIManager.HideUiElement("BtnHaveNewSkin");
    }

    public void BtnSettingsClick()
    {
        _isBtnSettingsClicked = !_isBtnSettingsClicked;

        if (_isBtnSettingsClicked)
        {
            UIManager.ShowUiElement("BtnSound");
            UIManager.ShowUiElement("BtnInaps");
            UIManager.ShowUiElement("BtnGameServices");
        }
        else
        {
            UIManager.HideUiElement("BtnSound");
            UIManager.HideUiElement("BtnInaps");
            UIManager.HideUiElement("BtnGameServices");
        }
    }

    public void OnMoreAppsClicked()
    {
        //PublishingService.Instance.ShowAppShelf();
        FlurryEventsManager.SendEvent("more_games");
    }

    private void GetReward(OnGiveReward e)
    {
        if (_isWaitReward)
        {
            _isWaitReward = false;
            if (e.isAvailable)
            {
                for (var i = 0; i < 25; i++)
                {
                    var _coin = Instantiate(coin,
                        Camera.main.ScreenToWorldPoint(videoAdsButton.transform.position),
                        Quaternion.identity);
                    var coinScript = _coin.GetComponent<Coin>();
                    coinScript.MoveToEnd();
                }
                FlurryEventsManager.SendEvent("RV_strawberries_complete", "start_screen", true, 25);
            }
        }
    }

    public void OnVideoAdsClicked()
    {
        FlurryEventsManager.SendEvent("RV_strawberries", "start_screen");
        MyAds.ShowRewardedAds();
        _isWaitReward = true;
    }

    public void RateUs()
    {
//		FlurryEventsManager.SendEvent ("rate_us_impression", "start_screen");
//		Defs.Rate.RateUs ();
        Defs.PlaySound(sndBtnClick, 1f);
    }

    public void Share()
    {
//		FlurryEventsManager.SendEvent ("share");
        if (SystemInfo.deviceModel.Contains("iPad"))
        {
//			Defs.shareVoxel.ShareClick ();
        }
        Defs.PlaySound(sndBtnClick, 1f);
    }

    public void BtnPlusClick()
    {
        hideButtons();
        DefsGame.ScreenCoins.Show("start_screen");
        Defs.PlaySound(sndBtnClick, 1f);
    }

    public void BtnSkinsClick()
    {
        FlurryEventsManager.SendEvent("candy_shop");
        hideButtons();
        Defs.PlaySound(sndBtnClick, 1f);
    }
}