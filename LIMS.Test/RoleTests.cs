using NUnit.Framework;

namespace LIMS.Test
{
    [TestFixture]
    public class RoleTests
    {
        [Test]
        public void PrivilegedContainsAll()
        {
            var expected = new[] { Roles.Faculty, Roles.Administrator };
            var actual = Roles.Privileged.Split(',');
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
