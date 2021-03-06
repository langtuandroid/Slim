﻿using DarkTonic.MasterAudio;
using DoozyUI;
using UnityEngine;

public class ScreenGame : MonoBehaviour
{
    [SerializeField] private ScreenColorAnimation _screenAnimation;

    private bool _isScreenRateDone;
    private bool _isScreenReviveDone;
    private bool _isScreenShareDone;

    enum GameState {Init, Gameplay, GameOver, RedScreen, RedScreenWait, BackToMenu}
    private GameState _gameState;

    private bool _isRewardedVideoReadyToShow;
    private bool _isWaitReward;

    private void Awake()
    {
        DefsGame.LoadVariables();
    }

    private void Start()
    {
        _gameState = GameState.Init;
    }

    private void Init()
    {
        DefsGame.GameServices.ReportProgressWithGlobalID(DefsGame.GameServices.ACHIEVEMENT_FIRST_WIN,
            DefsGame.GameBestScore);

        ++DefsGame.QUEST_GAMEPLAY_Counter;
        PlayerPrefs.SetInt("QUEST_GAMEPLAY_Counter", DefsGame.QUEST_GAMEPLAY_Counter);

        DefsGame.GameServices.ReportProgressWithGlobalID(DefsGame.GameServices.ACHIEVEMENT_MASTER,
            DefsGame.QUEST_GAMEPLAY_Counter);

        ++DefsGame.GameplayCounter;

        PlayerPrefs.SetInt("QUEST_THROW_CounterCounter", DefsGame.QUEST_THROW_Counter);

        DefsGame.GameServices.ReportProgressWithGlobalID(DefsGame.GameServices.ACHIEVEMENT_FiFIELD_OF_CANDIES,
            DefsGame.QUEST_THROW_Counter);

        // Сохраняемся тут, чтобы не тормозить игру
        PlayerPrefs.SetInt("QUEST_BOMBS_Counter", DefsGame.QUEST_BOMBS_Counter);
        DefsGame.GameServices.ReportProgressWithGlobalID(DefsGame.GameServices.ACHIEVEMENT_EXPLOSIVE,
            DefsGame.QUEST_BOMBS_Counter);

        
        GlobalEvents<OnBestScoreUpdate>.Call(new OnBestScoreUpdate());

        _isScreenReviveDone = false;
        _isScreenShareDone = false;
        _isScreenRateDone = false;

        if (DefsGame.GameplayCounter > 1)
            GlobalEvents<OnGameOverScreenShow>.Call(new OnGameOverScreenShow());
    }

    private void OnEnable()
    {
        GlobalEvents<OnGameOver>.Happened += OnGameOver;
        GlobalEvents<OnStartGame>.Happened += OnStartGame;
        GlobalEvents<OnGiveReward>.Happened += GetReward;
        GlobalEvents<OnGifSaved>.Happened += OnGifSaved;
    }

    private void OnGifSaved(OnGifSaved obj)
    {
    }

    private void OnGameOver(OnGameOver e)
    {
        if (_gameState == GameState.GameOver)
            return;

//        Record.DOSave();
        MasterAudio.PlaySoundAndForget("GameOver");

        if (DefsGame.IS_ACHIEVEMENT_MISS_CLICK == 0)
        {
            ++DefsGame.QUEST_MISS_Counter;
            PlayerPrefs.SetInt("QUEST_MISS_Counter", DefsGame.QUEST_MISS_Counter);
            DefsGame.GameServices.ReportProgressWithGlobalID(DefsGame.GameServices.ACHIEVEMENT_MISS_CLICK,
                DefsGame.QUEST_MISS_Counter);
        }

        _gameState = GameState.GameOver;
    }

    public void StartGame()
    {
        GlobalEvents<OnStartGame>.Call(new OnStartGame());
    }

