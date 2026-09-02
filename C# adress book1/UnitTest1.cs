using NUnit.Framework;

namespace C__adress_book1
{
    [TestFixture]
    public class Untitled
    {
        [Test]
        public void TestMethod1()
        {
            object firstObject = new object();
            object secondObject = new object();
            object anotherReferenceToFirstObject = firstObject;

            NUnit.Framework.Assert.That(secondObject, Is.Not.SameAs(firstObject));
            NUnit.Framework.Assert.That(anotherReferenceToFirstObject, Is.SameAs(firstObject));
        }
    }
}
