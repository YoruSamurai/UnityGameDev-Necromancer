using UnityEngine;
using DG.Tweening;

public class BloodEffect : MonoBehaviour
{
    private Material mat;
    [SerializeField] private float duration = 10f;

    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        mat = sr.material;
        // 使用 DOTween 动画渐变 _DripAmount 和 _ColorLerp，从 0 到 11
        Sequence seq = DOTween.Sequence();
        seq.Append(mat.DOFloat(.6f, "_DripAmount", duration));
        //seq.Join(mat.DOFloat(1f, "_ColorLerp", duration));
        seq.OnComplete(() => Destroy(gameObject));
    }
}