using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zm
{
    public class DontDestroyOnLoadScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this);
        }


    }
}