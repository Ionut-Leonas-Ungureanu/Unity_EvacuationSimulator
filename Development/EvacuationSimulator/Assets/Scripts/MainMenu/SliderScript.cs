using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    public Slider Slider;
    public TextMeshProUGUI Text;

    // Start is called before the first frame update
    void Start()
    {
        Text.text = Slider.value.ToString("0.00");

        Slider.onValueChanged.AddListener((value) =>
        {
            Text.text = value.ToString("0.00"); ;
        });
    }
}
