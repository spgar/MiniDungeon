using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/SPGMetaStates/BattleMetaState")]
public class BattleMetaState : MonoBehaviour
{
    public MetaStateManager MetaStateManager;
    public Texture2D BackgroundTexture;
    public Texture2D MonsterTexture;
    public Hero Hero;

	void Start()
	{
	}
	
	void Update()
    {
        if (MetaStateManager.GetCurrentMetaState() != MetaState.Battle)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            Hero.AddXP(10);
            Hero.TakeDamage(Random.Range(0, 3));
            MetaStateManager.SetNewMetaState(MetaState.Dungeon);
        }
	}

    void OnGUI()
    {
        if (MetaStateManager.GetCurrentMetaState() != MetaState.Battle)
        {
            return;
        }

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BackgroundTexture);
        GUI.DrawTexture(new Rect(Screen.width / 2 - MonsterTexture.width / 2, Screen.height / 2 - MonsterTexture.height / 2, MonsterTexture.width, MonsterTexture.height), MonsterTexture);
        GUI.Label(new Rect(Screen.width / 2 - MonsterTexture.width / 2 + 100, Screen.height / 2 - MonsterTexture.height / 2 - 50, 500, 50), "You have been attacked by a monster toad! Press X to defeat.");
    }
}
