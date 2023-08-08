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
    public class SpriteGroup
    {
        
        public Sprite jobBg;
        public Sprite Sword;
        public Sprite Shield;
        public Sprite Bow;
		public Sprite Magic;
    }
	[SerializeField]
    public SpriteGroup job_img;
}