    private void OnStartGame(OnStartGame e)
    {
        if (_gameState == GameState.GameOver)
            return;

        MasterAudio.PlaySoundAndForget("GameStart");
//        Record.DORec();

        ++DefsGame.QUEST_THROW_Counter;

        if (DefsGame.GameplayCounter == 1) GlobalEvents<OnPointsShow>.Call(new OnPointsShow());
            
        DefsGame.CurrentScreen = DefsGame.SCREEN_GAME;
        GlobalEvents<OnPointsReset>.Call(new OnPointsReset());
    }

    public void EndCurrentGame()
    {
        /*if (!isScreenReviveDone) {
            isScreenReviveDone = true;
            if (PublishingService.Instance.IsRewardedVideoReady() && DefsGame.currentPointsCount >= 4) {
                UIManager.ShowUiElement ("ScreenRevive");
                UIManager.ShowUiElement ("ScreenReviveBtnRevive");
                UIManager.ShowUiElement ("ScreenReviveBtnBack");
                D.Log ("isScreenReviveDone"); 
                Defs.PlaySound (sndShowScreen);
                return;
            }
        }

        if (!isScreenShareDone) {
            isScreenShareDone = true;
            if ((DefsGame.currentPointsCount >= 50) && (DefsGame.currentPointsCount == DefsGame.gameBestScore)) {
                UIManager.ShowUiElement ("ScreenShare");
                UIManager.ShowUiElement ("ScreenShareBtnShare");
                UIManager.ShowUiElement ("ScreenShareBtnBack");
                Defs.PlaySound (sndShowScreen);
                D.Log ("isScreenShareDone"); 
                return;
            }
        }

        if (!isScreenRateDone) {
            isScreenRateDone = true;
            if ((DefsGame.rateCounter < 3) && (DefsGame.currentPointsCount >= 100)
                && (DefsGame.gameplayCounter % 20 == 0)) {
                ++DefsGame.rateCounter;
                PlayerPrefs.SetInt ("rateCounter", DefsGame.rateCounter);
                UIManager.ShowUiElement ("ScreenRate");
                UIManager.ShowUiElement ("ScreenRateBtnRate");
                UIManager.ShowUiElement ("ScreenRateBtnBack");
                Defs.PlaySound (sndShowScreen);
                D.Log ("isScreenRateDone"); 
                return;
            }
        }*/

        _gameState = GameState.BackToMenu;
    }

    private void Update()
    {
        BtnEscapeUpdate();

        switch (_gameState)
        {
            case GameState.Init:
                Init();
                _gameState = GameState.Gameplay;
                return;
            case GameState.Gameplay:
                // Gameplay
                break;
            case GameState.GameOver:
                _screenAnimation.SetAlphaMax(0.75f);
                _screenAnimation.SetAnimation(false, 0.1f);
                _screenAnimation.Show();
                _screenAnimation.SetAnimation(true, 0.02f);
                _screenAnimation.SetColor(1.0f, 0.21f, 0.21f);
                _screenAnimation.SetAutoHide(true);

                _gameState = GameState.RedScreen;
                break;
            case GameState.RedScreen:
                if (!_screenAnimation.isActiveAndEnabled)
                {
                    _gameState = GameState.RedScreenWait;
                    EndCurrentGame();
                }
                break;
            case GameState.RedScreenWait:
                break;
            case GameState.BackToMenu:

//            if ((DefsGame.gameBestScore == DefsGame.currentPointsCount)&&(DefsGame.gameBestScore != 0)) {
//                DefsGame.gameServices.SubmitScore (DefsGame.gameBestScore);
//                PlayerPrefs.SetInt ("BestScore", DefsGame.gameBestScore);
//            }
                PlayerPrefs.SetInt("coinsCount", DefsGame.CoinsCount);

                GlobalEvents<OnShowMenu>.Call(new OnShowMenu());

                _gameState = GameState.Init;
                break;
        }
    }

    public void Revive()
    {
        GlobalEvents<OnShowRewarded>.Call(new OnShowRewarded());
        _isWaitReward = true;
    }

