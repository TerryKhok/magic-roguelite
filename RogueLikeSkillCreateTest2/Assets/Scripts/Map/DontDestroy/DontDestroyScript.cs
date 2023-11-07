using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class DontDestroyScript : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
