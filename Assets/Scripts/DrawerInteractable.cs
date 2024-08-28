using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawerInteractable : MonoBehaviour
{
    [SerializeField] XRSocketInteractor keySocket;
    [SerializeField] bool isLocked;
    // Start is called before the first frame update
    void Start()
    {
        if (keySocket != null)
        {
            keySocket.selectEntered.AddListener(OnDrawerUnlocked);
            keySocket.selectExited.AddListener(OnDrawerLocked);
        }
    }

    private void OnDrawerLocked(SelectExitEventArgs arg0)
    {
        isLocked = true;
        Debug.Log("***** Drawer Locked");
    }

    private void OnDrawerUnlocked(SelectEnterEventArgs arg0)
    {
        isLocked = false;
        Debug.Log("***** Drawer Unlocked");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
