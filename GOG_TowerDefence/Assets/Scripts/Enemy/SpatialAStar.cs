/*
The MIT License

Copyright (c) 2010 Christoph Husse

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SettlersEngine
{
    public interface IPathNode<in TUserContext>
    {
        bool IsWalkable(TUserContext inContext);
    }

    public interface IIndexedObject
    {
        int Index { get; set; }
    }

    /// <summary>
    /// Uses about 50 MB for a 1024x1024 Grid.
    /// </summary>
    public class SpatialAStar<TPathNode, TUserContext> where TPathNode : IPathNode<TUserContext>
    {
        private readonly OpenCloseMap _closedSet;
        private readonly OpenCloseMap _openSet;
        private readonly PriorityQueue<PathNode> _orderedOpenSet;
        private readonly PathNode[,] _cameFrom;
        private readonly OpenCloseMap _runtimeGrid;
        private readonly PathNode[,] _searchSpace;

        public TPathNode[,] SearchSpace { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        protected class PathNode : IPathNode<TUserContext>, IComparer<PathNode>, IIndexedObject
        {
            public TPathNode UserContext { get; internal set; }
            public double G { get; internal set; }
            public double H { get; internal set; }
            public double F { get; internal set; }
            public int Index { get; set; }

            public bool IsWalkable(TUserContext inContext)
            {
                return UserContext.IsWalkable(inContext);
            }

            public int X { get; internal set; }
            public int Y { get; internal set; }

            public int Compare(PathNode x, PathNode y)
            {
                return x.F < y.F ? -1 : (x.F > y.F ? 1 : 0);
            }

            public PathNode(int inX, int inY, TPathNode inUserContext)
            {
                X = inX;
                Y = inY;

                UserContext = inUserContext;
            }
        }

        public SpatialAStar(TPathNode[,] inGrid)
        {
            SearchSpace = inGrid;
            Width = inGrid.GetLength(0);
            Height = inGrid.GetLength(1);

            _searchSpace = new PathNode[Width, Height];
            _closedSet = new OpenCloseMap(Width, Height);
            _openSet = new OpenCloseMap(Width, Height);
            _cameFrom = new PathNode[Width, Height];
            _runtimeGrid = new OpenCloseMap(Width, Height);
            _orderedOpenSet = new PriorityQueue<PathNode>(new PathNode(0, 0, default(TPathNode)));

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    if (inGrid[x, y] == null)
                        throw new ArgumentNullException();

                    _searchSpace[x, y] = new PathNode(x, y, inGrid[x, y]);
                }
            }
        }

        protected virtual double Heuristic(PathNode inStart, PathNode inEnd)
        {
            return Math.Sqrt((inStart.X - inEnd.X) * (inStart.X - inEnd.X) + (inStart.Y - inEnd.Y) * (inStart.Y - inEnd.Y));
        }

        protected virtual double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            var diffX = Math.Abs(inStart.X - inEnd.X);
            var diffY = Math.Abs(inStart.Y - inEnd.Y);

            switch (diffX + diffY)
            {
                case 1: return 1;
                case 2: return Math.Sqrt(2);
                case 0: return 0;
                default:
                    throw new ApplicationException();
            }
        }

        /// <summary>
        /// Returns null, if no path is found. Start- and End-node are included in returned path. The user context
        /// is passed to IsWalkable().
        /// </summary>
        public LinkedList<TPathNode> Search(Vector2 inStartNode, Vector2 inEndNode, TUserContext inUserContext)
        {
            var startNode = _searchSpace[(int)inStartNode.x, (int)inStartNode.y];
            var endNode = _searchSpace[(int)inEndNode.x, (int)inEndNode.y];

            if (startNode == endNode)
                return new LinkedList<TPathNode>(new[] { startNode.UserContext });

            var neighborNodes = new PathNode[4];

            _closedSet.Clear();
            _openSet.Clear();
            _runtimeGrid.Clear();
            _orderedOpenSet.Clear();

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    _cameFrom[x, y] = null;
                }
            }

            startNode.G = 0;
            startNode.H = Heuristic(startNode, endNode);
            startNode.F = startNode.H;

            _openSet.Add(startNode);
            _orderedOpenSet.Push(startNode);

            _runtimeGrid.Add(startNode);

            while (!_openSet.IsEmpty)
            {
                var x = _orderedOpenSet.Pop();

                if (x == endNode)
                {
                    var result = ReconstructPath(_cameFrom, _cameFrom[endNode.X, endNode.Y]);
                    result.AddLast(endNode.UserContext);

                    return result;
                }

                _openSet.Remove(x);
                _closedSet.Add(x);

                StoreNeighborNodes(x, neighborNodes);

                foreach (var y in neighborNodes)
                {
                    bool tentativeIsBetter;

                    if (y == null)
                        continue;

                    if (!y.UserContext.IsWalkable(inUserContext))
                        continue;

                    if (_closedSet.Contains(y))
                        continue;

                    var tentativeGScore = _runtimeGrid[x].G + NeighborDistance(x, y);
                    var wasAdded = false;

                    if (!_openSet.Contains(y))
                    {
                        _openSet.Add(y);
                        tentativeIsBetter = true;
                        wasAdded = true;
                    }
                    else if (tentativeGScore < _runtimeGrid[y].G)
                    {
                        tentativeIsBetter = true;
                    }
                    else
                    {
                        tentativeIsBetter = false;
                    }

                    if (tentativeIsBetter)
                    {
                        _cameFrom[y.X, y.Y] = x;

                        if (!_runtimeGrid.Contains(y))
                            _runtimeGrid.Add(y);

                        _runtimeGrid[y].G = tentativeGScore;
                        _runtimeGrid[y].H = Heuristic(y, endNode);
                        _runtimeGrid[y].F = _runtimeGrid[y].G + _runtimeGrid[y].H;

                        if (wasAdded)
                            _orderedOpenSet.Push(y);
                        else
                            _orderedOpenSet.Update(y);
                    }
                }
            }

            return null;
        }

        private static LinkedList<TPathNode> ReconstructPath(PathNode[,] cameFrom, PathNode currentNode)
        {
            var result = new LinkedList<TPathNode>();

            ReconstructPathRecursive(cameFrom, currentNode, result);

            return result;
        }

        private static void ReconstructPathRecursive(PathNode[,] cameFrom, PathNode currentNode, LinkedList<TPathNode> result)
        {
            var item = cameFrom[currentNode.X, currentNode.Y];

            if (item != null)
            {
                ReconstructPathRecursive(cameFrom, item, result);

                result.AddLast(currentNode.UserContext);
            }
            else
                result.AddLast(currentNode.UserContext);
        }

        private void StoreNeighborNodes(PathNode inAround, IList<PathNode> inNeighbors)
        {
            var x = inAround.X;
            var y = inAround.Y;

            if (x > 0)
                inNeighbors[0] = _searchSpace[x - 1, y];
            else
                inNeighbors[0] = null;

            if (x < Width - 1)
                inNeighbors[1] = _searchSpace[x + 1, y];
            else
                inNeighbors[1] = null;

            if (y > 0)
                inNeighbors[2] = _searchSpace[x, y - 1];
            else
                inNeighbors[2] = null;

            if (y < Height - 1)
                inNeighbors[3] = _searchSpace[x, y + 1];
            else
                inNeighbors[3] = null;
        }

        private class OpenCloseMap
        {
            private PathNode[,] _map;
            private int Width { get; set; }
            private int Height { get; set; }
            private int Count { get; set; }

            public PathNode this[int x, int y]
            {
                get { return _map[x, y]; }
            }

            public PathNode this[PathNode node]
            {
                get { return _map[node.X, node.Y]; }
            }

            public bool IsEmpty
            {
                get { return Count == 0; }
            }

            public OpenCloseMap(int inWidth, int inHeight)
            {
                _map = new PathNode[inWidth, inHeight];
                Width = inWidth;
                Height = inHeight;
            }

            public void Add(PathNode inValue)
            {
                var item = _map[inValue.X, inValue.Y];

#if DEBUG
                if (item != null)
                    throw new ApplicationException();
#endif

                Count++;
                _map[inValue.X, inValue.Y] = inValue;
            }

            public bool Contains(PathNode inValue)
            {
                var item = _map[inValue.X, inValue.Y];

                if (item == null)
                    return false;

#if DEBUG
                if (!inValue.Equals(item))
                    throw new ApplicationException();
#endif

                return true;
            }

            public void Remove(PathNode inValue)
            {
                var item = _map[inValue.X, inValue.Y];

#if DEBUG
                if (!inValue.Equals(item))
                    throw new ApplicationException();
#endif

                Count--;
                _map[inValue.X, inValue.Y] = null;
            }

            public void Clear()
            {
                Count = 0;

                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        _map[x, y] = null;
                    }
                }
            }
        }
    }
}