using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum NavigationMapTypes
{
    GroundMap,
    FlyingMap,
    NoObstaclesMap
}

[System.Serializable]
public class NavigationMap : Pathfinding.INodeContainer
{
    public Action<List<Vector2Int>> StateChanged;
    public NavigationMapTypes Type { get; private set; }
    private bool NeedUpdateZones;
    private readonly Node[,] Nodes;
    public readonly RectInt Rect;

    [System.Serializable]
    public class Node : Pathfinding.INode
    {
        public Node(int x, int y, NavigationMapTypes type)
        {
            X = x;
            Y = y;
            MapType = type;
            Walkable = true;
        }

        private NavigationMapTypes MapType;
        public FieldTile Tile { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }

        public bool Walkable { get; private set; }
        public bool HasObstacle { get; private set; }

        public bool Hill { get; private set; }
        public bool HillUp { get; private set; }

        public int gCost { get; set; }
        public int hCost { get; set; }
        public int Depth { get; set; }
        public int Distance { get; set; }
        public bool Zonable { get { return Walkable; } }
        public int Zone { get; set; }
        public Pathfinding.INode Parent { get; set; }

        public void SetTile(FieldTile tile)
        {
            Tile = tile;
            //Walkable = Tile == null || (MapType == NavigationMapTypes.GroundMap ? Tile.IsWalkable : Tile.IsFlyable);
            Walkable = Tile == null || IsWalkable(MapType);
            HasObstacle = MapType == NavigationMapTypes.NoObstaclesMap ? false : Tile != null && Tile.IsObstacle;
            Hill = Tile != null && (MapType == NavigationMapTypes.GroundMap ? Tile.IsHill : false);
            HillUp = Tile != null && (MapType == NavigationMapTypes.GroundMap ? Tile.HillUp : false);
        }

        public override string ToString()
        {
            return $"{{X: {X}, Y: {Y}, Tile: {Tile}, Walkable: {Walkable}, Obstacle: {HasObstacle}, Hill: {Hill}, HillUp: {Hill}}}";
        }

        private bool IsWalkable(NavigationMapTypes mapType)
        {
            bool result = true;
            switch (mapType)
            {
                case NavigationMapTypes.GroundMap: result = Tile.IsWalkable; break;
                case NavigationMapTypes.FlyingMap: result = Tile.IsFlyable; break;
                case NavigationMapTypes.NoObstaclesMap: result = true; break;
            }

            return result;
        }
    }

    public NavigationMap(RectInt rect, NavigationMapTypes type)
    {
        Rect = rect;
        Nodes = new Node[rect.width, rect.height];
        Type = type;

        for (var i = 0; i < Nodes.GetLength(0); i++)
        {
            for (var j = 0; j < Nodes.GetLength(1); j++)
            {
                Nodes[i, j] = new Node(i, j, type);
            }
        }

    }

    IEnumerable<Pathfinding.INode> Pathfinding.INodeContainer.GetNodes() => Nodes.Cast<Pathfinding.INode>().ToList();

    public void UpdateCell(int x, int y, FieldTile tile)
    {
        var node = Nodes[x, y];
        var walkable = node.Walkable;
        node.SetTile(tile);
        if (walkable != node.Walkable)
        {
            NeedUpdateZones = true;
        }
        StateChanged?.Invoke(new List<Vector2Int> { new Vector2Int(node.X, node.Y) });
    }

    public void UpdateZones(bool force = false)
    {
        if (force || NeedUpdateZones)
        {
            NeedUpdateZones = false;
            Pathfinding.RefreshZones(this);
        }
    }

    public bool CalculatePath(Vector2 start, Vector2 end, List<Vector2> path, bool excludeStartPoint = true)
    {
        var startInt = end.ToTilePosition() - Rect.position;
        var endInt = start.ToTilePosition() - Rect.position;
        if (!IsValidNode(startInt.x, startInt.y) || !IsValidNode(endInt.x, endInt.y)) { return false; }
        var nodePath = Pathfinding.GetReversedPath(this, startInt, endInt);
        if (nodePath == null) { return false; }
        path.AddRange(nodePath.Select(n => (new Vector2Int(n.X, n.Y) + Rect.position).ToCellCenter()));
        if (excludeStartPoint && path.Count > 0)
        {
            path.RemoveAt(0);
        }
        return true;
    }

    public Node GetCell(Vector2 fieldPosition)
    {
        var tilePosition = fieldPosition.ToTilePosition();
        var x = tilePosition.x - Rect.x;
        var y = tilePosition.y - Rect.y;
        if (IsValidNode(x, y))
        {
            return Nodes[x, y];
        }
        return null;
    }

