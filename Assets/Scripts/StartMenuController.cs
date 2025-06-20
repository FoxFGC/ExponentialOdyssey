using UnityEngine;
using UnityEngine.SceneManagement;


public class StartMenuController : MonoBehaviour
{
    public GameObject startMenuPanel;

    public MonoBehaviour[] controlScripts;

    void Awake()
    {
        startMenuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (var s in controlScripts) s.enabled = false;
    }
    public void OnPlayPressed()
    {
        startMenuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        foreach (var s in controlScripts) s.enabled = true;
    }

    public void OnOptionsPressed()
    {
        // TODO: show or toggle an options submenu
        Debug.Log("Options clicked!");
    }

    public void OnQuitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}