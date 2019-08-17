using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreDeposit : MonoBehaviour {

    public GameObject ore;
    public int depositSize;
    public Sprite outputImage;

    private int size;

    private void Start()
    {
        size = depositSize;
    }

    private void Update()
    {
        if (depositSize <= 0)
            Destroy(gameObject);
    }

    public void SetDefaults()
    {
        depositSize = size;
    }
}
