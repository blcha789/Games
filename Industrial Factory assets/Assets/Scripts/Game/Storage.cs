using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Storage : MonoBehaviour
{
    public GameObject storageListPrefab;
    public Transform contentParent;
    public GameObject StoragePanel;

    void Start()
    {
        
    }

    public void AssemblerStorage(StorageList[] storageLists)
    {
        StoragePanel.SetActive(true);

        for (int i = 0; i < storageLists.Length; i++)
        {
            GameObject l = Instantiate(storageListPrefab, contentParent);
            l.transform.GetChild(0).GetComponentInChildren<Image>().sprite = storageLists[i].image;
            l.transform.GetChild(1).GetComponentInChildren<Text>().text = storageLists[i].name;
            l.transform.GetChild(2).GetComponentInChildren<Text>().text = storageLists[i].amount + "/" + storageLists[i].amountNeed;
        }
    }

    public void RefineryStorage(StorageList[] storageLists)
    {
        StoragePanel.SetActive(true);

        for (int i = 0; i < storageLists.Length; i++)
        {
            GameObject l = Instantiate(storageListPrefab, contentParent);
            l.transform.GetChild(0).GetComponentInChildren<Image>().sprite = storageLists[i].image;
            l.transform.GetChild(1).GetComponentInChildren<Text>().text = storageLists[i].name;
            l.transform.GetChild(2).GetComponentInChildren<Text>().text = storageLists[i].amount + "/" + storageLists[i].amountNeed;
        }
    }

    public void SolidifierStorage(StorageList[] storageLists)
    {
        StoragePanel.SetActive(true);

        for (int i = 0; i < storageLists.Length; i++)
        {
            GameObject l = Instantiate(storageListPrefab, contentParent);
            l.transform.GetChild(0).GetComponentInChildren<Image>().sprite = storageLists[i].image;
            l.transform.GetChild(1).GetComponentInChildren<Text>().text = storageLists[i].name;
            l.transform.GetChild(2).GetComponentInChildren<Text>().text = storageLists[i].amount + "/" + storageLists[i].amountNeed;
        }
    }

    public void PowerPlantStorage(StorageList[] storageLists)
    {
        StoragePanel.SetActive(true);

        for (int i = 0; i < storageLists.Length; i++)
        {
            GameObject l = Instantiate(storageListPrefab, contentParent);
            l.transform.GetChild(0).GetComponentInChildren<Image>().sprite = storageLists[i].image;
            l.transform.GetChild(1).GetComponentInChildren<Text>().text = storageLists[i].name;
            l.transform.GetChild(2).GetComponentInChildren<Text>().text = storageLists[i].amount + "/" + storageLists[i].amountNeed;
        }
    }

    public void BuyerStorage(StorageList[] storageLists)
    {
        StoragePanel.SetActive(true);

        for (int i = 0; i < storageLists.Length; i++)
        {
            GameObject l = Instantiate(storageListPrefab, contentParent);
            l.transform.GetChild(0).GetComponentInChildren<Image>().sprite = storageLists[i].image;
            l.transform.GetChild(1).GetComponentInChildren<Text>().text = storageLists[i].name;
            l.transform.GetChild(2).GetComponentInChildren<Text>().text = storageLists[i].amount + "/" + storageLists[i].amountNeed;
        }
    }
}
