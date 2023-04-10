using Microsoft.Xna.Framework;
using Rosie.Entities;
using Rosie.Code.Environment;
using System;

namespace Rosie.Misc
{

    /// <summary>
    /// Implementation of "FOV using recursive shadowcasting - improved" as
    /// described on http://roguebasin.roguelikedevelopment.org/index.php?title=FOV_using_recursive_shadowcasting_-_improved
    /// 
    /// The FOV code is contained in the region "FOV Algorithm".
    /// The method GetVisibleCells() is called to calculate the cells
    /// visible to the player by examing each octant sequantially. 
    /// The generic list VisiblePoints contains the cells visible to the PlayerData.Location.
    /// 
    /// GetVisibleCells() is called everytime the player moves, and the event playerMoved
    /// is called when a successful move is made (the player moves into an empty cell)
    /// 
    /// Notes:
    /// 
    ///     1. The method
    /// 
    /// </summary>
    public class FOVRecurse
    {
        public FOVRecurse(Actor pPlayer, Tile[,] pMap)
        {
            _PlayerData = pPlayer;
            _Map = pMap;
        }

        /// <summary>
        /// The player
        /// </summary>
        private Actor _PlayerData { get; set; }

        /// <summary>
        /// Full Map
        /// </summary>
        private Tile[,] _Map { get; set; }

        /// <summary>
        /// The octants which a player can see
        /// </summary>
        readonly int[] _VisibleOctants = { 1, 2, 3, 4, 5, 6, 7, 8 };

        /// <summary>
        /// The rectangle which defines the game view
        /// </summary>
        private Rectangle _GameViewDefinition;

        /// <summary>
        /// Contains the cells visible to the player in the game view
        /// </summary>
        public bool[,] GameViewVisibilityGrid { get; set; }


        #region map point code

        /// <summary>
        /// Check if the provided coordinate is within the bounds of the mapp array
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        private bool Point_Valid(int pX, int pY)
        {
            return pX >= 0 & pX < _Map.GetLength(0)
                    & pY >= 0 & pY < _Map.GetLength(1);
        }


        #endregion

        /// <summary>
        /// Can the examined cell be seen through?
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        private bool CellOpen(int pX, int pY)
        {
            if (_Map[pX, pY] == null)
                return false;
            else
                return _Map[pX, pY].SeeThrough();

        }

        #region FOV algorithm

        //  Octant data
        //
        //    \ 1 | 2 /
        //   8 \  |  / 3
        //   -----+-----
        //   7 /  |  \ 4
        //    / 6 | 5 \
        //
        //  1 = NNW, 2 =NNE, 3=ENE, 4=ESE, 5=SSE, 6=SSW, 7=WSW, 8 = WNW

        /// <summary>
        /// Start here: go through all the octants which surround the player to
        /// determine which open cells are visible
        /// </summary>
        public void GetVisibleCells(Rectangle pGameViewDefinition)
        {
            _GameViewDefinition = pGameViewDefinition;

            //reset the grid to false
            GameViewVisibilityGrid = new bool[_GameViewDefinition.Width, _GameViewDefinition.Height];

            //the player can see himself
            GameViewVisibilityGrid[_PlayerData.X - _GameViewDefinition.X, _PlayerData.Y - _GameViewDefinition.Y] = true;

            foreach (int o in _VisibleOctants)
                ScanOctant(1, o, 1.0, 0.0);


        }

        /// <summary>
        /// Set a value in the visibility grid to true
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        protected void SetGameViewVisibilityGrid(int pX, int pY)
        {
            if (_GameViewDefinition.Contains(pX, pY))
                GameViewVisibilityGrid[pX - _GameViewDefinition.X, pY - _GameViewDefinition.Y] = true;
        }

