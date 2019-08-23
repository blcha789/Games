using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public Fade fade;

   public void LoadLevel()
    {
        fade.FadeToLevel("MainMenu");
    }
}
