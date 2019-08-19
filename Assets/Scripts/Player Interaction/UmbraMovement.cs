using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(PlayerController))]
public class UmbraMovement : MonoBehaviour
{
    NavMeshAgent agent;
    PlayerController pc;

	[SerializeField] Light[] lights;
    
    public bool isMoving = false;
    Vector3 velocity = Vector3.zero;
    Vector3 colorVelocity = Vector3.zero;
    Vector3 color = Vector3.zero;
    
    Vector3 realColor;
    Vector3 realScale;

    Vector3 shadowScale;
    Renderer rend;

   

    bool lightsOn = true;

    public float shiftTime = 0.25F;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        pc = this.GetComponent<PlayerController>();

        realScale = transform.localScale;
        shadowScale = new Vector3(transform.localScale.x, 0.01f, transform.localScale.z);

        rend = GetComponent<Renderer>();
        realColor = ToVector3(rend.material.GetColor("_BaseColor"));

       
    }


    void Update()
    {
        // If player is currentPlayerController, and player is not at destination
        if (pc == TurnManager.instance.ActivePlayerController && agent.remainingDistance > agent.stoppingDistance && !isMoving)
        {
            StartCoroutine(UmbraShift(shiftTime));
            isMoving = true;
			pc.agent.isStopped = true;
        }

               
    }

    // Rescale object to 0.01 on y, change color to black
    private IEnumerator UmbraShift(float t)
    {
		foreach (Light light in lights) { light.enabled = false; }
        while(!Mathf.Approximately(transform.localScale.y, 0.01f)) // Because SmoothDamp only gets arbitrarily close to the target value
        {
	
			if (transform.localScale.y <= 0.1f)	{ pc.agent.isStopped = false; }
			else { pc.agent.isStopped = true; }

			transform.localScale = Vector3.SmoothDamp(transform.localScale, shadowScale, ref velocity, shiftTime);
            
            color = Vector3.SmoothDamp(ToVector3(rend.material.GetColor("_BaseColor")), Vector3.zero, ref colorVelocity, shiftTime);
            rend.material.SetColor("_BaseColor", new Color(color.x, color.y, color.z));


            yield return new WaitForEndOfFrame();
        }


        // Wait until object is at destination
        while(agent.velocity != Vector3.zero) { yield return new WaitForEndOfFrame(); }

        StartCoroutine(ReSize());
		foreach (Light light in lights) { light.enabled = true; }
		yield return null;
    }

    // After motion stops, rescale object and return color to normal
    private IEnumerator ReSize()
    {
        // Setting moving here allows for the player to move again before completing the animation back up to normal size
        isMoving = false;
        while(!Mathf.Approximately(transform.localScale.y, realScale.y) && isMoving == false)
        {
            pc.agent.isStopped = true;

            transform.localScale = Vector3.SmoothDamp(transform.localScale, realScale, ref velocity, shiftTime);
            
            color = Vector3.SmoothDamp(ToVector3(rend.material.GetColor("_BaseColor")), realColor, ref colorVelocity, shiftTime);
            rend.material.SetColor("_BaseColor", new Color(color.x, color.y, color.z));

            yield return new WaitForEndOfFrame();
        }
        pc.agent.isStopped = false;
    }

    // For more convenient color handling
    private Vector3 ToVector3(Vector4 v4)
    {
        return new Vector3(v4.x, v4.y, v4.z);
    }
}
