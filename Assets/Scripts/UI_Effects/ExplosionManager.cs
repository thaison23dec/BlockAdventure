using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ExplosionManager : MonoBehaviour
{
    public Image piecePrefab;
    public ShapeStorage shapeStorage;
    public int pieceCount = 8;
    public float force = 15;
    public float duration = 5;

    private void Awake()
    {
        DOTween.SetTweensCapacity(2000, 100);
    }

    public void PlayExplosion(Vector3 position, Color color)
    {
        for (int i = 0; i < pieceCount; i++)
        {
            Image piece = Instantiate(piecePrefab, transform);
            piece.transform.position = position;
            piece.sprite = shapeStorage.lastShapeSprite;

            float randomLocalScale = (float) Random.Range(0, 100) / 100f;

            Debug.Log(randomLocalScale);

            piece.transform.localScale *= randomLocalScale;

            RectTransform rect = piece.rectTransform;

            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector2 targetPos = (Vector2)position + randomDir * force;

            Sequence seq = DOTween.Sequence();

            seq.Join(rect.DOMove(targetPos, duration).SetEase(Ease.OutQuad));
            seq.Join(rect.DOScale(0, duration));
            seq.Join(piece.DOFade(0, duration));

            seq.OnComplete(() =>
            {
                if (piece != null)
                    Destroy(piece.gameObject);
            });
        }
    }
}
