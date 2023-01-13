using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameObject islandPrefab, treasurePrefab, turretPrefab, chestPrefab, Paths;
    public GameObject[] enemyPrefabs;
    public Transform enemies;
    public int countOfIslands, border, minSize, maxSize, sizeOfConvoy, maxCounOfEnemies;
    public Vector3[] spawnPointForShip;
    public Material[] materials, probabilityOfMaterials;
    public byte[] treasureIndex;
    public bool doNotTouchPlayer;
    bool canCallConvoy = true;

    void Start()
    {
        Time.timeScale = 1f;
        SpawnShip();
        Invoke("SpawnTreasure", 10f);
    }

    void SpawnShip()
    {
        if (enemies.childCount < maxCounOfEnemies)
            for (int i = 0; i < Random.Range(1, 6); i++)
                GenerateShip(0, enemyPrefabs.Length - 1);
        Invoke("SpawnShip", 10f);
    }

    GameObject GenerateShip(int startIndex, int endIndex)
    {
        int probability = Random.Range(startIndex, endIndex);
        GameObject curEnemy = curEnemy = Instantiate(enemyPrefabs[probability], spawnPointForShip[Random.Range(0, spawnPointForShip.Length)], Quaternion.identity, enemies);
        curEnemy.GetComponent<EnemyScript>().Path = Paths.transform.GetChild(Random.Range(0, Paths.transform.childCount));
        return curEnemy;
    }

    void SpawnTreasure()
    {
        bool retry = false;
        byte possibleIndex = (byte)Random.Range(0, transform.GetChild(0).childCount);
        foreach(byte el in treasureIndex)
        {
            if (possibleIndex == el)
            {
                retry = true;
            }
        }
        if (!retry)
        {
            for (int i = 0; i < treasureIndex.Length; i++)
            {
                if (treasureIndex[i] == 0)
                {
                    treasureIndex[i] = possibleIndex;
                    Transform isle = transform.GetChild(0).GetChild(possibleIndex);
                    bool type = Random.Range(0, 2) == 0 ? false : true;
                    if (type)
                    {
                        GameObject curTreasure = Instantiate(treasurePrefab, isle.position, Quaternion.identity, isle);
                        curTreasure.transform.Rotate(Vector3.up, Random.Range(0, 360));
                        curTreasure.name = "" + possibleIndex;
                    }
                    else
                    {
                        Transform curTurret = Instantiate(turretPrefab, Vector3.zero, Quaternion.identity).transform;
                        curTurret.GetChild(1).GetComponent<TurretScript>().level = this;
                        curTurret.parent = isle;
                    }
                    break;
                }
            }
            Invoke("SpawnTreasure", 10f);
        }
        else SpawnTreasure();
    }

    public void GenerateChest(Vector3 pos, Quaternion rot)
    {
        Instantiate(chestPrefab, pos, rot);
    }

    public void SpawnConvoy(Transform target)
    {
        if (canCallConvoy)
        {
            canCallConvoy = false;
            StartCoroutine(CoolDown(10f, 1));
            for (int i = 0; i < sizeOfConvoy; i++)
            {
                GameObject curEnemy = GenerateShip(enemyPrefabs.Length - 1, enemyPrefabs.Length);
                curEnemy.GetComponent<EnemyScript>().target = target;
                curEnemy.GetComponent<EnemyScript>().convoyShip = true;
            }
        }
    }

    public void SafeZone()
    {
        doNotTouchPlayer = true;
        StartCoroutine(CoolDown(3f, 0));
    }

    IEnumerator CoolDown(float delay, byte type)
    {
        yield return new WaitForSeconds(delay);
        switch(type)
        {
            case 0:
                doNotTouchPlayer = !doNotTouchPlayer;
                break;
            case 1:
                canCallConvoy = !canCallConvoy;
                break;
        }
    }
}
