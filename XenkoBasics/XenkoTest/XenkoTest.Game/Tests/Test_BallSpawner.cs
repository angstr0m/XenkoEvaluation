using System;
using System.Collections.Concurrent;
using System.Threading;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Physics;
using SiliconStudio.Xenko.Rendering;
using XenkoTest.Tests;

namespace XenkoTest
{
    public class Test_BallSpawner : SyncScript
    {
        private ConcurrentQueue<Entity> initializedBalls = new ConcurrentQueue<Entity>();
        private int spawnIntervall = 10;
        private float spawnRadiusXZ = 10;
        private Timer timer;

        public override void Start()
        {
            var entity = Entity;
            timer = new Timer(SpawnBall, null, spawnIntervall, spawnIntervall);
        }

        public override void Update()
        {
            Entity newBallFromQueue;

            while (!initializedBalls.IsEmpty)
            {
                if (initializedBalls.TryDequeue(out newBallFromQueue))
                {
                    newBallFromQueue.GetOrCreate<ScriptComponent>().Scripts.Add(new Test_DestroyAfter());

                    SceneSystem.SceneInstance.Scene.Entities.Add(newBallFromQueue);

                    newBallFromQueue.GetOrCreate<PhysicsComponent>()[0].RigidBody.ApplyImpulse(10.0f * Vector3.UnitY);
                }
            }
        }

        private void SpawnBall(object state)
        {
            var rand = new Random();
            var spawnPos = new Vector3(-5.0f + (float) rand.NextDouble()*spawnRadiusXZ, 1,
                -5.0f + (float) rand.NextDouble()*spawnRadiusXZ);

            initializedBalls.Enqueue(SpawnBallAtPosition(spawnPos));
        }

        private Entity SpawnBallAtPosition(Vector3 position)
        {
            var newBall = new Entity(position);

            var ballModel = newBall.GetOrCreate<ModelComponent>();
            ballModel.Model = Asset.Load<Model>("Sphere");

            var ballPhysics = newBall.GetOrCreate<PhysicsComponent>();
            var rigidbody = new RigidbodyElement();

            var colliderShape = new SphereColliderShapeDesc();
            colliderShape.Radius = 0.5f;

            rigidbody.ColliderShapes.Add(colliderShape);

            ballPhysics.Elements.Add(rigidbody);

            return newBall;
        }
    }
}