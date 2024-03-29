using Microsoft.Xna.Framework;
using Rosie.Code.Environment;
using System;
using System.IO;
using System.Linq;

namespace Rosie.Misc
{

    /// <summary>
    /// Implementation of "FOV using recursive shadowcasting - improved" as
    /// described on https://web.archive.org/web/20090721163830/http://roguebasin.roguelikedevelopment.org/index.php?title=FOV_using_recursive_shadowcasting_-_improved
    /// 
    /// It references https://web.archive.org/web/20090721163830/http://roguebasin.roguelikedevelopment.org/index.php?title=FOV_using_recursive_shadowcasting
    /// 
    /// The Recursive Shadowcasting code is contained in the region "Shadowcast Algorithm".
    /// The method GetVisibleCells() is called to calculate the cells
    /// visible to the player by examing each octant sequantially. 
    /// The generic list VisiblePoints contains the cells visible to the PlayerData.Location.
    /// 
    /// GetVisibleCells() is called everytime the player moves and the property 
    /// 
    /// 
    /// </summary>
    public class RecursiveShadowcast
    {
        public RecursiveShadowcast(Tile[,] pMap)
        {
            _Map = pMap;
        }

        enum Mode
        {
            ShadowCast, InverseSquare, DistanceMap
        }

        private Mode _currentMode;

        /// <summary>
        /// The source of the shadow caster
        /// </summary>
        private Point _Source;

        /// <summary>
        /// The map the shadowcaster is using
        /// </summary>
        private Tile[,] _Map;

        /// <summary>
        /// The octants which the shadowcaster can see
        /// </summary>
        readonly int[] _VisibleOctants = { 1, 2, 3, 4, 5, 6, 7, 8 };

        /// <summary>
        /// A square which defines the limits of the shadowcasting, a maximum area of effect
        /// </summary>
        private Rectangle _GameViewDefinition;

        /// <summary>
        /// Contains the cells that are illuminated, a value of zero indicates it is unlit
        /// </summary>
        public double[,] LightGrid { get; private set; }

        /// <summary>
        /// Used in conjunction with CalculateInverseSquareValues
        /// </summary>
        private double _StartValue;

        /// <summary>
        /// The maximum size of the casting
        /// </summary>
        private int _CastLimit;


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

        #region Shadowcast algorithm

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
        /// Shadow cast using an inverse square rule calculating from the starting value
        /// </summary>
        /// <param name="pStartValue"></param>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pCastLimit"></param>
        /// <remarks>
        /// Upon completion populate the property LightGrid with inverse square values dervied
        /// from the starting value
        /// </remarks>
        public void CalculateDistanceMap(Double pStartValue, int pX, int pY, int pCastLimit)
        {
            _currentMode = Mode.DistanceMap;

            _Source = new Point(pX, pY);
            _CastLimit = pCastLimit;

            _GameViewDefinition = new Rectangle(pX - pCastLimit, pY - pCastLimit, 2 * pCastLimit, 2 * pCastLimit);
            _StartValue = pStartValue;

            //reset the grid to false
            LightGrid = new double[_GameViewDefinition.Width, _GameViewDefinition.Height];

            //the player can see himself
            LightGrid[_Source.X - _GameViewDefinition.X, _Source.Y - _GameViewDefinition.Y] = pStartValue;

            foreach (int o in _VisibleOctants)
                ScanOctant(1, o, 1.0, 0.0);
        }

        /// <summary>
        /// Shadow cast using an inverse square rule calculating from the starting value
        /// </summary>
        /// <param name="pStartValue"></param>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pCastLimit"></param>
        /// <remarks>
        /// Upon completion populate the property LightGrid with inverse square values dervied
        /// from the starting value
        /// </remarks>
        public void CalculateInverseSquareMap(Double pStartValue, int pX, int pY, int pCastLimit)
        {
            _currentMode = Mode.InverseSquare;

            _Source = new Point(pX, pY);
            _CastLimit = pCastLimit;

            _GameViewDefinition = new Rectangle(pX - pCastLimit, pY - pCastLimit, 2 * pCastLimit, 2 * pCastLimit);
            _StartValue = pStartValue;

            //reset the grid to false
            LightGrid = new double[_GameViewDefinition.Width, _GameViewDefinition.Height];

            //the player can see himself
            LightGrid[_Source.X - _GameViewDefinition.X, _Source.Y - _GameViewDefinition.Y] = pStartValue;

            foreach (int o in _VisibleOctants)
                ScanOctant(1, o, 1.0, 0.0);
        }

