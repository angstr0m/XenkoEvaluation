using SiliconStudio.Xenko.Engine;
using XenkoTest.Tests;

namespace XenkoTest
{
    class XenkoTestApp
    {
        static void Main(string[] args)
        {
            using (var game = new Test_GameWithPhysics())
            {
                game.Run();
            }
        }
    }
}
