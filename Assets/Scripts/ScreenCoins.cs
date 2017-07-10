﻿using System;
using DoozyUI;
using UnityEngine;

public class ScreenCoins : MonoBehaviour
{
    private bool _isWaitReward;

    private bool isShowBtnViveoAds;

    [HideInInspector] public string PrevScreenName;
    public static event Action<int> OnAddCoinsVisual;

    private void Start()
    {
        DefsGame.ScreenCoins = this;
    }

    private void OnEnable()
    {
        GlobalEvents<OnGiveReward>.Happened += GetReward;
        GlobalEvents<OnRewardedAvailable>.Happened += IsRewardedVideoAvailable;
    }

    private void OnDisable()
    {
        GlobalEvents<OnGiveReward>.Happened -= GetReward;
        GlobalEvents<OnRewardedAvailable>.Happened -= IsRewardedVideoAvailable;
    }

    public void ShowButtons()
    {
        UIManager.ShowUiElement("ScreenCoinsBtnBack");
        UIManager.ShowUiElement("BtnTier1");
        UIManager.ShowUiElement("BtnTier2");
        if (isShowBtnViveoAds)
        {
            UIManager.ShowUiElement("ScreenCoinsBtnVideo");
            FlurryEventsManager.SendEvent("RV_strawberries_impression", "shop");
        }
        if (MyAds.noAds < 1) UIManager.ShowUiElement("ScreenCoinsBtnNoAds");
#if UNITY_IPHONE
        UIManager.ShowUiElement("ScreenCoinsBtnRestore");
#endif
    }

    public void HideButtons()
    {
        UIManager.HideUiElement("ScreenCoinsBtnBack");
        UIManager.HideUiElement("BtnTier1");
        UIManager.HideUiElement("BtnTier2");
        UIManager.HideUiElement("ScreenCoinsBtnVideo");
        UIManager.HideUiElement("ScreenCoinsBtnNoAds");
        UIManager.HideUiElement("ScreenCoinsBtnRestore");
    }

    private void IsRewardedVideoAvailable(OnRewardedAvailable e)
    {
        isShowBtnViveoAds = e.isAvailable;
        if (isShowBtnViveoAds)
        {
            if (DefsGame.CurrentScreen == DefsGame.SCREEN_IAPS)
            {
                UIManager.ShowUiElement("ScreenCoinsBtnVideo");
                FlurryEventsManager.SendEvent("RV_strawberries_impression", "shop");
            }
        }
        else
        {
            UIManager.HideUiElement("ScreenCoinsBtnVideo");
        }
    }

    public void BtnTier3()
    {
        FlurryEventsManager.SendEvent("RV_strawberries", "shop");
        MyAds.ShowRewardedAds();
        _isWaitReward = true;
    }

    private void GetReward(OnGiveReward e)
    {
        if (_isWaitReward)
        {
            _isWaitReward = false;
            if (e.isAvailable)
                GameEvents.Send(OnAddCoinsVisual, 25);
        }
    }

    public void Show(string prevScreenName)
    {
        PrevScreenName = prevScreenName;
        FlurryEventsManager.SendStartEvent("iap_shop_length");
        FlurryEventsManager.SendEvent("iap_shop", PrevScreenName);

        DefsGame.CurrentScreen = DefsGame.SCREEN_IAPS;
        DefsGame.IsCanPlay = false;
        ShowButtons();
    }

    public void Hide()
    {
        FlurryEventsManager.SendEndEvent("iap_shop_length");

        DefsGame.CurrentScreen = DefsGame.SCREEN_MENU;
        DefsGame.IsCanPlay = true;
        HideButtons();

        FlurryEventsManager.SendEvent("iap_shop_home", PrevScreenName);
    }
}