        public void OutputMask()
        {
            string file = @"c:\temp\" + DateTime.Now.ToString("yyyyMMddHHmmssff") + ".json";
            for (int i = 0; i < LightGrid.GetLength(1); i++)
            {
                File.AppendAllText(file, String.Join("\t",
                    Enumerable.Range(0, LightGrid.GetLength(0)).Select(j => LightGrid[j, i].ToString())) + "\r\n"
                    );
                File.AppendAllText(file, "\r\n");
            }
        }

        /// <summary>
        /// Cast a light from the specified point - used exclusively by the player
        /// for field of vision in the gameview.
        /// </summary>
        /// <param name="pGameViewDefinition">The camera view of the map</param>
        /// <param name="pX">Light source location</param>
        /// <param name="pY">Light source location</param>
        /// <param name="pCastLimit">The limit of the light casting</param>
        /// 
        /// <remarks>
        /// Upon completion populate the property LightGrid with values 1 / 0 indicting light / dark.
        /// </remarks>
        public void CastLight(Rectangle pGameViewDefinition, int pX, int pY, int pCastLimit)
        {
            _currentMode = Mode.ShadowCast;

            _Source = new Point(pX, pY);
            _CastLimit = pCastLimit;

            _GameViewDefinition = pGameViewDefinition;

            //reset the grid to false
            LightGrid = new double[_GameViewDefinition.Width, _GameViewDefinition.Height];

            //the player can see himself
            LightGrid[_Source.X - _GameViewDefinition.X, _Source.Y - _GameViewDefinition.Y] = 1;

            foreach (int o in _VisibleOctants)
                ScanOctant(1, o, 1.0, 0.0);
        }

        /// <summary>
        /// Set a value in the visibility grid,the value is dependent upon tuse 
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        private void SetGameViewVisibilityGrid(int pX, int pY)
        {
            if (_GameViewDefinition.Contains(pX, pY))
            {
                Double val = 1;

                switch (_currentMode)
                {
                    case Mode.ShadowCast:
                        val = 1;
                        break;
                    case Mode.InverseSquare:
                        double distance = Math.Sqrt(Math.Pow(pX - _Source.X, 2) + Math.Pow(pY - _Source.Y, 2));
                        val = _StartValue / Math.Pow(distance + 1, 2);
                        break;
                    case Mode.DistanceMap:
                        val = _StartValue - (int)Math.Sqrt(Math.Pow(pX - _Source.X, 2) + Math.Pow(pY - _Source.Y, 2));
                        break;
                }


                LightGrid[pX - _GameViewDefinition.X, pY - _GameViewDefinition.Y] = val;
            }
        }

