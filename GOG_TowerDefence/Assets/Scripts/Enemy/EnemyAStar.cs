using System;
using System.Collections;
using MatrixMap;
using SettlersEngine;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class GridPosition
    {
        public int X;
        public int Y;

        public GridPosition() { }

        public GridPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class EnemyAStar : MonoBehaviour
    {
        [SerializeField] private MatrixMap.MatrixMap _matrixMap;
        [SerializeField] private float _moveSpeed;

        private MatrixMapCell _nextNode;

        private GridPosition _currentGridPosition = new GridPosition(0, 0);
        private GridPosition _startGridPosition = new GridPosition();
        private GridPosition _endGridPosition = new GridPosition();
    
        private Vector2 _input;
        private bool _isMoving = true;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private float _time;
        private float _factor;

        public class MySolver<TPathNode, TUserContext> : SpatialAStar<TPathNode, TUserContext> where TPathNode : IPathNode<TUserContext>
        {
            protected override double Heuristic(PathNode inStart, PathNode inEnd)
            {
                var formula = 2;
                var dx = Math.Abs(inStart.X - inEnd.X);
                var dy = Math.Abs(inStart.Y - inEnd.Y);

                switch (formula)
                {
                    case 0: return Math.Sqrt(dx * dx + dy * dy); //Euclidean distance
                    case 1: return (dx * dx + dy * dy); //Euclidean distance squared
                    case 2: return Math.Min(dx, dy); //Diagonal distance
                    case 3: return (dx * dy) + (dx + dy); //Manhatten distance
                }

                return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
            }

            protected override double NeighborDistance(PathNode inStart, PathNode inEnd)
            {
                return Heuristic(inStart, inEnd);
            }

            public MySolver(TPathNode[,] inGrid) : base(inGrid) { }
        }

        private void Awake()
        {
            _startGridPosition = new GridPosition(0, 0);
            _endGridPosition = new GridPosition(10, 10);

            InitializePosition();

            UpdatePath();
        }

        public void InitializePosition()
        {
            gameObject.transform.position = new Vector2(_startGridPosition.X, _startGridPosition.Y);

            _currentGridPosition = new GridPosition(_startGridPosition.X, _startGridPosition.Y);

            _isMoving = false;
        }

        public void FindUpdatedPath(int currentX, int currentY)
        {
            var aStar = new MySolver<MatrixMapCell, object>(_matrixMap.MatrixMapCells);
            var path = aStar.Search(new Vector2(currentX, currentY), new Vector2(_endGridPosition.X, _endGridPosition.Y), null);

            var x = 0;

            if (path != null)
            {
                foreach (var node in path)
                {
                    if (x == 1)
                    {
                        _nextNode = node;
                        break;
                    }

                    x++;

                }

                //foreach(GameObject g in GameObject.FindGameObjectsWithTag("GridBox"))
                //{
                //	if(g.GetComponent<Renderer>().material.color != Color.red && g.GetComponent<Renderer>().material.color == _myColor)
                //		g.GetComponent<Renderer>().material.color = Color.white;
                //}


                //foreach (MyPathNode node in path)
                //{
                //	GameObject.Find(node.X + "," + node.Y).GetComponent<Renderer>().material.color = _myColor;
                //}
            }
        }

        private void Update()
        {
            if (!_isMoving)
            {
                StartCoroutine(Co_Move());
            }
        }
    
        public IEnumerator Co_Move()
        {
            _isMoving = true;
            _startPosition = transform.position;
            _time = 0;
            _factor = 1f;

            _endPosition = new Vector2(_startPosition.x + Math.Sign(_input.x),
                                       _startPosition.y + Math.Sign(_input.y));

            _currentGridPosition.X += Math.Sign(_input.x);
            _currentGridPosition.Y += Math.Sign(_input.y);

            while (_time < 1f)
            {
                _time += Time.deltaTime * _moveSpeed * _factor;
                transform.position = Vector2.Lerp(_startPosition, _endPosition, _time);
                yield return null;
            }
            
            _isMoving = false;
            GetNextMovement();

            yield return 0;
        }

        private void UpdatePath()
        {
            FindUpdatedPath(_currentGridPosition.X, _currentGridPosition.Y);
        }

        private void GetNextMovement()
        {
            UpdatePath();
            
            _input.x = 0;
            _input.y = 0;

            if (_nextNode.X > _currentGridPosition.X)
            {
                _input.x = 1;
                //this.GetComponent<SpriteRenderer>().sprite = Game.carFront;
            }
            if (_nextNode.Y > _currentGridPosition.Y)
            {
                _input.y = 1;
                //this.GetComponent<SpriteRenderer>().sprite = Game.carUp;
            }
            if (_nextNode.Y < _currentGridPosition.Y)
            {
                _input.y = -1;
                //this.GetComponent<SpriteRenderer>().sprite = Game.carDown;
            }
            if (_nextNode.X < _currentGridPosition.X)
            {
                _input.x = -1;
                //this.GetComponent<SpriteRenderer>().sprite = Game.carBack;
            }

            StartCoroutine(Co_Move());
        }
    }
}