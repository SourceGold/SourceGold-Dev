namespace Assets.Script.Backend
{
    public class GameSceneTest : GameSceneBase
    {
        public override void InitializeCharacters()
        {
            AddGameObject(new Enemy("EnemyDefault", LoadEnemyStats(), LoadDefaultEnvironmentalStats(isEnemy: true)));
            base.InitializeCharacters();
        }
    }
}
