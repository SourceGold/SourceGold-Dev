using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.Backend
{
    public class Backend : MonoBehaviour
    {
        public static GameSceneBase GameLoop;

        public void Awake()
        {
            GameEventLogger.LogEvent("Game Backend Awaken", EventLogType.SystemEvent);
            GameLoop = new GameSceneTest();
            GameLoop.InitializeStage();
            DontDestroyOnLoad(this);
        }

        public void SetNextStage(string stageName)
        {
            // broadcast to all game objects the new state
            var newStage = LoadStage(stageName);
            newStage.InitializeStage(GameLoop);
            GameLoop = newStage;
        }

        private GameSceneBase LoadStage(string stageName)
        {
            return new GameSceneTest();
        }
    }
}
