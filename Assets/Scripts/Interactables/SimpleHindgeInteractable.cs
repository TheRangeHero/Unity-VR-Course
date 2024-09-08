using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class SimpleHindgeInteractable : XRSimpleInteractable
{
    [SerializeField] bool isLocked;
    [SerializeField] Vector3 positionLimits;
    private const string Default_Layer = "Default";
    private const string Grab_Layer = "Grab";

    private Transform grabHand;
    private Collider hingeCollider;
    private Vector3 hingePositions;
    protected virtual void Start()
    {
        hingeCollider = GetComponent<Collider>();
    }
    public void LockHinge()
    {
        isLocked = true;
    }
    public void UnlockHinge()
    {
        isLocked = false;
    }
    protected virtual void Update()
    {
        if (grabHand != null)
        {
            TrackHand();
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
        ResetHinge();
    }
    private void TrackHand()
    {
        transform.LookAt(grabHand, transform.forward);
        hingePositions = hingeCollider.bounds.center;
        if (grabHand.position.x >= hingePositions.x + positionLimits.x ||
            grabHand.position.x <= hingePositions.x - positionLimits.x)
        {
            ReleaseHinge();
            Debug.Log("******* FUCKKK XXXXXXX*********");
        }
        else if (grabHand.position.y >= hingePositions.y + positionLimits.y ||
            grabHand.position.y <= hingePositions.y - positionLimits.y)
        {
            ReleaseHinge();
            Debug.Log("******* FUCKKK       YYYYYYY*********");
        }
        else if (grabHand.position.z >= hingePositions.z + positionLimits.z ||
            grabHand.position.z <= hingePositions.z - positionLimits.z)
        {
            ReleaseHinge();
            Debug.Log("******* FUCKKK     ZZZZ*********");
        }
    }
    public void ReleaseHinge()
    {
        ChangeLayerMaks(Default_Layer);
    }

    protected abstract void ResetHinge();
    private void ChangeLayerMaks(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
