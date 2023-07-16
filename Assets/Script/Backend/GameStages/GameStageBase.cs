﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Assets.Script.Backend
{
    public class GameStageBase
    {
        protected ConcurrentDictionary<string, GameObject> AllGameObjectCollection { get; set; }

        protected EventLogger Logger { get; set; }

        public GameStageBase()
        {
            AllGameObjectCollection = new ConcurrentDictionary<string, GameObject>();
            Logger = new EventLogger();
        }

        public void InitializeStage()
        {
            InitializeCharacters();
        }

        public void InitializeStage(List<GameObject> savedGameObjects)
        {
            foreach (var gameObject in savedGameObjects)
            {
                RegisterGameObject(gameObject);
            }
            InitializeStage();
        }

        public void InitializeStage(GameStageBase previousStage)
        {
            var savedGameObjects = previousStage.GetSavedGameObjects();
            InitializeStage(savedGameObjects);
        }

        public virtual void InitializeCharacters()
        {
            AddGameObject(new PlayableCharacter("PlayerDefault", LoadCharacterStats(), LoadDefaultEnvironmentalStats()));
        }

        public virtual void ProcessDamage(DamangeSource damangeSource, DamageTarget damageTarget)
        {
            Logger.LogEvent($"{damangeSource.SrcObjectName} hit {damageTarget.TgtObjectName} with {damangeSource.AttackWeapon} weapon and {damangeSource.AttackName} attack");
            var targetObj = AllGameObjectCollection[damageTarget.TgtObjectName];
            if (targetObj is HittableObject hittableObject)
            {
                hittableObject.GotHit(CalculateDamage(damangeSource, damageTarget), Logger);
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

        public GameObject GetGameObject(string objectName)
        {
            return AllGameObjectCollection[objectName];
        }

        public List<GameObject> GetGameObjects(List<string> objectNames)
        {
            return objectNames.Select(o => AllGameObjectCollection[o]).ToList();
        }

        public void RegisterGameObject(GameObject gameObject)
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

        public void RegisterGameObjects(List<GameObject> gameObjects)
        {
            foreach(var gameObject in gameObjects)
            {
                RegisterGameObject(gameObject);
            }
        }

        public bool UnregisterGameObject(string objectName)
        {
            return AllGameObjectCollection.Remove(objectName, out _);
        }

        public List<HittableObject> GetHittableObjects()
        {
            return FilterObjectsByType<HittableObject>();
        }

        public List<GameObject> GetNonRegisteredGameObjects()
        {
            return FilterObjectsBySelector(o => !o.RegisteredByGame);
        }

        public List<HittableObject> GetAliveHittableObjectObjects()
        {
            var hittableObjects = GetHittableObjects();
            return hittableObjects.Where(o => o.IsAlive).ToList();
        }

        public List<GameObject> GetSavedGameObjects()
        {
            return FilterObjectsBySelector(o => o.SaveToNextStage);
        }

        protected List<T> FilterObjectsByType<T>() where T : GameObject
        {
            return AllGameObjectCollection.Where(o => o.Value is T).Select(o => o.Value as T).ToList();
        }

        protected List<GameObject> FilterObjectsBySelector(Func<GameObject, bool> selector)
        {
            return AllGameObjectCollection.Where(o => selector(o.Value)).Select(o => o.Value).ToList();
        }

        protected void RegisterHittableObject(HittableObject hittableObject)
        {
            if (hittableObject is PlayableCharacter playableCharacter)
            {
                playableCharacter.HittableObjectStats = LoadCharacterStats();
                AddGameObject(playableCharacter);
            }
            else if (hittableObject is Enemy enemy)
            {
                enemy.HittableObjectStats = LoadEnemyStats();
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

        protected void AddGameObject(GameObject gameObject)
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

        protected void SendFetchNewGameObjectsEvent()
        {
            EventManager.TriggerEvent(EventType.FetchNewGameObjectsEventName);
            Logger.LogEvent($"Event Triggered: {EventType.FetchNewGameObjectsEventName}");
        }

        protected virtual EnemyStats LoadEnemyStats()
        {
            return new EnemyStats(maxHitPoint: 100, attackDmg: 30, defense: 10);
        }

        protected virtual PlayableCharacterStats LoadCharacterStats()
        {
            return new PlayableCharacterStats(maxHitPoint: 100, attackDmg: 30, defense: 10);
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
                return hittableObject.HittableObjectStats.AttackDmg;
            }
            else
            {
                throw new NotSupportedException($"Not supported target object type: {srcObject.GetType()}");
            }
        }
    }
}
