using UnityEngine;

public class ManaGeneratorDeployPreview : BehaviourWithBattleSimulator, DeployView.ISetCard
{ 
    [SerializeField] private OverlappingShadedRectAreaView OverlappingShadedRectAreaPrefab;
    
    private RectInt FieldSize;
    private Transform Image;
    private OverlappingShadedRectAreaView OverlappingShadedRectArea;
    private DeployView DeployViewBase;
    private Unit Caster;
    public Vector2 TargetPoint { get; private set; }

    void DeployView.ISetCard.Set(Card card)
    {
        Image = transform.GetChild(0);
        DeployViewBase = GetComponent<DeployView>();
        DeployViewBase.BecameInvisible += BecameInvisibleHandler;

        FieldSize = BattleSimulator.State.Field.LevelConfig.SizeInTiles;
        Caster = card.Config.GetCasterForUI(BattleSimulator);
        Image.localScale = Vector3.one * (card.GetStat(StatType.ViewScale) * .5f);
    }

    private void OnDestroy()
    {
        if(DeployViewBase != null)
            DeployViewBase.BecameInvisible -= BecameInvisibleHandler;

        if (OverlappingShadedRectArea != null)
            Destroy(OverlappingShadedRectArea.gameObject);
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
            Image.localPosition = transform.InverseTransformPoint(p);

            var gp = Image.TransformPoint(Image.localPosition);
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

            Image.localPosition = Vector3.zero;
            TargetPoint = transform.position.ConvertWorldPositionToField();
        }
    }
}