        /// <summary>
        /// Examine the provided octant and calculate the visible cells within it.
        /// </summary>
        /// <param name="pDepth">Depth of the scan</param>
        /// <param name="pOctant">Octant being examined</param>
        /// <param name="pStartSlope">Start slope of the octant</param>
        /// <param name="pEndSlope">End slope of the octance</param>
        private void ScanOctant(int pDepth, int pOctant, double pStartSlope, double pEndSlope)
        {

            int visrange2 = (int)Math.Pow(_CastLimit, 2);
            int x = 0;
            int y = 0;

            switch (pOctant)
            {

                case 1: //nnw
                    y = _Source.Y - pDepth;
                    if (y < 0) return;

                    x = _Source.X - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x < 0) x = 0;

                    while (GetSlope(x, y, _Source.X, _Source.Y, false) >= pEndSlope)
                    {
                        if (GetVisDistance(x, y, _Source.X, _Source.Y) <= visrange2)
                        {
                            //SetGameViewVisibilityGrid(x, y);//here, it marks a wall as being visible and then stops

                            if (!CellOpen(x, y)) //current cell blocked
                            {

                                if (x - 1 >= 0 && CellOpen(x - 1, y))
                                {
                                    //prior cell within range AND open...
                                    //...incremenet the depth, adjust the endslope and recurse
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, _Source.X, _Source.Y, false));
                                }
                            }
                            else
                            {

                                if (x - 1 >= 0 && !CellOpen(x - 1, y))
                                {
                                    //prior cell within range AND open...
                                    //..adjust the startslope
                                    pStartSlope = GetSlope(x - 0.5, y - 0.5, _Source.X, _Source.Y, false);
                                }

                                //Placing this here will cause cells that vision stops at, will not be seen
                                //if a ray can't pass thrugh it, it can't be seen
                                SetGameViewVisibilityGrid(x, y);

                            }
                        }
                        x++;
                    }
                    x--;
                    break;

                case 2: //nne

                    y = _Source.Y - pDepth;
                    if (y < 0) return;

