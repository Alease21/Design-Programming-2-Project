using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WFC
{
    public static class AStarPathfinding
    {
        private static byte[,] currByteMap;

        private static PathMarker startNode;
        private static PathMarker goalNode;
        private static PathMarker lastPos;
        private static bool done = false;

        private static List<PathMarker> open = new List<PathMarker>();
        private static List<PathMarker> closed = new List<PathMarker>();

        private static List<MapLocation> directions = new List<MapLocation>() {
                                                      new MapLocation(1,0),
                                                      new MapLocation(0,1),
                                                      new MapLocation(-1,0),
                                                      new MapLocation(0,-1) };

        public static byte[,] FindTruePathThroughRoom(byte[,] roomByteMap, Vector2Int[] pathPositions)
        {
            currByteMap = roomByteMap;
            Vector2Int[] trimmedPathPosition = RemoveNeighborPathPoints(pathPositions);

            for (int i = trimmedPathPosition.Length - 1; i >= 0; i--)
            {
                for (int j = 0; j <= i - 1; j++)
                {
                    BeginSearch(trimmedPathPosition[i], trimmedPathPosition[j]);
                    do
                    {
                        Search(lastPos);
                    } while (!done);
                    SetTruePath();
                }
            }

            return currByteMap;
        }
        private static Vector2Int[] RemoveNeighborPathPoints(Vector2Int[] pathPositions)
        {
            List<Vector2Int> temp = new List<Vector2Int>(pathPositions);
            bool continueILoop = false;

            for (int i = temp.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    foreach (MapLocation dir in directions)
                    {
                        if (temp[i] + new Vector2Int((int)dir.x, (int)dir.y) == temp[j])
                        {
                            temp.RemoveAt(j);
                            continueILoop = true;
                            break;
                        }
                    }
                    if (continueILoop)
                        break;
                }
            }
            return temp.ToArray();
        }
        private static void BeginSearch(Vector2Int startPos, Vector2Int endPos)
        {
            done = false;

            startNode = new PathMarker(new MapLocation(startPos.x, startPos.y), 0.0f, 0.0f, 0.0f, null);

            goalNode = new PathMarker(new MapLocation(endPos.x, endPos.y), 0.0f, 0.0f, 0.0f, null);

            open.Clear();
            closed.Clear();
            open.Add(startNode);
            lastPos = startNode;  
        }

        private static void Search(PathMarker thisNode) 
        {
            if (thisNode == null) return; 
            if (thisNode.Equals(goalNode)) { done = true; return; } //goal has been found
            
            foreach (MapLocation dir in directions)
            {
                MapLocation neighbour = dir + thisNode.location;
                if (neighbour.x < 0 || neighbour.x > currByteMap.GetLength(0) - 1 || neighbour.y < 0 ||
                    neighbour.y > currByteMap.GetLength(1) - 1) continue; //if neighbor is out of bounds
                if (currByteMap[neighbour.x, neighbour.y] == 1 || currByteMap[neighbour.x, neighbour.y] == 2) continue; 
                if (IsClosed(neighbour)) continue; 

                float newG = Vector2.Distance(thisNode.location.ToVector(), neighbour.ToVector()) + thisNode.G;
                float newH = Vector2.Distance(neighbour.ToVector(), goalNode.location.ToVector());
                float newF = newG + newH;

                if (!UpdateMarker(neighbour, newG, newH, newF, thisNode))
                    open.Add(new PathMarker(neighbour, newG, newH, newF, thisNode));
            }

            open = open.OrderBy(p => p.F).ThenBy(n => n.H).ToList(); //orders by F val, and then by H val
            PathMarker pm = (PathMarker)open.ElementAt(0);
            closed.Add(pm);
            open.RemoveAt(0);
            lastPos = pm;
        }

        private static bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt)
        {
            foreach (PathMarker p in open)
            {
                if (p.location.Equals(pos))
                {
                    p.G = g;
                    p.H = h;
                    p.F = f;
                    p.parent = prt;
                    return true;
                }
            }
            return false;
        }

        private static bool IsClosed(MapLocation marker)
        {
            foreach (PathMarker p in closed)
            {
                if (p.location.Equals(marker)) return true;
            }
            return false;
        }

        private static void SetTruePath()
        {
            PathMarker begin = lastPos; //last post will be goal, then work backwards using parents

            while (begin != null)
            {
                if (currByteMap[begin.location.x, begin.location.y] != 4 && currByteMap[begin.location.x,begin.location.y] != 5)
                    currByteMap[begin.location.x, begin.location.y] = 3;//set byte to 3 to indicate true path
                begin = begin.parent;
            }
        }
    }
    public class PathMarker
    {
        public MapLocation location;
        public float G, H, F;
        public PathMarker parent;

        public PathMarker(MapLocation l, float g, float h, float f, PathMarker p)
        {

            location = l;
            G = g;
            H = h;
            F = f;
            parent = p;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;
            else
                return location.Equals(((PathMarker)obj).location);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
    public class MapLocation
    {
        public int x;
        public int y;

        public MapLocation(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public Vector2 ToVector()
        {
            return new Vector2(x, y);
        }

        public static MapLocation operator +(MapLocation a, MapLocation b)
           => new MapLocation(a.x + b.x, a.y + b.y);

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;
            else
                return x == ((MapLocation)obj).x && y == ((MapLocation)obj).y;
        }
        public override int GetHashCode()
        {
            return 0;
        }
    }
}