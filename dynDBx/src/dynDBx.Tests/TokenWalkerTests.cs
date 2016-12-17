using dynDBx.Utilities;
using Newtonsoft.Json.Linq;
using Xunit;

namespace dynDBx.Tests
{
    public class TokenWalkerTests
    {
        [Fact]
        public void ParentsAreCreated()
        {
            JObject root = new JObject(
                new JProperty("lol", new JObject())
            );
            var tokWalker = new JTokenWalker(root, "lol.food.bread.tasty");
            var node = tokWalker.WalkAndCreateNode();
            Assert.NotNull(node);
        }
    }
}