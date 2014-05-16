using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/SPGMetaStates/StoryMetaState")]
public class StoryMetaState : MonoBehaviour
{
    public MetaStateManager MetaStateManager;
    public DungeonGenerator DungeonGenerator;
    public Texture2D BackgroundTexture;

	void Start()
	{
	}
	
	void Update()
    {
        if (MetaStateManager.GetCurrentMetaState() != MetaState.Story)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {                
            DungeonGenerator.GoToNextLevel();
            MetaStateManager.SetNewMetaState(MetaState.Dungeon);
        }
	}

    void OnGUI()
    {
        if (MetaStateManager.GetCurrentMetaState() != MetaState.Story)
        {
            return;
        }

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BackgroundTexture);
       
        GUI.Box(new Rect(25, 100, 700, 25), "Here is a chance for some storyline in between dungeon floors. Maybe add some tales of magic and heroism here.");
        GUI.Box(new Rect(25, 150, 200, 25), "Press enter to continue.");
    }
}
