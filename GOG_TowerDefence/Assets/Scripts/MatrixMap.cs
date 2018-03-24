using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MatrixMap
{
    public class MatrixMapCell
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
        //[SerializeField] private Vector2 _cellSize = Vector2.one;
        [SerializeField] private Vector2 _cellMatrixSize = new Vector2(25, 25);
        [SerializeField] private Vector2[] _schemeRadius;

        [SerializeField] private GridElement _gridElementPrefab;

        public GameObject TowerEpta;

        private readonly MatrixMapCellList _matrixMapCells = new MatrixMapCellList();
        private readonly MatrixMapCellList _mapCellsAroundTower = new MatrixMapCellList();

        //public GameObject[] RadiusObjects;

        private void Awake()
        {
            //_gridElementPrefab.transform.localScale = new Vector3(_cellSize.x, 1, _cellSize.y);

            for (var i = 0; i < _cellMatrixSize.x; i++)
            {
                for (var j = 0; j < _cellMatrixSize.y; j++)
                {
                    var x = (int) _gridElementPrefab.transform.position.x + i;
                    var y = (int) _gridElementPrefab.transform.position.y + j;

                    var gridElement = Instantiate(_gridElementPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    gridElement.gameObject.transform.SetParent(transform);

                    var cell = new MatrixMapCell(gridElement, x, y);
                    _matrixMapCells.Add(cell);

                    gridElement.Init(cell, 
                        (element, hover) => OnGridElementHover(element, hover, x, y),
                        plant => OnGridElementPlant(gridElement, x, y));
                }
            }

            _schemeRadius = standart_region();
        }

        private void OnGridElementHover(GridElement gridElement, bool status, int x, int y)
        {
            if (TowerEpta == null) return;

            _mapCellsAroundTower.Clear();

            foreach (var element in _schemeRadius)
            {
                var elementAround = _matrixMapCells[x + (int)element.x, y + (int)element.y];

                _mapCellsAroundTower.Add(elementAround);

                if (status)
                {
                    gridElement.SetActiveTower(TowerEpta);

                    TowerEpta.transform.SetParent(gridElement.transform);
                    TowerEpta.transform.localPosition = Vector3.zero;

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
            if (_mapCellsAroundTower.Any(element => !element.FreeCell)) {
                return;
            }

            gridElement.SetActiveTower(TowerEpta);

            foreach (var element in _mapCellsAroundTower)
            {
                element.Object.SetAreaActive(EGridElementState.Default);
                element.FreeCell = false;
            }

            TowerEpta = null;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                RefreshMatrixCursor();
            }
        }

        public void RefreshMatrixCursor()
        {
            //RadiusObjects = new GameObject[_schemeRadius.Length];

            //for (var i = 0; i < _schemeRadius.Length; i++) {
            //    RadiusObjects[i] = Instantiate(_gridElementPrefab, Vector3.zero, Quaternion.identity);
            //}
        }

        //public Vector2 GetPositionMatrix(float x, float y)
        //{
        //    var positionMatrix = new Vector2();

        //    for (var i = 0; i < _matrixMapCells.Count; i++)
        //    {
        //        var point = _matrixMapCells[i];

        //        var xcalc = (point.X - point.SizeX) / 2;
        //        var ycalc = (point.Y - point.SizeY) / 2;

        //        if (point.X + xcalc < x
        //            && point.X + point.SizeX + xcalc > x
        //            && point.Y + ycalc < y
        //            && point.Y + ycalc + point.SizeY > y)
        //        {
        //            positionMatrix.x = point.X;
        //            positionMatrix.y = point.Y;

        //            RenderMatrixToMap(i);
        //        }
        //    }

        //    return positionMatrix;
        //}

        //public void RenderMatrixToMap(int index)
        //{
        //    for (var i = 0; i < _schemeRadius.Length; i++)
        //    {
        //        var sRadius = _schemeRadius[i];
        //        var indexEnd = index;

        //        if (Math.Abs(sRadius.y) > Mathf.Epsilon)
        //        {
        //            var error = Mathf.Abs((int) sRadius.y) * (int) _cellMatrixSize.y;

        //            indexEnd = sRadius.y < 0
        //                ? index + error
        //                : index - error;
        //        }

        //        var xRadius = Mathf.Abs((int)sRadius.x);

        //        if (sRadius.x < 0)
        //        {
        //            indexEnd += xRadius;
        //        }
        //        else if (sRadius.x > 0)
        //        {
        //            indexEnd -= xRadius;
        //        }

        //        //indexEnd = Mathf.Clamp(indexEnd, 0, _matrixMapCells.Count);

        //        if (indexEnd >= 0 && indexEnd < _matrixMapCells.Count && _matrixMapCells[indexEnd].FreeCell)
        //        {
        //            RadiusObjects[i].transform.GetComponent<Renderer>().material = ActiveRegion;

        //            RadiusObjects[i].transform.position = new Vector3(_matrixMapCells[indexEnd].X, _matrixMapCells[indexEnd].Y, 0.1f);
        //        }
        //        else
        //        {
        //            RadiusObjects[i].transform.GetComponent<Renderer>().material = NoActiveRegion;
        //        }
        //    }
        //}
    }
}