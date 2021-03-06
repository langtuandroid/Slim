﻿using System.Collections.Generic;
using DoozyUI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ScreenGameOver : MonoBehaviour
{
    [SerializeField] private Text _nextCharacterText;
    [SerializeField] private Text _shareText;
    [SerializeField] private Text _wordsProgressText;
    [SerializeField] private Text _wordsText;
    
    private bool _isVisual;
    private float _centerPointY = 20f;
    private const float ItemHeightHalf = 50f;
    private const float HeightStep = 120f;
    private readonly List<string> _activeNamesList  = new List<string>();
    
    private bool _isGiftAvailable;
    private bool _isRewardedAvailable;
    private bool _isGotNewCharacter;
    private bool _isSkinsAllGeneralOpened;
    private bool _isGotWord;
    private bool _isWaitNewWord;
    private bool _isWordActive;
    
    private int _shareRewardValue;
    private int _giftValue;
    
    enum GiftCollectedType
    {
        None, Gift, Skin, Word
    }

    private GiftCollectedType _giftCollectedType = GiftCollectedType.None;

    private void OnEnable()
    {
        // Глобальные
        GlobalEvents<OnStartGame>.Happened += OnHideGameOverScreen;
        GlobalEvents<OnGameOverScreenShow>.Happened += OnShowGameOverScreen;
        GlobalEvents<OnGameOverScreenShowActiveItems>.Happened += OnGameOverScreenShowActiveItems;
        GlobalEvents<OnRewardedLoaded>.Happened += IsRewardedAvailable;
        GlobalEvents<OnGiftAvailable>.Happened += IsGiftAvailable;
        GlobalEvents<OnCoinsAdded>.Happened += OnCoinsAdded;
        GlobalEvents<OnWordCollected>.Happened += OnWordCollected;
        GlobalEvents<OnWordUpdateProgress>.Happened += OnWordGotChar;
        GlobalEvents<OnWordNeedToWait>.Happened += OnWordNeedToWait;
        GlobalEvents<OnWordsAvailable>.Happened += OnWordsAvailable;
        GlobalEvents<OnSkinAllGeneralOpened>.Happened += OnSkinAllGeneralOpened;
        
        // Внутренние
        GlobalEvents<OnGotNewCharacter>.Happened += OnGotNewCharacter;
        GlobalEvents<OnGiftCollected>.Happened += OnGiftCollected;
		GlobalEvents<OnGifShared>.Happened += OnGifShared;
//		Record.OnShareGIFEvent += OnShareGIFEvent;
    }

    private void OnGameOverScreenShowActiveItems(OnGameOverScreenShowActiveItems obj)
    {
        ShowActiveItems();
    }

    private void OnShowGameOverScreen(OnGameOverScreenShow e)
    {

        DefsGame.CurrentScreen = DefsGame.SCREEN_NOTIFICATIONS;

        float ran = Random.value;
        
        // Важность - Высокая

        
        AddNotifySkin();

        if (_isGiftAvailable)
        {
            AddNotifyGift();
        }
        
        if (_isGotWord)
        {
            AddNotifyWord();
        }
        
        // Важность - Средняя
        
        if (_activeNamesList.Count < 4 && _isGotNewCharacter && DefsGame.RateCounter == 0)
        {
            _activeNamesList.Add("NotifyRate");
            _isGotNewCharacter = false;
        }
        
        if (_activeNamesList.Count < 4 && (DefsGame.GameplayCounter == 3 || (DefsGame.GameplayCounter-3) % 5 == 0)/* && _isRewardedAvailable*/)
        {
            _activeNamesList.Add("NotifyRewarded");
        }
        
        if (Random.value > 0.5f) AddWordTimerOrProgress();

        if (_activeNamesList.Count < 4 && (DefsGame.CurrentPointsCount > DefsGame.GameBestScore * 0.5f
                                           || _isGotNewCharacter
                                           || _activeNamesList.Count == 0 && ran < 0.3f
                                           || _activeNamesList.Count == 1 && ran < 0.25f))
        {
            _activeNamesList.Add("NotifyShare");
			UIManager.ShowUiElement ("ScreenGameOverImageShareGif");
            if (DefsGame.CoinsCount > 100 && DefsGame.CoinsCount < 155)
				_shareRewardValue = 180-DefsGame.CoinsCount;
            else
            {
				if (ran < 0.5f) _shareRewardValue = 25;
				else _shareRewardValue = 30;
            }
			_shareText.text = _shareRewardValue.ToString();
        }
        
        // Важность - Низкая
        ran = Random.value;
        if (_giftValue == 0 && _activeNamesList.Count < 4 && (_activeNamesList.Count == 0 && ran > 0.7f
                                           || _activeNamesList.Count == 1 && ran > 0.75f
                                           || _activeNamesList.Count == 2 && ran > 0.80f))
        {
            AddNotifyGiftWaiting();
        }

        if (_activeNamesList.Count < 4 && (_activeNamesList.Count == 0 && ran > 0.7f
                                        || _activeNamesList.Count == 1 && ran > 0.75f
                                        || _activeNamesList.Count == 2 && ran > 0.80f)
        && ran > 0.4f)
        {
            AddNotifyNextSkin();
        } 
        
        if (MyAds.NoAds == 0 && _activeNamesList.Count < 4 && Random.value > 0.5f)
        {
            _activeNamesList.Add("NotifyNoAds");
        }
        
        if (_activeNamesList.Count < 4 && Random.value > 0.5f)
        {
            _activeNamesList.Add("NotifyTier1");
        }
        
        if (_activeNamesList.Count < 4 && Random.value > 0.5f)
        {
            _activeNamesList.Add("NotifyTier2");
        }

        // Перемешиваем элементы списка, чтобы они располагались рандомно по оси У
        ShuffleItems();
        SetItemsPositions();
        ShowActiveItems();
    }

    private void AddWordTimerOrProgress()
    {
        if (_activeNamesList.Count < 4 && _isWordActive && !_isGotWord)
        {
            if (_isWaitNewWord)
            {
                _activeNamesList.Add("NotifyWordTimer");
            }
            else
            {
                _activeNamesList.Add("NotifyWordProgress");
            }
        }
    }

    private bool AddNotifySkin()
    {
        if (!_isSkinsAllGeneralOpened && DefsGame.CoinsCount >= 200)
        {
            _activeNamesList.Add("NotifyNewCharacter");
            return true;
        }
        return false;
    }
    
    private void AddNotifyNextSkin()
    {
        if (!_isSkinsAllGeneralOpened && DefsGame.CoinsCount < 200)
        {
            _activeNamesList.Add("NotifyNextCharacter");
        }
    }

    private void AddNotifyGift()
    {
        _activeNamesList.Add("NotifyGift");
        if (DefsGame.CoinsCount > 100 && DefsGame.CoinsCount < 155)
            _giftValue = 190-DefsGame.CoinsCount;
        else
        {
            if (Random.value < 0.5f) _giftValue = 40;
            else _giftValue = 45;
        }
    }

    private void AddNotifyGiftWaiting()
    {
        _activeNamesList.Add("NotifyGiftWaiting");
    }
    
    private void AddNotifyWord()
    {
        _activeNamesList.Add("NotifyWord");
    }
    
    private void ShuffleItems()
    {
        // Перемешиваем 10 раз
        for (int n = 0; n < 10; n++)
        for (int i = 0; i < _activeNamesList.Count; i++)
        {
            int j = Random.Range(0, _activeNamesList.Count-1);
            if (j != i)
            {
                string str = _activeNamesList[i];
                _activeNamesList[i] = _activeNamesList[j];
                _activeNamesList[j] = str;
            }
        }
    }

    private void SetItemsPositions()
    {
        float startPos = CalcStartPosition(_activeNamesList.Count);
        bool isLeft = true;
        for (int i = 0; i < _activeNamesList.Count; i++)
        {
            var element = GetUIElement(_activeNamesList[i]);
            if (element)
            {
                if (isLeft)
                {
                    element.customStartAnchoredPosition = new Vector3(0, startPos + i * HeightStep, 0f);
                    element.inAnimations.move.moveDirection = Move.MoveDirection.Left;
                    element.outAnimations.move.moveDirection = Move.MoveDirection.Left;
                }
                else
                {
                    element.customStartAnchoredPosition = new Vector3(0, startPos + i * HeightStep, 0f);
                    element.inAnimations.move.moveDirection = Move.MoveDirection.Right;
                    element.outAnimations.move.moveDirection = Move.MoveDirection.Right;
                }
                isLeft = !isLeft;
                element.inAnimations.move.startDelay = i * 0.1f;
                element.outAnimations.move.startDelay = i * 0.1f;
                element.useCustomStartAnchoredPosition = true;
            }
        }
    }

    public void ShowActiveItems()
    {
        if (_activeNamesList.Count == 0)
        {
            GlobalEvents<OnNoGameOverButtons>.Call(new OnNoGameOverButtons());
            return;
        } 
        
//        UIManager.ShowUiElement("ScreenGameOver");
        for (int i = 0; i < _activeNamesList.Count; i++)
        {
            var element = GetUIElement(_activeNamesList[i]);
            if (element)
            {
                UIManager.ShowUiElement(_activeNamesList[i]);
            }
        }
        
        _isVisual = true;
    }

    private float CalcStartPosition(int notificationCounter)
    {
        return _centerPointY - notificationCounter * HeightStep * 0.5f + ItemHeightHalf;
    }

    private UIElement GetUIElement(string elementName)
    {
        List<UIElement> list = UIManager.GetUiElements(elementName);
        if (list.Count > 0)
        {
            return list[0];
        }
        return null;
    }

    public void Hide()
    {
        HideActiveItems();
        _activeNamesList.Clear();

//        UIManager.HideUiElement("ScreenGameOver");
    }

    public void HideActiveItems()
    {
        foreach (string t in _activeNamesList)
        {
            var element = GetUIElement(t);
            if (element)
            {
                UIManager.HideUiElement(t);
            }
        }
        _isVisual = false;

		// TEMP
		UIManager.HideUiElement ("ScreenGameOverImageShareGif");
    }

    private void OnHideGameOverScreen(OnStartGame e)
    {
        Hide();
    }

    private void IsRewardedAvailable(OnRewardedLoaded e)
    {
        _isRewardedAvailable = e.IsAvailable;
    }

    private void IsGiftAvailable(OnGiftAvailable e)
    {
        _isGiftAvailable = e.IsAvailable;
        
        int idNotifyOld = _activeNamesList.IndexOf("NotifyGiftWaiting");
        if (!_isGiftAvailable || !_isVisual || idNotifyOld == -1) return;

        AddNotifyGift();
            
        var element = GetUIElement(_activeNamesList[idNotifyOld]);
        UIManager.HideUiElement(_activeNamesList[idNotifyOld]);
        var element2 = GetUIElement("NotifyGift");
        if (element)
        {
            element2.customStartAnchoredPosition = element.customStartAnchoredPosition;
            element2.useCustomStartAnchoredPosition = true;
            element2.inAnimations.move.moveDirection = element.inAnimations.move.moveDirection;
            element2.outAnimations.move.moveDirection = element.outAnimations.move.moveDirection;
            element2.inAnimations.move.startDelay = element.inAnimations.move.startDelay;
            element2.outAnimations.move.startDelay = element.outAnimations.move.startDelay;
        }
        _activeNamesList.RemoveAt(idNotifyOld);
        
        UIManager.ShowUiElement("NotifyGift");
    }
    
    private void OnCoinsAdded(OnCoinsAdded obj)
    {
        int toNextSkin = 200 - obj.Total;
        _nextCharacterText.text = toNextSkin.ToString();
        
        int idNotifyOld = _activeNamesList.IndexOf("NotifyNextCharacter");
        if (!_isVisual || idNotifyOld == -1) return;

        if (obj.Total >= 200)
        {
            if (AddNotifySkin()) {
                var element = GetUIElement(_activeNamesList[idNotifyOld]);
                UIManager.HideUiElement(_activeNamesList[idNotifyOld]);
                var element2 = GetUIElement("NotifyNewCharacter");
                if (element)
                {
                    element2.customStartAnchoredPosition = element.customStartAnchoredPosition;
                    element2.inAnimations.move.moveDirection = element.inAnimations.move.moveDirection;
                    element2.outAnimations.move.moveDirection = element.outAnimations.move.moveDirection;
                    element2.inAnimations.move.startDelay = element.inAnimations.move.startDelay;
                    element2.outAnimations.move.startDelay = element.outAnimations.move.startDelay;
                    element2.useCustomStartAnchoredPosition = true;
                }
                UIManager.ShowUiElement("NotifyNewCharacter");
            }
            _activeNamesList.RemoveAt(idNotifyOld);   
        } 
    }

    private void OnGotNewCharacter(OnGotNewCharacter obj)
    {
        _isGotNewCharacter = true;
    }
    
    private void OnGiftCollected(OnGiftCollected obj)
    {
        if (_giftCollectedType == GiftCollectedType.Gift)
        {
            _activeNamesList.Add("NotifyGiftWaiting");
        } else if (_giftCollectedType == GiftCollectedType.Skin)
        {
            AddNotifySkin();
            AddNotifyNextSkin();
        } else 
        if (_giftCollectedType == GiftCollectedType.Word)
        {
            AddWordTimerOrProgress();
        }
        
        SetItemsPositions();
        ShowActiveItems();
        DefsGame.CurrentScreen = DefsGame.SCREEN_MENU;
    }
    
    private void OnWordsAvailable(OnWordsAvailable obj)
    {
        _isWordActive = obj.IsAvailable;
    }
    
    private void OnWordNeedToWait(OnWordNeedToWait obj)
    {
        _isWaitNewWord = obj.IsWait;
        
        int idNotifyOld = _activeNamesList.IndexOf("NotifyWordTimer");
        
        if (!_isVisual || idNotifyOld == -1) return;

        _activeNamesList.Add("NotifyWordProgress");
        
        var element = GetUIElement(_activeNamesList[idNotifyOld]);
        UIManager.HideUiElement(_activeNamesList[idNotifyOld]);
        var element2 = GetUIElement("NotifyWordProgress");
        if (element)
        {
            element2.customStartAnchoredPosition = element.customStartAnchoredPosition;
            element2.useCustomStartAnchoredPosition = true;
            element2.inAnimations.move.moveDirection = element.inAnimations.move.moveDirection;
            element2.outAnimations.move.moveDirection = element.outAnimations.move.moveDirection;
            element2.outAnimations.move.startDelay = element.outAnimations.move.startDelay;
        }
        _activeNamesList.RemoveAt(idNotifyOld);
        
        UIManager.ShowUiElement("NotifyWordProgress");
    }
    
    private void OnSkinAllGeneralOpened(OnSkinAllGeneralOpened obj)
    {
        _isSkinsAllGeneralOpened = true;
    }

	private void OnGifShared(OnGifShared obj)
	{
		int id = _activeNamesList.IndexOf("NotifyShare"); 
		if (id != -1) {
			_activeNamesList.RemoveAt (id);

//			Record.DOReset ();

			GlobalEvents<OnBtnShareGifClick>.Call (new OnBtnShareGifClick{CoinsCount = _shareRewardValue});
			_shareRewardValue = 0;

			HideActiveItems ();

			UIManager.HideUiElement ("NotifyShare");
			GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
		}
	}
    
    private void OnWordGotChar(OnWordUpdateProgress obj)
    {
        _wordsProgressText.text = obj.Text;
    }
    
    private void OnWordCollected(OnWordCollected obj)
    {
        _wordsText.text = obj.Text;
        _isGotWord = true;
    }
    
    //----------------------------------------------------
    // Touches
    //----------------------------------------------------

    public void BtnShareClick()
    {
        GlobalEvents<OnBtnShareClick>.Call(new OnBtnShareClick());
        UIManager.HideUiElement("NotifyShare");
    }
    
    public void BtnRateClick()
    {
        GlobalEvents<OnBtnRateClick>.Call(new OnBtnRateClick());
        UIManager.HideUiElement("NotifyRate");
    }
    
    public void BtnNewSkinClick()
    {
        _giftCollectedType = GiftCollectedType.Skin;
        HideActiveItems();
        int id = _activeNamesList.IndexOf("NotifyNewCharacter"); 
        if (id != -1) _activeNamesList.RemoveAt(id);
        
        GlobalEvents<OnBtnGetRandomSkinClick>.Call(new OnBtnGetRandomSkinClick());
        GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
        GlobalEvents<OnHideTubes>.Call(new OnHideTubes());
    }
    
    public void BtnWordClick()
    {
        _giftCollectedType = GiftCollectedType.Word;
        HideActiveItems();
        int idNotifyOld = _activeNamesList.IndexOf("NotifyWord");
        if (idNotifyOld != -1)
            _activeNamesList.RemoveAt(idNotifyOld);

        _isGotWord = false;
        
        GlobalEvents<OnBtnWordClick>.Call(new OnBtnWordClick{CoinsCount = 100, IsResetTimer = false});
        GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
        GlobalEvents<OnHideTubes>.Call(new OnHideTubes());
    }
    
    public void BtnGiftClick()
    {
        _giftCollectedType = GiftCollectedType.Gift;
        HideActiveItems();
        int id = _activeNamesList.IndexOf("NotifyGift"); 
        if (id != -1) _activeNamesList.RemoveAt(id);
            
        GlobalEvents<OnBtnGiftClick>.Call(new OnBtnGiftClick{CoinsCount = _giftValue, IsResetTimer = true});
        _giftValue = 0;
        GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
        GlobalEvents<OnHideTubes>.Call(new OnHideTubes());
    }
    
    public void BtnRewardedClick()
    {
        GlobalEvents<OnShowRewarded>.Call(new OnShowRewarded());
        UIManager.HideUiElement("NotifyRewarded");
    }
    
    public void BtnNoAds()
    {
        UIManager.HideUiElement("NotifyNoAds");
    }
    
    public void BtnTier1()
    {
        UIManager.HideUiElement("NotifyTier1");
    }
    
    public void BtnTier2()
    {
        UIManager.HideUiElement("NotifyTier2");
    }
}