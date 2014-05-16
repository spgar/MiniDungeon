using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{
    public MetaStateManager MetaStateManager;
    private string heroName;
    private int heroLevel;
    private int heroXP;
    private int heroCurrentHP;
    private int heroMaxHP;
    private int heroCurrentMP;
    private int heroMaxMP;

	void Start ()
	{
	    heroName = "Superfly";
	    heroLevel = 1;
	    heroXP = 0;
	    heroCurrentHP = 10;
	    heroMaxHP = 10;
        heroCurrentMP = 5;
        heroMaxMP = 5;
	}

    public void AddXP(int award)
    {
        heroXP += award;
        CheckForLevelUp();
    }

    public void CheckForLevelUp()
    {
        if (heroLevel == 1 && heroXP >= 100)
        {
            heroLevel = 2;
            heroMaxHP = 20;
            heroMaxMP = 8;
            heroCurrentHP = heroMaxHP;
            heroCurrentMP = heroMaxMP;
        }

        if (heroLevel == 2 && heroXP >= 300)
        {
            heroLevel = 3;
            heroMaxHP = 35;
            heroMaxMP = 15;
            heroCurrentHP = heroMaxHP;
            heroCurrentMP = heroMaxMP;
        }
    }

    public void TakeDamage(int amount)
    {
        heroCurrentHP -= amount;
    }

    public void Heal(int amount)
    {
        heroCurrentHP += amount;
        if (heroCurrentHP > heroMaxHP)
        {
            heroCurrentHP = heroMaxHP;
        }
    }

    void OnGUI()
    {
        if (MetaStateManager.GetCurrentMetaState() != MetaState.Dungeon 
            && MetaStateManager.GetCurrentMetaState() != MetaState.Inventory
            && MetaStateManager.GetCurrentMetaState() != MetaState.Battle)
        {
            return;
        }

        GUI.depth = 0;
        GUI.Box(new Rect(Screen.width - 150, 25, 125, 200), "Name: " + heroName);
        GUI.Label(new Rect(Screen.width - 150, 50, 125, 25), "Level: " + heroLevel);
        GUI.Label(new Rect(Screen.width - 150, 75, 125, 25), "XP: " + heroXP);
        GUI.Label(new Rect(Screen.width - 150, 100, 125, 25), "HP: [" + heroCurrentHP + " / " + heroMaxHP + "]");
        GUI.Label(new Rect(Screen.width - 150, 125, 125, 25), "MP: [" + heroCurrentMP + " / " + heroMaxMP + "]");
    }
}