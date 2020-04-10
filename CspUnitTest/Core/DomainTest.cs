using System.Linq;
using Csp.Csp.Model;
using Xunit;

namespace CspUnitTest.Core
{
    public class DomainTest
    {
        private readonly Domain<DummyCspValue> _sut;

        public DomainTest()
        {
            _sut = new Domain<DummyCspValue>("X", new []{ new DummyCspValue("Y"), new DummyCspValue("Z") });
        }

        [Fact]
        public void ShouldPruneAValue()
        {
            // Setup
            var val = new DummyCspValue("Y");

            // Act
            _sut.Prune(val);

            // Verify
            Assert.True(!_sut.Values.Exists(v => v.C == val.C));
            Assert.True(_sut.Pruned.Exists(v => v.C == val.C));
            Assert.True(!_sut.RemovedByGuess.Exists(v => v.C == val.C));

            // Act
            _sut.RestorePruned();

            // Verify
            Assert.True(_sut.Values.Exists(v => v.C == val.C));
            Assert.True(!_sut.Pruned.Exists(v => v.C == val.C));
            Assert.True(!_sut.RemovedByGuess.Exists(v => v.C == val.C));
        }

        [Fact]
        public void ShouldRemoveByGuess()
        {
            // Setup
            var val = new DummyCspValue("Y");

            // Act
            _sut.Suppose(val);

            // Verify
            Assert.True(_sut.Values.Exists(v => v.C == val.C));
            Assert.True(!_sut.Pruned.Exists(v => v.C == val.C));
            Assert.True(!_sut.RemovedByGuess.Exists(v => v.C == val.C));
            Assert.True(_sut.RemovedByGuess.Any());

            // Act
            _sut.RestoreGuess();

            // Verfy
            Assert.True(_sut.Values.Exists(v => v.C == val.C));
            Assert.True(!_sut.Pruned.Exists(v => v.C == val.C));
            Assert.True(!_sut.RemovedByGuess.Exists(v => v.C == val.C));
        }

        [Fact]
        public void ShouldShrink()
        {
            // Setup
            var val = new DummyCspValue("Y");

            // Act
            _sut.Shrink(val);

            // Verify
            Assert.True(_sut.Values.Exists(v => v.C == val.C));
            Assert.False(_sut.Pruned.Any());
            Assert.Single(_sut.Values);
            Assert.False(_sut.RemovedByGuess.Any());
        }
    }
}