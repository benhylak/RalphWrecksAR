using UnityEngine;

public class Brick : MonoBehaviour
{
    private Vector3 targetPos;

    float speed = 3.5f;

    public void Start()
    {

    }

    public void Update()
    {
        if(targetPos != default(Vector3))
        {
            float step = speed * Time.deltaTime;

            this.transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

            if( Vector3.Magnitude(this.transform.position - targetPos) < 0.1 )
            {
                targetPos = default(Vector3);
            }
        }
    }

    public void SetOrigin(Transform start)
    {
        this.transform.position = start.position;
        this.transform.rotation = start.rotation;
    }

    public void SetTargetWindow(Window window)
    {
        targetPos = window.transform.position - window.transform.forward * 0.1f;
        window.bricks.Add(this);
    }
}