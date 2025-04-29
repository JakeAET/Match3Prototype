using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uiPlaySound : MonoBehaviour
{
    public void playSound(string soundName)
    {
        FindObjectOfType<AudioManager>().Play(soundName);
    }
}
