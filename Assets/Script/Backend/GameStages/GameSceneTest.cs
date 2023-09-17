namespace Assets.Script.Backend
{
    public class GameSceneTest : GameSceneBase
    {
        public override void InitializeCharacters()
        {
            // this is test code to register a default enemy
            //var enemyName = "EnemyDefault";
            //AddGameObject(new Enemy(enemyName, LoadEnemyStats(enemyName), LoadDefaultEnvironmentalStats(isEnemy: true)));
            base.InitializeCharacters();
        }
    }
}
