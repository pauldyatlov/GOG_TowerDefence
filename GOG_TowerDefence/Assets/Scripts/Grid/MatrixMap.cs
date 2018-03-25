using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MatrixMap
{
    public class MatrixMapCell : SettlersEngine.IPathNode<object>
    {
        public int X;
        public int Y;

        public GridElement @Object;
        public bool FreeCell = true;

        public MatrixMapCell(GridElement @object, int x, int y)
        {
            Object = @object;

            X = x;
            Y = y;
        }

        public bool IsWalkable(object inContext)
        {
            return FreeCell;
        }
    }

    public class MatrixMapCellList : List<MatrixMapCell>
    {
        public MatrixMapCell this[int x, int y]
        {
            get { return this.FirstOrDefault(arg => arg.X == x && arg.Y == y); }
        }
    }

    public class MatrixMap : MonoBehaviour
    {
        [SerializeField] private Vector2 _cellMatrixSize = new Vector2(25, 25);
        [SerializeField] private Vector2[] _schemeRadius;

        [SerializeField] private GridElement _gridElementPrefab;

        public MatrixMapCell[,] MatrixMapCells;

        private readonly MatrixMapCellList _mapCellsAroundTower = new MatrixMapCellList();
        private Action<Tower> _onTowerPlant;

        private Tower _selectedTower;

        public void Init(Action<Tower> onTowerPlant)
        {
            _onTowerPlant = onTowerPlant;

            MatrixMapCells = new MatrixMapCell[(int)_cellMatrixSize.x, (int)_cellMatrixSize.y];

            //_gridElementPrefab.transform.localScale = new Vector3(_cellSize.X, 1, _cellSize.Y);

            for (var i = 0; i < _cellMatrixSize.x; i++)
            {
                for (var j = 0; j < _cellMatrixSize.y; j++)
                {
                    var x = (int) _gridElementPrefab.transform.position.x + i;
                    var y = (int) _gridElementPrefab.transform.position.y + j;

                    var gridElement = Instantiate(_gridElementPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    gridElement.gameObject.transform.SetParent(transform);

                    var cell = new MatrixMapCell(gridElement, x, y);

                    MatrixMapCells[x, y] = cell;

                    gridElement.Init(cell, 
                        (element, hover) => OnGridElementHover(element, hover, x, y),
                        plant => OnGridElementPlant(gridElement, x, y));
                }
            }

            _schemeRadius = standart_region();
        }

        public void SetSelectedTower(Tower tower)
        {
            _selectedTower = tower;
        }

        private void OnGridElementHover(GridElement gridElement, bool status, int x, int y)
        {
            if (_selectedTower == null) return;

            _mapCellsAroundTower.Clear();

            foreach (var element in _schemeRadius)
            {
                var elementAround = MatrixMapCells[x + (int)element.x, y + (int)element.y];

                _mapCellsAroundTower.Add(elementAround);

                if (status)
                {
                    gridElement.SetActiveTower(_selectedTower);

                    //_selectedTower.transform.SetParent(gridElement.transform);
                    _selectedTower.transform.position = gridElement.transform.position;

                    var availableStatus = elementAround.FreeCell
                        ? EGridElementState.Available
                        : EGridElementState.Unavailable;

                    gridElement.SetAreaActive(availableStatus);
                    elementAround.Object.SetAreaActive(availableStatus);
                }
                else
                {
                    gridElement.SetAreaActive(EGridElementState.Default);
                    elementAround.Object.SetAreaActive(EGridElementState.Default);
                }
            }
        }

        private void OnGridElementPlant(GridElement gridElement, int x, int y)
        {
            if (_selectedTower == null)
                return;

            if (_mapCellsAroundTower.Any(element => !element.FreeCell)) {
                return;
            }

            _onTowerPlant.Invoke(_selectedTower);
            gridElement.SetActiveTower(_selectedTower);

            foreach (var element in _mapCellsAroundTower)
            {
                element.Object.SetAreaActive(EGridElementState.Default);
                element.FreeCell = false;
            }

            _selectedTower = null;

            _mapCellsAroundTower.Clear();
        }

        public Vector2[] standart_region()
        {
            var returnMatrix = new Vector2[9];

            // X X X
            // X 0 X
            // X X X

            returnMatrix[0] = new Vector2(-1, -1);
            returnMatrix[1] = new Vector2(-1, 0);
            returnMatrix[2] = new Vector2(-1, 1);

            returnMatrix[3] = new Vector2(0, -1);
            returnMatrix[4] = new Vector2(0, 1);


            returnMatrix[5] = new Vector2(1, -1);
            returnMatrix[6] = new Vector2(1, 0);
            returnMatrix[7] = new Vector2(1, 1);

            returnMatrix[8] = new Vector2(0, 0);

            return returnMatrix;
        }
    }
}