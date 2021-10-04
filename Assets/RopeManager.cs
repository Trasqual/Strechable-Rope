using System.Collections;
using UnityEngine;

public class RopeManager : MonoBehaviour
{
	public float averageLinkDistance { get; private set; }

	private Vector3[] currentRopePositions = new Vector3[15];
	private float[] ropeLerpArray = new float[15];
	private Vector3[] ropePositions = new Vector3[15];
	private float leftLerpValue = 0.1f;
	private float rightLerpValue = 0.1f;
	private Vector3[] previousRopePositions = new Vector3[15];
	private Quaternion[] currentRopeRotations = new Quaternion[15];
	private Quaternion[] ropeRotations = new Quaternion[15];
	private Quaternion[] previousRopeRotations = new Quaternion[15];
	public Rigidbody[] partnerRB;
	private Transform[] linkDisplay;
	private CapsuleCollider[] linkColliders;
	private Rigidbody[] linkRBs;
	public Transform[] links;
	public Transform[] ropeJoints;
    private bool isRespawningDontSmooth = true;

    private void Start()
	{
		linkColliders = new CapsuleCollider[links.Length];
		linkDisplay = new Transform[links.Length];
		linkRBs = new Rigidbody[links.Length];
		for (int i = 0; i < links.Length; i++)
		{
			linkColliders[i] = links[i].GetComponent<CapsuleCollider>();
			linkDisplay[i] = links[i].transform.GetChild(0);
			linkRBs[i] = links[i].GetComponent<Rigidbody>();
		}

		IniPreviousRopePositions();
		IniRopeLerpArray();
		StartCoroutine(SetRespawning(false));
	}

	//private void Update()
	//{
	//	averageLinkDistance = (Vector3.Distance(ropePositions[3], ropePositions[5]) + Vector3.Distance(ropePositions[9], ropePositions[11])) / 2f;
	//	UpdateLinkColliders(averageLinkDistance);
	//}

	private void LateUpdate()
	{
		UpdateLerpedRopePositions();
		UpdateRopeJoints();
	}

	private void IniRopeLerpArray()
	{
		for (int i = 0; i < ropeLerpArray.Length; i++)
		{
			float t = Mathf.InverseLerp(0f, (float)(ropeLerpArray.Length - 1), (float)i);
			ropeLerpArray[i] = Mathf.Lerp(leftLerpValue, rightLerpValue, t);
		}
	}

	private void IniPreviousRopePositions()
	{
		for (int i = 0; i < ropePositions.Length; i++)
		{
			ropePositions[i] = ropeJoints[i].position;
			previousRopePositions[i] = ropeJoints[i].position;
			currentRopePositions[i] = ropeJoints[i].position;
			ropeRotations[i] = ropeJoints[i].rotation;
			previousRopeRotations[i] = ropeJoints[i].rotation;
			currentRopeRotations[i] = ropeJoints[i].rotation;
			if (i == 0)
			{
				previousRopePositions[i] = partnerRB[0].transform.position;
				previousRopeRotations[i] = Quaternion.LookRotation(links[0].transform.position - partnerRB[0].transform.position);
			}
			else if (i < ropeJoints.Length - 2)
			{
				previousRopePositions[i] = links[i - 1].transform.position;
				previousRopeRotations[i] = Quaternion.LookRotation(links[i].transform.position - links[i - 1].transform.position);
			}
			else if (i < ropeJoints.Length - 1)
			{
				previousRopePositions[i] = links[i - 1].transform.position;
				previousRopeRotations[i] = Quaternion.LookRotation(partnerRB[1].transform.position - links[i - 1].transform.position);
			}
			else
			{
				previousRopePositions[i] = partnerRB[1].transform.position;
				previousRopeRotations[i] = ropeJoints[i - 1].rotation;
			}
		}
	}

	private void UpdateLerpedRopePositions()
	{
		for (int i = 0; i < ropePositions.Length; i++)
		{
			if (i == 0)
			{
				ropePositions[i] = partnerRB[0].transform.position;
				ropeRotations[i] = Quaternion.LookRotation(links[0].transform.position - partnerRB[0].transform.position);
			}
			else if (i < ropeJoints.Length - 2)
			{
				ropePositions[i] = links[i - 1].transform.position;
				ropeRotations[i] = Quaternion.LookRotation(links[i].transform.position - links[i - 1].transform.position);
			}
			else if (i < ropeJoints.Length - 1)
			{
				ropePositions[i] = links[i - 1].transform.position;
				ropeRotations[i] = Quaternion.LookRotation(partnerRB[1].transform.position - links[i - 1].transform.position);
			}
			else
			{
				ropePositions[i] = partnerRB[1].transform.position;
				ropeRotations[i] = ropeJoints[i - 1].rotation;
			}
		}
		for (int j = 0; j < ropePositions.Length; j++)
		{
			if (isRespawningDontSmooth)
			{
				currentRopePositions[j] = ropePositions[j];
				currentRopeRotations[j] = ropeRotations[j];
			}
			else
			{
				currentRopePositions[j] = Vector3.Lerp(previousRopePositions[j], ropePositions[j], ropeLerpArray[j]);
				currentRopeRotations[j] = Quaternion.Lerp(previousRopeRotations[j], ropeRotations[j], ropeLerpArray[j]);
			}
			currentRopePositions[j] = Vector3.Lerp(previousRopePositions[j], ropePositions[j], ropeLerpArray[j]);
			currentRopeRotations[j] = Quaternion.Lerp(previousRopeRotations[j], ropeRotations[j], ropeLerpArray[j]);
			
			previousRopePositions[j] = currentRopePositions[j];
			previousRopeRotations[j] = currentRopeRotations[j];
		}
	}

	private void UpdateRopeJoints()
	{
		for (int i = 0; i < ropeJoints.Length-1; i++)
		{
			ropeJoints[i].position = currentRopePositions[i];
			ropeJoints[i].rotation = currentRopeRotations[i];
		}
	}

	private void UpdateLinkColliders(float distance)
	{
		float num;
		if (distance < 0.82f)
		{
			num = 0.85f;
		}
		else if (distance < 1.1f)
		{
			distance = Mathf.InverseLerp(0.82f, 1.1f, distance);
			num = Mathf.Lerp(0.85f, 1.7f, distance);
		}
		else
		{
			num = 1.7f;
		}
		for (int i = 0; i < links.Length; i++)
		{
			linkColliders[i].height = num;
			linkDisplay[i].localScale = new Vector3(0.16666667f, num / 2f / 3f, 0.16666667f);
		}
	}

	private IEnumerator SetRespawning(bool respawning)
    {
		yield return new WaitForSeconds(0.1f);
		isRespawningDontSmooth = respawning;
    }
}
