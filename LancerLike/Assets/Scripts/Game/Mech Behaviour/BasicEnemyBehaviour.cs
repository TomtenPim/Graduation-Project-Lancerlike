using Actions;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Multiplayer.PlayMode;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BasicEnemyBehaviour : BaseBehaviour
{
    Mech thisMech;
    [SerializeField] int range;
    [SerializeField] int damage;
    int closestDistanceToTarget;
    Mech closestTargetMech;
    List<HexTile> pathToClosestTarget;
    bool firstTime = true;
    bool canAct = true;
    bool hasBoosted = false;
    public void Start()
    {
        thisMech = GetComponent<Mech>();
        
    }

    public override void OnUpdate()
    {
        if (canAct)
        {
            if ((1 << 10 & thisMech.statuses) != 0)
            {
                StartCoroutine(EnableActingAfterTime());
                return;
            }

            canAct = false;
            base.OnUpdate();

            closestDistanceToTarget = 100;
            FindClosestTarget();

            WalkAlongThePath();

            if (ClosestEnemyInRange() && CanSeeClosestTarget())
            {
                AttackClosestTarget();
            }

            StartCoroutine(EnableActingAfterTime());
        }
    }
    private bool ClosestEnemyInRange()
    {
        return closestDistanceToTarget <= range;
    }
    private bool CanSeeClosestTarget()
    {
        return HexCoordGrid.GetInstance().HasLineOfSight(closestTargetMech.OccupiedTiles, thisMech.OccupiedTiles, thisMech.ObjectSize);
    }

    private void WalkAlongThePath()
    {
        UpdateDistanceToClosestTarget();
        if (ClosestEnemyInRange() && CanSeeClosestTarget())
        {
            return;
        }


        if (thisMech.remainingMovement <= 0)
        {
            if (!hasBoosted)
            {
                Boost();
            }
            else
            {
                return;
            }
        }

        thisMech.remainingMovement--;

        thisMech.SetPrimaryTile(pathToClosestTarget.Last());
        pathToClosestTarget.Remove(pathToClosestTarget.Last());

        this.transform.position = thisMech.CurrentPrimaryTile.GetWorldPosition();

        ReassessTarget();
        WalkAlongThePath();
    }
    private void Boost()
    {
        hasBoosted = true;

        if ((1 << 7 & thisMech.statuses) != 0 || (1 << 9 & thisMech.statuses) != 0) // Cant Boost if 7 PRONE or 9 SLOWED  is true.
        {
            return;
        }

            thisMech.remainingActionPoints--;
        thisMech.remainingMovement += thisMech.GetSpeed();

    }
    private void ReassessTarget()
    {
        List<Mech> visibleTargets = GetVisibleTargets();

        if (visibleTargets.Count < 0)
        {
            return;
        }

        FindClosestTarget(visibleTargets);

    }
    private void FindClosestTarget()
    {
        foreach (Mech playerMech in FindPlayerTeam().GetComponentsInChildren<Mech>())
        {
            List<HexTile> currentPath = HexCoordGrid.GetInstance().AStarPathingToNeighbourTile(thisMech.CurrentPrimaryTile, thisMech, playerMech.CurrentPrimaryTile, true);


            int distanceBetweenMechs = currentPath.Count;
            if (distanceBetweenMechs < closestDistanceToTarget)
            {
                closestDistanceToTarget = distanceBetweenMechs;
                closestTargetMech = playerMech;
                pathToClosestTarget = currentPath;
            }
        }
    }

    private void FindClosestTarget(List<Mech> visibleTargets)
    {
        foreach (Mech playerMech in visibleTargets)
        {
            List<HexTile> currentPath = HexCoordGrid.GetInstance().AStarPathingToNeighbourTile(thisMech.CurrentPrimaryTile, thisMech, playerMech.CurrentPrimaryTile, true);


            int distanceBetweenMechs = currentPath.Count;
            if (distanceBetweenMechs < closestDistanceToTarget)
            {
                closestDistanceToTarget = distanceBetweenMechs;
                closestTargetMech = playerMech;
                pathToClosestTarget = currentPath;
            }
        }
    }

    private void UpdateDistanceToClosestTarget()
    {
        closestDistanceToTarget = HexCoordGrid.GetInstance().DistanceBetweenTiles(thisMech.GetClosestOccupiedTile(closestTargetMech.CurrentPrimaryTile), closestTargetMech.GetClosestOccupiedTile(thisMech.CurrentPrimaryTile));
    }

    private List<Mech> GetVisibleTargets()
    {

        List<Mech> visibleTargets = new();

        foreach (Mech playerMech in FindPlayerTeam().GetComponentsInChildren<Mech>())
        {
            if (HexCoordGrid.GetInstance().HasLineOfSight(playerMech.OccupiedTiles, thisMech.OccupiedTiles, thisMech.ObjectSize))
            {
                visibleTargets.Add(playerMech);
            }
        }
        return visibleTargets;
    }


    public void OnEnd()
    {
        if (thisMech.blowUpAtTheEndOfThisTurn)
        {
            thisMech.Meltdown();
        }
        hasBoosted = false;
        firstTime = true;

        thisMech.ClearEndOfTurnStatuses();
    }

    IEnumerator EnableActingAfterTime()
    {
        yield return new WaitForSeconds(0.7f);
        thisMech.remainingActionPoints--;
        canAct = true;

        if (thisMech.remainingActionPoints <= 0)
        {
            OnEnd();
        }
    }
    
    private Team FindPlayerTeam()
    {
        Team playerTeam;

        Team[] teams = thisMech.transform.parent.transform.parent.GetComponentsInChildren<Team>();
        foreach (Team team in teams)
        {
            if (team.m_bIsPlayerTeam)
            {
                playerTeam = team;
                return playerTeam;
            }
        }

        throw new Exception("FindPlayerTeam - Found no player team");
    }
    private void AttackClosestTarget()
    {
        if((1 << 5 & thisMech.statuses) != 0 || (1 << 10 & thisMech.statuses) != 0) // Cant attack if 5 JAMMED or 10 STUNNED is true
        {
            return;
        }

            if (AIHitRollAgainstEvasion(closestTargetMech))
        {
            closestTargetMech.TakeDamage(damage);
            // closestEnemy.TakeHeat(damage);
        }
        else
        {
            DisplayMiss(closestTargetMech);
            Debug.Log("Miss");
        }
    }
    private bool AIHitRollAgainstEvasion(Mech target)
    {
        float totalBonusDice = ((target.statuses) & 1 << 7) >> 7; // +1 accuracy if target has 7 PRONE
        totalBonusDice -= ((thisMech.statuses) & 1 << 3) >> 3; // +1 difficulty if attacker has 3 IMPAIRED

        if (((target.statuses) & 1 << 6) >> 6 == 1) // if target has 6 LOCK ON...
        {
            totalBonusDice++; // +1 accuracy
            target.statuses &= ~(1 << 6); // remove 6 LOCK ON
        }

        float largestBonusRoll = 0;
        for (int i = 0; i < Mathf.Abs(totalBonusDice); i++)
        {
            largestBonusRoll = Mathf.Max(largestBonusRoll, UnityEngine.Random.Range(1, 6));
        }
        if (totalBonusDice != 0)
        {
            largestBonusRoll = largestBonusRoll * totalBonusDice / MathF.Abs(totalBonusDice);
        }

        return UnityEngine.Random.Range(1, 20) + largestBonusRoll >= target.Evasion;
    }
    private void DisplayMiss(Mech target)
    {
        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        TextPopup.Create(canvas.transform, target.transform.position, "Miss");
    }

    /*private void MoveTowardsClosestTarget(Mech closestTarget)
    {
        if (!hasMoved)
        {
            thisMech.remainingActionPoints++;
            hasMoved = true;
        }

        if ((1 << 10 & thisMech.statuses) != 0) // Cant move if 10 STUNNED is true
        {
            return;
        }

        HexTile tileToGoTo = CalculateTileToGoTo(thisMech, closestTarget);

        int distanceToTarget = HexCoordGrid.GetInstance().DistanceBetweenTiles(tileToGoTo, closestTarget.CurrentPrimaryTile);
        if (distanceToTarget > range && thisMech.remainingActionPoints > 0)
        {
            if (!((1 << 7 & thisMech.statuses) != 0 || (1 << 9 & thisMech.statuses) != 0)) // Cant Boost if 7 PRONE or 9 SLOWED  is true.
            {
                Boost();
                tileToGoTo = CalculateTileToGoTo(thisMech, closestTarget);
            }
        }

        thisMech.remainingMovement -= HexCoordGrid.GetInstance().DistanceBetweenTiles(tileToGoTo, thisMech.CurrentPrimaryTile);
        thisMech.SetPrimaryTile(tileToGoTo);
        this.transform.position = thisMech.CurrentPrimaryTile.GetWorldPosition();
    }
    */
    /*private HexTile CalculateTileToGoTo(Mech self, Mech target)
    {

        if (self.remainingMovement > closestDistanceToTarget)
        {
            return changeTileIfOccupied(HexCoordGrid.GetInstance().Lerp(self.CurrentPrimaryTile, target.CurrentPrimaryTile, 1f / closestDistanceToTarget * (closestDistanceToTarget - 1 - targetMoveoffset)), self, target);
        }
        else
        {
            return changeTileIfOccupied(HexCoordGrid.GetInstance().Lerp(self.CurrentPrimaryTile, target.CurrentPrimaryTile, 1f / closestDistanceToTarget * (self.remainingMovement - targetMoveoffset)), self, target);
        }
    }
    */
    /*private HexTile changeTileIfOccupied(HexTile tileToCheck, Mech self, Mech target)
    {
        if (tileToCheck.occupied)
        {
            if (targetMoveoffset > 3)
            {
                return self.CurrentPrimaryTile;
            }

            targetMoveoffset++;
            return CalculateTileToGoTo(self, target);
        }
        else
        {
            return tileToCheck;
        }
    }
    */
}
