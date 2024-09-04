using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableFloor : MonoBehaviour
{
    public GameObject objectToDestroy;     
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.Dmg == 2)
        {
            Destroy(objectToDestroy);
        }
    }
}
