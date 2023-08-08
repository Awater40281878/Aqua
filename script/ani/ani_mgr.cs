using System;
using System.Collections.Generic;
using UnityEngine;

public class ani_mgr : MonoBehaviour
{
    public static ani_mgr instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [System.Serializable]
    public class FoldableProperties
    {
        public List<spirt_Date> ain;
    }

    [System.Serializable]
    public class AnimationGroup
    {
        [SerializeField]
        private string title = "Animation Group";
        public List<spirt_Date> Animation = new List<spirt_Date>();

        public string Title => title;
    }

    [SerializeField]
    public List<AnimationGroup> soldierAnimation = new List<AnimationGroup>();
}