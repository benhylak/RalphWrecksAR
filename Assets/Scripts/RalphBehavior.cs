using UnityEngine;
using DG.Tweening;

public class RalphBehavior: MonoBehaviour
{
    public Transform camera;
    public GameObject head;

	private Animator m_AnimationController;

	public GameObject brickInHand;
	public GameObject brickPrefab;

	public GameManager gameManager;

	public bool LookAt = false;
	public bool outOfRange = false;

	public bool hasEntered = false;

	public float nextUpdateTime = 0f;

	public Quaternion lastRotation = Quaternion.identity;

    void Start()
    {
		m_AnimationController = GetComponent<Animator>();
    }

    void Update()
    {
		if(hasEntered && Time.time > nextUpdateTime)
		{
			ThrowBrick();
			nextUpdateTime = Time.time + Random.Range(8, 15);
		}
    }

	public void ThrowBrick()
	{
		brickInHand.SetActive(true);
		m_AnimationController.SetTrigger("Throw");
	}

	public void ReleaseBrick()
	{
		var newBrick = Instantiate(brickPrefab).GetComponent<Brick>();
		
		var targetWindow = gameManager.windows.PickRandom();

		newBrick.SetOrigin(brickInHand.transform);
		newBrick.SetTargetWindow(targetWindow.GetComponent<Window>());

		newBrick.transform.localScale = brickInHand.transform.lossyScale;
	
		brickInHand.SetActive(false);
	}

    public void GrandEntrance()
    {
        GetComponentInChildren<Collider>().isTrigger = true;

        transform
            .DOScale(new Vector3(1.45f, 1.45f, 1.45f), 1.2f)
            .SetEase(Ease.OutElastic)
            .Play();

		hasEntered = true;

		nextUpdateTime = Time.time + Random.Range(8, 15);
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
		if(a > 50f) outOfRange = true;
		else if(a < 35f) outOfRange = false;
		
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