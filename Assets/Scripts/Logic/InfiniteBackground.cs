using UnityEngine;

// 사용법:
// 1. 빈 게임오브젝트에 이 컴포넌트 추가
// 2. Tile Sprite를 비워두면 격자 타일 자동 생성, 또는 직접 스프라이트 연결
// 3. Tile World Size를 스프라이트 1장이 차지할 월드 크기로 설정
public class InfiniteBackground : MonoBehaviour
{
    [SerializeField] Sprite tileSprite;
    [SerializeField] float tileWorldSize = 10f;
    [SerializeField] int sortingOrder = -100;

    [Header("Auto-generated Tile (Tile Sprite가 없을 때)")]
    [SerializeField] int textureSize = 64;
    [SerializeField] Color fillColor = new Color(0.18f, 0.18f, 0.18f);
    [SerializeField] Color borderColor = new Color(0.12f, 0.12f, 0.12f);
    [SerializeField] int borderWidth = 2;

    Transform _cam;
    readonly Transform[,] _tiles = new Transform[3, 3];
    float _lastOx = float.MaxValue;
    float _lastOy = float.MaxValue;

    void Awake()
    {
        _cam = Camera.main.transform;

        if (tileSprite == null)
            tileSprite = GenerateGridSprite();

        float spriteWorldSize = tileSprite.rect.width / tileSprite.pixelsPerUnit;
        float scale = tileWorldSize / spriteWorldSize;

        for (int x = 0; x < 3; x++)
        for (int y = 0; y < 3; y++)
        {
            var go = new GameObject($"BG_{x}_{y}");
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one * scale;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = tileSprite;
            sr.sortingOrder = sortingOrder;

            _tiles[x, y] = go.transform;
        }
    }

    void LateUpdate()
    {
        float ox = Mathf.Floor(_cam.position.x / tileWorldSize) * tileWorldSize;
        float oy = Mathf.Floor(_cam.position.y / tileWorldSize) * tileWorldSize;

        // 그리드 원점이 바뀔 때(타일 경계를 넘을 때)만 재배치
        if (ox == _lastOx && oy == _lastOy) return;
        _lastOx = ox;
        _lastOy = oy;

        for (int x = 0; x < 3; x++)
        for (int y = 0; y < 3; y++)
        {
            _tiles[x, y].position = new Vector3(
                ox + (x - 1) * tileWorldSize,
                oy + (y - 1) * tileWorldSize,
                transform.position.z
            );
        }
    }

    Sprite GenerateGridSprite()
    {
        var tex = new Texture2D(textureSize, textureSize)
        {
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Point
        };

        for (int x = 0; x < textureSize; x++)
        for (int y = 0; y < textureSize; y++)
        {
            bool isBorder = x < borderWidth || x >= textureSize - borderWidth
                         || y < borderWidth || y >= textureSize - borderWidth;
            tex.SetPixel(x, y, isBorder ? borderColor : fillColor);
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f), textureSize);
    }
}
