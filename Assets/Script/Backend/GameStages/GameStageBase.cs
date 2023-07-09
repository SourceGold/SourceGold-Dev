using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;

namespace Assets.Script.Backend
{
    public class GameStageBase
    {
        protected ConcurrentDictionary<string, HittableObject> HittableObjectCollection { get; set; }

        protected EventLogger Logger { get; set; }

        public GameStageBase()
        {
            HittableObjectCollection = new ConcurrentDictionary<string, HittableObject>();
            Logger = new EventLogger();
        }

        public void InitializeStage()
        {
            InitializeCharacters();
        }

        public virtual void InitializeCharacters()
        {
            AddHittableObject("Player", new PlayableCharacter("Player", LoadCharacterStats(), LoadDefaultEnvironmentalStats()));
        }

        public virtual void ProcessDamage(DamangeSource damangeSource, DamageTarget damageTarget)
        {
            Logger.LogEvent($"{damangeSource.SrcObjectName} hit {damageTarget.TgtObjectName} with {damangeSource.AttackWeapon} weapon and {damangeSource.AttackName} attack");
            var targetObj = HittableObjectCollection[damageTarget.TgtObjectName];
            targetObj.GotHit(CalculateDamage(damangeSource, damageTarget), Logger);

            if (!targetObj.IsAlive)
            {
                HittableObjectCollection.Remove(targetObj.Name, out _);
            }
        }

        public virtual List<HittableObject> GetHittableObjects()
        {
            return new List<HittableObject>(HittableObjectCollection.Values);
        }

        protected void AddHittableObject(string objectName, HittableObject hittableObjects)
        {
            if (HittableObjectCollection.ContainsKey(objectName))
            {
                throw new Exception($"Object with name: {objectName} already exist in current stage");
            }
            HittableObjectCollection[objectName] = hittableObjects;
        }

        protected virtual EnemyStats LoadEnemyStats()
        {
            return new EnemyStats(maxHitPoint: 100, attackDmg: 30, defense: 10);
        }

        protected virtual PlayableCharacterStats LoadCharacterStats()
        {
            return new PlayableCharacterStats(maxHitPoint: 100, attackDmg: 30, defense: 10);
        }

        protected virtual GameObjectEnvironmentalStats LoadDefaultEnvironmentalStats()
        {
            return new GameObjectEnvironmentalStats(
                spawnLocation: new Vector3()
        {
                    X = 0,
                    Y = 0,
                    Z = 0
                },
                spawnRotation: new Vector2()
            {
                    X = 1,
                    Y = 0
                });
        }

        protected virtual int CalculateDamage(DamangeSource damangeSource, DamageTarget damageTarget)
        {
            var srcObject = HittableObjectCollection[damangeSource.SrcObjectName];
            return srcObject.HittableObjectStats.AttackDmg;
        }
    }
}
