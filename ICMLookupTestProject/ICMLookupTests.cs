using NUnit.Framework;
using System;
using System.Linq;
using ICMLookup;

namespace ICMLookupTests
{
    [TestFixture]
    public class ICMLookupTests
    {
        private Lookup _icmLookup;

        [SetUp]
        public void Setup() => _icmLookup = new Lookup();

        [Test]
        public void TestFind_ValidCode_ReturnsICMCode()
        {
            var code = "A000";
            var result = _icmLookup.Find(code, CodeType.ICM10Diag);

            Assert.IsNotNull(result);
            Assert.AreEqual(code, result.Code);
        }

        [Test]
        public void TestFind_InvalidCode_ReturnsNull()
        {
            var code = "InvalidCode";
            var result = _icmLookup.Find(code, CodeType.ICM10Diag);

            Assert.IsNull(result);
        }

        [Test]
        public void TestSearch_ValidCode_ReturnsTop10()
        {
            var code = "A000";
            var results = _icmLookup.Search(code);

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count <= 10);
            Assert.IsTrue(results.Any(icmCode => icmCode.Code == code));
        }

        [Test]
        public void TestGetSamples_ValidCount_ReturnsICMCodes()
        {
            var count = 5;
            var results = _icmLookup.GetSamples(CodeType.ICM10Diag, count);

            Assert.IsNotNull(results);
            Assert.AreEqual(count, results.Count);
        }

        [Test]
        public void TestGetSamples_InvalidCount_ThrowsArgumentException()
        {
            var count = 100000;

            Assert.Throws<ArgumentException>(() => _icmLookup.GetSamples(CodeType.ICM10Diag, count));
        }

        [Test]
        public void TestGetSamples_InvalidCodeType_ThrowsKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => _icmLookup.GetSamples((CodeType)100, 5));
        }
    }
}