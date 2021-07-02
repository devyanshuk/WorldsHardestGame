using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using WorldsHardestGameModel.Entities;
using WorldsHardestGameModel.Environment;
using WorldsHardestGameModel.MovementTypes;
using WorldsHardestGameModel.ConfigTemplates;

namespace WorldsHardestGameModel.Levels.Parser
{
    public class Parser : IParser
    {
        public const int MAP_WIDTH = 23;
        public const int MAP_HEIGHT = 15;

        #region identifiers

        private readonly List<char> WallIdentifiers = new List<char>() { 'H', 'W' };
        private readonly List<char> ObstalceBoundaryIdentifiers = new List<char> { 'H' };
        private readonly List<char> LeftAdjustedPositionIdentifiers = new List<char> { '<', '(', '[' };
        private readonly List<char> RightAdjustedPositionIdentifiers = new List<char> { '>', ')', ']' };
        private readonly List<char> ExtraRightAdjustedPositionIdentifiers = new List<char> { ';', '.' };
        private readonly List<char> ExtraVerticalAdjustedPositionIdentifiers = new List<char> { 'V', '^' };

        private const char CoinIdentifier = 'X';
        private const char PlayerIdentifier = 'P';
        private const char FreeSpaceIdentifier = '#';
        private const char MovableSpaceIdentifier = '1';
        private const char CoinAndObstacleIdentifier = '2';
        private const char VerticalAdjustmentIdentifier = '!';

        private const string XMovementIdentifier = "-";
        private const string YMovementIdentifier = "|";
        private const string CheckpointIdentifier = "ch";
        private const string SquareMovementIdentifier = "[]";
        private const string CircularMovementIdentifier = "()";
        private const string ClockwiseMovementIdentifier = "(";
        private const string AnticlockWiseMovementIdentifier = ")";

        #endregion



        #region pre-process movement types

        /// <summary>
        /// Dictionary<Identifier, x-Direction-velocity>
        /// </summary>
        private Dictionary<char, float> xMovementId;

        /// <summary>
        /// Dictionary<Identifier, y-Direction-velocity>
        /// </summary>
        private Dictionary<char, float> yMovementId;

        /// <summary>
        /// Dictionary<Identifier, Tuple<centre-of-movement, direction-of-movement, velocity > >
        /// </summary>
        private Dictionary<char, Tuple<PointF, Dir_C, float>> circularMovementId;

        /// <summary>
        /// Dictionary<Identifier, Tuple<top-left-position-of-square-boundary,
        ///                              direction-of-movement,
        ///                              velocity,
        ///                              length-of-square-boundary,
        ///                              width-of-square-boundary>
        /// </summary>
        private Dictionary<char, Tuple<PointF, Dir_C, float, float, float>> rectangularMovementId;


        #endregion



        private readonly ILocalSettings localSettings;


        public Parser(ILocalSettings localSettings)
        {
            this.localSettings = localSettings;
            xMovementId = new Dictionary<char, float>();
            yMovementId = new Dictionary<char, float>();
            circularMovementId = new Dictionary<char, Tuple<PointF, Dir_C, float>>();
            rectangularMovementId = new Dictionary<char, Tuple<PointF, Dir_C, float, float, float>>();
        }

        public void ClearAll()
        {
            xMovementId.Clear();
            yMovementId.Clear();
            circularMovementId.Clear();
            rectangularMovementId.Clear();
        }

        public void ParseLevel(int level, IGameEnvironment environment)
        {
            try
            {
                var filePath = localSettings.LevelDir + level.ToString();
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    PreprocessMovementDetails(reader, level, environment);
                    ReadBoardAndUpdateEnvironment(reader, level, environment);
                }

            }
            catch(Exception ex) when (ex is FileNotFoundException ||
                                      ex is IOException ||
                                      ex is UnauthorizedAccessException ||
                                      ex is System.Security.SecurityException)
            {
                throw new Exception
                    (
                        $@"file {level} is not present in {localSettings.LevelDir}.
                           Add it before you run the game"
                    );
            }
        }


