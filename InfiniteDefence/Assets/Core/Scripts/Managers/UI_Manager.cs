using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using static DataTypes;
using static Enums;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public GameObject ChallengeChoicePanel;

    [Space(5f)]
    public GameObject MainPanel;

    [Space(5f)]
    public GameObject EndGamePanel;
    public Button RestartButton;

    public List<Button> choiceButtons;
    [SerializeField] List<RandomChallengeButton> randomChallengeButtons;
    public Button joinBattleButton;

    bool canMakeChoice;

    public ReactiveCommand<Challenge> ChallengeWasChosen = new ReactiveCommand<Challenge>();
    public ReactiveCommand PlayButtonPressed = new ReactiveCommand();
    public ReactiveCommand RestartButtonPressed = new ReactiveCommand();

    List<IDisposable> LifetimeDisposables;
   
    public void SetUiState(GameState gameState)
    {
        LifetimeDisposables = new List<IDisposable>();

        switch (gameState)
        {
            case GameState.ChallengeChoice:
                ChallengeChoicePanel.SetActive(true);
                MainPanel.SetActive(false);
                EndGamePanel.SetActive(false);

                canMakeChoice = true;
                PrepareChoiceButtons();
                joinBattleButton.gameObject.SetActive(false);
                break;
            case GameState.Game:
                ChallengeChoicePanel.SetActive(false);
                MainPanel.SetActive(true);
                break;
            case GameState.End:
                MainPanel.SetActive(false);
                EndGamePanel.SetActive(true);

                RestartButton.OnClickAsObservable().First().Subscribe(_ => 
                {
                    RestartButtonPressed.Execute();
                }).AddTo(LifetimeDisposables);
                break;
        }
    }

    void PrepareChoiceButtons()
    {
        choiceButtons = choiceButtons.OrderBy(emp => Guid.NewGuid()).ToList();
        for (int i = 0; i < choiceButtons.Count; i++)
            choiceButtons[i].transform.SetSiblingIndex(i);

        foreach (var a in randomChallengeButtons) a.Hide();
    }
    public void OnClick_ChoiceButtonClicked(int i)
    {
        ManageFaithChoice(i);
    }

    async void ManageFaithChoice(int choice)
    {
        if (!canMakeChoice) return;
        canMakeChoice = false;

        Debug.Log($"ManageFaithChoice {choice}");

        RandomChallengeButton button = randomChallengeButtons[choice];

        await Observable.Timer(TimeSpan.FromSeconds(0.4f));
        button.Unveil(true);

        foreach (var a in randomChallengeButtons)
        {
            if (a == button) continue;

            await Observable.Timer(TimeSpan.FromSeconds(0.4f));
            a.Unveil();
        }

        await Observable.Timer(TimeSpan.FromSeconds(0.4f));

        ChallengeWasChosen.Execute(button.challenge);
        joinBattleButton.gameObject.SetActive(true);
        joinBattleButton.OnClickAsObservable().First().Subscribe(_ =>
        {
            PlayButtonPressed.Execute();
        }).AddTo(LifetimeDisposables);
    }

    private void OnDestroy()
    {
        foreach (var a in LifetimeDisposables)
            a.Dispose();
    }
}
