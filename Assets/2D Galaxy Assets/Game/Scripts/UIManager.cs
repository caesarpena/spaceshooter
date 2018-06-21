using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Sprite[] lives;

    [SerializeField]
    private Image Player_Lifes;

    public Text Player_Score;
    public int score;

    public void UPdateScore () {

        score += 1;
        Player_Score.text = "Score: "+score;
    }
	
	public void UpdateLives (int _lives) {

        Player_Lifes.sprite = lives[_lives];
    }
}
