using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/SPGMetaStates/HomeBaseMetaState")]
public class HomeBaseMetaState : MonoBehaviour
{
    public MetaStateManager MetaStateManager;
    public Texture2D BackgroundTexture;

	void Start()
    {	
	}
	
	void Update()
    {	
	}

    void OnGUI()
    {
        if (MetaStateManager.GetCurrentMetaState() != MetaState.HomeBase)
        {
            return;
        }

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BackgroundTexture);

        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 40), "Go on an Adventure"))
        {
            MetaStateManager.SetNewMetaState(MetaState.Dungeon);
        }

        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 40), "Back to Main Menu"))
        {
            MetaStateManager.SetNewMetaState(MetaState.MainMenu);
        }
    }
}
