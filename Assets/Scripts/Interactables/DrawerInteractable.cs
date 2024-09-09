using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class DrawerInteractable : XRGrabInteractable
{
    [SerializeField] Transform drawerTransform;
    [SerializeField] XRSocketInteractor keySocket;
    [SerializeField] XRPhysicsButtonInteractable physicsButton;
    [SerializeField] GameObject keyIndicatorLight;
    [SerializeField] bool isLocked;
    [SerializeField] bool isDetachable;
    [SerializeField] bool isDetached;
    [SerializeField] private Vector3 limitDistance = new Vector3(.02f, .02f, 0);
    [SerializeField] float drawerLimitZ = 0.8f;
    [SerializeField] AudioClip drawerMoveClip;
    [SerializeField] AudioClip socketedClip;

    private Transform parentTransform;
    private const string Default_Layer = "Default";
    private const string Grab_Layer = "Grab";
    private bool isGrabbed;
    private Vector3 limitPositions;

    public AudioClip GetDrawerMoveClip => drawerMoveClip;
    public AudioClip GetSocketedClip => socketedClip;
    public XRSocketInteractor GetKeySocket => keySocket;
    public XRPhysicsButtonInteractable GetPhysicsButton => physicsButton;

    public UnityEvent OnDrawerDetach;

    void Start()
    {
        if (keySocket != null)
        {
            keySocket.selectEntered.AddListener(OnDrawerUnlocked);
            keySocket.selectExited.AddListener(OnDrawerLocked);
        }
        parentTransform = transform.parent.transform;
        limitPositions = drawerTransform.localPosition;

        if (physicsButton != null)
        {
            physicsButton.OnBaseEnter.AddListener(OnIsDetachable);
            physicsButton.OnBaseExit.AddListener(OnisNotDetachable);
        }
    }

    private void OnIsDetachable()
    {
        isDetachable = true;
    }

    private void OnisNotDetachable()
    {
        isDetachable = false;
    }

    private void OnDrawerLocked(SelectExitEventArgs arg0)
    {
        isLocked = true;
        Debug.Log("***** Drawer Locked");
    }

    private void OnDrawerUnlocked(SelectEnterEventArgs arg0)
    {
        isLocked = false;
        if (keyIndicatorLight != null)
        {
            keyIndicatorLight.SetActive(false);
        }
        Debug.Log("***** Drawer Unlocked");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (!isLocked)
        {
            transform.SetParent(parentTransform);
            isGrabbed = true;
        }
        else
        {
            ChangeLayerMaks(Default_Layer);
        }

    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        ChangeLayerMaks(Grab_Layer);
        isGrabbed = false;

        transform.localPosition = drawerTransform.localPosition;
    }

    void Update()
    {
        if (!isDetached)
        {
            if (isGrabbed && drawerTransform != null)
            {
                drawerTransform.localPosition = new Vector3(drawerTransform.localPosition.x, drawerTransform.localPosition.y, transform.localPosition.z);

                CheckLimits();
            }
        }

    }

    private void CheckLimits()
    {
        if (transform.localPosition.x >= limitPositions.x + limitDistance.x ||
            transform.localPosition.x <= limitPositions.x - limitDistance.x)
        {
            ChangeLayerMaks(Default_Layer);
        }
        else if (transform.localPosition.y >= limitPositions.y + limitDistance.y ||
                transform.localPosition.y <= limitPositions.y - limitDistance.y)
        {
            ChangeLayerMaks(Default_Layer);
        }
        else if (drawerTransform.localPosition.z <= limitPositions.z - limitDistance.z)
        {
            isGrabbed = false;
            drawerTransform.localPosition = limitPositions;
            ChangeLayerMaks(Default_Layer);
        }
        else if (drawerTransform.localPosition.z >= drawerLimitZ + limitDistance.z)
        {
            if (!isDetachable)
            {
                isGrabbed = false;
                drawerTransform.localPosition = new Vector3(
                    drawerTransform.localPosition.x,
                    drawerTransform.localPosition.y,
                    drawerLimitZ
                );
                ChangeLayerMaks(Default_Layer);
            }
            else
            {
                DetachDrawer();
            }
        }

    }

    private void DetachDrawer()
    {
        isDetached = true;
        drawerTransform.SetParent(this.transform);

        OnDrawerDetach?.Invoke();
    }

    private void ChangeLayerMaks(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
