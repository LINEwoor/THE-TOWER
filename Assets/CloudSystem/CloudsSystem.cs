using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsSystem : MonoBehaviour
{
    [SerializeField] GameObject[] cloudsPrefabs;
    [SerializeField] private float cloudSpeed;
    [SerializeField] int maxClouds;
    
    [SerializeField] int startPositionX;
    [SerializeField] int endPositionX;
    
    [SerializeField] int startPositionZ;
    [SerializeField] int endPositionZ;
    
    List<GameObject> clouds = new List<GameObject>();
    bool isCreatingCloud = false;

    void Update()
    {
        if (clouds.Count < maxClouds && !isCreatingCloud)
        {
            StartCoroutine(CreateCloud());
        }

        for (int i = clouds.Count - 1; i >= 0; i--)
        {
            if (clouds[i] == null)
            {
                clouds.RemoveAt(i);
                continue;
            }
            
            if (clouds[i].transform.position.x <= endPositionX)
            {
                Destroy(clouds[i]);
                clouds.RemoveAt(i);
            }
        }
    }

    IEnumerator CreateCloud()
    {
        isCreatingCloud = true;
        yield return new WaitForSeconds(Random.Range(0f, 3f));
        
        float randomZ = Random.Range(startPositionZ, endPositionZ + 1);
        
        GameObject cloud = Instantiate(
            cloudsPrefabs[Random.Range(0, cloudsPrefabs.Length)], 
            new Vector3(startPositionX, 50, randomZ), 
            transform.rotation
        );
        float randSize = Random.Range(0.5f, 2f);
        cloud.transform.localScale = new Vector3(randSize, randSize, randSize);
        
        CloudScript cloudScript = cloud.GetComponent<CloudScript>();
        if (cloudScript != null)
        {
            cloudScript.speed = -cloudSpeed;
        }
        
        clouds.Add(cloud);
        isCreatingCloud = false;
    }
}