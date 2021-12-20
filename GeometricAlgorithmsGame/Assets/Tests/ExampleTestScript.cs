using NUnit.Framework;

namespace Tests
{
    public class ExampleTestScript
    {
        // Example method
        private bool GetInvertedValue(bool value)
        {
            return !value;
        }

        [SetUp]
        public void SetUp()
        {
            // Add here to code the set up the tests 
        }

        [Test]
        public void GetInvertedValue_Should_ReturnTrue_When_ValueIsFalse()
        {
            // Arrange
            var value = false;

            // Act
            var result = GetInvertedValue(value);

            // Assert
            Assert.True(result);
            Assert.AreEqual(expected: true, actual: result);
        }
        
        [Test]
        public void GetInvertedValue_Should_ReturnFalse_When_ValueIsTrue()
        {
            // Arrange
            var value = true;

            // Act
            var result = GetInvertedValue(value);

            // Assert
            Assert.False(result);
            Assert.AreEqual(expected: false, actual: result);
        }

        [TearDown]
        public void TearDown()
        {
            // Stuff that needs to be stopped or removed or whatever
        }
    }
}
