using UnityEngine;
using DG.Tweening;

public class RalphBehavior: MonoBehaviour
{
    public Transform camera;
    public GameObject head;


	public bool LookAt = false;
	public bool outOfRange = false;

	public Quaternion lastRotation = Quaternion.identity;

    void Start()
    {

    }

    void Update()
    {

    }

    public void GrandEntrance()
    {
        GetComponentInChildren<Collider>().isTrigger = true;

        transform
            .DOScale(new Vector3(1.5f, 1.5f, 1.5f), 1.2f)
            .SetEase(Ease.OutElastic)
            .Play();
    }

    void LateUpdate()
	{
		if(LookAt) TryLookAtCamera();
	
		if(!LookAt || outOfRange)
		{
			lastRotation = Quaternion.Slerp(lastRotation, head.transform.rotation, 0.2f);
		}

		head.transform.rotation = lastRotation;
	}
	

   private void TryLookAtCamera()
	{
		var dir = camera.transform.position - head.transform.position;
		var targetRot = Quaternion.LookRotation(dir) * Quaternion.Euler(45f, 0, 0);
		//targetRot *= Quaternion.Euler(0f, -90f, -90f); //flip for orientation

		//constrain
		var identity = (head.transform.parent != null) ? head.transform.parent.rotation : Quaternion.identity;
		var a = Quaternion.Angle(identity, targetRot);

		//deadband of 60<->90, prevents flickering
		if(a > 90f) outOfRange = true;
		else if(a < 60f) outOfRange = false;
		
		outOfRange = false;

		if (!outOfRange)
		{
			if(lastRotation == Quaternion.identity) //initialize lastRotation if a perfect identity quaternion
			{
				lastRotation = head.transform.rotation;
			}

			lastRotation = Quaternion.Slerp(lastRotation, targetRot, 0.3f);
			head.transform.rotation = lastRotation;
		}
	}

}