//using UnityEditor.Il2Cpp;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager _instance;

    [SerializeField] private bool onDialog = false;
    [SerializeField] private bool onPauseMenu = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void PausePlayerInput(bool paused)
    {
        if (paused)
        {
            PlayerStats._instance.playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            PlayerStats._instance.playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public void OnPause(bool paused)
    {
        onPauseMenu = paused;
        if (onPauseMenu || onDialog)
        {
            Time.timeScale = 0f;
            PlayerStats._instance.playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            Time.timeScale = 1f;
            PlayerStats._instance.playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public void OnDialog(bool paused)
    {
        onDialog = paused;
        if (onPauseMenu || onDialog)
        {
            Time.timeScale = 0f;
            PlayerMovement._instance.audSource.mute = true;
            PlayerStats._instance.playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            Time.timeScale = 1f;
            PlayerStats._instance.playerInput.SwitchCurrentActionMap("Player");
        }
    }
}
