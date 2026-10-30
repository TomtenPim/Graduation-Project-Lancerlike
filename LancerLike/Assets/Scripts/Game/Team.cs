using Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace Game
{

    public class Team : ActionStack.ActionBehavior
    {
        private List<Mech> m_mechs;
        private bool firstEverActivation = true;

        [SerializeField] public bool m_bIsPlayerTeam = false;



        public override void OnBegin(bool bFirstTime)
        {
            base.OnBegin(bFirstTime);

            if (firstEverActivation)
            {
                firstEverActivation = false;
                if (m_bIsPlayerTeam && GetComponentsInChildren<Mech>().Length == 0)
                {
                    SafeOnLoadObject safeOnLoadObject = GameObject.FindGameObjectWithTag("safeObject").GetComponent<SafeOnLoadObject>();
                    safeOnLoadObject.LoadObjects(transform);

                    m_mechs = new List<Mech>(GetComponentsInChildren<Mech>());

                    foreach (Mech mech in m_mechs)
                    {
                        mech.attachToTeam(this);
                    }
                }
            }

            m_mechs = new List<Mech>(GetComponentsInChildren<Mech>());


            if (m_mechs.Count <= 0)
            {
                if (m_bIsPlayerTeam)
                {
                    Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
                    LoseAction lose = LoseAction.Create(canvas.transform);
                    ActionStack.Main.PushAction(lose);
                }
                else
                {
                    Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
                    WinAction win = WinAction.Create(canvas.transform);
                    ActionStack.Main.PushAction(win);
                }
            }

            if (bFirstTime)
            {
                // let units know a new turn has started!
                foreach (Mech mech in m_mechs)
                {
                    mech.OnNewTurn();
                }
            }

            // remove all units that have no remaining action points
            m_mechs.RemoveAll(u => u == null || u.remainingActionPoints == 0);

            // pick next unit
            Mech nextMech = m_mechs.Find(u => u.remainingActionPoints > 0);
            if (nextMech != null)
            {
                Battlefield.sm_instance.m_camera.ChangeFocus(nextMech.gameObject);
                ActionStack.Main.PushAction(nextMech);
            }
        }

        public List<Mech> GetMechs()
        {
            return m_mechs;
        }

        public override bool IsDone()
        {
            return m_mechs.Count == 0;
        }

        public override string ToString()
        {
            return "Team: " + name;
        }
    }
}