                    x = _Source.X + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x >= _Map.GetLength(0)) x = _Map.GetLength(0) - 1;

                    while (GetSlope(x, y, _Source.X, _Source.Y, false) <= pEndSlope)
                    {
                        if (GetVisDistance(x, y, _Source.X, _Source.Y) <= visrange2)
                        {
                            //SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {

                                if (x + 1 < _Map.GetLength(0) && CellOpen(x + 1, y))
                                {
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, _Source.X, _Source.Y, false));
                                }
                            }
                            else
                            {
                                if (x + 1 < _Map.GetLength(0) && !CellOpen(x + 1, y))
                                {
                                    pStartSlope = -GetSlope(x + 0.5, y - 0.5, _Source.X, _Source.Y, false);
                                }

                                SetGameViewVisibilityGrid(x, y);

                            }
                        }
                        x--;
                    }
                    x++;
                    break;

                case 3:

                    x = _Source.X + pDepth;
                    if (x >= _Map.GetLength(0)) return;

                    y = _Source.Y - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y < 0) y = 0;

                    while (GetSlope(x, y, _Source.X, _Source.Y, true) <= pEndSlope)
                    {

                        if (GetVisDistance(x, y, _Source.X, _Source.Y) <= visrange2)
                        {
                            //SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {

                                if (y - 1 >= 0 && CellOpen(x, y - 1))
                                {
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, _Source.X, _Source.Y, true));
                                }
                            }
                            else
                            {
                                if (y - 1 >= 0 && !CellOpen(x, y - 1))
                                    pStartSlope = -GetSlope(x + 0.5, y - 0.5, _Source.X, _Source.Y, true);

                                if (_GameViewDefinition.Contains(x, y))
                                {
                                    LightGrid[x - _GameViewDefinition.X, y - _GameViewDefinition.Y] = 1;
                                }

                                SetGameViewVisibilityGrid(x, y);
                            }
                        }
                        y++;
                    }
                    y--;
                    break;

                case 4:

                    x = _Source.X + pDepth;
                    if (x >= _Map.GetLength(0)) return;

                    y = _Source.Y + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y >= _Map.GetLength(1)) y = _Map.GetLength(1) - 1;

                    while (GetSlope(x, y, _Source.X, _Source.Y, true) >= pEndSlope)
                    {

                        if (GetVisDistance(x, y, _Source.X, _Source.Y) <= visrange2)
                        {
                            //SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {

                                if (y + 1 < _Map.GetLength(1) && CellOpen(x, y + 1))
                                {
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, _Source.X, _Source.Y, true));
                                }
                            }
                            else
                            {
                                if (y + 1 < _Map.GetLength(1) && !CellOpen(x, y + 1))
                                {
                                    pStartSlope = GetSlope(x + 0.5, y + 0.5, _Source.X, _Source.Y, true);
                                }

                                SetGameViewVisibilityGrid(x, y);
                            }
                        }
                        y--;
                    }
                    y++;
                    break;

                case 5:

                    y = _Source.Y + pDepth;
                    if (y >= _Map.GetLength(1)) return;

                    x = _Source.X + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x >= _Map.GetLength(0)) x = _Map.GetLength(0) - 1;

                    while (GetSlope(x, y, _Source.X, _Source.Y, false) >= pEndSlope)
                    {
                        if (GetVisDistance(x, y, _Source.X, _Source.Y) <= visrange2)
                        {
                            //SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {

                                if (x + 1 < _Map.GetLength(1) && CellOpen(x + 1, y))
                                {
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, _Source.X, _Source.Y, false));
                                }
                            }
                            else
                            {
                                if (x + 1 < _Map.GetLength(1) && !CellOpen(x + 1, y))
                                {
                                    pStartSlope = GetSlope(x + 0.5, y + 0.5, _Source.X, _Source.Y, false);
                                }

                                SetGameViewVisibilityGrid(x, y);

                            }
                        }
                        x--;
                    }
                    x++;
                    break;

                case 6:

                    y = _Source.Y + pDepth;
                    if (y >= _Map.GetLength(1)) return;

                    x = _Source.X - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x < 0) x = 0;

                    while (GetSlope(x, y, _Source.X, _Source.Y, false) <= pEndSlope)
                    {
                        if (GetVisDistance(x, y, _Source.X, _Source.Y) <= visrange2)
                        {
                            //SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {

                                if (x - 1 >= 0 && CellOpen(x - 1, y))
                                {
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, _Source.X, _Source.Y, false));
                                }
                            }
                            else
                            {
                                if (x - 1 >= 0 && !CellOpen(x - 1, y))
                                {
                                    pStartSlope = -GetSlope(x - 0.5, y + 0.5, _Source.X, _Source.Y, false);
                                }

                                SetGameViewVisibilityGrid(x, y);
                            }
                        }
                        x++;
                    }
                    x--;
                    break;

                case 7:

                    x = _Source.X - pDepth;
                    if (x < 0) return;

                    y = _Source.Y + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y >= _Map.GetLength(1)) y = _Map.GetLength(1) - 1;

                    while (GetSlope(x, y, _Source.X, _Source.Y, true) <= pEndSlope)
                    {

                        if (GetVisDistance(x, y, _Source.X, _Source.Y) <= visrange2)
                        {

                            //SetGameViewVisibilityGrid(x, y);

                            if (!CellOpen(x, y))
                            {
                                if (y + 1 < _Map.GetLength(1) && CellOpen(x, y + 1))
                                {
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, _Source.X, _Source.Y, true));
                                }
                            }
                            else
                            {
                                if (y + 1 < _Map.GetLength(1) && !CellOpen(x, y + 1))
                                {
                                    pStartSlope = -GetSlope(x - 0.5, y + 0.5, _Source.X, _Source.Y, true);
                                }

                                SetGameViewVisibilityGrid(x, y);

                            }
                        }
                        y--;
                    }
                    y++;
                    break;

                case 8: //wnw

                    x = _Source.X - pDepth;
                    if (x < 0) return;

                    y = _Source.Y - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y < 0) y = 0;

                    while (GetSlope(x, y, _Source.X, _Source.Y, true) >= pEndSlope)
                    {

                        if (GetVisDistance(x, y, _Source.X, _Source.Y) <= visrange2)
                        {
                            //SetGameViewVisibilityGrid(x, y); //here, it marks a wall as being visible and then stops

                            if (!CellOpen(x, y))
                            {
                                if (y - 1 >= 0 && CellOpen(x, y - 1))
                                {
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, _Source.X, _Source.Y, true));
                                }

                            }
                            else
                            {
                                if (y - 1 >= 0 && !CellOpen(x, y - 1))
                                {
                                    pStartSlope = GetSlope(x - 0.5, y - 0.5, _Source.X, _Source.Y, true);
                                }

                                SetGameViewVisibilityGrid(x, y);

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

            if (pDepth < _CastLimit & CellOpen(x, y))
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
