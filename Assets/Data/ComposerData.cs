using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    [CreateAssetMenu(menuName = "Data System/Composer", fileName = "Composer", order = 1)]
    public class ComposerData : ScriptableObject
    {
        [SerializeField]
        private List<AudioClip> songs = new();
        public List<AudioClip> Songs => songs;

        [SerializeField]
        private new string name;
        [SerializeField]
        public string Name => name;

        [SerializeField]
        private Sprite portrait;
        public Sprite Portrait => portrait;

        [SerializeField]
        private int level;
        public int Level => level;

        [SerializeField]
        private AudioClip answerAudio;
        public AudioClip AnswerAudio => answerAudio;


        [SerializeField]
        private AudioClip selectionAudio;
        public AudioClip SelectionAudio => selectionAudio;

    }
}