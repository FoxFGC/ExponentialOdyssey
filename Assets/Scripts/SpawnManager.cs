using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawner : MonoBehaviour
{

    public BoxCollider spawnArea;

    public GameObject slimePrefab;

    public float spawnInterval = 2f;

    public int maxSlimes = 10;

    private int currentSlimes = 0;

    void Start()
    {
        if (spawnArea == null)
            spawnArea = GetComponent<BoxCollider>();

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (currentSlimes < maxSlimes)
                SpawnOne();
        }
    }

    void SpawnOne()
    {


        Bounds b = spawnArea.bounds;

        //makes sure the slime doesnt spawn inside the ground incase the prefab is not configured correctly
        Vector3 randomPos = new Vector3(
            Random.Range(b.min.x, b.max.x),
            b.max.y, 
            Random.Range(b.min.z, b.max.z)
        );

        if (Physics.Raycast(randomPos, Vector3.down, out var hit, b.size.y + 1f))
        {
            Instantiate(slimePrefab, hit.point, Quaternion.identity);
            currentSlimes++;
        }
    }


    public void NotifySlimeDied()
    {
        currentSlimes = Mathf.Max(0, currentSlimes - 1);
    }


    void OnDrawGizmosSelected()
    {
        if (spawnArea != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnArea.bounds.center, spawnArea.bounds.size);
        }
    }
}