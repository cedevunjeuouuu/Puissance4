using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomIa : MonoBehaviour
{
    [SerializeField] private Game gameReference;
    [SerializeField] private float waitBeforeIaPlay;

    public void IaTurn()
    {
        StartCoroutine(Play());
    }
    IEnumerator Play()
    {
        yield return new WaitForSeconds(waitBeforeIaPlay);
        
        gameReference.AddCoin(Random.Range(0,7));
    }
}
