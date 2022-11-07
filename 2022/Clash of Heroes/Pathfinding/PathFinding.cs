using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Pathfinding
{
    public interface INode
    {
        int X { get; }
        int Y { get; }

        bool Zonable { get; }
        
        FieldTile Tile { get; }

        int gCost { get; set; }
        int hCost { get; set; }

		int Depth { get; set; }
        int Distance { get; set; }

		int Zone { get; set; }

        INode Parent { get; set; }
    }

    public interface INodeContainer
    {
		INode GetNode(int x, int y);
        INode GetNodeSafe(int x, int y);
        IEnumerable<INode> GetNodes();
		IEnumerable<INode> GetCells(Func<INode, bool> condition);
        IEnumerable<INode> GetNeighbours(INode node);
		void ClearZones();
    }

    public static void RefreshZones(INodeContainer map)
    {
        map.ClearZones();
        var walkable = new HashSet<INode>();
        foreach (var c in map.GetCells(c => c.Zonable))
        {
            walkable.Add(c);
        }
        var zoneIndex = 0;
        while (walkable.Count > 0)
        {
            var start = walkable.First();
            var zone = WalkableZone(map, start);
            foreach (var a in zone)
            {
                a.Zone = zoneIndex;
                walkable.Remove(a);
            }
            zoneIndex++;
        }
        foreach (var c in map.GetCells(c => c.Zone == -1 && c.Zonable))
        {
            foreach (var n in map.GetNeighbours(c))
            {
                if (n.Zone != -1)
                {
                    c.Zone = n.Zone;
                    break;
                }
            }
        }
    }

    static IEnumerable<INode> WalkableZone(INodeContainer map, INode start)
    {
        var openSet = new HashSet<INode>() { start };
        var closedSet = new HashSet<INode>();
        while (openSet.Count > 0)
        {
            var currentNode = openSet.First();
            foreach (var n in map.GetNeighbours(currentNode))
            {
                if (!closedSet.Contains(n))
                {
                    openSet.Add(n);
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
        }
        return closedSet;
    }

	public static void RefreshDistanceField(INodeContainer map, Vector2Int center, float maxLength)
	{
		INode startNode = map.GetNode(center.x, center.y);
		var depth = (int)(1.5 * maxLength);
		var minX = startNode.X - depth;
		var maxX = startNode.X + depth;
		var minY = startNode.Y - depth;
		var maxY = startNode.Y + depth;
		for (var i = minX; i <= maxX; i++) {
			for (var j = minY; j <= maxY; j++)
            {
				var n = map.GetNode(i,j);
				if(n != null) {
					n.Depth = 0;
					n.Distance = int.MaxValue;
				}
            }	
		}
		startNode.Distance = 0;
		DoCalcultePathField(map, startNode, startNode, (int)(10*maxLength), depth);
	}
    
	static void DoCalcultePathField(INodeContainer map, INode startNode, INode currentNode, int maxDistance, int maxDepth)
	{
		var depth = currentNode.Depth + 1;
        if (depth > maxDepth)
        {
            return;
        }      
		var distance = GetDistance(startNode, currentNode);
		if(distance > maxDistance) {
			return;
		}      
		foreach(var n in map.GetNeighbours(currentNode)) {
			var d = currentNode.Distance + GetDistance(currentNode, n);
			if(n.Distance >  d || (n.Distance == d &&  n.Depth > depth)) {
				n.Distance = d;
				n.Depth = depth;
				DoCalcultePathField(map, startNode, n, maxDistance, maxDepth);
			}        
		}
	}

    public static IEnumerable<INode> GetReversedPath(INodeContainer map, Vector2Int startPos, Vector2Int targetPos)
    {
        INode startNode = map.GetNode(startPos.x, startPos.y);
        INode targetNode = map.GetNode(targetPos.x, targetPos.y);
		if(startNode == targetNode) {
			return new List<INode>() { targetNode };
		}

        if (startNode.Zone != targetNode.Zone) { return null; }


        startNode.gCost = 0;
        startNode.hCost = 0;

        var openSet = new List<INode>();
        var closedSet = new HashSet<INode>(new NodeComparer());
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            
            var currentNode = openSet[0];
            var fCost = currentNode.gCost + currentNode.hCost;
            for (int i = 1; i < openSet.Count; i++)
            {
                var it_fCost = openSet[i].gCost + openSet[i].hCost;
                if (it_fCost < fCost || (it_fCost == fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                    fCost = it_fCost;
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (var neighbour in map.GetNeighbours(currentNode))
            {
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return null;
    }

    static IEnumerable<INode> RetracePath(INode startNode, INode endNode)
    {
        var currentNode = endNode;
        while (currentNode != startNode)
        {
            yield return currentNode;
            currentNode = currentNode.Parent;
        }
        yield return startNode;
    }

	static int GetDistance(INode nodeA, INode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.X - nodeB.X);
        int dstY = Mathf.Abs(nodeA.Y - nodeB.Y);

        if (dstX >= dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
	
    class NodeComparer : IEqualityComparer<INode>
    {
        public bool Equals(INode x, INode y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(INode obj)
        {
            return obj.X * 10000 + obj.Y;
        }
    }
}

