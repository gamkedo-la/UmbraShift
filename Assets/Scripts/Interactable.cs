using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private Material defaultMat;
    private Renderer currentRenderer;

    // Start is called before the first frame update
    void Start()
    {
        currentRenderer = GetComponent<Renderer>();
        defaultMat = currentRenderer.sharedMaterial;  // might need to do individual, check later
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ObjectDeselected()
    {
//        Debug.Log("Object has become deselected!");
        currentRenderer.material = defaultMat;
    }

    public void ObjectSelected()
    {
//        Debug.Log("Setting highlight material");
        currentRenderer.material = InputManager.instance.highlightedMat;
    }
}
