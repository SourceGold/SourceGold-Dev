using System.Numerics;

namespace Assets.Script.Backend
{
    public class GameObjectEnvironmentalStats
    {
        public Vector3 SpawnLocation { get; set; }

        public Vector2 SpawnRotation { get; set; }

        public Vector3 ObjectScale { get; set; }

        public GameObjectEnvironmentalStats(Vector3 spawnLocation, Vector2 spawnRotation, Vector3 objectScale)
        {
            SpawnLocation = spawnLocation;
            SpawnRotation = spawnRotation;
            ObjectScale = objectScale;
        }
    }
}
