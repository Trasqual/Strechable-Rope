using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeColliderAndSizeAdjuster : MonoBehaviour
{
    [SerializeField] Transform[] ropeParts;
    CapsuleCollider[] ropeColliders;
    Rigidbody[] ropeRBs;
    Transform[] ropeDisplay;
    [SerializeField] Vector3[] ropePositions;
    private float averageLinkDistance;

    private void Awake()
    {
        ropeDisplay = new Transform[ropeParts.Length];
        ropePositions = new Vector3[ropeParts.Length];
        ropeRBs = new Rigidbody[ropeParts.Length];
        ropeColliders = new CapsuleCollider[ropeParts.Length];
        for (int i = 0; i < ropeParts.Length; i++)
        {
            ropeRBs[i] = ropeParts[i].GetComponent<Rigidbody>();
            ropeDisplay[i] = ropeParts[i].GetChild(0);
            ropeColliders[i] = ropeParts[i].GetComponent<CapsuleCollider>();
            ropePositions[i] = ropeRBs[i].position;
        }
    }

    private void Update()
    {
        for (int i = 0; i < ropeParts.Length; i++)
        {
            ropePositions[i] = ropeRBs[i].position;
        }
        averageLinkDistance = (Vector3.Distance(ropePositions[3], ropePositions[5]) + Vector3.Distance(ropePositions[9], ropePositions[11])) / 2f;
        UpdateCollidersAndSize(averageLinkDistance);
        UpdateRotations();
    }

    private void UpdateCollidersAndSize(float distance)
    {
        float num;
        if (distance < 0.4f)
        {
            num = 0.4f;
        }
        else if (distance < 1.1f)
        {
            distance = Mathf.InverseLerp(0.4f, 1.1f, distance);
            num = Mathf.Lerp(0.4f, 1.7f, distance);
        }
        else
        {
            num = 1.7f;
        }
        for (int i = 0; i < ropeParts.Length; i++)
        {
            ropeColliders[i].height = num/2;
            ropeDisplay[i].localScale = new Vector3(0.16666667f, 0.16666667f, num / 2f);
        }
    }

    private void UpdateRotations()
    {
        for (int i = 1; i < ropeParts.Length; i++)
        {
            ropeRBs[i].rotation = Quaternion.LookRotation(ropeRBs[i-1].position - ropeRBs[i].position);
        }
    }
}
