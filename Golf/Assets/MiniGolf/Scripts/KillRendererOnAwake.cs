using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillRendererOnAwake : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        transform.GetComponent<Renderer>().enabled = false;
    }
}
