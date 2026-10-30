using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Math
{
    [System.Serializable]
    public struct HexCoord
    {
        public enum Directions
        {
            East,
            SouthEast,
            SouthWest,
            West,
            NorthWest,
            NorthEast
        }

        [SerializeField]
        private short x, y, z;

        static Vector3[] sm_corners;

        private static readonly float RADIUS = 0.5f / Mathf.Cos(30.0f * Mathf.Deg2Rad);
        private static readonly float SQRT_THREE = Mathf.Sqrt(3.0f);
        private const float THREE_BY_TWO = 3.0f / 2.0f;
        private const float TWO_BY_THREE = 2.0f / 3.0f;

        public static readonly HexCoord[] NeighborDirections = new HexCoord[]
        {
            new HexCoord(1, 0, -1),     // East
            new HexCoord(1, -1, 0),     // SouthEast
            new HexCoord(0, -1, 1),     // SouthWest
            new HexCoord(-1, 0, 1),     // West
            new HexCoord(-1, 1, 0),     // NorthWest
            new HexCoord(0, 1, -1),     // NorthEast
        };

        #region Properties

        public short X => x;
        public short Y => y;
        public short Z => z;

        public Vector3 Position
        {
            get
            {
                return new Vector3(RADIUS * SQRT_THREE * (x + y * 0.5f),
                                   0.0f,
                                   RADIUS * THREE_BY_TWO * y);
            }
        }

        public Vector3[] Corners
        {
            get
            {
                Vector3 vPosition = Position;
                return System.Array.ConvertAll(sm_corners, c => c + vPosition);
            }
        }

        public IEnumerable<HexCoord> Neighbors
        {
            get
            {
                foreach (HexCoord neighbor in NeighborDirections)
                {
                    yield return this + neighbor;
                }
            }
        }

        public HexCoord GetNeighbor(Directions direction)
        {
            switch (direction)
            {
                case Directions.East:
                    return this + new HexCoord(1, 0, -1);
                case Directions.SouthEast:
                    return this + new HexCoord(1, -1, 0);
                case Directions.SouthWest:
                    return this + new HexCoord(0, -1, 1);
                case Directions.West:
                    return this + new HexCoord(-1, 0, 1);
                case Directions.NorthWest:
                    return this + new HexCoord(-1, 1, 0);
                case Directions.NorthEast:
                    return this + new HexCoord(0, 1, -1);
            }
            Debug.Log("Error in HexCoord - GetNeighbor");
            return this + new HexCoord(0, 0, 0);
        }


        #endregion

        static HexCoord()
        {
            sm_corners = new Vector3[6];
            for (int i = 0; i < 6; ++i)
            {
                float fAngle = (i / 6.0f) * Mathf.PI * 2.0f + Mathf.PI * 0.5f;
                sm_corners[i] = new Vector3(Mathf.Cos(fAngle), 0.0f, Mathf.Sin(fAngle)) * RADIUS;
            }
        }

        public HexCoord(int _x, int _y, int _z)
        {
            if (_x < short.MinValue || _x > short.MaxValue ||
                _y < short.MinValue || _y > short.MaxValue ||
                _z < short.MinValue || _z > short.MaxValue)
            {
                throw new System.ArgumentOutOfRangeException("HexCoord: Only short values supported");
            }

            x = (short)_x;
            y = (short)_y;
            z = (short)_z;

            // check that they add up to zero
            if (x + y + z != 0)
            {
                throw new System.Exception("HexCoord: coordinates do not add up to zero");
            }
        }

        public static HexCoord operator +(HexCoord A, HexCoord B)
        {
            return new HexCoord(A.x + B.x, A.y + B.y, A.z + B.z);
        }

        public static HexCoord operator -(HexCoord A, HexCoord B)
        {
            return new HexCoord(A.x - B.x, A.y - B.y, A.z - B.z);
        }

        public static HexCoord operator *(HexCoord A, int B)
        {
            return new HexCoord(A.x * B, A.y * B, A.z * B);
        }

        public static bool operator ==(HexCoord A, HexCoord B)
        {
            return A.x == B.x && A.y == B.y && A.z == B.z;
        }

        public static bool operator !=(HexCoord A, HexCoord B)
        {
            return A.x != B.x || A.y != B.y || A.z != B.z;
        }

        public override bool Equals(object obj)
        {
            if (obj is HexCoord other)
            {
                return this == other;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hashCode = y.GetHashCode();
            int hashCode2 = z.GetHashCode();
            return x.GetHashCode() ^ (hashCode << 4) ^ (hashCode >> 28) ^ (hashCode2 >> 4) ^ (hashCode2 << 28);
        }

        public static IEnumerable<HexCoord> RectangularGrid(int iWidth, int iHeight)
        {
            for (int iRow = 0; iRow < iHeight; ++iRow)
            {
                for (int iCol = 0; iCol < iWidth; ++iCol)
                {
                    int x = iCol - iRow / 2;
                    int y = iRow;
                    int z = -(x + y);
                    yield return new HexCoord(x, y, z);
                }
            }
        }

        public static HexCoord GetHexCoordAt(Vector3 vWorldPos)
        {

            // from world to cube coordinates
            float fX = ((SQRT_THREE / 3.0f) * vWorldPos.x - vWorldPos.z / 3.0f) / RADIUS;
            float fY = (vWorldPos.z * TWO_BY_THREE) / RADIUS;
            float fZ = -(fX + fY);

            // round to integers
            int x = Mathf.RoundToInt(fX);
            int y = Mathf.RoundToInt(fY);
            int z = Mathf.RoundToInt(fZ);

            // get differences between original value and rounded values
            float fDiffX = Mathf.Abs(fX - x);
            float fDiffY = Mathf.Abs(fY - y);
            float fDiffZ = Mathf.Abs(fZ - z);

            // fix the largest error!
            if (fDiffX > fDiffY && fDiffX > fDiffZ)
            {
                x = -(y + z);
            }
            else if (fDiffY > fDiffZ)
            {
                y = -(x + z);
            }
            else
            {
                z = -(x + y);
            }

            // create hex coord
            return new HexCoord(x, y, z);
        }

        public static HexCoord CreateRandom(int iRadius)
        {
            int x = Random.Range(-iRadius, iRadius);
            int y = Random.Range(-iRadius, iRadius);
            int z = -(x + y);
            return new HexCoord(x, y, z);
        }

        public static int GetDistance(HexCoord A, HexCoord B)
        {
            return (Mathf.Abs(A.x - B.x) + Mathf.Abs(A.y - B.y) + Mathf.Abs(A.z - B.z)) / 2;
        }

        public override string ToString()
        {
            return "HexCoord: " + x + ", " + y + ", " + z;
        }
    }
}
