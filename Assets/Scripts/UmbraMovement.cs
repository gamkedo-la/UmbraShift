using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(PlayerController))]
public class UmbraMovement : MonoBehaviour
{
    NavMeshAgent agent;
    PlayerController pc;
    Vector3 realScale;
    Vector3 realColor;

    
    private bool moving;
    private Vector3 velocity = Vector3.zero;
    private Vector3 colorVelocity = Vector3.zero;
    private Vector3 color = Vector3.zero;
    private Vector4 tempColor = Vector4.zero;

    private Vector3 shadowScale;
    Renderer rend;

    public float shiftTime = 0.25F;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        pc = this.GetComponent<PlayerController>();
        realScale = transform.localScale;
        moving = false;
        shadowScale = new Vector3(transform.localScale.x, 0.01f, transform.localScale.z);
        rend = GetComponent<Renderer>();
        realColor = new Vector3(rend.material.GetColor("_BaseColor").r, rend.material.GetColor("_BaseColor").g, rend.material.GetColor("_BaseColor").b);
    }


    void Update()
    {
        // If player is currentPlayerController, and player is not at destination
        if (pc == InputManager.instance.currentPlayerController && (transform.position.x != agent.destination.x || transform.position.z != agent.destination.z) && !moving)
        {
            StartCoroutine(UmbraShift(shiftTime));
            moving = true;
        }
            
        
    }

    // Rescale object to 0.01 on y, change color to black
    private IEnumerator UmbraShift(float t)
    {
        while(transform.localScale.y > 0.011f)
        {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, shadowScale, ref velocity, shiftTime);
            tempColor = rend.material.GetColor("_BaseColor");
            color = Vector3.SmoothDamp(new Vector3(tempColor.x, tempColor.y, tempColor.z), new Vector3(0, 0, 0), ref colorVelocity, shiftTime);
            rend.material.SetColor("_BaseColor", new Color(color.x, color.y, color.z));
            yield return new WaitForEndOfFrame();
        }
        
        // Waits until object is at destination
        while (transform.position.x != agent.destination.x || transform.position.z != agent.destination.z)
            yield return new WaitForEndOfFrame();
        StartCoroutine(ReSize());
        yield return null;
    }

    // After motion stops, rescale object and return color to normal
    private IEnumerator ReSize()
    {

        while(transform.localScale.y < realScale.y - 0.001)
        {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, realScale, ref velocity, shiftTime);
            tempColor = rend.material.GetColor("_BaseColor");
            color = Vector3.SmoothDamp(new Vector3(tempColor.x, tempColor.y, tempColor.z), realColor, ref colorVelocity, shiftTime);
            rend.material.SetColor("_BaseColor", new Color(color.x, color.y, color.z));
            yield return new WaitForEndOfFrame();
            print(velocity);
        }
        moving = false;
    }
}
