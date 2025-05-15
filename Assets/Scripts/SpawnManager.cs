using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    [SerializeField]
    private GameObject Enemy;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
        
    }

    IEnumerator SpawnRoutine()
    {
        Vector3 spawnPos = new Vector3(537, 0, 635);
        bool _spawnEnemies = true;
        while (_spawnEnemies == true)
        {
            yield return new WaitForSeconds(5f);

           
            spawnPos.x += Random.Range(-8f, 8f);
            spawnPos.z += Random.Range(-8f, 8f);
            spawnPos.y = 3f;
            Instantiate(Enemy, spawnPos, Quaternion.identity);
        }
    }
}
