using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Debugger : MonoBehaviour
{
    [SerializeField] float emergencyInterval;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(Emergency());
        }
    }

    IEnumerator Emergency()
    {
        Props[] props = FindObjectsOfType<Props>();
        foreach (var prop in props)
        {
            prop.Test();
        }

        yield return new WaitForSeconds(emergencyInterval);

        foreach (var prop in props)
        {
            prop.Test2();
        }
    }
}
