using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    public bool playTutorialOnStart = false;
    public bool playTutorialForSteel = false;
    public GameObject tutorialpanel;
    public GameObject previousButton;
    public GameObject nextButton;
    public GameObject endButton;
    public GameObject[] tutorials;
    public GameObject TutorialForSteel;
    private int currentPage = 0;

    private void Start()
    {
        if (playTutorialOnStart)
        {
            tutorialpanel.SetActive(true);
            tutorials[currentPage].SetActive(true);
        }

        if (playTutorialForSteel)
            TutorialForSteel.SetActive(true);
	}

    public void ShowTutorial()
    {
        tutorials[currentPage].SetActive(true);
    }

    public void NextTutorial()
    {
        if (currentPage < tutorials.Length - 1)
        {
            tutorials[currentPage].SetActive(false);
            currentPage++;
            tutorials[currentPage].SetActive(true);
        }

        if (currentPage > 0)
            previousButton.SetActive(true);

        if (currentPage == tutorials.Length - 1)
        {
            nextButton.SetActive(false);
            endButton.SetActive(true);
        }
    }

    public void PreviousTutorial()
    {
        if (currentPage > 0)
        {
            tutorials[currentPage].SetActive(false);
            currentPage--;
            tutorials[currentPage].SetActive(true);
        }

        if (currentPage == 0)
            previousButton.SetActive(false);

        if (currentPage < tutorials.Length - 1)
        {
            nextButton.SetActive(true);
            endButton.SetActive(false);
        }
    }

    public void EndTutorial()
    {
        tutorials[currentPage].SetActive(false);
        tutorialpanel.SetActive(false);
    }

    public void CloseTutorialForSteal()
    {
        TutorialForSteel.SetActive(false);
    }
}
