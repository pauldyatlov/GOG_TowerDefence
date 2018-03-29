using System.Collections.Generic;
using Assets.Scripts.Enemy;

public class BarrierTower : Tower
{
    public override void Init() { }
    public override void SetParameters(TowerModel parameters, List<TowerModel> upgrades)
    {
        Model = parameters;
    }

    protected override void Update() { }
    public override void UpdateEnemiesList(List<Enemy> enemies) { }
}