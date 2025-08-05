using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonsManager : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
    public Image transitionImage;
    private Coroutine _currentCoroutine = null;

    public void LoadGame()
    {
        if(_currentCoroutine == null) _currentCoroutine = StartCoroutine(AnimateTransition());
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
        SceneManager.LoadScene("PrototypeScene");
    }

    private IEnumerator FadeTransition(Color oldColor, Color newColor, float time)
    {
        float elapsedTime = 0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float lerpAmount = Mathf.Clamp01(elapsedTime/time);
            transitionImage.color = Color.Lerp(oldColor, newColor, lerpAmount);
            foreach(TextMeshProUGUI text in _texts)
            {
                text.color = Color.Lerp(text.color, new Vector4(text.color.r, text.color.g, text.color.b, 0f), lerpAmount);
            }

            yield return null;
        }
    }
}
