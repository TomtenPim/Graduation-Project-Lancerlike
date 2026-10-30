using Actions;
using Graphs;
using Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.UI.CanvasScaler;

namespace Game
{
    public class Battlefield : ActionStack.ActionBehavior
    {
        public BattlefieldCamera m_camera;
        private Queue<Team> m_teamQueue;

        #region Properties

        public Ray MouseRay => m_camera.thisCamera.ScreenPointToRay(Input.mousePosition);

        #endregion

        public static Battlefield sm_instance;

        void Start()
        {
            sm_instance = this;
            m_camera = GetComponentInChildren<BattlefieldCamera>();
            ActionStack.Main.PushAction(this);
        }

        public override void OnBegin(bool bFirstTime)
        {
            base.OnBegin(bFirstTime);

            if (bFirstTime)
            {
                m_teamQueue = new Queue<Team>(GetComponentsInChildren<Team>());
            }

            // do the team loop!
            Team team = m_teamQueue.Dequeue();
            m_teamQueue.Enqueue(team);
            ActionStack.Main.PushAction(team);
        }

        public override bool IsDone()
        {
            return false;
        }
    }
}