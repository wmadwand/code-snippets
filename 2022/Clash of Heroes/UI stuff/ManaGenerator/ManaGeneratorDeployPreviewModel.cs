using UnityEngine;

public class ManaGeneratorDeployPreviewModel : BehaviourWithBattleSimulator, DeployView.ISetCard
{
    [SerializeField] private UnitConfig DeployedUnit;    
    [SerializeField] private Material TransparencyMaterial;
    [SerializeField] private OverlappingShadedRectAreaView OverlappingShadedRectAreaPrefab;
    private Transform SummonedUnitsContainer;

    private RectInt FieldSize;
    private OverlappingShadedRectAreaView OverlappingShadedRectArea;
    private DeployView DeployViewBase;
    private Unit Caster;
    public Vector2 TargetPoint { get; private set; }

    public class DeployCard : DeployView.ISetCardData
    {
        public UnitConfig UnitToDeploy;
    }

    void DeployView.ISetCard.Set(Card card)
    {
        SetView(card);

        DeployViewBase = GetComponent<DeployView>();
        DeployViewBase.BecameInvisible += BecameInvisibleHandler;

        FieldSize = BattleSimulator.State.Field.LevelConfig.SizeInTiles;
        Caster = card.Config.GetCasterForUI(BattleSimulator);

    }

    private void OnDestroy()
    {
        if (DeployViewBase != null)
            DeployViewBase.BecameInvisible -= BecameInvisibleHandler;

        if (OverlappingShadedRectArea != null)
            Destroy(OverlappingShadedRectArea.gameObject);
    }



    void SetView(Card card)
    {
        SummonedUnitsContainer = transform.GetChild(0);

        if (!SummonedUnitsContainer.gameObject.activeSelf)
        {
            SummonedUnitsContainer.gameObject.SetActive(true);
        }

        if (card.Config.SetCardData is DeployCard deployInfo)
        {
            DeployedUnit = deployInfo.UnitToDeploy;
        }

        var scale = DeployedUnit.GetStat(StatType.ViewScale, modificator: null);
        var unit = Instantiate(DeployedUnit.View, SummonedUnitsContainer);
        var body = unit.GetComponentInChildren<UnitBody>();
        var transparencySetter = body.gameObject.AddComponent<TransparencySetter>();
        transparencySetter.Init(TransparencyMaterial, 0.5f);
        unit.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void BecameInvisibleHandler()
    {
        if (OverlappingShadedRectArea != null)
            Destroy(OverlappingShadedRectArea.gameObject);
    }

    private void Update()
    {
        if (Caster == null)
            return;

        var casterPosition = Caster.Position.ConvertFieldPositionToWorld();
        if (transform.position.z < casterPosition.z)
        {
            var p = new Vector3(transform.position.x, 0, casterPosition.z);
            SummonedUnitsContainer.localPosition = transform.InverseTransformPoint(p);

            var gp = SummonedUnitsContainer.TransformPoint(SummonedUnitsContainer.localPosition);
            TargetPoint = gp.ConvertWorldPositionToField();

            if (OverlappingShadedRectArea == null)
            {
                OverlappingShadedRectArea = Instantiate(OverlappingShadedRectAreaPrefab);
                OverlappingShadedRectArea.Init(new Vector3(0, p.y, p.z), FieldSize.width * 1.5f, FieldSize.height * 1.2f);
                OverlappingShadedRectArea.SetAlpha(0.2f);
            }
        }
        else
        {
            if (OverlappingShadedRectArea != null)
                Destroy(OverlappingShadedRectArea.gameObject);

            SummonedUnitsContainer.localPosition = Vector3.zero;
            TargetPoint = transform.position.ConvertWorldPositionToField();
        }
    }
}
