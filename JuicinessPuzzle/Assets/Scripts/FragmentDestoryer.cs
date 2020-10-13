using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentDestoryer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyFragmentsAfter());
    }

    // Update is called once per frame

    public IEnumerator DestroyFragmentsAfter()
    {
        yield return new WaitForSeconds(5f);

        Destroy(gameObject);
    }
}