    private void GetReward(OnGiveReward e)
    {
        if (_isWaitReward)
        {
            _isWaitReward = false;
            if (e.IsAvailable)
            {
                _gameState = GameState.Gameplay;

                ReviveClose();
                MasterAudio.PlaySoundAndForget("GUI_Grab");
            }
            else
            {
                ReviveClose();
                _gameState = GameState.BackToMenu;
            }
        }
    }

    public void Share()
    {
        UIManager.HideUiElement("ScreenShare");
        UIManager.HideUiElement("ScreenShareBtnShare");
        UIManager.HideUiElement("ScreenShareBtnBack");
//        if (SystemInfo.deviceModel.Contains("iPad")) Defs.ShareVoxel.ShareClick();
//        else Defs.Share.ShareClick();
        GlobalEvents<OnBtnShareClick>.Call(new OnBtnShareClick());
        MasterAudio.PlaySoundAndForget("GUI_Grab");
        EndCurrentGame();
    }


    public void Rate()
    {
        UIManager.HideUiElement("ScreenRate");
        UIManager.HideUiElement("ScreenRateBtnRate");
        UIManager.HideUiElement("ScreenRateBtnBack");
        MasterAudio.PlaySoundAndForget("GUI_Grab");
        GlobalEvents<OnBtnRateClick>.Call(new OnBtnRateClick());
        EndCurrentGame();
    }

    public void ReviveClose()
    {
        UIManager.HideUiElement("ScreenRevive");
        UIManager.HideUiElement("ScreenReviveBtnRevive");
        UIManager.HideUiElement("ScreenReviveBtnBack");
        EndCurrentGame();
    }

    public void ShareClose()
    {
        UIManager.HideUiElement("ScreenShare");
        UIManager.HideUiElement("ScreenShareBtnShare");
        UIManager.HideUiElement("ScreenShareBtnBack");
        EndCurrentGame();
    }

    public void RateClose()
    {
        UIManager.HideUiElement("ScreenRate");
        UIManager.HideUiElement("ScreenRateBtnRate");
        UIManager.HideUiElement("ScreenRateBtnBack");
        EndCurrentGame();
    }

    private void BtnEscapeUpdate()
    {
        /*if (InputController.IsTouchOnScreen(TouchPhase.Began)) {
            DefsGame.QUEST_BOMBS_Counter += 50;
            DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_EXPLOSIVE, DefsGame.QUEST_BOMBS_Counter);

            //if (DefsGame.QUEST_BOMBS_Counter % 100 == 0) {
                DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_FiFIELD_OF_CANDIES, DefsGame.QUEST_BOMBS_Counter);
            //}
        }*/

        //if (Input.GetKeyDown (KeyCode.A))
        if (InputController.IsEscapeClicked())
//            if (DefsGame.CurrentScreen == DefsGame.SCREEN_EXIT)
//            {
//                HideExitPanel();
//            }            else 
            if (DefsGame.CurrentScreen == DefsGame.SCREEN_MENU)
            {
                ShowExitPanel();
            }
            else if (DefsGame.CurrentScreen == DefsGame.SCREEN_GAME)
            {
                if (_isScreenReviveDone)
                    ReviveClose();
                else if (_isScreenShareDone)
                    ShareClose();
                else if (_isScreenRateDone)
                    RateClose();
                else
                    GameOver();
            }       
    }


    private void GameOver()
    {
        _gameState = GameState.GameOver;
    }

    public void HideExitPanel()
    {
        DefsGame.CurrentScreen = DefsGame.SCREEN_MENU;
        UIManager.HideUiElement("PanelExit");
        UIManager.HideUiElement("PanelExitBtnYes");
        UIManager.HideUiElement("PanelExitBtnNo");
    }

    private void ShowExitPanel()
    {
        DefsGame.CurrentScreen = DefsGame.SCREEN_EXIT;
        UIManager.ShowUiElement("PanelExit");
        UIManager.ShowUiElement("PanelExitBtnYes");
        UIManager.ShowUiElement("PanelExitBtnNo");
    }

    public void Quit()
    {
        Application.Quit();
    }
}