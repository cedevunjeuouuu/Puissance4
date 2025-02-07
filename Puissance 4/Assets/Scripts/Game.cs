using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    int[,] grille = new int[7,6];
    private bool canPlay = true;
    private bool whosTurn;
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
                    // faudras faire des trucs ici avec ça en mode if check win ou if equality tu captes bg
                    CheckWin(column, i);
                    Equality();
                    StartCoroutine(WaitForPlay());
                    whosTurn = !whosTurn;
                    return;
                }
            }
        }
    }

    IEnumerator WaitForPlay()
    {
        yield return new WaitForSeconds(0.5f);
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
        // Victoire colonne
        int cpt = 1;
        for (int i = 1; i < 6; i++)
        {
            if (grille[column, i] == grille[column, i - 1] && grille[column, i] != 0)
            {
                cpt++;
            }
            else
            {
                cpt = 1;
            }

            if (cpt >= 4)
            {
                Debug.Log("Il y a une victoire en colonne");
                return true;
            }
        }

        // Victoire ligne
        cpt = 1;
        for (int i = 1; i < 7; i++)
        {
            if (grille[i, row] == grille[i - 1, row] && grille[i, row] != 0)
            {
                cpt++;
            }
            else
            {
                cpt = 1;
            }

            if (cpt >= 4)
            {
                Debug.Log("Il y a une victoire en ligne");
                return true;
            }
        }

        // Victoire diagonale
        if (CheckDiagonalWin(column, row))
        {
            Debug.Log("Il y a une victoire en diagonale");
            return true;
        }

        return false;
    }
    
    bool CheckDiagonalWin(int column, int row)
    {
        int player = grille[column, row];
        if (player == 0) return false; 
        int count = 1;
        int i = column - 1, j = row - 1;
        while (i >= 0 && j >= 0 && grille[i, j] == player)
        {
            count++;
            i--;
            j--;
        }
        i = column + 1;
        j = row + 1;
        while (i < 7 && j < 6 && grille[i, j] == player)
        {
            count++;
            i++;
            j++;
        }
        if (count >= 4) return true;
        count = 1;
        i = column - 1;
        j = row + 1;
        while (i >= 0 && j < 6 && grille[i, j] == player)
        {
            count++;
            i--;
            j++;
        }
        i = column + 1;
        j = row - 1;
        while (i < 7 && j >= 0 && grille[i, j] == player)
        {
            count++;
            i++;
            j--;
        }
        return count >= 4;
    }

    bool Equality()
    {
        for (int i = 0; i < 7; i++)
        {
            if (grille[i,5] == 0)
            {
                return false;
            }
        }
        Debug.Log("egalité");
        return true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}