using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Backend
{
    public class GameStageTest: GameStageBase
    {
        public override void InitializeCharacters()
        {
            AddHittableObject("Enemy1", new HittableObject("Enemy1", LoadEnemyStats(), HittableObjectType.Enemy));
            base.InitializeCharacters();
        }
    }
}
