using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    public GameObject linePrefab;
    private GameObject currentLine;
    private LineRenderer lineRenderer;
    private Vector3 mousePos;
    [SerializeField] private float minDistance = 0.1f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            lineRenderer = currentLine.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            AddPoint(mousePos);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 curPosi = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            curPosi.z = 0f;

            if (Vector3.Distance(curPosi, mousePos) > minDistance)
            {
                AddPoint(curPosi);
            }
        }
    }

    private void AddPoint(Vector3 newPoint)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPoint);
        mousePos = newPoint;
    }
}
