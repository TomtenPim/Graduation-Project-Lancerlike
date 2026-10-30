using Actions;
using Graphs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    /*
    public class Unit : ActionStack.ActionBehavior
    {
        public abstract class UnitAction : ActionStack.Action
        {
            protected Unit m_unit;

            public UnitAction(Unit unit)
            {
                m_unit = unit;
            }
        }

        [SerializeField, Range(1, 100)]
        public int                  m_iHP = 2;

        [SerializeField, Range(1, 10)]
        public int                  m_iMovementRange = 4;

        private Battlefield.Tile    m_tile;

        public static List<Unit>    AllUnits = new List<Unit>();

        #region Properties

        public Battlefield.Tile Tile
        {
            get => m_tile;
            set
            {
                if (m_tile != value)
                {
                    if (m_tile != null)
                    {
                        m_tile.Unit = null;
                    }

                    m_tile = value;

                    if (m_tile != null)
                    {
                        m_tile.Unit = this;
                    }
                }
            }
        }

        public Team Team => GetComponentInParent<Team>();

        public List<Unit> Enemies => AllUnits.FindAll(u => IsEnemy(u));



        #endregion

        private void OnEnable()
        {
            AllUnits.Add(this);
        }

        private void OnDisable()
        {
            Tile = null;
            AllUnits.Remove(this);
        }

        private void Start()
        {
            // get tile
            Battlefield bf = GetComponentInParent<Battlefield>();
            Tile = GraphAlgorithms.GetClosestNode<Battlefield.Tile>(bf, transform.position);
            transform.position = Tile.WorldPosition;
        }

        public override void OnBegin(bool bFirstTime)
        {
            base.OnBegin(bFirstTime);

            if (bFirstTime)
            {
                if (Team.m_bIsPlayerTeam)
                {
                    // TODO: do player unit input
                }
                else
                {
                    // TODO: do AI unit logic
                }
            }
        }

        private void Update()
        {
            if (Tile == null)
            {
                return;
            }

            HexUtils.DrawHex(Tile.m_coord, new Color(1.0f, 1.0f, 0.0f, 0.3f));
            foreach (Link link in Tile.m_links)
            {
                HexUtils.DrawHex((link.Target as Battlefield.Tile).m_coord, new Color(0.0f, 1.0f, 0.0f, 0.3f));
            }
        }

        public void TakeDamage(int iDmg)
        {
            m_iHP -= iDmg;
            if (m_iHP <= 0)
            {
                // TODO: Death
            }
        }

        public bool IsEnemy(Unit unit)
        {
            return unit.Team != Team;
        }
    }*/
}