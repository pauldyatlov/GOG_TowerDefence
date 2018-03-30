using System;
using System.Collections;
using Assets.Scripts.Grid;
using SettlersEngine;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class GridPosition
    {
        public float X;
        public float Y;

        public GridPosition() { }

        public GridPosition(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    public class EnemyPathfinding : MonoBehaviour
    {
        private MatrixMap _matrixMap;
        private Enemy _enemy;
        private float _moveSpeed;

        private MatrixMapCell _nextNode;
        private Action _onEnemyPassed;

        private GridPosition _currentGridPosition = new GridPosition(0, 0);
        private GridPosition _startGridPosition = new GridPosition();
        private GridPosition _endGridPosition = new GridPosition();

        private Vector2 _input;
        private bool _isMoving = true;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private float _time;
        private float _factor;
        private EPathFormula _formula;

        public class MySolver<TPathNode, TUserContext> : SpatialAStar<TPathNode, TUserContext> where TPathNode : IPathNode<TUserContext>
        {
            protected override double Heuristic(PathNode inStart, PathNode inEnd, EPathFormula formula)
            {
                var dx = Math.Abs(inStart.X - inEnd.X);
                var dy = Math.Abs(inStart.Y - inEnd.Y);

                switch (formula)
                {
                    case EPathFormula.Eucledian:
                        return Math.Sqrt(dx * dx + dy * dy);
                    case EPathFormula.EucledianSquared:
                        return dx * dx + dy * dy;
                    case EPathFormula.Diagonal:
                        return Math.Min(dx, dy);
                    case EPathFormula.Manhatten:
                        return dx * dy + dx + dy;
                }

                return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
            }

            protected override double NeighborDistance(PathNode inStart, PathNode inEnd, EPathFormula formula)
            {
                return Heuristic(inStart, inEnd, formula);
            }

            public MySolver(TPathNode[,] inGrid) : base(inGrid)
            {
            }
        }

        public void Init(MatrixMap matrixMap, Enemy enemy, Action onEnemyPassed, EPathFormula formula, float moveSpeed, float x, float y, int targetX, int targetY)
        {
            _matrixMap = matrixMap;
            _enemy = enemy;
            _onEnemyPassed = onEnemyPassed;
            _formula = formula;
            _moveSpeed = moveSpeed;

            _startGridPosition = new GridPosition(x, y);
            _endGridPosition = new GridPosition(targetX, targetY);

            InitializePosition();

            UpdatePath();
        }

        public void InitializePosition()
        {
            gameObject.transform.position = new Vector2(_startGridPosition.X, _startGridPosition.Y);

            _currentGridPosition = new GridPosition(_startGridPosition.X, _startGridPosition.Y);

            _isMoving = false;
        }

        public void FindUpdatedPath(float currentX, float currentY)
        {
            if (_matrixMap.MatrixMapCells == null)
            {
                Debug.LogError("_matrixMap.MatrixMapCells == null");
                return;
            }

            var aStar = new MySolver<MatrixMapCell, object>(_matrixMap.MatrixMapCells);
            var path = aStar.Search(new Vector2(currentX, currentY), new Vector2(_endGridPosition.X, _endGridPosition.Y), null, _formula, _enemy.Flying);

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
            }
            else
            {
                Debug.LogError("I'm stuck!");

                _matrixMap.RemoveLastTower();
            }
        }

        private void Update()
        {
            if (!_isMoving)
            {
                StartCoroutine(Co_Move());
            }

            //todo: enemy passed on stuck
            if (Vector3.Distance(transform.position, new Vector2(_endGridPosition.X, _endGridPosition.Y)) <= .1f)
            {
                _onEnemyPassed();
            }
        }

        public IEnumerator Co_Move()
        {
            _isMoving = true;
            _startPosition = transform.position;
            _time = 0;
            _factor = UnityEngine.Random.Range(.5f, 2f);

            _endPosition = new Vector2(_startPosition.x + Math.Sign(_input.x), _startPosition.y + Math.Sign(_input.y));

            _currentGridPosition.X += Math.Sign(_input.x);
            _currentGridPosition.Y += Math.Sign(_input.y);

            while (_time < 1f)
            {
                _time += Time.deltaTime*_moveSpeed*_factor;
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

            if (_nextNode == null) {
                return;
            }

            if (_nextNode.X > _currentGridPosition.X)
            {
                _input.x = 1;
                //this.GetComponent<SpriteRenderer>().sprite = Game.carFront;
            }
            if (_nextNode.Y > _currentGridPosition.Y)
            {
                _input.y = 1;
            }
            if (_nextNode.Y < _currentGridPosition.Y)
            {
                _input.y = -1;
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