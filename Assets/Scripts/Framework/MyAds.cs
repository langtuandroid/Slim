﻿using UnityEngine;

public class MyAds : MonoBehaviour
{
    private static int _videoAdCounter;
    public static int noAds;

    private void OnEnable()
    {
        GlobalEvents<OnRewardedAvailable>.Happened += IsRewardedVideoAvailable;
    }

    private void OnDisable()
    {
        GlobalEvents<OnRewardedAvailable>.Happened -= IsRewardedVideoAvailable;
    }

    private void IsRewardedVideoAvailable(OnRewardedAvailable e)
    {
        if (!e.IsAvailable)
            _videoAdCounter = 0;
    }

    public static void ShowVideoAds()
    {
        if (_videoAdCounter >= 4)
            GlobalEvents<OnShowVideoAds>.Call(new OnShowVideoAds());
        else
            ++_videoAdCounter;
    }

    public static void ShowRewardedAds()
    {
        GlobalEvents<OnShowRewarded>.Call(new OnShowRewarded());
    }
}