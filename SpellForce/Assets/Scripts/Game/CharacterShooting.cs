using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum typeOfSpell { projectile, wall, laser}

public class CharacterShooting : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    public void CastSpell(typeOfSpell typeOfSpell)
    {
        if (typeOfSpell == typeOfSpell.projectile)
            CastProjectile();
        else if (typeOfSpell == typeOfSpell.wall)
            CastWall();
        else if (typeOfSpell == typeOfSpell.laser)
            CastLaser();

    }

    private void CastProjectile()
    {

    }

    private void CastWall()
    {

    }

    private void CastLaser()
    {

    }
}
