using SiliconStudio.Xenko.Engine;

namespace XenkoTest.Tests
{
    public class Test_GameWithPhysics : Game
    {
        protected override void BeginRun()
        {
            base.BeginRun();
            //Profiler.EnableAll();
            //Profiler.Enable(GameProfilingKeys.GameDrawFPS);
        }
    }
}