    public Node GetCell(int x, int y)
    {
        x -= Rect.x;
        y -= Rect.y;
        if (IsValidNode(x, y))
        {
            return Nodes[x, y];
        }
        return null;
    }

    public bool IsValidNode(int x, int y)
    {
        return x >= 0 && x < Nodes.GetLength(0) && y >= 0 && y < Nodes.GetLength(1);
    }

    public bool IsWalkable(Vector2 fieldPosition)
    {
        return IsWalkable(fieldPosition.ToTilePosition());
    }

    public bool IsWalkable(Vector2Int tilePosition)
    {
        return IsWalkable(tilePosition.x, tilePosition.y);
    }

    public bool IsWalkable(int x, int y)
    {
        x -= Rect.x;
        y -= Rect.y;
        if (x >= 0 && x < Nodes.GetLength(0) && y >= 0 && y < Nodes.GetLength(1))
        {
            return Nodes[x, y].Walkable;
        }
        return false;
    }

    public HashSet<Node> GetNodeGroup(Vector2 startPosition, Func<Node, bool> condition)
    {
        var nodes = new HashSet<Node>();
        var startNode = GetCell(startPosition);
        if (startNode != null)
        {
            InternalCalculateNodeGroup(startNode, condition, nodes);
        }
        return nodes;
    }

    void InternalCalculateNodeGroup(Node startNode, Func<Node, bool> condition, HashSet<Node> nodes)
    {
        if (condition(startNode) && nodes.Add(startNode))
        {
            foreach (var n in GetNeighbours(startNode))
            {
                InternalCalculateNodeGroup(n, condition, nodes);
            }
        }
    }

    Pathfinding.INode Pathfinding.INodeContainer.GetNode(int x, int y)
    {
        return Nodes[x, y];
    }

    Pathfinding.INode Pathfinding.INodeContainer.GetNodeSafe(int x, int y)
    {
        if (x < 0 || x >= Nodes.GetLength(0) || y < 0 || y >= Nodes.GetLength(1)) { return null; }
        return Nodes[x, y];
    }

    IEnumerable<Pathfinding.INode> Pathfinding.INodeContainer.GetCells(Func<Pathfinding.INode, bool> condition)
    {
        for (var i = 0; i < Nodes.GetLength(0); i++)
        {
            for (var j = 0; j < Nodes.GetLength(1); j++)
            {
                if (condition(Nodes[i, j])) { yield return Nodes[i, j]; }
            }
        }
    }

    IEnumerable<Pathfinding.INode> Pathfinding.INodeContainer.GetNeighbours(Pathfinding.INode node)
    {
        foreach (var n in GetNeighbours((Node)node))
        {
            yield return n;
        }
    }

    IEnumerable<Node> GetNeighbours(Node node)
    {
        for (var i = -1; i <= 1; i++)
        {
            for (var j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) { continue; }
                var x = node.X + i;
                var y = node.Y + j;
                if (x >= 0 && x < Nodes.GetLength(0) && y >= 0 && y < Nodes.GetLength(1))
                {
                    var neighbour = Nodes[x, y];
                    if (!neighbour.Walkable) { continue; }
                    if (node.Hill == neighbour.Hill)
                    {
                        yield return neighbour;
                    }
                    else if (node.Hill && neighbour.HillUp)
                    {
                        yield return neighbour;
                    }
                    else if (neighbour.Hill && node.HillUp)
                    {
                        yield return neighbour;
                    }

                }
            }
        }
    }

    void Pathfinding.INodeContainer.ClearZones()
    {
        foreach (var n in Nodes)
        {
            n.Zone = 0;
        }
    }

    public Vector2 GetValidPosition(Vector2 target, int neighborhoodSize)
    {
        var targetCell = GetCell(target);
        if (targetCell != null && targetCell.Walkable)
        {
            return target;
        }

        var multipler = 1;
        while (multipler < neighborhoodSize)
        {
            for (var i = -multipler; i <= multipler; i++)
            {
                for (var j = -multipler; j <= multipler; j += 2 * multipler)
                {
                    var offset = new Vector2(i, j);
                    var c = GetCell(target + offset);
                    if (c != null && c.Walkable)
                    {
                        return target + offset;
                    }
                    offset = new Vector2(j, i);
                    c = GetCell(target + offset);
                    if (c != null && c.Walkable)
                    {
                        return target + offset;
                    }
                }
            }
            multipler++;
        }

        return target;
    }
}