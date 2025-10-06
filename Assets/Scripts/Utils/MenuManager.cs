using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
    public Image transitionImage;
    public Image background;
    public Sprite normalBackground;
    public Sprite reversedBackground;
    private Coroutine _currentCoroutine = null;

    private Coroutine _currentBackgroundCoroutine;
    private float _secondsToWait;

    public void LoadGame()
    {
        if (_currentCoroutine == null) _currentCoroutine = StartCoroutine(AnimateTransition());
    }

    public void DeactivateScreen(GameObject screen)
    {
        screen.SetActive(false);
    }

    public void ActivateScreen(GameObject screen)
    {
        screen.SetActive(true);
    }

    private IEnumerator AnimateTransition()
    {
        transitionImage.transform.SetAsLastSibling();
        Color newColor = new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 1f);
        StartCoroutine(FadeTransition(transitionImage.color, newColor, 0.5f));

        yield return new WaitForSeconds(0.5f);
        PlayerPrefs.SetString("pastScene", "Menu");
        PlayerPrefs.SetString("currentState", "Start");
        SceneManager.LoadScene("Cutscene");
    }

    private IEnumerator FadeTransition(Color oldColor, Color newColor, float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float lerpAmount = Mathf.Clamp01(elapsedTime / time);
            transitionImage.color = Color.Lerp(oldColor, newColor, lerpAmount);
            foreach (TextMeshProUGUI text in _texts)
            {
                text.color = Color.Lerp(text.color, new Vector4(text.color.r, text.color.g, text.color.b, 0f), lerpAmount);
            }

            yield return null;
        }
    }

    void Update()
    {
        if (_currentBackgroundCoroutine == null)
        {
            _secondsToWait = Random.Range(3, 11);
            _currentBackgroundCoroutine = StartCoroutine(ChangeBGImage(_secondsToWait));
        }
    }

    IEnumerator ChangeBGImage(float secondsToWait)
    {
        background.sprite = normalBackground;
        yield return new WaitForSeconds(secondsToWait);

        background.sprite = reversedBackground;
        yield return new WaitForSeconds(0.3f);

        background.sprite = normalBackground;
        _currentBackgroundCoroutine = null;
    }
}
