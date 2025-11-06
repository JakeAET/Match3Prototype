using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSceneChanger : MonoBehaviour
{
    public void changeScene(string sceneName)
    {
        AudioManager.instance.sceneChanged(sceneName, false);
        SceneManager.LoadScene(sceneName);
    }
}
