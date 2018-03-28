using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Grid
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
        private Action<Tower> _onTowerSelected;

        private Tower _selectedTower;

        public void Init(Action<Tower> onTowerPlant, Action<Tower> onTowerSelected)
        {
            _onTowerPlant = onTowerPlant;
            _onTowerSelected = onTowerSelected;

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
                        OnGridElementSelected);
                }
            }

            _schemeRadius = standart_region();
        }

        public void SetSelectedTower(Tower tower)
        {
            _selectedTower = tower;
        }

        private void OnGridElementHover(GridElement gridElement, bool hoverStatus, int x, int y)
        {
            if (_selectedTower == null) return;

            _mapCellsAroundTower.Clear();

            foreach (var element in _schemeRadius)
            {
                var xPos = x + (int) element.x;
                var yPos = y + (int) element.y;

                try
                {
                    var elementAround = MatrixMapCells[xPos, yPos];

                    _mapCellsAroundTower.Add(elementAround);

                    if (hoverStatus)
                    {
                        _selectedTower.transform.position = gridElement.transform.position;

                        elementAround.Object.SetAreaActive(elementAround.FreeCell ? EGridElementState.Available : EGridElementState.Unavailable);
                    }
                    else
                    {
                        elementAround.Object.SetAreaActive(elementAround.FreeCell ? EGridElementState.Default : EGridElementState.Occupied);
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }

        private void OnGridElementSelected(GridElement gridElement)
        {
            if (gridElement.PlantedTower != null)
            {
                _onTowerSelected(gridElement.PlantedTower);
                return;
            }

            if (_selectedTower == null)
                return;

            if (_mapCellsAroundTower.Any(element => !element.FreeCell)) {
                return;
            }

            _selectedTower.ParentGridElement = gridElement;
            _onTowerPlant(_selectedTower);

            foreach (var element in _mapCellsAroundTower)
            {
                element.Object.SetAreaActive(EGridElementState.Occupied);
                element.Object.SetPlantedTower(_selectedTower);
                element.FreeCell = false;
            }

            gridElement.SetPlantedTower(_selectedTower);

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