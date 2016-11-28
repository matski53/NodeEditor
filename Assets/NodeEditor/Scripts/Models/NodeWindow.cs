﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "this2", menuName = "Custom/This2", order = 1)]
public class NodeWindow : ScriptableObject
{
    //Descritptive Attributes
    public Rect nodeRect;
    public string nodeTitle = "";
    public string nodeName = "";
    public NodeWindowSizes nodeSize;
    public bool isSelected = false;

    //Standard Sizings
    public static float minWidth { get { return 200f; } }
    public static float minHeight { get { return 260f; } }

    //NodeConnections
    [SerializeField]
    public List<NodeConnection> nodeConnections = new List<NodeConnection>();
    public bool hasoutputNodes { get { if (nodeConnections != null) return nodeConnections.Count > 0; else return false; } }

    public static NodeWindow CreateInstance( Rect nodeRect )
    {
        var instance = CreateInstance<NodeWindow>();
        //instance.nodeConnections = new List<NodeConnection>();
        instance.nodeRect = nodeRect;
        return instance;
    }

    public virtual void DrawNode()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("NodeName:", GUILayout.Width(70f));
            nodeName = GUILayout.TextField(nodeName);
        }
        GUILayout.EndHorizontal();

        GUI.DragWindow();
    }

    public virtual void DrawCurves()
    {
        if (hasoutputNodes)
            for (int i = 0; i < nodeConnections.Count; i++)
                nodeConnections[i].DrawConnection();      
    }

    public virtual bool ClickedOn(Vector2 clickPos)
    {
        if (nodeRect.Contains(clickPos))
            return true;
        else
            return false;
    }

    //Connections
    public virtual void SetConnection(NodeWindow node, NodeConnectionType connectionType)
    {
        NodeConnection connection = new NodeConnection(this, node, connectionType);

        if (MoreConnectionsOfTypeAllowed(connectionType))
            nodeConnections.Add(connection);
        else
            Debug.Log("Cannot create any more connections of this type");
    }

    //Checks whether the number of connections of this type is greater than the number that have alreay been created
    public bool MoreConnectionsOfTypeAllowed(NodeConnectionType connectionType)
    {
        int numConnectionTypeAlreadyCreated = 0;

        for (int i = 0; i < nodeConnections.Count; i++)
            if (nodeConnections[i].connectionType.connectionName.Equals(connectionType.connectionName))
                numConnectionTypeAlreadyCreated++;

        return numConnectionTypeAlreadyCreated < connectionType.maxNumConnections ? true : false;
    }

    //Can't delete from a list whilst iterating through it, hence the overly complicated method
    public virtual void DeleteConnectionsToNode(NodeWindow nodeToDelete)
    {
        List<int> indexesToRemove = new List<int>();

        for (int i = 0; i < nodeConnections.Count; i++)
            if (nodeConnections[i].inputNode.Equals(nodeToDelete))
                indexesToRemove.Add(i);

        int num = indexesToRemove.Count;

        for (int i = num-1; i >= 0; i--)
            nodeConnections.RemoveAt(indexesToRemove[i]);
    }

    public virtual void ClearConnections()
    {
        nodeConnections.Clear();
    }

    public virtual void SelectWindow()
    {
        isSelected = true;
        nodeTitle = "-------Selected-------";
    }

    public virtual void DeselectWindow()
    {
        isSelected = false;
        nodeTitle = "";
    }
    
    public virtual string GetTitle()
    {
        return nodeTitle;
    }
}
