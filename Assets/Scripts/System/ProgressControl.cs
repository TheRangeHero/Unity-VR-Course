using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class ProgressControl : MonoBehaviour
{
    public UnityEvent<string> OnStartGame;
    public UnityEvent<string> OnChallengeComplete;

    [SerializeField] XRButtonInteractable startButton;
    [SerializeField] GameObject keyIndicatorLight;
    [SerializeField] string[] challengeStrings;
    [SerializeField] string startGameString;

    private bool startGameBool;
    private int challengeNumber;
    void Start()
    {
        if (startButton != null)
        {
            startButton.selectEntered.AddListener(StartButtonPressed);
        }

        OnStartGame?.Invoke(startGameString);
    }

    private void StartButtonPressed(SelectEnterEventArgs arg0)
    {
        if (!startGameBool)
        {
            startGameBool = true;

            if (keyIndicatorLight != null)
            {

                keyIndicatorLight.SetActive(true);
            }

            if (challengeNumber < challengeStrings.Length)
            {
                OnStartGame?.Invoke(challengeStrings[challengeNumber]);
            }
        }
    }
}
