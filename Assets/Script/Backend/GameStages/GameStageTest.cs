namespace Assets.Script.Backend
{
    public class GameStageTest : GameStageBase
    {
        public override void InitializeCharacters()
        {
            AddHittableObject("Enemy1", new Enemy("Enemy1", LoadEnemyStats(), LoadDefaultEnvironmentalStats(true)));
            base.InitializeCharacters();
        }
    }
}
