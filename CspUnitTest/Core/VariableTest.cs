using Csp.Csp.Model;
using Moq;
using Xunit;

namespace CspUnitTest.Core
{
    public class VariableTest
    {
        private readonly Variable<DummyCspValue> _sut;

        public VariableTest()
        {
            _sut = new Variable<DummyCspValue>("X");
        }

        [Fact]
        public void ShouldAssignAValue()
        {
            // Setup
            var val = new Mock<DummyCspValue>("Y");
            val.Setup(v => v.RevokeCallback()).CallBase();
            val.Setup(v => v.AssignmentCallback()).CallBase();

            // Act
            _sut.Value = val.Object;

            // Verify
            Assert.True(_sut.Assigned);
            val.Verify(v => v.AssignmentCallback(), Times.Exactly(1));
            val.Verify(v => v.RevokeCallback(), Times.Never);
        }

        [Fact]
        public void ShouldRevokeValue()
        {
            // Setup
            var val = new Mock<DummyCspValue>("Y");
            val.Setup(v => v.RevokeCallback()).CallBase();
            val.Setup(v => v.AssignmentCallback()).CallBase();

            _sut.Value = val.Object;

            // Act
            _sut.Value = null;

            // Verify
            Assert.True(!_sut.Assigned);
            val.Verify(v => v.RevokeCallback(), Times.Exactly(1));
        }
    }
}