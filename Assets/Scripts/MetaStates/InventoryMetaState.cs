using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/SPGMetaStates/InventoryMetaState")]
public class InventoryMetaState : MonoBehaviour
{
    public MetaStateManager MetaStateManager;
    public Texture2D BackgroundTexture;
    public Hero Hero;

	void Start()
    {	
	}
	
	void Update()
    {
        if (MetaStateManager.GetCurrentMetaState() != MetaState.Inventory)
        {
            return;
        }
	}

    void OnGUI()
    {
        if (MetaStateManager.GetCurrentMetaState() != MetaState.Inventory)
        {
            return;
        }

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BackgroundTexture);

        if (GUI.Button(new Rect(200, 200, 300, 25), "Click to Use Healing Potion"))
        {
            Hero.Heal(5);
        }

        if (GUI.Button(new Rect(200, 230, 300, 25), "Close Inventory"))
        {
            MetaStateManager.SetNewMetaState(MetaState.Dungeon);
        }
    }
}
