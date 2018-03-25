using System;
using UnityEngine;

namespace MatrixMap
{
    public enum EGridElementState
    {
        Default,
        Available,
        Unavailable
    }

    public class GridElement : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _availableColor;
        [SerializeField] private Color _unavailableColor;

        public MatrixMapCell Cell { get; private set; }

        private Color _defaultColor;

        private Action<GridElement, bool> _onHover;
        private Action<GridElement> _onPlant;

        private EGridElementState _currentState;
        private Tower _currentTower;

        public void Init(MatrixMapCell cell, Action<GridElement, bool> onHover, Action<GridElement> onPlant)
        {
            _defaultColor = _spriteRenderer.color;

            Cell = cell;

            _onHover = onHover;
            _onPlant = onPlant;
        }

        public void SetActiveTower(Tower tower)
        {
            _currentTower = tower;
        }

        private void OnMouseOver()
        {
            _onHover(this, true);
        }

        private void OnMouseExit()
        {
            _onHover(this, false);
        }

        private void OnMouseDown()
        {
            _onPlant(this);
        }

        public void SetAreaActive(EGridElementState state)
        {
            if (_currentState == state)
                return;

            _currentState = state;

            switch (state)
            {
                case EGridElementState.Default:
                    _spriteRenderer.color = _defaultColor;
                    break;
                case EGridElementState.Available:
                    _spriteRenderer.color = _availableColor;
                    break;
                case EGridElementState.Unavailable:
                    _spriteRenderer.color = _unavailableColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("state", state, null);
            }
        }
    }
}