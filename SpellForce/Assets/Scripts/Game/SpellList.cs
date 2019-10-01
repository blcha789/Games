using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellList : MonoBehaviour
{
  public TextAsset[] spellTxt;
  
  public Transform manaParent;
  
  public SpellList spellList;
  public List<SpellList> spellList = new List<SpellList>();
  
  private void Start()
  {
    LoadSpells();
  }
  
  private void LoadSpells()
  {
  for (int i = 0; i < spellTxt.Length; i++)
        {
            string[] lines = spellTxt[i].text.Split('\n');

            for (int j = 1; j < lines.Length; j++)
            {
                string[] line = lines[j].Split('\t');
                SpellGame spell = new SpellGame();

                spell.name = line[1];
                spell.prefab = Resources.Load("Images/Spells/" + line[1], typeof(GameObject)) as GameObject;
                spell.manaType = line[4];
                spell.manaAmount = int.Parse(line[5]); 

                spellList.spells.Add(spell);
            }
            spellLists.Add(spellList);
        }
    }
}

[System.Serializable]
public class SpellListGame
{
public List<SpellGame> spells;
}

[System.Serializable]
public class SpellGame
{
  public string name;
  public GameObject prefab;
  public string manaType;
  public float manaAmount;
}
