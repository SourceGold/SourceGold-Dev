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

        public void Start()
        {
            GameLoop = new GameStageTest();
            GameLoop.InitializeStage();
        }
    }
}
