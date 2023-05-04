using Microsoft.VisualStudio.TestTools.UnitTesting;

using OpenCqs2.Abstractions;

using T = System.String;

namespace OpenCqs2.Tests.Abstractions
{
    [TestClass]
    public class HandlerResult_1Tests
    {
        private HandlerResult<T> testClass = null!;
        private HandlerResult<T>.HandlerResults code;
        private T result = null!;

        [TestInitialize]
        public void SetUp()
        {
            this.code = HandlerResult<T>.HandlerResults.Fail;
            this.result = "TestValue581290552";
            this.testClass = new HandlerResult<T>(this.code, this.result);
        }

        [TestMethod]
        public void CanConstruct()
        {
            // Act
            var instance = new HandlerResult<T>();

            // Assert
            Assert.IsNotNull(instance);

            // Act
            instance = new HandlerResult<T>(this.code, this.result);

            // Assert
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void CanGetOK()
        {
            // Assert
            Assert.IsInstanceOfType(HandlerResult<T>.OK, typeof(HandlerResult<T>));
        }

        [TestMethod]
        public void CodeIsInitializedCorrectly()
        {
            Assert.AreEqual(this.code, this.testClass.Code);
        }

        [TestMethod]
        public void ResultIsInitializedCorrectly()
        {
            Assert.AreEqual(this.result, this.testClass.Result);
        }

        [TestMethod]
        public void CanCallToString()
        {
            // Act
            var result = this.testClass.ToString();

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
        }
    }

    [TestClass]
    public class HandlerResultTests
    {
        private HandlerResult testClass = null!;
        private HandlerResult<string>.HandlerResults code;
        private string message = null!;

        [TestInitialize]
        public void SetUp()
        {
            this.code = HandlerResult<string>.HandlerResults.Success;
            this.message = "TestValue161821775";
            this.testClass = new HandlerResult(this.code, this.message);
        }

        [TestMethod]
        public void CanConstruct()
        {
            // Act
            var instance = new HandlerResult(this.code, this.message);

            // Assert
            Assert.IsNotNull(instance);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void CanConstructWithInvalidMessage(string value)
        {
            // Act
            var instance = new HandlerResult(this.code, value);

            // Assert
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void CanGetOK()
        {
            // Assert
            Assert.IsInstanceOfType(HandlerResult.OK, typeof(HandlerResult));
        }
    }
}