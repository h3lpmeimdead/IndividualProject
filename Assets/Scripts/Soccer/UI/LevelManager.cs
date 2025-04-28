using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private GameObject _transitionContainers;
    private SceneTransition[] _transitions;

    private void Start()
    {
        _transitions = _transitionContainers.GetComponentsInChildren<SceneTransition>();
    }

    public void LoadScene(int index, string transitionName)
    {
        StartCoroutine(LoadSceneAsync(index, transitionName));
    }

    private IEnumerator LoadSceneAsync(int index, string transitionName)
    {
        SceneTransition sceneTransition = _transitions.First(t => t.name == transitionName);
        AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(index);
        sceneAsync.allowSceneActivation = false;
        
        yield return sceneTransition.TransitionIn();
        
        sceneAsync.allowSceneActivation = true;
        
        yield return sceneTransition.TransitionOut();
    }
}