        private void PreprocessMovementDetails(StreamReader reader, int level, IGameEnvironment environment)
        {
            int lineNumber = 0;
            while (reader.ReadLine() is string line && !String.IsNullOrEmpty(line))
            {
                lineNumber++;
                var split = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch(split[0])
                {
                    case XMovementIdentifier:
                        ParseXMovements(split, level, lineNumber);
                        break;

                    case YMovementIdentifier:
                        ParseYMovements(split, level, lineNumber);
                        break;

                    case CircularMovementIdentifier:
                        ParseCircularMovements(split, level, lineNumber, environment);
                        break;

                    case SquareMovementIdentifier:
                        ParseRectangularMovements(split, level, lineNumber);
                        break;

                    case CheckpointIdentifier:
                        ParseCheckpoints(split, level, lineNumber, environment);
                        break;

                    default:
                        throw new FormatException($"{split[0]} is not a recognized movement type. Review {level} file");
                }
            }
        }


        private void ReadBoardAndUpdateEnvironment(StreamReader reader, int level, IGameEnvironment environment)
        {
            for(int i = 0; i < MAP_HEIGHT; i++)
            {
                var line = reader.ReadLine();
                for (int j = 0; j < MAP_WIDTH; j++)
                {
                    var currChar = line[j];
                    var pos = new PointF(GameEnvironment.CELL_WIDTH * j, GameEnvironment.CELL_HEIGHT * i);
                    var adjustedPos = new PointF(pos.X + GameEnvironment.CELL_WIDTH / 2, pos.Y + GameEnvironment.CELL_HEIGHT / 2);

                    if (circularMovementId.ContainsKey(currChar))
                    {
                        environment.obstacles.AddRange(AdjustedCircularPosition(currChar, pos, adjustedPos, environment));
                        environment.freeSpaces[i, j] = true;
                    }
                    if (rectangularMovementId.ContainsKey(currChar))
                    {
                        environment.obstacles.AddRange(AdjustedRectangularPositions(currChar, pos, adjustedPos, environment));
                        environment.freeSpaces[i, j] = true;
                    }
                    if (xMovementId.ContainsKey(currChar))
                    {
                        var xVelocity = xMovementId[currChar];
                        environment.obstacles.Add
                                        (
                                            new Obstacle
                                            (
                                                adjustedPos,
                                                xVelocity,
                                                new XYMovement(xVelocity, adjustedPos, Dir_4.LEFT)
                                            )

                                        );
                        environment.freeSpaces[i, j] = true;
                    }
                    if (yMovementId.ContainsKey(currChar))
                    {
                        var yVelocity = yMovementId[currChar];
                        environment.obstacles.Add
                                        (
                                            new Obstacle
                                            (
                                                adjustedPos,
                                                yVelocity,
                                                new XYMovement(yVelocity, adjustedPos, Dir_4.DOWN)
                                            )

                                        );
                        environment.freeSpaces[i, j] = true;
                    }
                    GetOtherGameInfo(currChar, level, i, j, pos, adjustedPos, environment);
                }
            }
        }

        private void GetOtherGameInfo(char currChar, int level, int i, int j, PointF pos, PointF adjustedPos, IGameEnvironment environment)
        {
            if (currChar == PlayerIdentifier)
            {
                environment.player = new Player(topLeftPosition: pos);
                environment.freeSpaces[i, j] = true;
            }
            else if (WallIdentifiers.Contains(currChar))
            {
                var wall = new Wall(GameEnvironment.CELL_WIDTH, GameEnvironment.CELL_HEIGHT, pos);
                environment.walls.Add(wall);
                if (ObstalceBoundaryIdentifiers.Contains(currChar))
                {
                    environment.obstacleBoundaries.Add(wall);
                }
            }
            else if (currChar == CoinIdentifier || currChar == CoinAndObstacleIdentifier)
            {
                environment.coins.Add(new Coin(adjustedPos));
                environment.numberOfCoins++;
                environment.freeSpaces[i, j] = true;
            }
            if (currChar == MovableSpaceIdentifier)
            {
                environment.freeSpaces[i, j] = true;
            }
        }


