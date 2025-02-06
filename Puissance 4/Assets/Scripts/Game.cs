using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    int[,] grille = new int[7,6];
    private bool canPlay = true;
    private bool whosTurn = true;
    [SerializeField] private GameObject redCoin;
    [SerializeField] private GameObject yellowCoin;
    [SerializeField] private Transform column0;
    [SerializeField] private Transform column1;
    [SerializeField] private Transform column2;
    [SerializeField] private Transform column3;
    [SerializeField] private Transform column4;
    [SerializeField] private Transform column5;
    [SerializeField] private Transform column6;
    [SerializeField] private GameObject redTurn;
    [SerializeField] private GameObject yellowTurn;
    private void Start()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                grille[i, j] = 0;
            }
        }
    }

    public void AddCoin(int column)
    {
        if (canPlay)
        {
            for (int i = 0; i < 6; i++)
            {
                if (grille[column,i] == 0)
                {
                    SpawnCoin(column);
                    CheckWin(column, i);
                    if (whosTurn)
                    {
                        grille[column,i] = 1;
                        redTurn.SetActive(true);
                        yellowTurn.SetActive(false);
                    }
                    else
                    {
                        grille[column,i] = 2;
                        redTurn.SetActive(false);
                        yellowTurn.SetActive(true);
                    }
                    canPlay = false;
                    StartCoroutine(WaitForPlay());
                    whosTurn = !whosTurn;
                    return;
                }
            }
        }
    }

    IEnumerator WaitForPlay()
    {
        yield return new WaitForSeconds(0.8f);
        canPlay = true;
    }
    void SpawnCoin(int column)
    {
        Transform[] columns = { column0, column1, column2, column3, column4, column5, column6 };

        if (column >= 0 && column < columns.Length && whosTurn)
        {
            Instantiate(yellowCoin, columns[column].position, Quaternion.identity);
        }
        else if (column >= 0 && column < columns.Length && whosTurn == false)
        {
            Instantiate(redCoin, columns[column].position, Quaternion.identity);
        }
        
    }

    bool CheckWin(int column, int row)
    {
        // victoire colonne
        int cpt = 1;
        for (int i = 1; i < 6; i++)
        {
            if (grille[column, i] == grille[column, i-1] && grille[column, i] != 0)
            {
                cpt++;
            }
            else if (grille[column, i] != grille[column, i-1])
            {
                cpt = 1;
            }
            if (cpt >= 3)
            {
                Debug.Log("il y a une victoire");
                return true;
            }
        }
        // victoire ligne
        cpt = 1;
        for (int i = 1; i < 7; i++)
        {
            if (grille[i, row] == grille[i-1, row] && grille[i, row] != 0)
            {
                cpt++;
            }
            else if (grille[i, row] == grille[i-1, row])
            {
                cpt = 1;
            }
            if (cpt >= 3)
            {
                Debug.Log("il y a une victoire");
                return true;
            }
        }
        // victoire diagonale
        // check en mode le point et on verif ses 4 diagonales en -4 4, -4 -4 , 4 4 et 4 -4
        return false;
    }
}
