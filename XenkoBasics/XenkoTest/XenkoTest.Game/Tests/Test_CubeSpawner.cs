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
        private readonly int columnsZ = 30;
        private readonly float cubeAdditionalOffsetY = 1;
        private readonly float cubebaseOffsetY = -0.5f;
        private readonly int moveDelay = 1000;
        private readonly Vector3 offestX = Vector3.UnitX;
        private readonly Vector3 offsetPos = new Vector3(-15, 0, -15);
        private readonly Vector3 offsetZ = Vector3.UnitZ;
        private readonly Random rand = new Random();
        private readonly int rowsX = 30;
        private readonly int spawnDelay = 10000;

        public override async Task Execute()
        {
            var cubes = await SpawnCubesInGrid();
            await InitializeCubePositions(cubes);

            var moveTasks = new List<Task>();
            

            while (Game.IsRunning)
            {
                moveTasks.Clear();

                foreach (var cube in cubes)
                {
                    var baseOffsetVector = cube.Transform.Position;
                    baseOffsetVector.Y = 0;

                    var tempOffsetY = Vector3.UnitY*
                                      (float) (cubebaseOffsetY + rand.NextDouble()*cubeAdditionalOffsetY);

                    var delay = (int) (moveDelay*rand.NextDouble());
                    var moveTask = MoveCubeToPositionWithDelay(cube, baseOffsetVector + tempOffsetY, delay);

                    moveTasks.Add(moveTask);
                }

                foreach (var moveTask in moveTasks)
                {
                    await moveTask;
                }

                await ToggleCubesActiveAtRandom(cubes);
            }
        }

        private async Task ToggleCubesActiveAtRandom(List<Entity> cubes)
        {
            var toggleTasks = new List<Task>();

            foreach (var cube in cubes)
            {
                toggleTasks.Add(ToggleCubeIsActiveAtRandom(cube));
            }

            foreach (var toggleTask in toggleTasks)
            {
                await toggleTask;
            }
        }

        private async Task AddCubeToScene(Entity newCube)
        {
            await Task.Delay((int) (spawnDelay*rand.NextDouble()));
            await Script.NextFrame();

            SceneSystem.SceneInstance.Scene.Entities.Add(newCube);
        }

        private async Task InitializeCubePositions(List<Entity> cubes)
        {
            foreach (var cube in cubes)
            {
                var baseOffsetVector = cube.Transform.Position;
                baseOffsetVector.Y = 0;

                var tempOffsetY = Vector3.UnitY*
                                  (float) (cubebaseOffsetY + rand.NextDouble()*cubeAdditionalOffsetY);

                var delay = (int) (moveDelay*rand.NextDouble()*0.1f);
                var moveTask = MoveCubeToPositionWithDelay(cube, baseOffsetVector + tempOffsetY, delay);

                if (rand.NextDouble() < 0.1f)
                {
                    await moveTask;
                }
            }
        }

        private async Task MoveCubeToPositionWithDelay(Entity cube, Vector3 position, int delay)
        {
            await Task.Delay(delay);
            await Script.NextFrame();

            cube.Transform.Position = position;
        }

        private async Task ToggleCubeIsActiveAtRandom(Entity cube)
        {
            await Task.Delay((int) (spawnDelay*rand.NextDouble()));
            await Script.NextFrame();

            var toggleActive = rand.NextDouble() >= 0.5f;

            if (toggleActive)
            {
                foreach (var physicsElement in cube.Get<PhysicsComponent>().Elements)
                {
                    physicsElement.Collider.Enabled = !physicsElement.Collider.Enabled;
                    physicsElement.RigidBody.Enabled = !physicsElement.RigidBody.Enabled;
                    
                }
                cube.Get<ModelComponent>().Enabled = !cube.Get<ModelComponent>().Enabled;
            }
        }

        private Entity GetNewCube(Vector3 position)
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

        private async Task<List<Entity>> SpawnCubesInGrid()
        {
            var cubes = new List<Entity>();
            var tasks = new List<Task>();

            for (var i = 0; i < rowsX; i++)
            {
                for (var j = 0; j < columnsZ; j++)
                {
                    var newCube = GetNewCube(offsetPos + offestX*i + offsetZ*j);

                    cubes.Add(newCube);
                    tasks.Add(AddCubeToScene(newCube));
                }
            }

            foreach (var task in tasks)
            {
                await task;
            }

            return cubes;
        }
    }
}