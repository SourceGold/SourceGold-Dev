namespace Assets.Script.Backend
{
    public class GameStageTest : GameStageBase
    {
        public override void InitializeCharacters()
        {
            AddHittableObject("Enemy1", new Enemy("Enemy1", LoadEnemyStats(), LoadDefaultEnvironmentalStats(isEnemy: true)));
            base.InitializeCharacters();
        }
    }
}
