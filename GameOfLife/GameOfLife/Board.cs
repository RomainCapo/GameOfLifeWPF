﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameOfLife
{
    class Board
    {
        Cell[,] board;
        public int NbCellX { get; }
        public int NbCellY { get; }

        Random rand = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nbCellX"></param>
        /// <param name="nbCellY"></param>
        public Board(int nbCellX, int nbCellY)
        {
            NbCellX = nbCellX;
            NbCellY = nbCellY;
            board = new Cell[NbCellX, NbCellY];
            InitBoard();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Cell this[int x, int y]
        {
            get { return board[x, y]; }
            set { board[x, y] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitBoard()
        {
            for (int i = 0; i < NbCellX; i++)
            {
                for(int j = 0; j < NbCellY; j++)
                {
                    board[i, j] = new Cell();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void AleaInit()
        {
            int initLoop = rand.Next(40, 50);
            for (int i = 0; i < initLoop; i++)
            {
                board[rand.Next(0, NbCellX - 1), rand.Next(0, NbCellY - 1)].IsAlive = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int ComputeCellXYNeighbours(int x, int y)
        {
            int xToTest;
            int yToTest;
            int nbNeighbours = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    xToTest = x + i;
                    yToTest = y + j;
                    if ((xToTest >= 0 && yToTest >= 0) && (xToTest < NbCellX && yToTest < NbCellY))
                    {
                        if (board[xToTest, yToTest].IsAlive && (xToTest != 0) && (xToTest != 0))
                        {
                            nbNeighbours++;
                        }
                    }
                }
            }
            return nbNeighbours;
        }

        public void NextIteration()
        {
            for(int i = 0; i < NbCellX; i++)
            {
                for(int j = 0; j < NbCellY; j++)
                {
                    int nbNeighbours = ComputeCellXYNeighbours(i, j);
                    if(nbNeighbours < 2)
                    {
                        board[i, j].IsAlive = false;
                    }

                    if(nbNeighbours > 3)
                    {
                        if (!board[i, j].IsAlive)
                        {
                            board[i, j].IsAlive = true;
                        }
                        else
                        {
                            board[i, j].IsAlive = false;
                        }
                    }
                }
            }
        }
    }
}
