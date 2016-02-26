using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Physics;
using SiliconStudio.Xenko.Rendering;

namespace XenkoTest.Tests
{
    public class Test_CubeSpawner : AsyncScript
    {
        private int columnsZ = 30;
        private float cubeAdditionalOffsetY = 1;
        
        private float cubebaseOffsetY = -0.5f;
        private int moveDelay = 1000;

        private Vector3 offestX = Vector3.UnitX;

        private Vector3 offsetPos = new Vector3(-15, 0, -15);
        private Vector3 offsetZ = Vector3.UnitZ;
        private Random rand = new Random();

        private int rowsX = 30;

        private int spawnDelay = 10000;

        public override async Task Execute()
        {
            var cubes = new List<Entity>();
            var tasks = new List<Task>();

            for (var i = 0; i < rowsX; i++)
            {
                for (var j = 0; j < columnsZ; j++)
                {
                    var newCube = SpawnCubeAtPosition(offsetPos + offestX*i + offsetZ*j);

                    cubes.Add(newCube);
                    tasks.Add(AddCubeToScene(newCube));
                }
            }

            foreach (var task in tasks)
            {
                await task;
            }
            tasks.Clear();

            var moveTasks = new List<Task>();
            var toggleTasks = new List<Task>();

            foreach (var cube in cubes)
            {
                var baseOffsetVector = cube.Transform.Position;
                baseOffsetVector.Y = 0;

                var tempOffsetY = Vector3.UnitY *
                                  (float)((cubebaseOffsetY + (rand.NextDouble() * cubeAdditionalOffsetY)));

                var delay = (int)(moveDelay * rand.NextDouble() * 0.1f);
                var moveTask = MoveCubeToPositionWithDelay(cube, baseOffsetVector + tempOffsetY, delay);

                if (rand.NextDouble() < 0.1f)
                {
                    await moveTask;
                }
            }

            while (Game.IsRunning)
            {
                moveTasks.Clear();
                toggleTasks.Clear();

                foreach (var cube in cubes)
                {
                    var baseOffsetVector = cube.Transform.Position;
                    baseOffsetVector.Y = 0;

                    var tempOffsetY = Vector3.UnitY*
                                      (float) ((cubebaseOffsetY + (rand.NextDouble()*cubeAdditionalOffsetY)));

                    var delay = (int) (moveDelay*rand.NextDouble());
                    var moveTask = MoveCubeToPositionWithDelay(cube, baseOffsetVector + tempOffsetY, delay);

                    moveTasks.Add(moveTask);
                }

                foreach (var moveTask in moveTasks)
                {
                    await moveTask;
                }

                //foreach (var cube in cubes)
                //{
                //    toggleTasks.Add(RandomCubeActive(cube));
                //}

                //foreach (var toggleTask in toggleTasks)
                //{
                //    await toggleTask;
                //}
            }
        }
        
        private async Task RandomCubeActive(Entity cube)
        {
            await Task.Delay((int)(spawnDelay * rand.NextDouble()));
            await Script.NextFrame();

            var toggleActive = rand.NextDouble() >= 0.5f;
            
            if (toggleActive)
            {
                foreach (var physicsElement in cube.Get<PhysicsComponent>().Elements)
                {
                    physicsElement.RigidBody.Type = RigidBodyTypes.Kinematic;
                    physicsElement.Collider.Enabled = !physicsElement.Collider.Enabled;
                    physicsElement.RigidBody.Enabled = !physicsElement.RigidBody.Enabled;
                }
                cube.Get<ModelComponent>().Enabled = !cube.Get<ModelComponent>().Enabled;
            }
        }

        private async Task AddCubeToScene(Entity newCube)
        {
            await Task.Delay((int) (spawnDelay*rand.NextDouble()));
            await Script.NextFrame();

            SceneSystem.SceneInstance.Scene.Entities.Add(newCube);
        }

        private async Task MoveCubeToPositionWithDelay(Entity cube, Vector3 position, int delay)
        {
            await Task.Delay(delay);
            await Script.NextFrame();

            cube.Transform.Position = position;
        }

        private Entity SpawnCubeAtPosition(Vector3 position)
        {
            var newBall = new Entity(position);

            var ballModel = newBall.GetOrCreate<ModelComponent>();
            ballModel.Model = Asset.Load<Model>("Cube");

            var ballPhysics = newBall.GetOrCreate<PhysicsComponent>();
            var rigidbody = new RigidbodyElement();

            var colliderShape = new BoxColliderShapeDesc();
            colliderShape.Size = Vector3.One;

            rigidbody.ColliderShapes.Add(colliderShape);
            rigidbody.IsKinematic = true;
            
            ballPhysics.Elements.Add(rigidbody);

            return newBall;
        }
    }
}