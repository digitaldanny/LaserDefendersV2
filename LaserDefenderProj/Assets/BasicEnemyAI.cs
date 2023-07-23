using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
 * CLASS DESCRIPTION:
 * This class implements a basic enemy AI that starts shooting as soon as it is instantiated.
 * +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
 */
public class BasicEnemyAI : MonoBehaviour
{
    Shooter shooter;

    private void Awake()
    {
        shooter = GetComponent<Shooter>();
    }

    void Start()
    {
        shooter.FireContinuously(true);
    }
}