        /// <summary>
        /// Examine the provided octant and calculate the visible cells within it.
        /// </summary>
        /// <param name="pDepth">Depth of the scan</param>
        /// <param name="pOctant">Octant being examined</param>
        /// <param name="pStartSlope">Start slope of the octant</param>
        /// <param name="pEndSlope">End slope of the octance</param>
        protected void ScanOctant(int pDepth, int pOctant, double pStartSlope, double pEndSlope)
        {

            int visrange2 = _PlayerData.VisionRange * _PlayerData.VisionRange;
            int x = 0;
            int y = 0;

            switch (pOctant)
            {

                case 1: //nnw
                    y = _PlayerData.Location.Y - pDepth;
                    if (y < 0) return;

                    x = _PlayerData.Location.X - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x < 0) x = 0;

                    while (GetSlope(x, y, _PlayerData.Location.X, _PlayerData.Location.Y, false) >= pEndSlope)
                    {
                        if (GetVisDistance(x, y, _PlayerData.Location.X, _PlayerData.Location.Y) <= visrange2)
                        {
                            SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y)) //current cell blocked
                            {
                                if (x - 1 >= 0 && CellOpen(x - 1, y))
                                {
                                    //prior cell within range AND open...
                                    //...incremenet the depth, adjust the endslope and recurse
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, false));
                                }
                            }
                            else
                            {

                                if (x - 1 >= 0 && !CellOpen(x - 1, y))
                                {
                                    //prior cell within range AND open...
                                    //..adjust the startslope
                                    pStartSlope = GetSlope(x - 0.5, y - 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, false);
                                }
                            }
                        }
                        x++;
                    }
                    x--;
                    break;

                case 2: //nne

                    y = _PlayerData.Location.Y - pDepth;
                    if (y < 0) return;

                    x = _PlayerData.Location.X + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x >= _Map.GetLength(0)) x = _Map.GetLength(0) - 1;

                    while (GetSlope(x, y, _PlayerData.Location.X, _PlayerData.Location.Y, false) <= pEndSlope)
                    {
                        if (GetVisDistance(x, y, _PlayerData.Location.X, _PlayerData.Location.Y) <= visrange2)
                        {
                            SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {
                                if (x + 1 < _Map.GetLength(0) && CellOpen(x + 1, y))
                                {
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, false));
                                }
                            }
                            else
                            {
                                if (x + 1 < _Map.GetLength(0) && !CellOpen(x + 1, y))
                                {
                                    pStartSlope = -GetSlope(x + 0.5, y - 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, false);
                                }
                            }
                        }
                        x--;
                    }
                    x++;
                    break;

                case 3:

                    x = _PlayerData.Location.X + pDepth;
                    if (x >= _Map.GetLength(0)) return;

                    y = _PlayerData.Location.Y - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y < 0) y = 0;

                    while (GetSlope(x, y, _PlayerData.Location.X, _PlayerData.Location.Y, true) <= pEndSlope)
                    {

                        if (GetVisDistance(x, y, _PlayerData.Location.X, _PlayerData.Location.Y) <= visrange2)
                        {
                            SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {
                                if (y - 1 >= 0 && CellOpen(x, y - 1))
                                {
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, true));
                                }
                            }
                            else
                            {
                                if (y - 1 >= 0 && !CellOpen(x, y - 1))
                                    pStartSlope = -GetSlope(x + 0.5, y - 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, true);

                                if (_GameViewDefinition.Contains(x, y))
                                {
                                    GameViewVisibilityGrid[x - _GameViewDefinition.X, y - _GameViewDefinition.Y] = true;
                                }
                            }
                        }
                        y++;
                    }
                    y--;
                    break;

                case 4:

                    x = _PlayerData.Location.X + pDepth;
                    if (x >= _Map.GetLength(0)) return;

                    y = _PlayerData.Location.Y + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y >= _Map.GetLength(1)) y = _Map.GetLength(1) - 1;

                    while (GetSlope(x, y, _PlayerData.Location.X, _PlayerData.Location.Y, true) >= pEndSlope)
                    {

                        if (GetVisDistance(x, y, _PlayerData.Location.X, _PlayerData.Location.Y) <= visrange2)
                        {

                            SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {
                                if (y + 1 < _Map.GetLength(1) && CellOpen(x, y + 1))
                                {
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, true));
                                }
                            }
                            else
                            {
                                if (y + 1 < _Map.GetLength(1) && !CellOpen(x, y + 1))
                                {
                                    pStartSlope = GetSlope(x + 0.5, y + 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, true);
                                }
                            }
                        }
                        y--;
                    }
                    y++;
                    break;

                case 5:

                    y = _PlayerData.Location.Y + pDepth;
                    if (y >= _Map.GetLength(1)) return;

                    x = _PlayerData.Location.X + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x >= _Map.GetLength(0)) x = _Map.GetLength(0) - 1;

                    while (GetSlope(x, y, _PlayerData.Location.X, _PlayerData.Location.Y, false) >= pEndSlope)
                    {
                        if (GetVisDistance(x, y, _PlayerData.Location.X, _PlayerData.Location.Y) <= visrange2)
                        {
                            SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {
                                if (x + 1 < _Map.GetLength(1) && CellOpen(x + 1, y))
                                {
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, false));
                                }
                            }
                            else
                            {
                                if (x + 1 < _Map.GetLength(1) && !CellOpen(x + 1, y))
                                {
                                    pStartSlope = GetSlope(x + 0.5, y + 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, false);
                                }
                            }
                        }
                        x--;
                    }
                    x++;
                    break;

                case 6:

                    y = _PlayerData.Location.Y + pDepth;
                    if (y >= _Map.GetLength(1)) return;

                    x = _PlayerData.Location.X - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x < 0) x = 0;

                    while (GetSlope(x, y, _PlayerData.Location.X, _PlayerData.Location.Y, false) <= pEndSlope)
                    {
                        if (GetVisDistance(x, y, _PlayerData.Location.X, _PlayerData.Location.Y) <= visrange2)
                        {
                            SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {
                                if (x - 1 >= 0 && CellOpen(x - 1, y))
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, false));
                            }
                            else
                            {
                                if (x - 1 >= 0 && !CellOpen(x - 1, y))
                                {
                                    pStartSlope = -GetSlope(x - 0.5, y + 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, false);
                                }
                            }
                        }
                        x++;
                    }
                    x--;
                    break;

                case 7:

                    x = _PlayerData.Location.X - pDepth;
                    if (x < 0) return;

                    y = _PlayerData.Location.Y + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y >= _Map.GetLength(1)) y = _Map.GetLength(1) - 1;

                    while (GetSlope(x, y, _PlayerData.Location.X, _PlayerData.Location.Y, true) <= pEndSlope)
                    {

                        if (GetVisDistance(x, y, _PlayerData.Location.X, _PlayerData.Location.Y) <= visrange2)
                        {
                            SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {
                                if (y + 1 < _Map.GetLength(1) && CellOpen(x, y + 1))
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, true));
                            }
                            else
                            {
                                if (y + 1 < _Map.GetLength(1) && !CellOpen(x, y + 1))
                                {
                                    pStartSlope = -GetSlope(x - 0.5, y + 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, true);
                                }
                            }
                        }
                        y--;
                    }
                    y++;
                    break;

                case 8: //wnw

                    x = _PlayerData.Location.X - pDepth;
                    if (x < 0) return;

                    y = _PlayerData.Location.Y - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y < 0) y = 0;

                    while (GetSlope(x, y, _PlayerData.Location.X, _PlayerData.Location.Y, true) >= pEndSlope)
                    {

                        if (GetVisDistance(x, y, _PlayerData.Location.X, _PlayerData.Location.Y) <= visrange2)
                        {
                            SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {
                                if (y - 1 >= 0 && CellOpen(x, y - 1))
                                {
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, true));
                                }

                            }
                            else
                            {
                                if (y - 1 >= 0 && !CellOpen(x, y - 1))
                                {
                                    pStartSlope = GetSlope(x - 0.5, y - 0.5, _PlayerData.Location.X, _PlayerData.Location.Y, true);
                                }
                            }
                        }
                        y++;
                    }
                    y--;
                    break;
            }


            if (x < 0)
                x = 0;
            else if (x >= _Map.GetLength(0))
                x = _Map.GetLength(0) - 1;

            if (y < 0)
                y = 0;
            else if (y >= _Map.GetLength(1))
                y = _Map.GetLength(1) - 1;

            if (pDepth < _PlayerData.VisionRange & CellOpen(x, y))
                ScanOctant(pDepth + 1, pOctant, pStartSlope, pEndSlope);

        }

        /// <summary>
        /// Get the gradient of the slope formed by the two points
        /// </summary>
        /// <param name="pX1"></param>
        /// <param name="pY1"></param>
        /// <param name="pX2"></param>
        /// <param name="pY2"></param>
        /// <param name="pInvert">Invert slope</param>
        /// <returns>Distance</returns>
        private double GetSlope(double pX1, double pY1, double pX2, double pY2, bool pInvert)
        {
            double numerator = pInvert ? pY1 - pY2 : pX1 - pX2;
            double denominator = pInvert ? pX1 - pX2 : pY1 - pY2;
            return numerator / denominator;
        }


        /// <summary>
        /// Calculate the distance between the two points
        /// </summary>
        /// <param name="pX1"></param>
        /// <param name="pY1"></param>
        /// <param name="pX2"></param>
        /// <param name="pY2"></param>
        /// <returns>Distance</returns>
        private int GetVisDistance(int pX1, int pY1, int pX2, int pY2)
        {
            int deltaX = pX1 - pX2;
            int deltaY = pY1 - pY2;
            return (deltaX * deltaX) + (deltaY * deltaY);
        }

        #endregion


    }
}
