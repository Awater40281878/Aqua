using UnityEngine;

public class ImgMgr : MonoBehaviour
{
    public static ImgMgr instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    [System.Serializable]
    public class SpriteGroup1
    {

        public Sprite jobBg;
        public Sprite Sword;
        public Sprite Shield;
        public Sprite Bow;
        public Sprite Magic;
    }
	[System.Serializable]
	public class SpriteGroup2
	{

		public Sprite PlayerTurn;
		public Sprite EnemyTurn;
	}
	[SerializeField]
    public SpriteGroup2 BattleUIanime;
	public SpriteGroup1 job_img;
}