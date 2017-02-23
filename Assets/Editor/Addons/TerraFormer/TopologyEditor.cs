using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TerraFormer
{
    [CustomEditor(typeof(Topology))]
    public class TopologyEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Topology top = target as Topology;

            if (GUILayout.Button("Generate"))
            {
                top.Generate();
            }
        }
    }
}