using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UndoSystem : MonoBehaviour {


    public List<string> action = new List<string>();//list off actions
    public List<BuildList> buildList = new List<BuildList>(); //list of builded buildings
    public List<DemolishList> demolishList = new List<DemolishList>();//list of demolished buildings

    private BuildingList buildingList;

    private int amount;

    private void Start()
    {
        buildingList = GetComponent<BuildingList>();
    }

    public void Undo()
    {
        if (action.Count > 0) //if there are more than zero actions
        {
            amount = 0;

            if (action[action.Count - 1].Equals("Build"))  //if action is build
            {
                if (buildList[buildList.Count - 1].iBuildingCorountine != null)
                    buildingList.StopCoroutine(buildList[buildList.Count - 1].iBuildingCorountine); //stop corountite for placing conveyors

                //for each building in build list destroy building that was builded last
                for (int i = 0; i < buildList[buildList.Count - 1].buildings.Length; i++)
                {
                    if (buildList[buildList.Count - 1].buildings[i] != null)
                    {
                        if (buildList[buildList.Count - 1].buildings[i].activeSelf)
                            amount++;

                        Destroy(buildList[buildList.Count - 1].buildings[i]);
                    }
                }

                buildingList.BuildingsCount(buildList[buildList.Count - 1].iBuilding, amount);

                buildList.RemoveAt(buildList.Count - 1);
            }
            else if (action[action.Count - 1].Equals("Demolish"))//if action is demolish
            {
                //for each building in demolish list active all building that was destroyed
                for (int i = 0; i < demolishList[demolishList.Count - 1].buildings.Count; i++)
                {
                    demolishList[demolishList.Count - 1].buildings[i].SetActive(true); 
                    demolishList[demolishList.Count - 1].buildings[i].GetComponent<BuildingInfo>().BuildingUnselect();

                    buildingList.BuildingsCount(int.Parse(demolishList[demolishList.Count - 1].buildings[i].gameObject.name), -1);
                }
                demolishList.RemoveAt(demolishList.Count - 1);
            }         
            action.RemoveAt(action.Count - 1);
        }
    }
}
