using System.Drawing;
using NUnit.Framework;

using WorldsHardestGameModel.Entities;
using WorldsHardestGameModel.Extensions;
using WorldsHardestGameModel.EntityBase;

namespace WorldsHardestGameTests.ExtensionsTests
{
    public class ExtensionMethodsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(300F, 300F, 30F, 30F, 300F, 300F, true, TestName ="Test for circle and rectangle with the same centre and topleftpos respectively.")]
        [TestCase(300F, 300F, 10F, 10F, 900F, 900F, false, TestName ="Test for disjoint square and circle.")]
        [TestCase(300F, 300F, 10F, 10F, 310, 310, true, TestName ="Test for a narrow collision")]
        [TestCase(300F, 300F, 10F, 10F, 316F, 300F, false, TestName ="Test for objects very close but not touching each other.")]
        public void TestForCircleAndRectangleCollision(float circleCentreX, float circleCentreY, float rectWidth, float rectHeight, float rectangleTlpX, float rectangleTlpY, bool expected)
        {
            //Arrange
            var circularEntity = new CircularEntity(new PointF(circleCentreX, circleCentreY));
            var rectangularEntity = new RectangularEntity(rectWidth, rectHeight, new PointF(rectangleTlpX, rectangleTlpY));

            //Act
            var result = circularEntity.IsCollision(rectangularEntity);

            //Assert
            Assert.AreEqual(result, expected);
            
        }
    }
}