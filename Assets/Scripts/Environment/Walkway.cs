using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walkway : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "PlayerArmature")
        {
            GameObject walkway = GameObject.Find("Walkway");
            Destroy(walkway);
            Destroy(gameObject);
        }
    }
}
