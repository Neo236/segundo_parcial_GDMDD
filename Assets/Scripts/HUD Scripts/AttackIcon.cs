using UnityEngine;
using UnityEngine.UI;

public class AttackIcon : MonoBehaviour
{
    [SerializeField] private Image _attackIcon;
    [SerializeField] private Image _elementOverlay;

    private void Awake()
    {
        if (_attackIcon == null)
            _attackIcon = GetComponent<Image>();
        if (_elementOverlay == null)
            _elementOverlay = transform.Find("ElementOverlay")?.GetComponent<Image>();
        
        AttackSelector.OnAttackChanged += UpdateAttackIcon;
        InkSelector.OnInkTypeChanged += UpdateElementOverlay;
    }

    private void OnDestroy()
    {
        AttackSelector.OnAttackChanged -= UpdateAttackIcon;
        InkSelector.OnInkTypeChanged -= UpdateElementOverlay;
    }

    private void UpdateAttackIcon(AttackData attackData)
    {
        if (attackData.attackIcon != null)
        {
            _attackIcon.sprite = attackData.attackIcon;
        }
        else
        {
            Debug.LogWarning($"No attack icon sprite assigned for attack: {attackData.attackName}");
        }
    }

    private void UpdateElementOverlay(ElementType elementType)
    {
        if (_elementOverlay == null) return;
        
        _elementOverlay.color = GetColorForElementType(elementType);
    }

    private Color GetColorForElementType(ElementType elementType)
    {
        Color baseColor = elementType switch
        {
            ElementType.Fire => new Color(1f, 0.3f, 0.3f, 0.5f),
            ElementType.Water => new Color(0.3f, 0.7f, 1f, 0.5f),
            ElementType.Electric => new Color(1f, 0.92f, 0.016f, 0.5f),
            _ => new Color(1f, 1f, 1f, 0f)
        };
        return baseColor;
    }
}