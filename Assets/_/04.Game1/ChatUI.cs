using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    private TMP_Text text;
    private float LivingTime;

    private void Start()
    {
        gameObject.SetActive(false);
        text = GetComponentInChildren<TMP_Text>();
    }
    private void Update()
    {
        if(gameObject.activeInHierarchy)
        {
            LivingTime -= Time.deltaTime;

            if(LivingTime < 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
    public void SetTxt(string sentence, float time)
    {
        text.text = sentence;
        LivingTime = time;
    }

}
