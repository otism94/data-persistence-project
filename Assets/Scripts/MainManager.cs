using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    [SerializeField]
    private GameObject GameOverText;
    [SerializeField]
    private GameObject NameInput;
    [SerializeField]
    private GameObject SubmitButton;
    [SerializeField]
    private Text HighScoreDisplay;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;
    private bool isNewHighScore = false;

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        SaveManager.Instance.LoadHighScore();
        HighScoreDisplay.text = 
            SaveManager.Instance.highScoreName == null ? "No high score recorded" : $"High Score: {SaveManager.Instance.highScoreName} - {SaveManager.Instance.highScorePoints}pts";
    }

    private void Update()
    {
        if (!m_Started && !m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver && !isNewHighScore)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                m_Started = true;
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        if (m_Points > SaveManager.Instance.highScorePoints) 
        {
            isNewHighScore = true;
            GameOverText.GetComponent<Text>().text = "New High Score! \n Please enter your name.";
            NameInput.SetActive(true);
            SubmitButton.SetActive(true);
        }
        else
        {
            GameOverText.GetComponent<Text>().text = "Game Over \n Press space to restart";
        }

        GameOverText.SetActive(true);
    }

    public void SubmitHighScore()
    {
        SaveManager.Instance.highScoreName = NameInput.GetComponent<InputField>().text;
        SaveManager.Instance.highScorePoints = m_Points;
        SaveManager.Instance.SaveHighScore();

        HighScoreDisplay.text = $"High Score: {SaveManager.Instance.highScoreName} - {SaveManager.Instance.highScorePoints}pts";
        NameInput.SetActive(false);
        SubmitButton.SetActive(false);
        isNewHighScore = false;
        m_Points = 0;

        GameOver();
    }

    
}
