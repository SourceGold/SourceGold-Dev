using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Assets.Script.Backend
{
    public partial class GameSceneBase
    {
        public void InitializeStageGameObject(List<BackendGameObject> savedGameObjects)
        {
            foreach (var gameObject in savedGameObjects)
            {
                RegisterGameObject(gameObject);
            }
            InitializeStage();
        }

        public virtual void InitializeCharacters()
        {
        }

        public virtual void ProcessDamage(DamangeSource damangeSource, DamageTarget damageTarget)
        {
            GameEventLogger.LogEvent($"{damangeSource.SrcObjectName} hit {damageTarget.TgtObjectName} with {damangeSource.AttackWeapon} weapon and {damangeSource.AttackName} attack");
            var targetObj = AllGameObjectCollection[damageTarget.TgtObjectName];
            if (targetObj is HittableObject hittableObject)
            {
                hittableObject.GotDamanged(CalculateDamage(damangeSource, damageTarget));
                //if (!hittableObject.IsAlive)
                //{
                //    AllGameObjectCollection.Remove(targetObj.Name, out _);
                //}
            }
            else
            {
                throw new NotSupportedException($"Not supported target object type: {targetObj.GetType()}");
            }
        }

        public BackendGameObject GetGameObject(string objectName)
        {
            return AllGameObjectCollection[objectName];
        }

        public List<BackendGameObject> GetGameObjects(List<string> objectNames)
        {
            return objectNames.Select(o => AllGameObjectCollection[o]).ToList();
        }

        public void TriggerNetGameObjectEvent()
        {
            EventManager.TriggerEvent(GameEventTypes.FetchNewGameObjectsEvent);
        }

        public List<Enemy> GetSpawnedEnemies()
        {
            return GetSpawnedEnemies(null);
        }

        public void RegisterGameObject(BackendGameObject gameObject)
        {
            gameObject.RegisteredByGame = true;
            if (gameObject is HittableObject hittableObject)
            {
                RegisterHittableObject(hittableObject);
            }
            else if (gameObject is InvincibleObject invincibleObject)
            {
                RegisterInvincibleObject(invincibleObject);
            }
            else
            {
                throw new NotSupportedException($"Not supported game object type: {gameObject.GetType()}");
            }
        }

        public void RegisterGameObjects(List<BackendGameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                RegisterGameObject(gameObject);
            }
        }

        public void RegisterPlayerOnStatsChangeCallBack(Action<PlayableCharacterStats> onStatsChangedCallback)
        {
            var playableChars = GetPlayableCharacters();
            foreach (var playableChar in playableChars)
            {
                playableChar.SetOnStatsChangedCallback(onStatsChangedCallback);
            }
        }

        public void SetMainCharacter(string playerName)
        {
            var playableChars = GetPlayableCharacters();
            foreach (var playableChar in playableChars)
            {
                playableChar.IsMainCharacter = playableChar.Name == playerName;
            }
        }

        public PlayableCharacter GetMainCharacters()
        {
            return GetPlayableCharacters().Where(o => o.IsMainCharacter).Single();
        }

        public bool UnregisterGameObject(string objectName)
        {
            return AllGameObjectCollection.Remove(objectName, out _);
        }

        public List<PlayableCharacter> GetPlayableCharacters()
        {
            return FilterObjectsByType<PlayableCharacter>();
        }

        public List<HittableObject> GetHittableObjects()
        {
            return FilterObjectsByType<HittableObject>();
        }

        public List<BackendGameObject> GetNonRegisteredGameObjects()
        {
            return FilterObjectsBySelector(o => !o.RegisteredByGame);
        }

        public List<HittableObject> GetAliveHittableObjectObjects()
        {
            var hittableObjects = GetHittableObjects();
            return hittableObjects.Where(o => o.IsAlive).ToList();
        }

        public List<BackendGameObject> GetSavedGameObjects()
        {
            return FilterObjectsBySelector(o => o.SaveToNextStage);
        }

        protected List<Enemy> GetSpawnedEnemies(InvincibleObject spawnSettings)
        {
            return new List<Enemy>();
        }

        protected List<T> FilterObjectsByType<T>() where T : BackendGameObject
        {
            return AllGameObjectCollection.Where(o => o.Value is T).Select(o => o.Value as T).ToList();
        }

        protected List<BackendGameObject> FilterObjectsBySelector(Func<BackendGameObject, bool> selector)
        {
            return AllGameObjectCollection.Where(o => selector(o.Value)).Select(o => o.Value).ToList();
        }

        protected void RegisterHittableObject(HittableObject hittableObject)
        {
            if (hittableObject is PlayableCharacter playableCharacter)
            {
                playableCharacter.HittableObjectStats = LoadCharacterStats(playableCharacter.Name);
                AddGameObject(playableCharacter);
                SetMainCharacter(playableCharacter.Name);
            }
            else if (hittableObject is Enemy enemy)
            {
                enemy.HittableObjectStats = LoadEnemyStats(enemy.Name);
                AddGameObject(enemy);
            }
            else
            {
                throw new NotSupportedException($"Not supported hittable object type: {hittableObject.GetType()}");
            }
        }

        protected void RegisterInvincibleObject(InvincibleObject invincibleObject)
        {
            AddGameObject(invincibleObject);
        }

        protected void AddGameObject(BackendGameObject gameObject)
        {
            var objectName = gameObject.Name;
            if (AllGameObjectCollection.ContainsKey(objectName))
            {
                if (AllGameObjectCollection[objectName].SaveToNextStage)
                {
                    gameObject.SetGameObjectStates(AllGameObjectCollection[objectName].GetGameObjectStates());
                }
                else
                {
                    throw new Exception($"Object with name: {objectName} already exist in current stage's AllGameObjectCollection");
                }
            }
            AllGameObjectCollection[objectName] = gameObject;
        }

        protected virtual EnemyStats LoadEnemyStats(string parentName)
        {
            return new EnemyStats(parentName, maxHitPoint: 100, maxMagicPoint: 100, baseAttack: 30, baseDefence: 10);
        }

        protected virtual PlayableCharacterStats LoadCharacterStats(string parentName)
        {
            return new PlayableCharacterStats(parentName, maxMagicPoint: 100, maxHitPoint: 100, maxStamina: 100, baseAttack: 30, baseDefense: 10);
        }

        protected virtual GameObjectEnvironmentalStats LoadDefaultEnvironmentalStats(bool isEnemy = false)
        {
            if (isEnemy)
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
                },
                objectScale: new Vector3()
                {
                    X = 2,
                    Y = 2,
                    Z = 2
                });
            }
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
                },
                objectScale: new Vector3()
                {
                    X = 1,
                    Y = 1,
                    Z = 1
                });
        }

        protected virtual int CalculateDamage(DamangeSource damangeSource, DamageTarget damageTarget)
        {
            var srcObject = AllGameObjectCollection[damangeSource.SrcObjectName];
            if (srcObject is HittableObject hittableObject)
            {
                return hittableObject.HittableObjectStats.Attack;
            }
            else
            {
                throw new NotSupportedException($"Not supported target object type: {srcObject.GetType()}");
            }
        }
    }
}
