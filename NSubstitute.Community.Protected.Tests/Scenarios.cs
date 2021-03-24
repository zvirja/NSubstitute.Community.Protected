using System;
using Xunit;

namespace NSubstitute.Community.Protected.Tests
{
    public class Scenarios
    {
        [Fact]
        public void Test1()
        {
            var sut = Substitute.ForPartsOf<TestClass>();
            var sutProtected = sut.Protected<ITestClassPrivateInterface>();

            // sutProtected.GetValue().Returns(42);

            var result = sut.ValueAccessor();

            sut.Protected<ITestClassPrivateInterface>().Received().GetValue();
        }
    }

    public class TestClass
    {
        protected virtual int GetValue() => 0;
        
        protected virtual int Prop => 0;

        public int ValueAccessor() => GetValue();

        public int PropAccessor() => Prop;
    }

    public interface ITestClassPrivateInterface
    {
        int GetValue();
        
        int Prop { get; }
    }
}