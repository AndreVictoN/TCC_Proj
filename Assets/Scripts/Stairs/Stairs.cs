using UnityEngine;
using UnityEngine.SceneManagement;

public class Stairs : MonoBehaviour
{
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Classroom")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
