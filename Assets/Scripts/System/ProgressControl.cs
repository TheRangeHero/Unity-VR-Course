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

    [Header("Combo Lock")]
    [SerializeField] CombinationLock comboLock;

    [Header("The Wall")]
    [SerializeField] TheWall wall;
    [SerializeField] GameObject teleportationAreas;
    XRSocketInteractor wallSocket;

    [Header("Library")]
    [SerializeField] SimpleSliderControl librarySlider;

    [Header("The Robot")]
    [SerializeField] NavMeshRobot robot;

    [Header("Challenge Settings")]
    [SerializeField] string[] challengeStrings;
    [SerializeField] string endGameString;
    [SerializeField] string startGameString;
    [SerializeField] int wallCubesToDestroy;
    [SerializeField] private int challengeNumber;

    private int wallCubesDestroyed;
    private bool challengesCompletedBool;
    private bool startGameBool;
    void Start()
    {
        if (startButton != null)
        {
            startButton.selectEntered.AddListener(StartButtonPressed);
        }

        OnStartGame?.Invoke(startGameString);
        SetDrawerInteractable();
        if (comboLock != null)
        {
            comboLock.UnlockAction += OnComboUnlocked;
        }

        if (wall != null)
        {
            SetWall();
        }
        if (librarySlider != null)
        {
            librarySlider.OnSliderActive.AddListener(LibrarySliderActive);
        }
        if (robot != null)
        {
            robot.OnDestroyWallCube.AddListener(OnDestroyWallCube);
        }
    }

    private void OnDestroyWallCube()
    {
        wallCubesDestroyed++;
        if (wallCubesDestroyed >= wallCubesToDestroy && !challengesCompletedBool)
        {
            challengesCompletedBool = true;
            ChallengeComplete();
        }
    }

    private void LibrarySliderActive()
    {
        ChallengeComplete();
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
            OnChallengeComplete?.Invoke(endGameString);
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
            drawer.OnDrawerDetach.AddListener(OndDrawerDetach);

            drawerSocket = drawer.GetKeySocket;
            if (drawerSocket != null)
            {
                drawerSocket.selectEntered.AddListener(OnDrawerSocketed);
            }
        }
    }

    private void OndDrawerDetach()
    {
        ChallengeComplete();
    }

    private void OnDrawerSocketed(SelectEnterEventArgs arg0)
    {
        ChallengeComplete();
    }

    private void OnComboUnlocked()
    {
        ChallengeComplete();
    }

    private void SetWall()
    {
        wall.OnDetroy.AddListener(OnDestroyWall);

        wallSocket = wall.GetWallSocket;
        if (wallSocket != null)
        {
            wallSocket.selectEntered.AddListener(OnWallSocketed);
        }
    }

    private void OnWallSocketed(SelectEnterEventArgs arg0)
    {
        ChallengeComplete();
    }

    private void OnDestroyWall()
    {
        ChallengeComplete();
        if (teleportationAreas != null)
        {
            teleportationAreas.SetActive(true);
        }
    }
}