        /// <summary>
        /// In the input file, the X movement is represented by a line with the following signatures.
        /// "- {ID} {velocity} {ID2} {velocity2} ..."
        /// The parsing is done based on this signature
        /// </summary>
        /// <param name="line"></param>
        /// <param name="level"></param>
        /// <param name="lineNumber"></param>
        private void ParseXMovements(string[] line, int level, int lineNumber)
        {
            try
            {
                for (int i = 1; i < line.Length; i+=2)
                {
                    var id = char.Parse(line[i]);
                    var velocity = float.Parse(line[i + 1]);
                    xMovementId.Add(id, velocity);
                }
            }
            catch (FormatException)
            {
                throw new FormatException($"File {level}, Line {lineNumber} : Parse Error");
            }
        }


        /// <summary>
        /// In the input file, the Y movement is represented by a line with the following signatures.
        /// "| {ID} {velocity} {ID2} {velocity2} ..."
        /// The parsing is done based on this signature
        /// </summary>
        /// <param name="line"></param>
        /// <param name="level"></param>
        /// <param name="lineNumber"></param>
        private void ParseYMovements(string[] line, int level, int lineNumber)
        {
            try
            {
                for(int i = 1; i < line.Length; i += 2)
                {
                    var id = char.Parse(line[i]);
                    var velocity = float.Parse(line[i + 1]);
                    yMovementId.Add(id, velocity);
                }
            }
            catch(FormatException)
            {
                throw new FormatException($"File {level}, Line {lineNumber} : Parse Error");
            }
        }


        /// <summary>
        /// In the input file, the circular movement is represented by a line with the following signatures.
        /// "() {Direction-of-rotation} {angular-velocity} {centre-of-rotation-X} {centre-of-rotation-Y} {Identifier1} {Identifier2} ..."
        /// The parsing is done based on this signature
        /// </summary>
        /// <param name="line"></param>
        /// <param name="level"></param>
        /// <param name="lineNumber"></param>
        /// <param name="environment"></param>
        private void ParseCircularMovements(string[] line, int level, int lineNumber, IGameEnvironment environment)
        {
            try
            {
                var dir = line[1] == ClockwiseMovementIdentifier ? Dir_C.CLOCKWISE : Dir_C.ANTICLOCKWISE;
                var velocity = float.Parse(line[2]);
                var x = float.Parse(line[3]);
                var y = float.Parse(line[4]);
                var centre = new PointF(x * GameEnvironment.CELL_WIDTH + GameEnvironment.CELL_WIDTH / 2,
                                        y * GameEnvironment.CELL_HEIGHT + GameEnvironment.CELL_HEIGHT / 2);

                for (int i = 5; i < line.Length; i++)
                {
                    var id = char.Parse(line[i]);
                    circularMovementId.Add(id, Tuple.Create(centre, dir, velocity));
                }
            }
            catch (Exception ex) when (ex is FormatException ||
                                       ex is IndexOutOfRangeException)
            {
                throw new FormatException($"File {level}, Line {lineNumber} : Parse Error");
            }
        }


        /// <summary>
        /// In the input file, the rectangular movement is represented by a line with the following signatures.
        /// "[] {Direction-of-rotation} {velocity} {top-left-X} {top-left-Y} {width} {height} {ID1} {ID2} ..."
        /// The parsing is done based on this signature
        /// </summary>
        /// <param name="line"></param>
        /// <param name="level"></param>
        /// <param name="lineNumber"></param>
        public void ParseRectangularMovements(string[] line, int level, int lineNumber)
        {
            try
            {
                var dir = line[1] == ClockwiseMovementIdentifier ? Dir_C.CLOCKWISE : Dir_C.ANTICLOCKWISE;
                var velocity = float.Parse(line[2]);
                var x = float.Parse(line[3]) * GameEnvironment.CELL_WIDTH;
                var y = float.Parse(line[4]) * GameEnvironment.CELL_HEIGHT;
                var width = float.Parse(line[5]) * GameEnvironment.CELL_WIDTH;
                var height = float.Parse(line[6]) * GameEnvironment.CELL_HEIGHT;
                var topLeftPos = new PointF(x, y);

                for (int i = 7; i < line.Length; i++)
                {
                    var id = char.Parse(line[i]);
                    rectangularMovementId.Add(id, Tuple.Create(topLeftPos, dir, velocity, width, height));
                }
            }
            catch(Exception ex) when (ex is FormatException ||
                                      ex is IndexOutOfRangeException)
            {
                throw new FormatException($"File {level}, Line {lineNumber} : Parse Error");
            }
        }


