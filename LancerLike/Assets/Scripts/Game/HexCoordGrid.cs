using Math;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class HexTile
    {
        public HexCoord hexCoord;
        public bool occupied;
        public BattlefieldObject occupiedBy = null;
        public GameObject renderedTile;

        public void UpdateOccupiedStatus(bool inOccupiedStatus, BattlefieldObject inOccupiedBy)
        {
            occupied = inOccupiedStatus;
            occupiedBy = inOccupiedBy;
        }
        public void UpdateOccupiedStatus(bool inOccupiedStatus)
        {
            occupied = inOccupiedStatus;
            occupiedBy = null;
        }

        public HexTile(HexCoord inHexCoord, bool inOccupied)
        {
            hexCoord = inHexCoord;
            occupied = inOccupied;
        }

        public Vector3 GetWorldPosition()
        {
            return hexCoord.Position;
        }
    }

    class HexCoordGrid : MonoBehaviour
    {
        private static HexCoordGrid _instance;
        public static HexCoordGrid GetInstance()
        {
            return _instance;
        }

        [Header("Grid Settings")]

        public short hexGridHeight;
        public short hexGridWidth;

        [Header("Renderer Settings")]
        public float innerSize = 0.5f;
        public float outerSize = 0.55f;
        public float height = 0;
        public bool isFlatTopped;
        public Material material;

        public List<HexTile> hexTilesGrid = new List<HexTile>();

        void Awake()
        {
            _instance = this;


            foreach (HexCoord coord in HexCoord.RectangularGrid(hexGridWidth, hexGridHeight))
            {
                hexTilesGrid.Add(new HexTile(coord, false));
            }

            foreach (HexTile hexTile in hexTilesGrid)
            {

                hexTile.renderedTile = new GameObject($"Test", typeof(HexRenderer));
                hexTile.renderedTile.transform.position = hexTile.GetWorldPosition();

                HexRenderer hexRenderer = hexTile.renderedTile.GetComponent<HexRenderer>();
                hexRenderer.isFlatTopped = isFlatTopped;
                hexRenderer.innerSize = innerSize;
                hexRenderer.outerSize = outerSize;
                hexRenderer.height = height;
                hexRenderer.SetMaterial(material);
                hexRenderer.DrawMesh();

                hexTile.renderedTile.GetComponent<MeshRenderer>().material = hexRenderer.material;
                hexTile.renderedTile.transform.SetParent(transform, true);
                /*

                hexTile.hexRenderer = new HexRenderer();
                hexTile.hexRenderer.isFlatTopped = isFlatTopped;
                hexTile.hexRenderer.innerSize = innerSize;
                hexTile.hexRenderer.outerSize = outerSize;
                hexTile.hexRenderer.height = height;
                hexTile.hexRenderer.SetMaterial(material);
                hexTile.hexRenderer.DrawMesh();
                */
                //renderedTile.GetComponent<MeshRenderer>().material = hexRenderer.material;
                //renderedTile.transform.SetParent(transform, true);
            }
        }

        public HexTile GetHexTileAt(Vector3 pos)
        {
            //Vector3 clampedPos = new Vector3(Mathf.Clamp(pos.x,0.01f,hexGridWidth - 0.01f) ,pos.y, Mathf.Clamp(pos.z, 0, hexGridHeight * 3 / 4));

            HexCoord coordofTileToFind = HexCoord.GetHexCoordAt(pos);


            foreach (HexTile hexTile in hexTilesGrid)
            {
                if (hexTile.hexCoord.Position == coordofTileToFind.Position)
                {
                    return hexTile;
                }
            }
            //Debug.Log("Nu har något gått snett I GetTileAt(Vector3)");
            return null;
        }
        public HexTile GetHexTileAt(HexCoord pos)
        {
            foreach (HexTile hexTile in hexTilesGrid)
            {
                if (hexTile.hexCoord == pos)
                {
                    return hexTile;
                }
            }
            Debug.Log("Nu har något gått snett I GetTileAt(HexCoord)");
            return null;
        }
        public HexTile GetHexTileAt(int inX, int inY, int inZ)
        {
            foreach (HexTile hexTile in hexTilesGrid)
            {
                if (hexTile.hexCoord.X == inX && hexTile.hexCoord.Y == inY && hexTile.hexCoord.Z == inZ)
                {
                    return hexTile;
                }
            }
            //Debug.Log("Nu har något gått snett I GetTileAt(HexCoord)");
            return null;
        }

        public int DistanceBetweenTiles(HexTile hexTileA, HexTile hexTileB)
        {
            if (hexTileA == null || hexTileB == null)
            {
                return -0;
            }

            HexCoord difference = (hexTileA.hexCoord - hexTileB.hexCoord);
            return ((Mathf.Abs(difference.X) + Mathf.Abs(difference.Y) + Mathf.Abs(difference.Z)) / 2);
        }

        public List<HexTile> GetHexTilesInRange(int range, HexTile inStartPostition)
        {
            List<HexTile> HexTilesInRange = new List<HexTile>();

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    for (int z = -range; z <= range; z++)
                    {
                        if (x + y + z == 0)
                        {
                            //Debug.Log("This should be a tile: "+ GetHexTileAt(new HexCoord(-2, 6, -4)));
                            // HexTilesInRange.Add(GetHexTileAt(new HexCoord(-2, 6, -4)));
                            //Debug.Log("This should also be a tile: " + GetHexTileAt(inStartPostition.hexCoord.X + x, inStartPostition.hexCoord.Y + y, inStartPostition.hexCoord.Z + z));
                            try
                            {
                                HexTilesInRange.Add(GetHexTileAt(inStartPostition.hexCoord.X + x, inStartPostition.hexCoord.Y + y, inStartPostition.hexCoord.Z + z));
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }
                }
            }


            if (HexTilesInRange == null)
            {
                Debug.Log("Nu har något gått snett I GetHexTilesInRange()");
                return HexTilesInRange;
            }

            HexTilesInRange.RemoveAll(x => x == null);

            return HexTilesInRange;
        }
        public List<HexTile> GetHexTilesInRange(int range, HexTile inStartPostition, bool includesOccupiedTiles)
        {
            List<HexTile> HexTilesInRange = new List<HexTile>();

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    for (int z = -range; z <= range; z++)
                    {
                        if (x + y + z == 0)
                        {
                            //try
                            {
                                HexTilesInRange.Add(GetHexTileAt(inStartPostition.hexCoord.X + x, inStartPostition.hexCoord.Y + y, inStartPostition.hexCoord.Z + z));
                            }
                            //catch (Exception e)
                            {
                                //throw new Exception(e.Message); 
                            }
                        }
                    }
                }
            }


            if (HexTilesInRange == null)
            {
                Debug.Log("Nu har något gått snett I GetHexTilesInRange()");
                return HexTilesInRange;
            }
            HexTilesInRange.RemoveAll(x => x == null);
            if (!includesOccupiedTiles)
            {
                HexTilesInRange.RemoveAll(x => x.occupied == true);
            }

            return HexTilesInRange;
        }
        public List<HexTile> GetHexTilesInRange(int range, List<HexTile> inStartPostitions, bool includesOccupiedTiles)
        {
            List<HexTile> HexTilesInRange = new List<HexTile>();

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    for (int z = -range; z <= range; z++)
                    {
                        if (x + y + z == 0)
                        {
                            //try
                            {
                                foreach (HexTile inStartPosition in inStartPostitions)
                                {

                                    HexTilesInRange.Add(GetHexTileAt(inStartPosition.hexCoord.X + x, inStartPosition.hexCoord.Y + y, inStartPosition.hexCoord.Z + z));
                                }
                            }
                            //catch (Exception e)
                            {
                                //throw new Exception(e.Message); 
                            }
                        }
                    }
                }
            }


            if (HexTilesInRange == null)
            {
                Debug.Log("Nu har något gått snett I GetHexTilesInRange()");
                return HexTilesInRange;
            }
            HexTilesInRange.RemoveAll(x => x == null);
            if (!includesOccupiedTiles)
            {
                HexTilesInRange.RemoveAll(x => x.occupied == true);
            }

            return HexTilesInRange;
        }
        public List<HexTile> GetHexTilesInRange(int range, HexTile inStartPostition, bool includesOccupiedTiles, Mech mechToIgnore)
        {
            List<HexTile> HexTilesInRange = new List<HexTile>();

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    for (int z = -range; z <= range; z++)
                    {
                        if (x + y + z == 0)
                        {
                            //try
                            {
                                HexTilesInRange.Add(GetHexTileAt(inStartPostition.hexCoord.X + x, inStartPostition.hexCoord.Y + y, inStartPostition.hexCoord.Z + z));
                            }
                            //catch (Exception e)
                            {
                                //throw new Exception(e.Message); 
                            }
                        }
                    }
                }
            }


            if (HexTilesInRange == null)
            {
                Debug.Log("Nu har något gått snett I GetHexTilesInRange()");
                return HexTilesInRange;
            }
            HexTilesInRange.RemoveAll(x => x == null);
            if (!includesOccupiedTiles)
            {
                HexTilesInRange.RemoveAll(x => x.occupied == true && x.occupiedBy != mechToIgnore);
            }

            return HexTilesInRange;
        }

        public void DrawTileList(List<HexTile> hexTiles, Material material)
        {
            foreach (HexTile hexTile in hexTiles)
            {
                DrawTile(hexTile, material);
            }
        }
        public void DrawTile(HexTile hexTile, Material material)
        {
            HexRenderer hexRenderer = hexTile.renderedTile.GetComponent<HexRenderer>();
            hexRenderer.SetMaterial(material);
            hexRenderer.DrawMesh();
            hexTile.renderedTile.GetComponent<MeshRenderer>().material = hexRenderer.material;
        }

        public void ResetTileList(List<HexTile> hexTiles)
        {
            foreach (HexTile hexTile in hexTiles)
            {
                ResetTile(hexTile);
            }
        }
        public void ResetTile(HexTile hexTile)
        {
            DrawTile(hexTile, material);
        }

        public Vector3 WorldspaceLerp(HexTile A, HexTile B, float T)
        {
            return new Vector3(Mathf.Lerp(A.GetWorldPosition().x, B.GetWorldPosition().x, T),
                                Mathf.Lerp(A.GetWorldPosition().y, B.GetWorldPosition().y, T),
                                Mathf.Lerp(A.GetWorldPosition().z, B.GetWorldPosition().z, T));
        }

        public HexTile Lerp(HexTile A, HexTile B, float T)
        {
            return GetHexTileAt(WorldspaceLerp(A, B, T));
        }

        public List<HexTile> GetHexTilesBetween(HexTile tileA, HexTile tileB, int distanceToTileA)
        {
            float distance = DistanceBetweenTiles(tileA, tileB);
            List<HexTile> tilesBetween = new();

            for (float i = distance; i >= distanceToTileA; i--)
            {
                tilesBetween.Add(Lerp(tileA, tileB, (i / distance)));
            }

            return tilesBetween;
        }

        /*public List<HexTile> AStarPathingToTile(HexTile startPoint, HexTile goal)
        {
            List<HexTile> pathToGo = new();

            PriorityQueue<HexTile> frontier = new PriorityQueue<HexTile>();
            frontier.Enqueue(startPoint, 0);

            Dictionary<HexTile, HexTile> cameFrom = new();
            Dictionary<HexTile, int> costSoFar = new();

            cameFrom[startPoint] = null;
            costSoFar[startPoint] = 0;

            while (frontier.Count > 0)
            {
                HexTile currentTile = frontier.Dequeue();

                if (currentTile == goal)
                {
                    while (cameFrom[currentTile] != null)
                    {
                        pathToGo.Add(currentTile);
                        currentTile = cameFrom[currentTile];
                    }
                    break;
                }

                foreach (HexCoord neighbourCoord in currentTile.hexCoord.Neighbors)
                {
                    HexTile neighbourTile = GetHexTileAt(neighbourCoord);

                    int newCost = costSoFar[currentTile] + 1;

                    if ((!costSoFar.ContainsKey(neighbourTile) || newCost < costSoFar[neighbourTile]) && !neighbourTile.occupied)
                    {
                        costSoFar[neighbourTile] = newCost;
                        int priority = newCost + DistanceBetweenTiles(goal, neighbourTile);
                        frontier.Enqueue(neighbourTile, priority);
                        cameFrom[neighbourTile] = currentTile;
                    }
                }
            }



            return pathToGo;
        }*/
        public List<HexTile> AStarPathingToTile(HexTile startPoint, Mech inMech, HexTile goal, bool ignoreEnemyCollision)
        {
            List<HexTile> pathToGo = new();

            PriorityQueue<HexTile> frontier = new PriorityQueue<HexTile>();
            frontier.Enqueue(startPoint, 0);

            Dictionary<HexTile, HexTile> cameFrom = new();
            Dictionary<HexTile, int> costSoFar = new();

            Material debugColor = new(material);
            debugColor.color = Color.orange;

            cameFrom[startPoint] = null;
            costSoFar[startPoint] = 0;

            while (frontier.Count > 0)
            {
                HexTile currentTile = frontier.Dequeue();


                if (currentTile == goal)
                {
                    while (cameFrom[currentTile] != null)
                    {
                        pathToGo.Add(currentTile);
                        currentTile = cameFrom[currentTile];
                    }
                    break;
                }

                //DrawTile(currentTile, debugColor);
                foreach (HexCoord neighbourCoord in currentTile.hexCoord.Neighbors)
                {
                    HexTile neighbourTile = GetHexTileAt(neighbourCoord);

                    if (neighbourTile != null)
                    {
                        int newCost = costSoFar[currentTile] + 1;

                        if ((!costSoFar.ContainsKey(neighbourTile) || newCost < costSoFar[neighbourTile]) && ((!ignoreEnemyCollision && CanAMechPassThroughHere(neighbourTile, inMech)) || ignoreEnemyCollision && CanAMechPassThroughHere(neighbourTile, inMech, true)))
                        {
                            costSoFar[neighbourTile] = newCost;
                            int priority = newCost + DistanceBetweenTiles(goal, neighbourTile);
                            frontier.Enqueue(neighbourTile, priority);
                            cameFrom[neighbourTile] = currentTile;
                        }
                    }
                }
            }
            return pathToGo;
        }
        public List<HexTile> AStarPathingToNeighbourTile(HexTile startPoint, Mech inMech, HexTile goal, bool ignoreEnemyCollision)
        {
            List<HexTile> pathToGo = new();

            PriorityQueue<HexTile> frontier = new PriorityQueue<HexTile>();
            frontier.Enqueue(startPoint, 0);

            Dictionary<HexTile, HexTile> cameFrom = new();
            Dictionary<HexTile, int> costSoFar = new();

            Material debugColor = new(material);
            debugColor.color = Color.orange;

            cameFrom[startPoint] = null;
            costSoFar[startPoint] = 0;

            while (frontier.Count > 0)
            {
                HexTile currentTile = frontier.Dequeue();


                if (DistanceBetweenTiles(currentTile, goal) <= 1)
                {
                    while (cameFrom[currentTile] != null)
                    {
                        pathToGo.Add(currentTile);
                        currentTile = cameFrom[currentTile];
                    }
                    break;
                }

                //DrawTile(currentTile, debugColor);
                foreach (HexCoord neighbourCoord in currentTile.hexCoord.Neighbors)
                {
                    HexTile neighbourTile = GetHexTileAt(neighbourCoord);

                    if (neighbourTile != null)
                    {
                        int newCost = costSoFar[currentTile] + 1;

                        if ((!costSoFar.ContainsKey(neighbourTile) || newCost < costSoFar[neighbourTile]) && ((!ignoreEnemyCollision && CanAMechPassThroughHere(neighbourTile, inMech)) || ignoreEnemyCollision && CanAMechPassThroughHere(neighbourTile, inMech, true)))
                        {
                            costSoFar[neighbourTile] = newCost;
                            int priority = newCost + DistanceBetweenTiles(goal, neighbourTile);
                            frontier.Enqueue(neighbourTile, priority);
                            cameFrom[neighbourTile] = currentTile;
                        }
                    }
                }
            }
            return pathToGo;
        }

        public List<HexTile> BreadthSearchTilesInRange(HexTile startPoint, int range)
        {
            List<HexTile> tilesInRange = new();
            tilesInRange.Add(startPoint);

            Queue<HexTile>[] frontier = new Queue<HexTile>[range];
            frontier[0].Enqueue(startPoint);

            for (int i = 0; i < range; i++)
            {
                while (frontier[i].Count > 0)
                {
                    HexTile currentTile = frontier[i].Dequeue();

                    foreach (HexCoord neighbourCoord in currentTile.hexCoord.Neighbors)
                    {
                        HexTile neighbourTile = GetHexTileAt(neighbourCoord);
                        if (!neighbourTile.occupied && !tilesInRange.Contains(neighbourTile))
                        {
                            frontier[i + 1].Enqueue(neighbourTile);
                            tilesInRange.Add(neighbourTile);
                        }
                    }
                }
            }
            return tilesInRange;
        }
        public List<HexTile> BreadthSearchTilesInRange(HexTile startPoint, Mech inMech, int range)
        {
            List<HexTile> tilesInRange = new();
            tilesInRange.Add(startPoint);

            Queue<HexTile>[] frontier = new Queue<HexTile>[range + 1];
            for (int i = 0; i < frontier.Length; i++)
            {
                frontier[i] = new Queue<HexTile>();
            }


            frontier[0].Enqueue(startPoint);

            for (int i = 0; i < range; i++)
            {
                while (frontier[i].Count > 0)
                {
                    HexTile currentTile = frontier[i].Dequeue();

                    foreach (HexCoord neighbourCoord in currentTile.hexCoord.Neighbors)
                    {
                        HexTile neighbourTile = GetHexTileAt(neighbourCoord);

                        if (neighbourTile != null)
                        {
                            if ((!neighbourTile.occupied || neighbourTile.occupiedBy.ObjectSize < inMech.ObjectSize || neighbourTile.occupiedBy == inMech) && !tilesInRange.Contains(neighbourTile))
                            {
                                frontier[i + 1].Enqueue(neighbourTile);
                                tilesInRange.Add(neighbourTile);
                            }
                        }
                    }
                }
            }
            return tilesInRange;
        }

        public bool CanAMechPassThroughHere(HexTile startTile, Mech inMech)
        {
            bool fits = true;

            foreach (HexTile tile in GetOccupiedSpaceForSizeXHere(startTile, inMech.ObjectSize))
            {
                if (tile != null)
                {
                    if (tile.occupied && tile.occupiedBy.ObjectSize >= inMech.ObjectSize && tile.occupiedBy != inMech)
                    {
                        fits = false;
                    }
                }
            }

            return fits;
        }
        public bool CanAMechPassThroughHere(HexCoord startCoord, Mech inMech)
        {
            bool fits = true;

            foreach (HexTile tile in GetOccupiedSpaceForSizeXHere(GetHexTileAt(startCoord), inMech.ObjectSize))
            {
                if (tile != null)
                {
                    if (tile.occupied && tile.occupiedBy.ObjectSize >= inMech.ObjectSize && tile.occupiedBy != inMech)
                    {
                        fits = false;
                    }
                }
            }

            return fits;
        }
        public bool CanAMechPassThroughHere(HexTile startTile, Mech inMech, bool ignoresMechs)
        {
            bool fits = true;

            foreach (HexTile tile in GetOccupiedSpaceForSizeXHere(startTile, inMech.ObjectSize))
            {
                if (tile != null)
                {
                    if (tile.occupied && 
                        tile.occupiedBy.ObjectSize >= inMech.ObjectSize &&
                        ((!ignoresMechs && tile.occupiedBy != inMech) 
                        || 
                        (ignoresMechs && tile.occupiedBy.GetType() != typeof(Mech))))
                    {
                        fits = false;
                    }
                }
            }

            return fits;
        }

        public bool CanAMechStayHere(HexTile startTile, Mech inMech)
        {
            bool fits = true;

            foreach (HexTile tile in GetOccupiedSpaceForSizeXHere(startTile, inMech.ObjectSize))
            {
                if (tile != null)
                {
                    if (tile.occupied && tile.occupiedBy != inMech)
                    {
                        fits = false;
                    }
                }
            }

            return fits;
        }
        public bool CanAMechStayHere(HexCoord startCoord, Mech inMech)
        {
            bool fits = true;

            foreach (HexTile tile in GetOccupiedSpaceForSizeXHere(GetHexTileAt(startCoord), inMech.ObjectSize))
            {
                if (tile != null)
                {
                    if (tile.occupied && tile.occupiedBy != inMech)
                    {
                        fits = false;
                    }
                }
            }

            return fits;
        }


        public List<HexTile> RemoveTilesOutsideOfLineOfSight(List<HexTile> listToModify, BattlefieldObject observer, bool caresAboutSize)
        {
            if (caresAboutSize)
            {
                return RemoveTilesOutsideOfLineOfSightWithSize(listToModify, observer);
            }
            else
            {
                return RemoveTilesOutsideOfLineOfSightNoSize(listToModify, observer);
            }
        }
        private List<HexTile> RemoveTilesOutsideOfLineOfSightNoSize(List<HexTile> listToModify, BattlefieldObject observer)
        {
            List<HexTile> modifiedTileList = new(listToModify);

            foreach (HexTile tile in listToModify)
            {
                if (!HasLineOfSight(tile, observer.OccupiedTiles))
                {
                    modifiedTileList.Remove(tile);
                }
            }
            return modifiedTileList;
        }
        private List<HexTile> RemoveTilesOutsideOfLineOfSightWithSize(List<HexTile> listToModify, BattlefieldObject observer)
        {
            List<HexTile> modifiedTileList = new(listToModify);

            foreach (HexTile tile in listToModify)
            {
                if (!HasLineOfSight(tile, observer.OccupiedTiles, observer.ObjectSize))
                {
                    modifiedTileList.Remove(tile);
                }
            }
            return modifiedTileList;
        }


        public bool HasLineOfSight(HexTile startTile, List<HexTile> observerTiles)
        {
            foreach (HexTile observerTile in observerTiles)
            {
                float distance = DistanceBetweenTiles(startTile, observerTile);

                bool hadSight = true;
                for (float i = distance; i >= 1; i--)
                {
                    HexTile currentTile = Lerp(startTile, observerTile, (i / distance));
                    if (currentTile.occupied && currentTile.occupiedBy.GetType() != typeof(Mech))
                    {
                        hadSight = false;
                        break;
                    }
                }
                if (hadSight)
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasLineOfSight(HexTile startTile, List<HexTile> observerTiles, BattlefieldObject.Size observerSize)
        {
            BattlefieldObject.Size sizeOfObjectOnStartTile = BattlefieldObject.Size.Half;
            if (startTile.occupied)
            {
                sizeOfObjectOnStartTile = startTile.occupiedBy.ObjectSize;
            }

            foreach (HexTile observerTile in observerTiles)
            {
                float distance = DistanceBetweenTiles(startTile, observerTile);

                bool hadSight = true;
                for (float i = distance; i >= 1; i--)
                {
                    HexTile currentTile = Lerp(startTile,observerTile, (i / distance));
                    if (currentTile.occupied && currentTile.occupiedBy.GetType() != typeof(Mech))
                    {
                        if (currentTile.occupiedBy.ObjectSize >= observerSize && currentTile.occupiedBy.ObjectSize >= sizeOfObjectOnStartTile)
                        {
                            hadSight = false;
                            break;
                        }
                    }
                }
                if (hadSight)
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasLineOfSight(List<HexTile> targetTiles, List<HexTile> observerTiles, BattlefieldObject.Size observerSize)
        {
            foreach (HexTile targetTile in targetTiles) 
            {
                if(HasLineOfSight(targetTile, observerTiles, observerSize)){
                    return true;
                }
            }
            return false;
        }

        public List<HexTile> GetOccupiedSpaceForSizeXHere(HexTile startTile, BattlefieldObject.Size mechSize)
        {
            List<HexTile> occupiedTiles = new();



            switch ((int)mechSize)
            {
                case 0:
                case 1:
                    occupiedTiles.Add(startTile);
                    break;

                case 2:
                    occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(startTile.hexCoord.GetNeighbor(HexCoord.Directions.NorthWest)));
                    occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(startTile.hexCoord.GetNeighbor(HexCoord.Directions.NorthEast)));
                    occupiedTiles.Add(startTile);
                    break;

                case 3:
                    foreach (HexCoord hexCoord in startTile.hexCoord.Neighbors)
                    {
                        occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(hexCoord));
                    }
                    occupiedTiles.Add(startTile);
                    break;

                case 4:
                    foreach (HexCoord hexCoord in startTile.hexCoord.Neighbors)
                    {
                        occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(hexCoord));
                    }
                    occupiedTiles.Add(startTile);

                    occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(startTile.hexCoord + new HexCoord(1, 1, -2)));

                    occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(startTile.hexCoord + new HexCoord(2, 0, -2)));
                    occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(startTile.hexCoord + new HexCoord(-1, 2, -1)));
                    occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(startTile.hexCoord + new HexCoord(-2, 0, 2)));

                    occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(startTile.hexCoord + new HexCoord(-2, 1, 1)));
                    break;

                case 5:
                    foreach (HexTile tile in HexCoordGrid.GetInstance().GetHexTilesInRange(2, startTile))
                    {
                        occupiedTiles.Add(tile);
                    }
                    break;

            }


            return occupiedTiles;
        }
    }
}
