using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SiliconStudio.Xenko.Engine;

namespace XenkoTest.Tests
{
    public class Test_DestroyAfter : AsyncScript
    {
        private Timer destroyTimer;

        public int Lifetime = 10000;
        
        public override async Task Execute()
        {
            await Task.Delay(Lifetime);
            await Script.NextFrame();

            SceneSystem.SceneInstance.Scene.Entities.Remove(Entity);
        }
    }
}
