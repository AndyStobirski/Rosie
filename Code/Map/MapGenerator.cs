using Microsoft.Xna.Framework;
using Rosie.Code.Environment;
using Rosie.Code.Map;
using Rosie.Code.Misc;
using Rosie.Misc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Rosie.Map
{
    /// <summary>
    /// This class demonstrates a simple map builder for a roguelike game. 
    /// 
    /// Notes:
    ///     1. The method Corridor_Build() adds doors to the beginning and end of a coridoor
    ///     2. It generators Waypoints which are nodes used in NPC navigation - waypoints are
    ///        placed in the centre of rooms and contain a list of connected waypoints, items
    ///        in the next room connected by a corridoor.
    /// 
    /// </summary>
    class MapGenerator : Generator
    {
        /// <summary>
        /// Build a series of rooms and corridoors and return the map data
        /// </summary>
        /// <returns>Instance of CurrentLevel class</returns>
        public override Level Build(int pWidth, int pHeight)
        {

            MapSize = new Size(pWidth, pHeight);

            Level = new Level();

            WayPoints = new();
            Build_OneStartRoom();
            //AddWalls();
            Level.Rooms = rctBuiltRooms;
            Level.WayPoints = WayPoints;


            // 
            //  Add Stair Cases
            //
            Point stair = Level.GetRandomEmptyRoomPoint();
            Level.Map[stair.X, stair.Y] = new Staircase(true); //up
            Level.StairCase_Up = new Point(stair.X, stair.Y); //up

            stair = Level.GetRandomEmptyRoomPoint();
            Level.Map[stair.X, stair.Y] = new Staircase(false); //up
            Level.StairCase_Down = new Point(stair.X, stair.Y); //up




            return Level;
        }

        /// <summary>
        /// This will add walls around the doors and corridors, to give it more of a Moria look
        /// THis is not efficent and needs to be added into the room and corridor generating code
        /// but I want to quickly add it.
        /// </summary>
        public void AddWalls()
        {
            for (int x = 0; x < MapSize.Width; x++)
            {
                for (int y = 0; y < MapSize.Height; y++)
                {
                    if (Level.Map[x, y] == null)
                    {


                        // if the null point has neighbours which are Floor or Door
                        if (MapUtils.GetSurroundingPoints(x, y)
                                .Where(p => Point_Valid(p.X, p.Y))
                                .Any(p => Level.Map[p.X, p.Y] is Floor || Level.Map[p.X, p.Y] is Door))
                        {
                            Level.Map[x, y] = new Wall();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Built rooms stored here
        /// </summary>
        public List<Rectangle> rctBuiltRooms;

        /// <summary>
        /// Built corridors stored here
        /// </summary>
        private List<Point> lBuilltCorridors;

        /// <summary>
        /// Corridor to be built stored here
        /// </summary>
        private List<Point> lCurrentCorridor;

        /// <summary>
        /// Room to be built stored here
        /// </summary>
        private Rectangle rctCurrentRoom;

        /// <summary>
        /// The Waypoint used in the current room
        /// </summary>
        private WayPoint currentWayPoint;

        #region Waypoint

        // The following properties are used in waypoint calculation

        /// <summary>
        /// The structure off which the current corridor is being built;
        /// </summary>
        private CorridorContactPoint corridorStartPoint;

        /// <summary>
        /// If a corridor is being build off a start room, it's stored here
        /// </summary>
        private Rectangle rctCorridorStartRoom;

        /// <summary>
        /// The structure which the current corridor terminates on
        /// </summary>
        private CorridorContactPoint corridorEndPoint;

        /// <summary>
        /// The room that a corridor hits when it's finished building
        /// </summary>
        private Rectangle rctCorridorEndRoom;

        #endregion


        #region builder public properties

        //room properties
        [Category("Room"), Description("Minimum Size"), DisplayName("Minimum Size")]
        public Size Room_Min { get; set; } = new Size(4, 4);
        [Category("Room"), Description("Max Size"), DisplayName("Maximum Size")]
        public Size Room_Max { get; set; } = new Size(10, 10);
        [Category("Room"), Description("Total number"), DisplayName("Rooms to build")]
        public int MaxRooms { get; set; } = 2;
        [Category("Room"), Description("Minimum distance between rooms"), DisplayName("Distance from other rooms")]
        public int RoomDistance { get; set; } = 3;
        [Category("Room"), Description("Minimum distance of room from existing corridors"), DisplayName("Corridor Distance")]
        public int CorridorDistance { get; set; } = 4;

        //corridor properties
        [Category("Corridor"), Description("Minimum corridor length"), DisplayName("Minimum length")]
        public int Corridor_Min { get; set; } = 3;
        [Category("Corridor"), Description("Maximum corridor length"), DisplayName("Maximum length")]
        public int Corridor_Max { get; set; } = 15;
        [Category("Corridor"), Description("Maximum corridor length"), DisplayName("Maximum length")]
        public int Corridor_MaxTurns { get; set; } = 5;
        [Category("Corridor"), Description("The distance a corridor has to be away from a closed cell for it to be built"), DisplayName("Corridor Spacing")]
        public int CorridorSpace { get; set; } = 2;


        [Category("Probability"), Description("Probability of building a corridor from a room or corridor. Greater than value = room"), DisplayName("Select room")]
        public int BuildProb { get; set; } = 30;


        [Category("Map"), DisplayName("Break Out"), Description("")]
        public int BreakOut { get; set; } = 250;

        /// <summary>
        /// Add doors to map
        /// </summary>
        public bool AddDoors { get; set; } = false;

        #endregion

        /// <summary>
        /// describes the outcome of the corridor building operation
        /// </summary>
        enum CorridorItemHit
        {

            invalid //invalid point generated
            ,
            self  //corridor hit self
                ,
            existingcorridor //hit a built corridor
                ,
            originroom //corridor hit origin room 
                ,
            existingroom //corridor hit existing room
                ,
            completed //corridor built without problem    
                ,
            tooclose
                , OK //point OK
        }



        public MapGenerator()
        {
            //Level = new Level();

            //MapSize = new Size(150, 150);
            //Level.Map = new Tile[MapSize.Width, MapSize.Height];
            //Corridor_MaxTurns = 5;
            //Room_Min = new Size(3, 3);
            //Room_Max = new Size(10, 10);
            //Corridor_Min = 3;
            //Corridor_Max = 15;
            //MaxRooms = 25;


            //RoomDistance = 5;
            //CorridorDistance = 4;
            //CorridorSpace = 2;

            //BuildProb = 30;
            //BreakOut = 250;
        }

        /// <summary>
        /// Initialise everything
        /// </summary>
        private void Clear()
        {
            lCurrentCorridor = new List<Point>();
            rctBuiltRooms = new List<Rectangle>();
            lBuilltCorridors = new List<Point>();

            Level.Map = new Tile[MapSize.Width, MapSize.Height];

        }

        #region build methods()

        /// <summary>
        /// Randomly choose a room and attempt to build a corridor terminated by
        /// a room off it, and repeat until MaxRooms has been reached. The map
        /// is started of by placing a room in approximately the centre of the map
        /// using the method PlaceStartRoom()
        /// 
        /// This method also connects together waypoints if a corridor built off a room
        /// ends in a room which is pre-existing or has been built specificially.
        /// 
        /// </summary>
        /// <returns>Bool indicating if the map was built, i.e. the property BreakOut was not
        /// exceed</returns>
        public bool Build_OneStartRoom()
        {
            int loopctr = 0;

            CorridorItemHit CorBuildOutcome;
            Point Location = new Point();
            Point Direction = new Point();

            Clear();

            PlaceStartRoom();

            //attempt to build the required number of rooms
            while (rctBuiltRooms.Count() < MaxRooms)
            {

                if (loopctr++ > BreakOut)//bail out if this value is exceeded
                    return false;

                if (Corridor_GetStart(out Location, out Direction))
                {

                    CorBuildOutcome = Corridor_Make(ref Location, ref Direction, RandomWithSeed.Next(1, Corridor_MaxTurns)
                        , RandomWithSeed.Next(0, 100) > 50 ? true : false);

                    switch (CorBuildOutcome)
                    {
                        case CorridorItemHit.existingroom:
                        case CorridorItemHit.existingcorridor:
                        case CorridorItemHit.self:
                            Corridor_Build();
                            break;

                        case CorridorItemHit.completed:
                            if (Room_AttemptBuildOnCorridor(Direction))
                            {
                                Corridor_Build();
                                Room_Build();

                                if (corridorStartPoint == CorridorContactPoint.room)
                                {
                                    //we have connected an existing room to a newly built room
                                    corridorEndPoint = CorridorContactPoint.room;
                                    rctCorridorEndRoom = rctCurrentRoom;
                                }
                            }
                            break;
                    }

                    //do waypoint calculations
                    if (corridorStartPoint == CorridorContactPoint.room && corridorEndPoint == CorridorContactPoint.room)
                    {
                        if (!rctCorridorEndRoom.IsEmpty)
                        {
                            //as a waypoint is added at the same time as a room, they will have the same index
                            var startIndex = rctBuiltRooms.IndexOf(rctCorridorStartRoom);
                            var endIndex = rctBuiltRooms.IndexOf(rctCorridorEndRoom);

                            var startWayPoint = WayPoints[startIndex];
                            var endWayPoint = WayPoints[endIndex];

                            startWayPoint.ConnectedPoints.Add(endWayPoint);
                            endWayPoint.ConnectedPoints.Add(startWayPoint);


                        }
                    }

                }
            }

            return true;
        }


        /*
         
            // DEPRECATED

        /// <summary>
        /// Randomly choose a room and attempt to build a corridor terminated by
        /// a room off it, and repeat until MaxRooms has been reached. The map
        /// is started of by placing two rooms on opposite sides of the map and joins
        /// them with a long corridor, using the method PlaceStartRooms()
        /// </summary>
        /// <returns>Bool indicating if the map was built, i.e. the property BreakOut was not
        /// exceed</returns>
        public bool Build_ConnectedStartRooms()
        {

            int loopctr = 0;

            CorridorItemHit CorBuildOutcome;
            Point Location = new Point();
            Point Direction = new Point();

            Clear();

            PlaceStartRooms();

            //attempt to build the required number of rooms
            while (rctBuiltRooms.Count() < MaxRooms)
            {

                if (loopctr++ > BreakOut)//bail out if this value is exceeded
                    return false;

                if (Corridor_GetStart(out Location, out Direction))
                {

                    CorBuildOutcome = Corridor_Make(ref Location, ref Direction, RandomWithSeed.Next(1, Corridor_MaxTurns)
                        , RandomWithSeed.Next(0, 100) > 50 ? true : false);

                    switch (CorBuildOutcome)
                    {
                        case CorridorItemHit.existingroom:
                        case CorridorItemHit.existingcorridor:
                        case CorridorItemHit.self:
                            Corridor_Build();
                            break;

                        case CorridorItemHit.completed:

                            //corridor testing has finished and it hasn't hasn't hit anything
                            if (Room_AttemptBuildOnCorridor(Direction))
                            {
                                Corridor_Build();
                                Room_Build();

                            }
                            break;
                    }
                }
            }

            return true;

        }

        */

        #endregion


        #region room utilities

        /// <summary>
        /// Place a random sized room in the middle of the map
        /// </summary>
        private void PlaceStartRoom()
        {
            rctCurrentRoom = new Rectangle()
            {
                Width = RandomWithSeed.Next(Room_Min.Width, Room_Max.Width)
                ,
                Height = RandomWithSeed.Next(Room_Min.Height, Room_Max.Height)
            };
            rctCurrentRoom.X = MapSize.Width / 2;
            rctCurrentRoom.Y = MapSize.Height / 2;
            Room_Build();
        }


        /*
        
        // DEPRCATED

        /// <summary>
        /// Randomly place two rooms on either the left/right or top bottom/bottom edges of the map
        /// and then attempt to connect them
        /// </summary>
        private void PlaceStartRooms()
        {

            Point startdirection;
            bool connection = false;
            Point Location = new Point();
            Point Direction = new Point();
            CorridorItemHit CorBuildOutcome;

            while (!connection)
            {

                Clear();
                startdirection = Direction_Get(new Point());

                //place a room on the top and bottom
                if (startdirection.X == 0)
                {

                    //room at the top of the map
                    rctCurrentRoom = new Rectangle()
                    {
                        Width = RandomWithSeed.Next(Room_Min.Width, Room_Max.Width)
                                ,
                        Height = RandomWithSeed.Next(Room_Min.Height, Room_Max.Height)
                    };
                    rctCurrentRoom.X = RandomWithSeed.Next(0, MapSize.Width - rctCurrentRoom.Width);
                    rctCurrentRoom.Y = 1;
                    Room_Build();

                    //at the bottom of the map
                    rctCurrentRoom = new Rectangle
                    {
                        Width = RandomWithSeed.Next(Room_Min.Width, Room_Max.Width),
                        Height = RandomWithSeed.Next(Room_Min.Height, Room_Max.Height)
                    };
                    rctCurrentRoom.X = RandomWithSeed.Next(0, MapSize.Width - rctCurrentRoom.Width);
                    rctCurrentRoom.Y = MapSize.Height - rctCurrentRoom.Height - 1;
                    Room_Build();


                }
                else//place a room on the east and west side
                {
                    //west side of room
                    rctCurrentRoom = new Rectangle
                    {
                        Width = RandomWithSeed.Next(Room_Min.Width, Room_Max.Width),
                        Height = RandomWithSeed.Next(Room_Min.Height, Room_Max.Height)
                    };
                    rctCurrentRoom.Y = RandomWithSeed.Next(0, MapSize.Height - rctCurrentRoom.Height);
                    rctCurrentRoom.X = 1;
                    Room_Build();

                    rctCurrentRoom = new Rectangle
                    {
                        Width = RandomWithSeed.Next(Room_Min.Width, Room_Max.Width),
                        Height = RandomWithSeed.Next(Room_Min.Height, Room_Max.Height)
                    };
                    rctCurrentRoom.Y = RandomWithSeed.Next(0, MapSize.Height - rctCurrentRoom.Height);
                    rctCurrentRoom.X = MapSize.Width - rctCurrentRoom.Width - 2;
                    Room_Build();

                }


                if (Corridor_GetStart(out Location, out Direction))
                {
                    CorBuildOutcome = Corridor_Make(ref Location, ref Direction, 100, true);

                    switch (CorBuildOutcome)
                    {
                        case CorridorItemHit.existingroom:
                            Corridor_Build();
                            connection = true;
                            break;
                    }
                }
            }
        }

        */


        /// <summary>
        /// Make a room off the last point of Corridor, using
        /// CorridorDirection as an indicator of how to offset the room.
        /// The potential room is stored in Room.
        /// </summary>
        private bool Room_AttemptBuildOnCorridor(Point pDirection)
        {
            rctCurrentRoom = new Rectangle()
            {
                Width = RandomWithSeed.Next(Room_Min.Width, Room_Max.Width)
                    ,
                Height = RandomWithSeed.Next(Room_Min.Height, Room_Max.Height)
            };

            //startbuilding room from this point
            Point lc = lCurrentCorridor.Last();

            if (pDirection.X == 0) //north/south direction
            {
                rctCurrentRoom.X = RandomWithSeed.Next(lc.X - rctCurrentRoom.Width + 1, lc.X);

                if (pDirection.Y == 1)
                    rctCurrentRoom.Y = lc.Y + 1;//south
                else
                    rctCurrentRoom.Y = lc.Y - rctCurrentRoom.Height - 1;//north


            }
            else if (pDirection.Y == 0)//east / west direction
            {
                rctCurrentRoom.Y = RandomWithSeed.Next(lc.Y - rctCurrentRoom.Height + 1, lc.Y);

                if (pDirection.X == -1)//west
                    rctCurrentRoom.X = lc.X - rctCurrentRoom.Width;
                else
                    rctCurrentRoom.X = lc.X + 1;//east
            }

            return Room_Verify();
        }


        /// <summary>
        /// Randomly get a point on the edge of a randomly selected room
        /// </summary>
        /// <param name="Location">Out: Location of point on room edge</param>
        /// <param name="Location">Out: Direction of point</param>
        /// <param name="pRoomBuiltOff">The room this start point is originiating from</param>
        /// <returns>Was it able to build?</returns>
        private Rectangle Room_GetEdge(out Point pLocation, out Point pDirection)
        {

            Rectangle pRoomBuiltOff = rctBuiltRooms[RandomWithSeed.Next(0, rctBuiltRooms.Count())];

            //pick a random point within a room
            //the +1 / -1 on the values are to stop a corner from being chosen
            pLocation = new Point(RandomWithSeed.Next(pRoomBuiltOff.Left + 1, pRoomBuiltOff.Right - 1)
                                  , RandomWithSeed.Next(pRoomBuiltOff.Top + 1, pRoomBuiltOff.Bottom - 1));


            //get a random direction
            pDirection = Directions[RandomWithSeed.Next(0, Directions.GetLength(0))];

            do
            {
                //move in that direction
                pLocation.X += pDirection.X;
                pLocation.Y += pDirection.Y;

                //until we meet an empty cell
            } while (Point_Get(pLocation.X, pLocation.Y) != null);

            return pRoomBuiltOff;
        }

        #endregion

        #region corridor utitlies

        /// <summary>
        /// Randomly get a point on an existing corridor
        /// 
        /// Sets the variables corridorStartPoint which are used by waypoints
        /// 
        /// </summary>
        /// <param name="Location">Out: location of point</param>
        /// <returns>Bool indicating success</returns>
        private void Corridor_GetEdge(out Point pLocation, out Point pDirection)
        {
            List<Point> validdirections = new List<Point>();

            do
            {
                //the modifiers below prevent the first of last point being chosen
                pLocation = lBuilltCorridors[RandomWithSeed.Next(1, lBuilltCorridors.Count - 1)];

                //attempt to locate all the empy map points around the location
                //using the directions to offset the randomly chosen point
                foreach (Point p in Directions)
                    if (Point_Valid(pLocation.X + p.X, pLocation.Y + p.Y))
                        if (Point_Get(pLocation.X + p.X, pLocation.Y + p.Y) != null)
                            validdirections.Add(p);


            } while (validdirections.Count == 0);

            pDirection = validdirections[RandomWithSeed.Next(0, validdirections.Count)]; ;

            pLocation.X += pDirection.X;
            pLocation.Y += pDirection.Y;

            corridorStartPoint = CorridorContactPoint.corridor;


        }

        /// <summary>
        /// Build the contents of lPotentialCorridor, adding it's points to the builtCorridors list then empty
        /// 
        /// </summary>
        private void Corridor_Build()
        {

            for (int ctr = 0; ctr < lCurrentCorridor.Count; ctr++)
            {
                var p = lCurrentCorridor[ctr];



                if (AddDoors)
                {
                    //For implementation in a later version
                    if (ctr == 0 || ctr == lCurrentCorridor.Count - 1)
                    {
                        var d = new Door();
                        d.IsOpen = RandomWithSeed.Next(2) == 1;

                        Point_Set(p.X, p.Y, d);
                    }
                    else
                    {
                        Point_Set(p.X, p.Y, new Floor());
                        lBuilltCorridors.Add(p);
                    }
                }
                else
                {
                    Point_Set(p.X, p.Y, new Floor());
                    lBuilltCorridors.Add(p);

                }
            }

            lCurrentCorridor.Clear();
        }

        /// <summary>
        /// Get a starting point for a corridor, randomly choosing between a room and a corridor.
        /// 
        /// Sets the variables corridorStartPoint and rctCorridorStartRoom which are used by waypoints
        /// 
        /// </summary>
        /// <param name="Location">Out: pLocation of point</param>
        /// <param name="Location">Out: pDirection of point</param>
        /// <returns>Bool indicating if location found is OK</returns>
        private bool Corridor_GetStart(out Point pLocation, out Point pDirection)
        {
            rctCurrentRoom = new Rectangle();
            lCurrentCorridor = new List<Point>();

            if (lBuilltCorridors.Count > 0)
            {
                if (RandomWithSeed.Next(0, 100) >= BuildProb)
                {
                    rctCorridorStartRoom = Room_GetEdge(out pLocation, out pDirection);
                    corridorStartPoint = CorridorContactPoint.room;

                }
                else
                {
                    Corridor_GetEdge(out pLocation, out pDirection);
                    corridorStartPoint = CorridorContactPoint.corridor;
                }
            }
            else
            {
                //no corridors present, so build off a room
                rctCorridorStartRoom = Room_GetEdge(out pLocation, out pDirection);
                corridorStartPoint = CorridorContactPoint.room;
            }

            //finally check the point we've found
            return Corridor_PointTest(pLocation, pDirection) == CorridorItemHit.OK;

        }

        /// <summary>
        /// Attempt to make a corridor, storing it in the lCurrentCorridor list.
        /// 
        /// If corridor building is succesful, pStart and pDirection are updated to be the
        /// final point and direction of that point, respectively.
        /// </summary>
        /// <param name="pStart">Starting point of corridor</param>
        /// <param name="pDirection">Starting direction of coridoor</param>
        /// <param name="pTurns">Number of turns the corridor is to have </param>
        /// <param name="pPreventBackTracking">Prevent the corridor from going back on itself</param>
        /// <returns>Corridor building success?</returns>
        private CorridorItemHit Corridor_Make(ref Point pStart, ref Point pDirection, int pTurns, bool pPreventBackTracking)
        {

            lCurrentCorridor = new List<Point>();
            lCurrentCorridor.Add(pStart);

            int corridorlength;
            Point startdirection = new Point(pDirection.X, pDirection.Y);
            CorridorItemHit outcome;

            while (pTurns > 0)
            {
                pTurns--;

                corridorlength = RandomWithSeed.Next(Corridor_Min, Corridor_Max);
                //build corridor
                while (corridorlength > 0)
                {
                    corridorlength--;

                    //make a point and offset it
                    pStart.X += pDirection.X;
                    pStart.Y += pDirection.Y;

                    outcome = Corridor_PointTest(pStart, pDirection);
                    if (outcome != CorridorItemHit.OK)
                        return outcome;
                    else
                        lCurrentCorridor.Add(pStart);
                }

                if (pTurns > 1)
                    if (!pPreventBackTracking)
                        pDirection = Direction_Get(pDirection);
                    else
                        pDirection = Direction_Get(pDirection, startdirection);
            }

            return CorridorItemHit.completed;
        }

        /// <summary>
        /// Test the provided point to see if it has empty cells on either side
        /// of it. This is to stop corridors being built adjacent to a room, check if it has made conatct with a room
        /// 
        /// Sets the variables corridorEndPoint and rctCorridorEndRoom which are used by waypoints
        /// 
        /// </summary>
        /// <param name="pPoint">Point to test</param>
        /// <param name="pDirection">Direction it is moving in</param>
        /// <returns></returns>
        private CorridorItemHit Corridor_PointTest(Point pPoint, Point pDirection)
        {

            if (!Point_Valid(pPoint.X, pPoint.Y))
            {
                corridorEndPoint = CorridorContactPoint.none;
                //invalid point hit, exit
                return CorridorItemHit.invalid;
            }
            else if (lBuilltCorridors.Contains(pPoint))
            {
                corridorEndPoint = CorridorContactPoint.corridor;
                //in an existing corridor
                return CorridorItemHit.existingcorridor;
            }
            else if (lCurrentCorridor.Contains(pPoint))
            {
                corridorEndPoint = CorridorContactPoint.corridor;
                // hit self
                return CorridorItemHit.self;
            }
            else if (rctCurrentRoom.Contains(pPoint))
            {
                corridorEndPoint = CorridorContactPoint.room;
                rctCorridorEndRoom = rctCurrentRoom;

                //the corridors origin room has been reached, exit
                return CorridorItemHit.originroom;
            }
            else
            {
                //is point in a room
                foreach (Rectangle r in rctBuiltRooms)
                    if (r.Contains(pPoint))
                    {

                        corridorEndPoint = CorridorContactPoint.room;
                        rctCorridorEndRoom = r;

                        return CorridorItemHit.existingroom;
                    }
            }


            //using the property corridor space, check that number of cells on
            //either side of the point are empty
            foreach (int r in Enumerable.Range(-CorridorSpace, 2 * CorridorSpace + 1).ToList())
            {
                if (pDirection.X == 0)//north or south
                {
                    if (Point_Valid(pPoint.X + r, pPoint.Y))
                        if (Point_Get(pPoint.X + r, pPoint.Y) != null)
                            return CorridorItemHit.tooclose;
                }
                else if (pDirection.Y == 0)//east west
                {
                    if (Point_Valid(pPoint.X, pPoint.Y + r))
                        if (Point_Get(pPoint.X, pPoint.Y + r) != null)
                            return CorridorItemHit.tooclose;
                }

            }

            return CorridorItemHit.OK;
        }


        #endregion

        #region direction methods

        /// <summary>
        /// Get a random direction, excluding the opposite of the provided direction to
        /// prevent a corridor going back on it's Build
        /// </summary>
        /// <param name="dir">Current direction</param>
        /// <returns></returns>
        private Point Direction_Get(Point pDir)
        {
            Point NewDir;
            do
            {
                NewDir = Directions[RandomWithSeed.Next(0, Directions.GetLength(0))];
            } while (Direction_Reverse(NewDir) == pDir);

            return NewDir;
        }

        /// <summary>
        /// Get a random direction, excluding the provided directions and the opposite of 
        /// the provided direction to prevent a corridor going back on it's self.
        /// 
        /// The parameter pDirExclude is the first direction chosen for a corridor, and
        /// to prevent it from being used will prevent a corridor from going back on 
        /// it'self
        /// </summary>
        /// <param name="dir">Current direction</param>
        /// <param name="pDirectionList">Direction to exclude</param>
        /// <param name="pDirExclude">Direction to exclude</param>
        /// <returns></returns>
        private Point Direction_Get(Point pDir, Point pDirExclude)
        {
            Point NewDir;
            do
            {
                NewDir = Directions[RandomWithSeed.Next(0, Directions.GetLength(0))];
            } while (
                        Direction_Reverse(NewDir) == pDir
                         | Direction_Reverse(NewDir) == pDirExclude
                    );


            return NewDir;
        }

        private Point Direction_Reverse(Point pDir)
        {
            return new Point(-pDir.X, -pDir.Y);
        }

        #endregion

        #region room test

        /// <summary>
        /// Check if rctCurrentRoom can be built
        /// </summary>
        /// <returns>Bool indicating success</returns>
        private bool Room_Verify()
        {
            //make it one bigger to ensure that testing gives it a border
            rctCurrentRoom.Inflate(RoomDistance, RoomDistance);

            //check it occupies legal, empty coordinates
            for (int x = rctCurrentRoom.Left; x <= rctCurrentRoom.Right; x++)
                for (int y = rctCurrentRoom.Top; y <= rctCurrentRoom.Bottom; y++)
                    if (!Point_Valid(x, y) || Point_Get(x, y) != null)
                        return false;

            //check it doesn't encroach onto existing rooms
            foreach (Rectangle r in rctBuiltRooms)
                if (r.Intersects(rctCurrentRoom))
                    return false;

            rctCurrentRoom.Inflate(-RoomDistance, -RoomDistance);

            //check the room is the specified distance away from corridors
            rctCurrentRoom.Inflate(CorridorDistance, CorridorDistance);

            foreach (Point p in lBuilltCorridors)
                if (rctCurrentRoom.Contains(p))
                    return false;

            // shrink it back to it's normal size
            rctCurrentRoom.Inflate(-CorridorDistance, -CorridorDistance);

            return true;
        }

        /// <summary>
        /// Add the global Room to the rooms collection and draw it on the map
        /// 
        /// Adds a WayPoint to the WayPoints listt
        /// 
        /// </summary>
        private void Room_Build()
        {

            rctBuiltRooms.Add(rctCurrentRoom);


            //build the room
            for (int x = rctCurrentRoom.Left; x <= rctCurrentRoom.Right; x++)
                for (int y = rctCurrentRoom.Top; y <= rctCurrentRoom.Bottom; y++)
                    Level.Map[x, y] = new Floor();


            // the middle of the current room
            WayPoints.Add(new WayPoint(rctCurrentRoom.Left + rctCurrentRoom.Width / 2, rctCurrentRoom.Top + rctCurrentRoom.Height / 2));

            currentWayPoint = WayPoints.Last();

        }

        #endregion

        #region Map Utilities

        /// <summary>
        /// Check if the point falls within the map array range
        /// </summary>
        /// <param name="x">x to test</param>
        /// <param name="y">y to test</param>
        /// <returns>Is point with map array?</returns>
        private bool Point_Valid(int x, int y)
        {
            return x >= 0 & x < Level.Map.GetLength(0) & y >= 0 & y < Level.Map.GetLength(1);
        }

        /// <summary>
        /// Set array point to specified tile type
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void Point_Set(int x, int y, Tile pTile)
        {
            Level.Map[x, y] = pTile;
        }

        /// <summary>
        /// Get the value of the specified point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Tile Point_Get(int x, int y)
        {
            return Level.Map[x, y];
        }

        public override Point GetStartLocation()
        {
            Rectangle r = rctBuiltRooms[RandomWithSeed.Next(0, rctBuiltRooms.Count())];

            return new Point(RandomWithSeed.Next(r.Left, r.Right), RandomWithSeed.Next(r.Top, r.Bottom));
        }


        #endregion

        enum CorridorContactPoint
        {
            room, corridor, none
        }



        public override Level Build()
        {
            throw new System.NotImplementedException();
        }
    }
}