        private void ParseCheckpoints(string[] line, int level, int lineNumber, IGameEnvironment environment)
        {
            try
            {
                var topLeftX = float.Parse(line[1]) * GameEnvironment.CELL_WIDTH;
                var topLeftY = float.Parse(line[2]) * GameEnvironment.CELL_HEIGHT;
                var width = float.Parse(line[3]) * GameEnvironment.CELL_WIDTH;
                var height = float.Parse(line[4]) * GameEnvironment.CELL_HEIGHT;
                var topLeftPosition = new PointF(topLeftX, topLeftY);

                environment.checkPoints.Add(new CheckPoint(width, height, topLeftPosition));
            }
            catch (Exception ex) when (ex is FormatException ||
                                      ex is IndexOutOfRangeException)
            {
                throw new FormatException($"File {level}, Line {lineNumber} : Parse Error");
            }
        }


        private bool CheckForSpecialCharacters(char ch, PointF newPos, IGameEnvironment environment, out PointF n)
        {
            n = new PointF(default(float), default(float));
            var containsVerticalChar = ExtraVerticalAdjustedPositionIdentifiers.Contains(ch);
            var containsHorizontalChar = ExtraRightAdjustedPositionIdentifiers.Contains(ch);

            if (containsHorizontalChar || containsVerticalChar)
            {
                n = new PointF
                {
                    X = newPos.X + (containsHorizontalChar ? GameEnvironment.CELL_WIDTH / 2 : 0),
                    Y = newPos.Y + (containsVerticalChar ? GameEnvironment.CELL_HEIGHT / 2 : 0)
                };
                return true;
            }
            return false;
        }


        private List<Obstacle> AdjustedCircularPosition(char currChar, PointF pos, PointF newPos, IGameEnvironment environment)
        {
            (var centreOfRotation, var direction, var velocity) = circularMovementId[currChar];
            var result = new List<Obstacle>();

            var x = pos.X + GameEnvironment.CELL_WIDTH / 2 +
                    (LeftAdjustedPositionIdentifiers.Contains(currChar) ? -GameEnvironment.CELL_WIDTH / 2 :
                    RightAdjustedPositionIdentifiers.Contains(currChar) ? GameEnvironment.CELL_WIDTH / 2 :
                    0);

            var y = pos.Y + GameEnvironment.CELL_HEIGHT / 2 +
                    (currChar == VerticalAdjustmentIdentifier ? GameEnvironment.CELL_HEIGHT / 2 :
                     0);

            result.Add(new Obstacle
                (
                    new PointF(x, y),
                    velocity,
                    new CircularMovement(velocity, new PointF(x, y), centreOfRotation, direction)
                ));

            if (CheckForSpecialCharacters(currChar, newPos, environment, out PointF n))
            {
                result.Add(new Obstacle
                            (
                                new PointF(x, y),
                                velocity,
                                new CircularMovement(velocity, n, centreOfRotation, direction))
                            );

            }
            //TODO .
            return result;
        }


        private List<Obstacle> AdjustedRectangularPositions(char currChar, PointF pos, PointF newPos, IGameEnvironment environment)
        {
            (var topLeftPos, var direction, var velocity, var width, var height) = rectangularMovementId[currChar];
            var result = new List<Obstacle>();

            result.Add(new Obstacle
                            (
                                newPos,
                                velocity,
                                new RectangularMovement(velocity, newPos, direction, topLeftPos, width, height)
                           )
                      );

            if (CheckForSpecialCharacters(currChar, newPos, environment, out PointF n))
            {
                result.Add(new Obstacle
                            (
                                n,
                                velocity,
                                new RectangularMovement(velocity, n, direction, topLeftPos, width, height)
                           )
                      );
            }
            return result;

        }


    }

}
