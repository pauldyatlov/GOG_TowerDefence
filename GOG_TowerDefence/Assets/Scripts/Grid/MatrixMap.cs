using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Grid
{
    [Serializable]
    public class MatrixMapCell : SettlersEngine.IPathNode<object>
    {
        public int X;
        public int Y;

        public bool Occupied;
        public bool CanBuildHere = true;

        public GridElement @Object { get; set; }

        public MatrixMapCell(GridElement @object, int x, int y)
        {
            Object = @object;

            X = x;
            Y = y;
        }

        public bool IsWalkable(object inContext)
        {
            return !Occupied;
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
        [SerializeField] private Vector2 _cellMatrixSize = new Vector2(15, 10);

        [SerializeField] private GridElement _gridElementPrefab;
        [SerializeField] private List<GridElement> _gridElements; 

        public MatrixMapCell[,] MatrixMapCells;

        private readonly MatrixMapCellList _mapCellsAroundTower = new MatrixMapCellList();

        private GameController _gameController;

        private Tower _selectedTower;

        [ContextMenu("Generate grid map")]
        public void GenerateGridMap()
        {
            foreach (var gridElement in _gridElements) {
                DestroyImmediate(gridElement.gameObject);
            }

            _gridElements.Clear();

            for (var i = 0; i < _cellMatrixSize.x; i++)
            {
                for (var j = 0; j < _cellMatrixSize.y; j++)
                {
                    var x = (int)_gridElementPrefab.transform.position.x + i;
                    var y = (int)_gridElementPrefab.transform.position.y + j;

                    var gridElement = Instantiate(_gridElementPrefab, new Vector3(x, y, 0), Quaternion.identity);

                    var cell = new MatrixMapCell(gridElement, x, y);

                    gridElement.Cell = cell;
                    gridElement.gameObject.transform.SetParent(transform);

                    _gridElements.Add(gridElement);
                }
            }
        }

        public void Init(GameController gameController)
        {
            _gameController = gameController;

            MatrixMapCells = new MatrixMapCell[(int)_cellMatrixSize.x, (int)_cellMatrixSize.y];

            foreach (var gridElement in _gridElements)
            {
                var x = gridElement.Cell.X;
                var y = gridElement.Cell.Y;
                
                MatrixMapCells[x, y] = gridElement.Cell;

                gridElement.Init((element, hover) => OnGridElementHover(element, hover, x, y), OnGridElementSelected);
            }
        }

        public void SetSelectedTower(Tower tower)
        {
            _selectedTower = tower;
        }

        private void OnGridElementHover(GridElement gridElement, bool hoverStatus, int x, int y)
        {
            if (_selectedTower == null) return;

            _mapCellsAroundTower.Clear();

            foreach (var element in _selectedTower.Region)
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

                        elementAround.Object.SetAreaActive(!elementAround.Occupied && elementAround.CanBuildHere 
                            ? EGridElementState.Available 
                            : EGridElementState.Unavailable);
                    }
                    else
                    {
                        elementAround.Object.SetAreaActive(elementAround.Occupied && elementAround.CanBuildHere 
                            ? EGridElementState.Occupied 
                            : EGridElementState.Default);
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }

        private void OnGridElementSelected(GridElement gridElement)
        {
            if (gridElement.PlantedTower != null)
            {
                _gameController.PlantedTowerSelected(gridElement.PlantedTower);
                return;
            }

            if (_selectedTower == null)
                return;

            if (_mapCellsAroundTower.Any(element => element.Occupied || !element.CanBuildHere)) {
                return;
            }

            _selectedTower.ParentGridElement = gridElement;
            
            if (_gameController.EnoughMoney(_selectedTower.Price))
            {
                _gameController.RegisterTower(_selectedTower);

                foreach (var element in _mapCellsAroundTower)
                {
                    element.Object.SetAreaActive(EGridElementState.Occupied);
                    element.Object.SetPlantedTower(_selectedTower);
                    element.Occupied = true;
                }

                gridElement.SetPlantedTower(_selectedTower);
                gridElement.PlantedTower.OccupiedCells = _mapCellsAroundTower.Select(x => x.Object);
            }
            else
            {
                ClearSelectedTower();
            }

            _selectedTower = null;

            _mapCellsAroundTower.Clear();
        }

        public void ClearSelectedTower()
        {
            if (_selectedTower == null)
                return;

            foreach (var element in _mapCellsAroundTower)
            {
                element.Object.SetAreaActive(element.Occupied && element.CanBuildHere
                    ? EGridElementState.Occupied
                    : EGridElementState.Default);
            }

            Destroy(_selectedTower.gameObject);
        }
    }
}