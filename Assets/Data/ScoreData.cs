using UnityEngine;

namespace DataSystem
{
    [CreateAssetMenu(menuName = "Data System/Score", fileName = "Score", order = 0)]
    public class ScoreData : ScriptableObject
    {
        [SerializeField]
        private int score = 0;
        public int Score => score-score%10;
        
        public void AddScore(int roundScore) {
            score += roundScore;
        }
        public void ClearScore() {
            score = 0;
        }
        
    }
}