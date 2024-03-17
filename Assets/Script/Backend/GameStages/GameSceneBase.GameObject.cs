﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Assets.Script.Backend
{
    public partial class GameSceneBase
    {
        private Action<PlayableCharacterStats> _playerOnStatsChangedCallback = null;

        protected WeaponProvidor WeaponProvidor;

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
            // this is test code to register a default player object
            //string playerName = "PlayerDefault";
            //AddGameObject(new PlayableCharacter(playerName, LoadCharacterStats(playerName), LoadDefaultEnvironmentalStats(), isMainCharacter: true));
        }

        public virtual void ProcessDamage(DamageSource damageSource, DamageTarget damageTarget)
        {
            GameEventLogger.LogEvent($"{damageSource.SrcObjectName} hit {damageTarget.TgtObjectName} with {damageSource.AttackWeapon} weapon and {damageSource.AttackName} attack");
            var targetObj = AllGameObjectCollection[damageTarget.TgtObjectName];
            if (targetObj is HittableObject hittableObject)
            {
                hittableObject.GotDamanged(CalculateDamage(damageSource, damageTarget));
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
            if (AllGameObjectCollection.ContainsKey(gameObject.Name))
            {
                throw new Exception($"Object with the name {gameObject.Name} already registered, please make sure each object has unique name.");
            }

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
            _playerOnStatsChangedCallback = onStatsChangedCallback;
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
                playableCharacter.SetOnStatsChangedCallback(_playerOnStatsChangedCallback);
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
                    gameObject.SetGameObjectStats(AllGameObjectCollection[objectName].GetGameObjectBaseStats());
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
            return new EnemyStats(parentName, maxHitPoint: 100, maxMagicPoint: 100, baseAttack: 15, baseDefence: 10);
        }

        protected virtual PlayableCharacterStats LoadCharacterStats(string parentName)
        {
            return new PlayableCharacterStats(parentName, maxMagicPoint: 100, maxHitPoint: 100, maxStamina: 100, baseAttack: 15, baseDefense: 10);
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

        protected virtual int CalculateDamage(DamageSource damageSource, DamageTarget damageTarget)
        {
            var srcObject = AllGameObjectCollection[damageSource.SrcObjectName];
            if (srcObject is HittableObject hittableObject)
            {
                var finalAttackStats = hittableObject.HittableObjectStats.Attack;
                var damageMultiplier = 1.0f;
                if (!string.IsNullOrEmpty(damageSource.AttackWeapon))
                {
                    var weaponStats = WeaponProvidor.GetWeaponStats(damageSource.AttackWeapon);
                    finalAttackStats += weaponStats.WeaponAttack;
                    damageMultiplier = weaponStats.GetDamageMultiplier();
                }
                var finalDamage = finalAttackStats * damageMultiplier;
                return (int)Math.Round(finalDamage);
            }
            else
            {
                throw new NotSupportedException($"Not supported target object type: {srcObject.GetType()}");
            }
        }
    }
}
