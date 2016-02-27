using System.Threading;
using System.Threading.Tasks;
using SiliconStudio.Xenko.Engine;

namespace XenkoTest.Tests
{
    public class Test_DestroyAfter : AsyncScript
    {
        public int Lifetime = 3000;
        private Timer destroyTimer;

        public override async Task Execute()
        {
            await Task.Delay(Lifetime);
            await Script.NextFrame();

            SceneSystem.SceneInstance.Scene.Entities.Remove(Entity);
        }
    }
}