using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Graphs;

namespace Game
{
    [CustomEditor(typeof(Battlefield))]
    public class BattlefieldEditor : Editor
    {
        private void OnSceneGUI()
        {
            //Battlefield bf = target as Battlefield;
            //Graphs.EditorGraphUtils.DrawGraph(bf);
        }
    }
}
