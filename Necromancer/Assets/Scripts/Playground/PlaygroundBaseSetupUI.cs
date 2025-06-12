using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaygroundBaseSetupUI : MonoBehaviour
{
    private PlaygroundManager playgroundManager;

    [SerializeField] private Button CloseBtn;
    [SerializeField] private Button ReturnBtn;

    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject UIPanel;

    [SerializeField] private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Tween currentTween;

    private Vector3 originalScale;

    protected virtual void Start()
    {
        playgroundManager = GetComponentInParent<PlaygroundManager>();
        CloseBtn.onClick.AddListener(() => ClosePanel(0));
        ReturnBtn.onClick.AddListener(ReturnToPlayground);

        // 获取组件
        canvasGroup = UIPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = UIPanel.AddComponent<CanvasGroup>();

        rectTransform = UIPanel.GetComponent<RectTransform>();

        originalScale = Vector3.one;

        // 初始化状态
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.zero;
        UICanvas.SetActive(false);

    }

    public void ReturnToPlayground()
    {
        ClosePanel(1);
    }

    public virtual void OpenPanel()
    {
        UICanvas.SetActive(true);

        UIPanel.SetActive(true);
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.zero;
        rectTransform.rotation = Quaternion.identity;

        // 停止旧动画
        currentTween?.Kill();

        // 打开动画：淡入 + 缩放
        currentTween = DOTween.Sequence()
            .Join(canvasGroup.DOFade(1f, 0.3f))
            .Join(rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack))
            .SetUpdate(true);
    }

    public void ClosePanel(int i)
    {
        currentTween?.Kill();

        playgroundManager.OpenPlayground();
        currentTween = DOTween.Sequence()
            .Join(canvasGroup.DOFade(0.2f, .5f))
            .Join(rectTransform.DOScale(Vector3.zero, .3f).SetEase(Ease.InBack))
            .SetUpdate(true)
            .OnComplete(() =>
            {
                UIPanel.SetActive(false);
                rectTransform.localScale = originalScale;
                canvasGroup.alpha = 1f; // 以防下次打开时透明
                if(i == 1)
                {
                    UICanvas.SetActive(false);
                }
            });
    }

   




}
