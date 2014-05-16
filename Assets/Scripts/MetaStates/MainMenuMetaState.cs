using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/SPGMetaStates/MainMenuMetaState")]
public class MainMenuMetaState : MonoBehaviour
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
        if (MetaStateManager.GetCurrentMetaState() != MetaState.MainMenu)
        {
            return;
        }

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BackgroundTexture);

        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 40), "Play"))
        {
            MetaStateManager.SetNewMetaState(MetaState.HomeBase);
        }
        
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2, 100, 40), "Quit"))
        {
            Application.Quit();
        }
    }
}
