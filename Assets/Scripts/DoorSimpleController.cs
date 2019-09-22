using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSimpleController : MonoBehaviour

       
{
    public int exitToLevel;


    public bool doorOpen=false;

   public void PlayOpeningSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(SoundConfiguration.instance.doorSimpleOpening,gameObject);
    }

    public void PlayClosingSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(SoundConfiguration.instance.doorSimpleClosing,gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        AgentMovement player = other.GetComponent<AgentMovement>();


        if (player != null)
        {
            GetComponent<Animator>().SetBool("isOpen", true);
            doorOpen = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        AgentMovement player = other.GetComponent<AgentMovement>();


        if (player != null)
        {
            GetComponent<Animator>().SetBool("isOpen", false);
            doorOpen = false;
        }
    }

    private void Update()
    {
        if (GetComponent<Animator>().GetBool("isOpen")&& Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(exitToLevel);
        }
    }





}
