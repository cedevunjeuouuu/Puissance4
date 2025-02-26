using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    #region Variables
    
    public int[,] grille = new int[7,6];
    private bool canPlay = true;
    [SerializeField] private bool haveRandomIa;
    [SerializeField] private RandomIa randomIaReference;
    [SerializeField] private bool haveIa;
    [SerializeField] private Ia IaReference;
    public bool whosTurn;
    private Stack<int[,]> playedTurns = new Stack<int[,]>();
    private Stack<GameObject> playedTurnsGo = new Stack<GameObject>();
    private Stack<int[,]> turnsRedo = new Stack<int[,]>();
    private Stack<GameObject> turnsRedoGo = new Stack<GameObject>();
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
    
    #endregion
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
    #region Game
    public void AddCoin(int column)
    {
        bool fullColumn = true;
        if (canPlay)
        {
            for (int i = 0; i < 6; i++)
            {
                if (grille[column,i] == 0)
                {
                    fullColumn = false;
                    DontRedo();
                    playedTurns.Push(CloneGrid(grille));
                    SpawnCoin(column);
                    if (whosTurn)
                    {
                        grille[column,i] = 1;
                    }
                    else
                    {
                        grille[column,i] = 2;
                    }
                    RefreshUi();
                    canPlay = false;
                    CheckWin(column, i, grille);
                    Equality();
                    StartCoroutine(WaitForPlay());
                    whosTurn = !whosTurn;
                    if (haveRandomIa && whosTurn)
                    {
                        randomIaReference.IaTurn();
                    }
                    if (haveIa && whosTurn)
                    {
                        IaReference.IaTurn();
                    }
                    return;
                }
            }

            if (fullColumn && haveRandomIa && whosTurn)
            {
                randomIaReference.IaTurn();
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
        GameObject newGo;
        Transform[] columns = { column0, column1, column2, column3, column4, column5, column6 };

        if (column >= 0 && column < columns.Length && whosTurn)
        {
            newGo = Instantiate(yellowCoin, columns[column].position, Quaternion.identity);
            playedTurnsGo.Push(newGo);
        }
        else if (column >= 0 && column < columns.Length && whosTurn == false)
        {
            newGo = Instantiate(redCoin, columns[column].position, Quaternion.identity);
            playedTurnsGo.Push(newGo);
        }
    }

    #endregion
    
    #region Win
    public bool CheckWin(int column, int row, int[,] grid)
    {
        // Victoire colonne
        int cpt = 1;
        for (int i = 1; i < 6; i++)
        {
            if (grid[column, i] == grid[column, i - 1] && grid[column, i] != 0)
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
            if (grid[i, row] == grid[i - 1, row] && grid[i, row] != 0)
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
        Debug.Log("Il y a une égalité");
        return true;
    }
    #endregion
    
    #region Undo/Redo/Restart
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void Undo()
    {
        if (playedTurns.Count > 0)
        {
            turnsRedo.Push(CloneGrid(grille));  
            turnsRedoGo.Push(playedTurnsGo.Peek());
            grille = playedTurns.Pop();
            playedTurnsGo.Pop().SetActive(false);
            RefreshUi();
            whosTurn = !whosTurn;
        }
    }

    public void Redo()
    {
        if (turnsRedo.Count > 0)
        {
            playedTurns.Push(CloneGrid(grille));  
            playedTurnsGo.Push(turnsRedoGo.Peek());
            grille = turnsRedo.Pop();
            turnsRedoGo.Pop().SetActive(true);
            RefreshUi();
            whosTurn = !whosTurn;
        }
    }

    void DontRedo()
    {
        for (int i = 0; i < turnsRedoGo.Count; i++)
        {
            Destroy(turnsRedoGo.Pop());
        }

        turnsRedo = new Stack<int[,]>();
    }
    #endregion

    #region  Utils Functions
    private int[,] CloneGrid(int[,] source)
    {
        int rows = source.GetLength(0);
        int cols = source.GetLength(1);
        int[,] copy = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                copy[i, j] = source[i, j];
            }
        }
        return copy;
    }
    void RefreshUi()
    {
        if (whosTurn)
        {
            redTurn.SetActive(true);
            yellowTurn.SetActive(false);
        }
        else
        {
            redTurn.SetActive(false);
            yellowTurn.SetActive(true);
        }
    }

    public int[,] TakeGrid()
    {
        return grille;
    }
    #endregion
}