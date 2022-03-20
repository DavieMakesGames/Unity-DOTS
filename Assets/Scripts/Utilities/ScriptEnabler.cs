using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptEnabler : MonoBehaviour
{
    public MonoBehaviour script;
    public bool Enable = true;
    public float Delay = .25f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ActivateScript", Delay);
    }
    void ActivateScript()
    {
        script.enabled = Enable;
    }
}
