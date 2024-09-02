using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleHindgeInteractable : XRSimpleInteractable
{
    [SerializeField] bool isLocked;
    private const string Default_Layer = "Default";
    private const string Grab_Layer = "Grab";

    private Transform grabHand;
    void Start()
    {

    }

    public void LockHinge()
    {
        isLocked = true;
    }
    public void UnlockHinge()
    {
        isLocked = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (grabHand != null)
        {
            transform.LookAt(grabHand, transform.forward);
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!isLocked)
        {
            base.OnSelectEntered(args);
            grabHand = args.interactorObject.transform;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        grabHand = null;
        ChangeLayerMaks(Grab_Layer);
    }

    public void ReleaseHinge()
    {
        ChangeLayerMaks(Default_Layer);
    }

    private void ChangeLayerMaks(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
