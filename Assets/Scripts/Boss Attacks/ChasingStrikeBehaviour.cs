using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingStrikeBehaviour : MonoBehaviour
{

    private float _deltaTime; 
    // Start is called before the first frame update
    void Start()
    {
        _deltaTime = 0.75f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_deltaTime >= 0){
            _deltaTime -= Time.deltaTime;
        }else if (gameObject.transform.position.y < 0){
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 5.0f, gameObject.transform.position.z);
            _deltaTime = 1.0f;
        }else{
            Destroy(gameObject);
        }

    }
}
