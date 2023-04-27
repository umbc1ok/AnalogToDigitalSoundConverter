namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Logic.SoundConverter.Main();
            Assert.AreEqual(1, 1);
        }
    }
}