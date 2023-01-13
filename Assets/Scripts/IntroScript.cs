using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{
    public Animator crossFade;

    private void Start()
    {
        StartCoroutine(CrossFadeController());
    }

    IEnumerator CrossFadeController()
    {
        yield return new WaitForSeconds(10f);
        crossFade.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        crossFade.SetTrigger("Start");
        yield return new WaitForSeconds(4f);
        crossFade.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        crossFade.SetTrigger("Start");
        yield return new WaitForSeconds(5f);
        SkipButton();
    }

    public void SkipButton()
    {
        crossFade.SetTrigger("End");
        StartCoroutine(GoToLevel(1, 1));
    }

    IEnumerator GoToLevel(float delay, int indexOfLevel)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(indexOfLevel);
    }
}
