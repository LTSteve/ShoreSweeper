using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    public NewGameView NewGameMenu;
    public LoadGameView LoadGameMenu;
    public SettingsView SettingsMenu;

    private SubmenuView CurrentView;

    private void Start()
    {
        StartCoroutine(_loadAsync());
    }


    private IEnumerator _loadAsync()
    {
        yield return null; // wait for everything to load
        LoadingView.Enable();

        yield return null;
        LoadingView.Set(0.5f, "Loading Main Menu...");

        yield return null;
        LoadingView.Set(1f);

        yield return null;
    }

    public void OpenNewGame()
    {
        if(CurrentView is NewGameView)
        {
            CurrentView.ToggleOpen();
            CurrentView = null;
            return;
        }

        if (CurrentView != null)
        {
            CurrentView.ToggleOpen();
        }

        NewGameMenu.ToggleOpen();
        CurrentView = NewGameMenu;
    }

    public void OpenLoadGame()
    {
        if (CurrentView is LoadGameView)
        {
            CurrentView.ToggleOpen();
            CurrentView = null;
            return;
        }

        if (CurrentView != null)
        {
            CurrentView.ToggleOpen();
        }

        LoadGameMenu.ToggleOpen();
        CurrentView = LoadGameMenu;
    }

    public void OpenSettings()
    {
        if (CurrentView is SettingsView)
        {
            CurrentView.ToggleOpen();
            CurrentView = null;
            return;
        }

        if (CurrentView != null)
        {
            CurrentView.ToggleOpen();
        }

        SettingsMenu.ToggleOpen();
        CurrentView = SettingsMenu;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
