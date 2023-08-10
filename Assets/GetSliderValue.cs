using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GetSliderValue : MonoBehaviour
{
    public Slider slider;

    public TextMeshProUGUI textMesh;

    public void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        textMesh.text = textMesh.text.Split(':')[0] + ": " + slider.value.ToString();
    }

    // Update is called once per frame
    private void OnSliderValueChanged(float a)
    {
        //playerCount.text = "player count: " + a.ToString();
        textMesh.text = textMesh.text.Split(':')[0] + ": " + a.ToString();
    }
}
