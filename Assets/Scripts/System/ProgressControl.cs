using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System;

public class ProgressControl : MonoBehaviour
{
    public UnityEvent<string> OnStartGame;
    public UnityEvent<string> OnChallengeComplete;

    [Header("Start Button")]
    [SerializeField] XRButtonInteractable startButton;
    [SerializeField] GameObject keyIndicatorLight;

    [Header("Drawer Interactable")]
    [SerializeField] DrawerInteractable drawer;
    XRSocketInteractor drawerSocket;

    [Header("Challenge Settings")]
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
        SetDrawerInteractable();
    }

    private void ChallengeComplete()
    {
        challengeNumber++;
        if (challengeNumber < challengeStrings.Length)
        {
            OnChallengeComplete?.Invoke(challengeStrings[challengeNumber]);
        }
        else if (challengeNumber >= challengeStrings.Length)
        {
            //******ALL CHALLENGES COMPLET
        }
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

    private void SetDrawerInteractable()
    {
        if (drawer != null)
        {
            drawerSocket = drawer.GetKeySocket;
            if (drawerSocket != null)
            {
                drawerSocket.selectEntered.AddListener(OnDrawerSocketed);
            }
        }
    }

    private void OnDrawerSocketed(SelectEnterEventArgs arg0)
    {
        ChallengeComplete();
    }
}
