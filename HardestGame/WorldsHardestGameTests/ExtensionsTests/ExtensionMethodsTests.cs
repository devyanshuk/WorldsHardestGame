using System.Drawing;
using NUnit.Framework;
using System.Collections.Generic;

using WorldsHardestGameModel.Entities;
using WorldsHardestGameModel.Extensions;
using WorldsHardestGameModel.EntityBase;
using WorldsHardestGameModel.Environment;
using WorldsHardestGameModel.MovementTypes;

namespace WorldsHardestGameTests.ExtensionsTests
{
    public class ExtensionMethodsTests
    {
        private Player player;
        private Wall wall;

        [SetUp]
        public void Setup()
        {
            player = new Player(new PointF(420, 240));
            wall = new Wall(GameEnvironment.CELL_WIDTH, GameEnvironment.CELL_HEIGHT, new PointF(480, 240));
        }

        [TestCase(300F, 300F, 30F, 30F, 300F, 300F, true, TestName = "Test for circle and rectangle with the same centre and topleftpos respectively.")]
        [TestCase(300F, 300F, 10F, 10F, 900F, 900F, false, TestName = "Test for disjoint square and circle.")]
        [TestCase(300F, 300F, 10F, 10F, 310F, 310F, true, TestName = "Test for a narrow collision")]
        [TestCase(300F, 300F, 10F, 10F, 316F, 300F, false, TestName = "Test for objects very close but not touching each other.")]
        public void TestForCircleAndRectangleCollision(float circleCentreX, float circleCentreY, float rectWidth, float rectHeight, float rectangleTlpX, float rectangleTlpY, bool expected)
        {
            //Arrange
            var circularEntity = new CircularEntity(new PointF(circleCentreX, circleCentreY));
            var rectangularEntity = new RectangularEntity(rectWidth, rectHeight, new PointF(rectangleTlpX, rectangleTlpY));

            //Act
            var result = circularEntity.IsCollision(rectangularEntity);
            var result2 = rectangularEntity.IsCollision(circularEntity);

            //Assert
            Assert.AreEqual(result, expected);
            Assert.AreEqual(result2, expected);
        }


        private static readonly object[] PlayerWallCollisionData =
        {
            new object[] { new PointF(480, 240), new PointF(420, 240), new List<Dir_4> { Dir_4.LEFT } },
            new object[] { new PointF(420, 240), new PointF(480, 240), new List<Dir_4>() },
            new object[] { new PointF(160, 200), new PointF(200, 200), new List<Dir_4> { Dir_4.RIGHT } },
            new object[] { new PointF(200, 260), new PointF(200, 200), new List<Dir_4> { Dir_4.UP } },
            new object[] { new PointF(167, 260), new PointF(200, 200), new List<Dir_4> { Dir_4.UP } },
            new object[] { new PointF(230, 160), new PointF(200, 200), new List<Dir_4> { Dir_4.DOWN } }
        };



        [Test, TestCaseSource(nameof(PlayerWallCollisionData))]
        public void CheckForPlayerAndWallCollision(PointF playerPos, PointF wallPos, List<Dir_4> expctedCollisionDirection)
        {
            //Arrange
            player.topLeftPosition = playerPos;
            wall.topLeftPosition = wallPos;

            //Act
            var collisionDirections = player.CheckCollision(wall);

            //Assert
            Assert.AreEqual(collisionDirections, expctedCollisionDirection);
        }

    }
}