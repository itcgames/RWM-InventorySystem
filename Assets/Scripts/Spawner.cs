using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Transform placeToSpawn;
    public float timeToWait;
    // Start is called before the first frame update
    void Start()
    {
        objectToSpawn = Instantiate(objectToSpawn);
        objectToSpawn.SetActive(false);
        placeToSpawn = objectToSpawn.transform;
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while(true)
        {
            yield return new WaitForSeconds(timeToWait);
            GameObject obj = Instantiate(objectToSpawn);
            obj.SetActive(true);
        }
    }
}
