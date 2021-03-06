﻿using DoozyUI;
using UnityEngine;

public class GiftRandomSkin : MonoBehaviour
{
	private bool _isSkinsAllGeneralOpened;

	private void OnEnable()
	{
		GlobalEvents<OnGiftShowRandomSkinAnimation>.Happened += OnGiftShowRandomSkinAnimation;
		GlobalEvents<OnSkinAllGeneralOpened>.Happened += OnSkinAllGeneralOpened;
	}

	private void OnSkinAllGeneralOpened(OnSkinAllGeneralOpened obj)
	{
		_isSkinsAllGeneralOpened = true;
	}

	private void OnGiftShowRandomSkinAnimation(OnGiftShowRandomSkinAnimation obj)
	{
		Debug.Log("OnGiftShowRandomSkinAnimation(OnGiftShowRandomSkinAnimation obj)");
		int id = GetRandomAvailableSkin();
		if (id != -1)
		{
			transform.localScale = Vector3.one;	

			GlobalEvents<OnBuySkin>.Call(new OnBuySkin {Id = id});
		}
		
		Invoke("ShowBtnClose", 1.5f);
	}

	private int GetRandomAvailableSkin()
	{
		Debug.Log("GetRandomAvailableSkin");
		
		if (_isSkinsAllGeneralOpened) return -1;
		int tryCount = Random.Range(DefsGame.FacesGeneralMin, DefsGame.FacesGeneralMax + 1);
		int i = DefsGame.FacesGeneralMin-1;
		while (i < tryCount)
		{
			for (int id = DefsGame.FacesGeneralMin+1; id < DefsGame.FacesGeneralMax; id++)
			{
				if (DefsGame.FaceAvailable[id] == 0)
				{
					++i;
					if (i == tryCount)
					{
						Debug.Log("GetRandomAvailableSkin RETURN id = " + id);
						return id;
					}
				}
			}
		}
		Debug.Log("GetRandomAvailableSkin RETURN id = " + -1);
		return -1;
	}

	public void BtnClose()
	{
		UIManager.HideUiElement("ScreenGiftBtnPlay");
		
		GlobalEvents<OnHideGiftScreen>.Call(new OnHideGiftScreen());
		GlobalEvents<OnHideTubes>.Call(new OnHideTubes());
	}

	private void ShowBtnClose()
	{
		UIManager.ShowUiElement("ScreenGiftBtnPlay");
	}
}
