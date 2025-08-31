using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public Button tryAgainButton; 
    private string lastDifficulty;

    public int totalMoves;
    private int remainingPairs;
    public Text movesLeftText;

    public AudioClip matchSound;
    private AudioSource audioSource;

    GameObject token;
    public Text clickCountTxt;
    public Button easyBtn;
    public Button mediumBtn;
    public Button hardBtn;
    MainToken tokenUp1 = null;
    MainToken tokenUp2 = null;
    List<int> faceIndexes =
        new List<int>{ 0, 1, 2, 3, 0, 1, 2, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11};
    public static System.Random rnd = new System.Random();
    private int shuffleNum = 0;
    float tokenScale = 4;
    float yStart = 2.5f;
    int numOfTokens = 8;
    float yChange = -5f;
    private int clickCount = 0;

    public GameObject gameFinishPanel;
    public Button backButton;
    private int pairsMatched = 0;

    private int movesLeft;
    private int totalPairs;

    void StartGame()
    {
        pairsMatched = 0;
        clickCount = 0;
        if (clickCountTxt != null) clickCountTxt.text = "Clicks: 0";

        int startTokenCount = numOfTokens;

        totalPairs = startTokenCount / 2;
        movesLeft = totalMoves;
        UpdateMovesText();

        float xPosition = -6.2f;
        float yPosition = yStart;
        int row = 1;
        float ortho = Camera.main.orthographicSize / 2.0f;

        List<MainToken> allTokens = new List<MainToken>();

        for (int i = 1; i < startTokenCount + 1; i++)
        {
            shuffleNum = rnd.Next(0, (numOfTokens));
            var temp = Instantiate(token, new Vector3(xPosition, yPosition, 0), Quaternion.identity);
            MainToken mainToken = temp.GetComponent<MainToken>();
            mainToken.faceIndex = faceIndexes[shuffleNum];
            temp.transform.localScale = new Vector3(ortho / tokenScale, ortho / tokenScale, 0);
            faceIndexes.Remove(faceIndexes[shuffleNum]);
            numOfTokens--;
            xPosition += 4;

            if (i % 4 < 1)
            {
                yPosition += yChange;
                xPosition = -6.2f;
                row++;
            }

            allTokens.Add(mainToken);
        }

        token.SetActive(false);

        StartCoroutine(PreviewTokens(allTokens));
    }


    IEnumerator PreviewTokens(List<MainToken> tokens)
    {
        foreach (MainToken token in tokens)
        {
            token.FlipToFront();
        }

        yield return new WaitForSeconds(3f);

        foreach (MainToken token in tokens)
        {
            token.FlipToBack();
        }
    }



    public void TokenDown(MainToken tempToken)
    {
        if (tokenUp1 == tempToken)
        {
            tokenUp1 = null;
        }
        else if (tokenUp2 == tempToken)
        {
            tokenUp2 = null;
        }
    }

    public bool TokenUp(MainToken tempToken)
    {
        bool flipCard = true;
        if (tokenUp1 == null)
        {
            tokenUp1 = tempToken;
        }
        else if (tokenUp2 == null)
        {
            tokenUp2 = tempToken;
        }
        else
        {
            flipCard = false;
        }

        if (flipCard)
        {
            clickCount++;
            if (clickCountTxt != null)
                clickCountTxt.text = "Clicks: " + clickCount;
        }

        return flipCard;
    }

        // 🔴 ADD: Check if moves are finished and restart on "Try Again"
    public void CheckTokens()
    {
        if (tokenUp1 != null && tokenUp2 != null)
        {
            movesLeft = Mathf.Max(0, movesLeft - 1);
            UpdateMovesText();

            // 🔴 ADD: If no moves left, show Try Again popup and stop the game
            if (movesLeft <= 0)
            {
                StartCoroutine(ShowTryAgainPopup());
                return; // Prevent further checking
            }

            if (tokenUp1.faceIndex == tokenUp2.faceIndex)
            {
                tokenUp1.matched = true;
                tokenUp2.matched = true;

                if (matchSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(matchSound);
                }

                StartCoroutine(MatchedAnimation(tokenUp1));
                StartCoroutine(MatchedAnimation(tokenUp2));

                pairsMatched++;
                if (pairsMatched >= totalPairs)
                {
                    StartCoroutine(ShowGameFinishPopup());
                }

                tokenUp1 = null;
                tokenUp2 = null;
            }
            else
            {
                StartCoroutine(FlipBack(tokenUp1, tokenUp2));
            }
        }
    }

    IEnumerator MatchedAnimation(MainToken token)
    {
        Transform t = token.transform;
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 originalScale = t.localScale;
        Vector3 targetScale = originalScale * 1.5f;

        while (elapsed < duration)
        {
            t.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            t.localScale = Vector3.Lerp(targetScale, Vector3.zero, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(token.gameObject);
    }

    IEnumerator FlipBack(MainToken token1, MainToken token2)
    {
        yield return new WaitForSeconds(0.3f);
        token1.FlipToBack();
        token2.FlipToBack();
        TokenDown(token1);
        TokenDown(token2);
    }


    public void EasySetup()
    {
        lastDifficulty = "Easy";
        HideButtons();
        tokenScale = 4;
        yStart = 2.5f;
        numOfTokens = 8;
        yChange = -5f;
        totalMoves = 10;
        StartGame();
    }

    public void MediumSetup()
    {
        lastDifficulty = "Medium";
        HideButtons();
        tokenScale = 8;
        yStart = 3.4f;
        numOfTokens = 16;
        yChange = -2.2f;
        totalMoves = 18;
        StartGame();
    }

    public void HardSetup()
    {
        lastDifficulty = "Hard";
        HideButtons();
        tokenScale = 12;
        yStart = 3.8f;
        numOfTokens = 24;
        yChange = -1.5f;
        totalMoves = 30;
        StartGame();
    }



    private void HideButtons()
    {
        easyBtn.gameObject.SetActive(false);
        mediumBtn.gameObject.SetActive(false);
        hardBtn.gameObject.SetActive(false);
        GameObject[] startImages = 
            GameObject.FindGameObjectsWithTag("startImage");
        foreach (GameObject item in startImages)
            Destroy(item);
    }

    private void Awake()
    {
        token = GameObject.Find("Token");
        audioSource = GetComponent<AudioSource>();

        if (gameFinishPanel != null)
        {
            CanvasGroup cg = gameFinishPanel.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = gameFinishPanel.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            gameFinishPanel.SetActive(false);
        }

        if (backButton != null)
            backButton.gameObject.SetActive(false);

        if (movesLeftText == null) Debug.LogWarning("movesLeftText is not assigned in Inspector.");
        if (clickCountTxt == null) Debug.LogWarning("clickCountTxt is not assigned in Inspector.");
        if (easyBtn == null || mediumBtn == null || hardBtn == null) Debug.LogWarning("Difficulty buttons missing assignment.");
        if (gameFinishPanel == null) Debug.LogWarning("gameFinishPanel is not assigned in Inspector (needed for popup).");
        if (backButton == null) Debug.LogWarning("backButton is not assigned in Inspector.");
    }

    void OnEnable()
    {
        easyBtn.onClick.AddListener(() => EasySetup());
        mediumBtn.onClick.AddListener(() => MediumSetup());
        hardBtn.onClick.AddListener(() => HardSetup());

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        }
    }

    IEnumerator ShowGameFinishPopup()
    {
        yield return new WaitForSeconds(0.5f);

        if (gameFinishPanel != null)
        {
            gameFinishPanel.SetActive(true);
            CanvasGroup cg = gameFinishPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                float t = 0f;
                float duration = 0.5f;
                while (t < duration)
                {
                    t += Time.deltaTime;
                    cg.alpha = Mathf.Lerp(0f, 1f, t / duration);
                    yield return null;
                }
                cg.alpha = 1f;
            }
        }

        if (backButton != null)
            backButton.gameObject.SetActive(true);
    }

    private void UpdateMovesText()
    {
        if (movesLeftText != null)
            movesLeftText.text = "Moves Left: " + movesLeft;
    }

    IEnumerator ShowTryAgainPopup()
{
    if (gameFinishPanel != null)
    {
        gameFinishPanel.SetActive(true);
        CanvasGroup cg = gameFinishPanel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameFinishPanel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        float t = 0f;
        float duration = 0.5f;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    if (tryAgainButton != null)
    {
        tryAgainButton.gameObject.SetActive(true);
        tryAgainButton.onClick.RemoveAllListeners();
        tryAgainButton.onClick.AddListener(() =>
        {
            // 🔴 Hide panel
            gameFinishPanel.SetActive(false);
            tryAgainButton.gameObject.SetActive(false);

            // 🔴 Remove all existing tokens before restarting
            foreach (MainToken token in FindObjectsOfType<MainToken>())
            {
                Destroy(token.gameObject);
            }

            // 🔴 Reset counters
            pairsMatched = 0;
            clickCount = 0;
            if (clickCountTxt != null) clickCountTxt.text = "Clicks: 0";

            // 🔴 Restart the correct difficulty
            if (lastDifficulty == "Easy") EasySetup();
            else if (lastDifficulty == "Medium") MediumSetup();
            else if (lastDifficulty == "Hard") HardSetup();
        });
    }
}

}
