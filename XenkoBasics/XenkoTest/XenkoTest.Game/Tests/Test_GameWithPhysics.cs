using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Diagnostics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Games;
using SiliconStudio.Xenko.Physics;

namespace XenkoTest.Tests
{
    public class Test_GameWithPhysics : Game
    {
        public Test_GameWithPhysics() : base()
        {
            
        }

        protected override void BeginRun()
        {
            base.BeginRun();
            //Profiler.EnableAll();
            //Profiler.Enable(GameProfilingKeys.GameDrawFPS);
        }
    }
}
