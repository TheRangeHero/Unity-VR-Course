using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRAudioManager : MonoBehaviour
{
    [Header("Grab Interactables")]
    [SerializeField] XRGrabInteractable[] grabInteractables;
    [SerializeField] AudioSource activatedSound;
    [SerializeField] AudioSource grabSound;
    [SerializeField] AudioClip grabClip;
    [SerializeField] AudioClip keyClip;
    [SerializeField] AudioClip grabActivatedClip;
    [SerializeField] AudioClip wandActivatedClip;

    [Header("Drawer Interactable")]
    [SerializeField] DrawerInteractable drawer;
    [SerializeField] XRSocketInteractor drawerSocket;
    [SerializeField] AudioSource drawerSound;
    [SerializeField] AudioSource drawerSocketSound;
    [SerializeField] AudioClip drawerMoveClip;
    [SerializeField] AudioClip drawerSocketClip;

    [Header("Hinge Interactables")]
    [SerializeField] SimpleHindgeInteractable[] cabinetDoors = new SimpleHindgeInteractable[2];
    [SerializeField] AudioSource[] cabinetDoorSound;
    [SerializeField] AudioClip cabinetDoorMoveClip;

    [Header("The Wall")]
    [SerializeField] TheWall wall;
    [SerializeField] XRSocketInteractor wallSocket;
    [SerializeField] AudioSource wallSound;
    [SerializeField] AudioSource wallSocketSound;
    [SerializeField] AudioClip destroyWallClip;
    [SerializeField] AudioClip wallSocketClip;
    [SerializeField] private AudioClip fallBackClip;
    private const string FallBackClip_Name = "fallBackClip";

    private void OnEnable()
    {
        if (fallBackClip == null)
        {
            fallBackClip = AudioClip.Create(FallBackClip_Name, 1, 1, 1000, true);
        }

        SetGrabbables();

        if (drawer != null)
        {
            SetDrawerInteractable();
        }
        cabinetDoorSound = new AudioSource[cabinetDoors.Length];
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (cabinetDoors[i] != null)
            {
                SetCabinetDoors(i);
            }
        }
        if (wall != null)
        {
            SetWall();
        }
    }

    private void SetDrawerInteractable()
    {
        drawerSound = drawer.transform.AddComponent<AudioSource>();
        drawerMoveClip = drawer.GetDrawerMoveClip;
        CheckClip(ref drawerMoveClip);
        drawerSound.clip = drawerMoveClip;
        drawerSound.loop = true;

        drawer.selectEntered.AddListener(OnDrawerMove);
        drawer.selectExited.AddListener(OnDrawerStop);

        drawerSocket = drawer.GetKeySocket;
        if (drawerSocket != null)
        {
            drawerSocketSound = drawerSocket.transform.AddComponent<AudioSource>();
            drawerSocketClip = drawer.GetSocketedClip;
            CheckClip(ref drawerSocketClip);
            drawerSocketSound.clip = drawerSocketClip;
            drawerSocket.selectEntered.AddListener(OnDrawerSocketed);
        }
    }

    private void OnDrawerSocketed(SelectEnterEventArgs arg0)
    {
        drawerSocketSound.Play();
    }

    private void SetCabinetDoors(int index)
    {
        cabinetDoorSound[index] = cabinetDoors[index].transform.AddComponent<AudioSource>();
        cabinetDoorMoveClip = cabinetDoors[index].GetHingeMoveClip;
        CheckClip(ref cabinetDoorMoveClip);
        cabinetDoorSound[index].clip = cabinetDoorMoveClip;
        cabinetDoors[index].OnHingeSelected.AddListener(OnDoorMove);
        cabinetDoors[index].selectExited.AddListener(OnDoorStop);

    }

    private void OnDoorStop(SelectExitEventArgs arg0)
    {
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (arg0.interactableObject == cabinetDoors[i])
            {
                cabinetDoorSound[i].Stop();
            }
        }
    }

    private void OnDoorMove(SimpleHindgeInteractable arg0)
    {
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (arg0 == cabinetDoors[i])
            {
                cabinetDoorSound[i].Play();
            }
        }
    }

    private void SetWall()
    {
        destroyWallClip = wall.GetDestroyClip;
        CheckClip(ref destroyWallClip);
        wall.OnDetroy.AddListener(OnDestroyWall);

        wallSocket = wall.GetWallSocket;
        if (wallSocket != null)
        {
            wallSocketSound = wallSocket.transform.AddComponent<AudioSource>();
            wallSocketClip = wall.GetSocketclip;
            CheckClip(ref wallSocketClip);
            wallSocketSound.clip = wallSocketClip;
            wallSocket.selectEntered.AddListener(OnWallSocketed);
        }
    }

    private void OnWallSocketed(SelectEnterEventArgs arg0)
    {
        wallSocketSound.Play();
    }

    private void SetGrabbables()
    {
        grabInteractables = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);
        for (int i = 0; i < grabInteractables.Length; i++)
        {
            grabInteractables[i].selectEntered.AddListener(OnSelectEnterGrabbable);
            grabInteractables[i].selectExited.AddListener(OnSelectExitGrabbable);
            grabInteractables[i].activated.AddListener(OnActivatedGrabbable);
        }
    }

    private void CheckClip(ref AudioClip clip)
    {
        if (clip == null)
        {
            clip = fallBackClip;
        }
    }

    private void OnDrawerStop(SelectExitEventArgs arg0)
    {
        drawerSound.Stop();
    }

    private void OnDrawerMove(SelectEnterEventArgs arg0)
    {
        drawerSound.Play();
    }

    private void OnActivatedGrabbable(ActivateEventArgs arg0)
    {
        GameObject tempGameObject = arg0.interactableObject.transform.gameObject;
        if (tempGameObject.GetComponent<WandControl>() != null)
        {
            activatedSound.clip = wandActivatedClip;
        }
        else
        {
            activatedSound.clip = grabActivatedClip;
        }
        activatedSound.Play();
    }

    private void OnSelectExitGrabbable(SelectExitEventArgs arg0)
    {
        grabSound.clip = grabClip;
        grabSound.Play();
    }

    private void OnSelectEnterGrabbable(SelectEnterEventArgs arg0)
    {
        if (arg0.interactableObject.transform.CompareTag("Key"))
        {
            grabSound.clip = keyClip;
        }
        else
        {
            grabSound.clip = grabClip;
        }
        grabSound.Play();
    }

    private void OnDestroyWall()
    {
        if (wallSound != null)
        {
            wallSound.Play();
        }
    }

    private void OnDisable()
    {
        if (wall != null)
        {
            wall.OnDetroy.RemoveListener(OnDestroyWall);
        }
    }
}
