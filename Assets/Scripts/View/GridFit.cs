using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridFit : MonoBehaviour
{
    [Header("Grid")]
    public int rows = 2;
    public int cols = 4;
    [Tooltip("width/height de la carta (200/300 = 0.6667)")]
    public float aspect = 200f / 300f;

    [Header("Layout")]
    public int padding = 32;           // margen interno del área jugable
    public Vector2 spacing = new(16, 24); // separación fija X/Y

    GridLayoutGroup grid;
    RectTransform rt;

    void Awake() { grid = GetComponent<GridLayoutGroup>(); rt = (RectTransform)transform; Apply(); }
    void OnRectTransformDimensionsChange() => Apply();

    public void SetGrid(int r, int c) { rows = Mathf.Max(1, r); cols = Mathf.Max(1, c); Apply(); }

    void Apply()
    {
        if (!grid || !rt || rows <= 0 || cols <= 0) return;

        grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        grid.constraintCount = rows;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.padding = new RectOffset(padding, padding, padding, padding);
        grid.spacing = spacing;

        // Área disponible
        float availW = rt.rect.width - grid.padding.left - grid.padding.right - (cols - 1) * grid.spacing.x;
        float availH = rt.rect.height - grid.padding.top - grid.padding.bottom - (rows - 1) * grid.spacing.y;

        if (availW <= 0 || availH <= 0) return;

        // Límite por ancho y por alto (respetando aspecto)
        float maxWPerCol = availW / cols;
        float maxHPerRow = availH / rows;

        // Convertir límites a ALTURA permitida por cada eje
        float hFromWidth = maxWPerCol / aspect; // si ajusto por ancho
        float hFromHeight = maxHPerRow;          // si ajusto por alto

        float cellH = Mathf.Floor(Mathf.Min(hFromWidth, hFromHeight));
        float cellW = Mathf.Floor(cellH * aspect);

        grid.cellSize = new Vector2(cellW, cellH);
    }
}
