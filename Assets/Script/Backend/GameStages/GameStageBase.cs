using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Assets.Script.Backend
{
    public class GameStageBase
    {
        protected ConcurrentDictionary<string, HittableObject> HittableObjectCollection { get; set; }

        public GameStageBase()
        {
            HittableObjectCollection = new ConcurrentDictionary<string, HittableObject>();
        }

        public void InitializeStage()
        {
            InitializeCharacters();
        }

        public virtual void InitializeCharacters()
        {
            AddHittableObject("Player", new PlayableCharacter("Player", LoadCharacterStats()));
        }

        public virtual void ProcessDamage(DamangeSource damangeSource, DamageTarget damageTarget)
        {
            var targetObj = HittableObjectCollection[damageTarget.TgtObjectName];
            targetObj.GotHit(CalculateDamage(damangeSource, damageTarget));

            if (!targetObj.IsAlive)
            {
                HittableObjectCollection.Remove(targetObj.Name, out _);
            }
        }

        protected void AddHittableObject(string objectName, HittableObject hittableObjects)
        {
            if (HittableObjectCollection.ContainsKey(objectName))
            {
                throw new Exception($"Object with name: {objectName} already exist in current stage");
            }
            HittableObjectCollection[objectName] = hittableObjects;
        }

        protected virtual HittableObjectStats LoadEnemyStats()
        {
            return new HittableObjectStats(maxHitPoint: 100, attackDmg: 30, defense: 10);
        }

        protected virtual PlayableCharacterStats LoadCharacterStats()
        {
            return new PlayableCharacterStats(maxHitPoint: 100, attackDmg: 30, defense: 10);
        }

        protected virtual int CalculateDamage(DamangeSource damangeSource, DamageTarget damageTarget)
        {
            var srcObject = HittableObjectCollection[damangeSource.SrcObjectName];
            return srcObject.HittableObjectStats.AttackDmg;
        }
    }
}
