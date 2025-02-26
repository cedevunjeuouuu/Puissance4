using System;
using System.Collections;
using UnityEngine;

public class Ia : MonoBehaviour
{
    public Game gameReference;
    public int maxDepth = 5;

    public void IaTurn()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        yield return new WaitForSeconds(0.7f);
        int bestMove = BestMove();
        gameReference.AddCoin(bestMove);
    }

    int BestMove()
    {
        int bestColumn = -1;
        int bestScore = int.MinValue;
        int[,] grille = gameReference.TakeGrid();

        for (int col = 0; col < 7; col++)
        {
            if (IsValidMove(grille, col))
            {
                int row = GetRow(grille, col);
                grille[col, row] = 2;
                int score = Minimax(grille, maxDepth, false, int.MinValue, int.MaxValue);
                grille[col, row] = 0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestColumn = col;
                }
            }
        }
        return bestColumn;
    }

    int Minimax(int[,] board, int depth, bool maximizingPlayer, int alpha, int beta)
    {
        if (depth == 0 || IsGameOver(board))
        {
            return EvaluateBoard(board);
        }

        if (maximizingPlayer)
        {
            int maxEval = int.MinValue;
            for (int col = 0; col < 7; col++)
            {
                if (IsValidMove(board, col))
                {
                    int row = GetRow(board, col);
                    board[col, row] = 2;
                    int eval = Minimax(board, depth - 1, false, alpha, beta);
                    board[col, row] = 0;
                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);
                    if (beta <= alpha) break;
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            for (int col = 0; col < 7; col++)
            {
                if (IsValidMove(board, col))
                {
                    int row = GetRow(board, col);
                    board[col, row] = 1;
                    int eval = Minimax(board, depth - 1, true, alpha, beta);
                    board[col, row] = 0;
                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);
                    if (beta <= alpha) break;
                }
            }
            return minEval;
        }
    }

    int EvaluateBoard(int[,] board)
    {
        int score = 0;
        for (int col = 0; col < 7; col++)
        {
            for (int row = 0; row < 6; row++)
            {
                if (board[col, row] != 0)
                {
                    int player = board[col, row];
                    int points = GetScoreForPosition(board, col, row, player);

                    if (player == 2) score += points;
                    else score -= points;
                }
            }
        }
        return score;
    }

    int GetScoreForPosition(int[,] board, int col, int row, int player)
    {
        int score = 0;
        score += CountAligned(board, col, row, 1, 0, player);
        score += CountAligned(board, col, row, 0, 1, player);
        score += CountAligned(board, col, row, 1, 1, player);
        score += CountAligned(board, col, row, 1, -1, player); 
        return score;
    }

    int CountAligned(int[,] board, int col, int row, int dX, int dY, int player)
    {
        int count = 0;
        int score = 0;
        for (int i = -3; i <= 3; i++)
        {
            int x = col + i * dX;
            int y = row + i * dY;

            if (x >= 0 && x < 7 && y >= 0 && y < 6)
            {
                if (board[x, y] == player)
                {
                    count++;
                }
                else if (board[x, y] != 0)
                {
                    count = 0;
                }
            }
        }
        if (count >= 4) score += 1000;
        else if (count == 3) score += 100;
        else if (count == 2) score += 10;
        return score;
    }

    bool IsValidMove(int[,] board, int col)
    {
        return board[col, 5] == 0;
    }

    int GetRow(int[,] board, int col)
    {
        for (int row = 0; row < 6; row++)
        {
            if (board[col, row] == 0)
                return row;
        }
        return -1;
    }

    bool IsGameOver(int[,] board)
    {
        for (int col = 0; col < 7; col++)
        {
            for (int row = 0; row < 6; row++)
            {
                if (board[col, row] != 0 && gameReference.CheckWin(col, row, board))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
