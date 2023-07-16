namespace Assets.Script.Backend
{
    public class GameStageTest : GameStageBase
    {
        public override void InitializeCharacters()
        {
            AddGameObject(new Enemy("EnemyDefault", LoadEnemyStats(), LoadDefaultEnvironmentalStats(isEnemy: true)));
            base.InitializeCharacters();
        }
    }
}
