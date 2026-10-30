using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Math;
using System.Runtime.InteropServices.WindowsRuntime;
using System;

namespace Game
{
   // [CustomEditor(typeof(HexCoordGrid))]
    public class HexCoordExampleEditor : Editor
    {
        Vector3     m_vWorldPosition;
        HexCoord    m_A;
        HexCoord    m_B;

        private void OnEnable()
        {
            m_A = HexCoord.CreateRandom(4);
            m_B = HexCoord.CreateRandom(4);
        }


        private void OnSceneGUI()
        {
            // draw cube axes
            #if true
            Handles.color = Color.red;
            Handles.ArrowHandleCap(0, Vector3.zero, Quaternion.AngleAxis(120.0f, Vector3.up), 0.5f, EventType.Repaint);
            Handles.color = Color.green;
            Handles.ArrowHandleCap(0, Vector3.zero, Quaternion.AngleAxis(0, Vector3.up), 0.5f, EventType.Repaint);
            Handles.color = Color.blue;
            Handles.ArrowHandleCap(0, Vector3.zero, Quaternion.AngleAxis(-120.0f, Vector3.up), 0.5f, EventType.Repaint);
#endif

            /*
            HexCoord A = new HexCoord(3, -1, -2);

            // draw A's neighbors
            foreach (HexCoord na in A.Neighbors)
            {
                DrawHex(na, Color.green);
            }

            // draw A
            DrawHex(A, Color.red);*/

            foreach (HexCoord hc in HexCoord.RectangularGrid(20, 20))
            {
                DrawHex(hc, Color.magenta);
            }

            // move world pos
            m_vWorldPosition = Handles.DoPositionHandle(m_vWorldPosition, Quaternion.identity);
            m_vWorldPosition.y = 0.0f;
            HexCoord wc = HexCoord.GetHexCoordAt(m_vWorldPosition);
            DrawHex(wc, new Color(0.0f, 1.0f, 1.0f, 0.4f));

            /*
            // Distance between coordinates
            DrawHex(m_A, Color.burlywood);
            DrawHex(m_B, Color.chartreuse);
            Handles.color = Color.yellow;
            Handles.DrawLine(m_A.Position, m_B.Position, 3.0f);
            Handles.Label((m_A.Position + m_B.Position) * 0.5f + Vector3.up * 0.5f, HexCoord.GetDistance(m_A, m_B).ToString());
            */
        }

        public void DrawHex(HexCoord c, Color color)
        {
            Handles.color = color;
            Vector3[] corners = c.Corners;
            for (int i = 0; i < 6; ++i)
            {
                Handles.DrawLine(corners[i], corners[(i + 1) % 6], 3.0f);
            }
        }
    }
}