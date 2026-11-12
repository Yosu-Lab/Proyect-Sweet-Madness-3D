using UnityEngine;
using UnityEngine.UI;

public class LectordeUISliders : MonoBehaviour
{
    public Slider miSlider;
    public Text textSlider;
    public void OnSliderChanged()
    {
        textSlider.text = (miSlider.value * 100).ToString("F0") + "%";

    }
}
