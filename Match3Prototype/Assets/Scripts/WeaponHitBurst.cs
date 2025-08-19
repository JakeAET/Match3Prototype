using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitBurst : MonoBehaviour
{
    [SerializeField] ParticleSystem[] particleSystems;

    public void initialize(Color color)
    {
        color.a = 1;
        foreach (ParticleSystem ps in particleSystems)
        {
            var main = ps.main;
            main.startColor = color;
        }
    }
}
