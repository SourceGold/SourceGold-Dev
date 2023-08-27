namespace Assets.Script.Backend
{
    public class GameSceneTest : GameSceneBase
    {
        public override void InitializeCharacters()
        {
            var enemyName = "EnemyDefault";
            AddGameObject(new Enemy(enemyName, LoadEnemyStats(enemyName), LoadDefaultEnvironmentalStats(isEnemy: true)));
            base.InitializeCharacters();
        }
    }
}
