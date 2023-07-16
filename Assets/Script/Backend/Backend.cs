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
        public static GameStageBase GameLoop;

        public void Awake()
        {
            GameLoop = new GameStageTest();
            GameLoop.InitializeStage();
            DontDestroyOnLoad(this);
        }

        public void SetNextStage(string stageName)
        {
            var newStage = LoadStage(stageName);
            newStage.InitializeStage(GameLoop);
            GameLoop = newStage;
        }

        private GameStageBase LoadStage(string stageName)
        {
            return new GameStageTest();
        }
    }
}
