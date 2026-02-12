using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public AudioSource hitSFX;
    public AudioSource missSfx;
    public TMPro.TextMeshPro scoreText;
    static int comboScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        comboScore = 0;
    }

    public static void hit() 
    {
        comboScore += 1;
        instance.hitSFX.Play();
    }

    public static void miss() 
    {
        comboScore = 0;
        instance.missSfx.Play();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = comboScore.ToString();
    }
}
