using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorController : MonoBehaviour
{
    [SerializeField] GameObject btnPrefab;
    [SerializeField] List<Color> colors;
    // Start is called before the first frame update
    void Start()
    {
        foreach(var cc in colors)
        {
            GameObject instance = Instantiate(btnPrefab,transform);
            instance.GetComponentInChildren<Image>().color = cc;
            instance.GetComponentInChildren<Button>().onClick.AddListener
                (() => UVDrawingTest.curColor = cc);
        }
    }
}
