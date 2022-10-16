using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrefsHelper
{
    public static readonly List<(int score, string name)> scoreList = new List<(int, string)>();
    //simple static function for loading ints from the PlayerPrefs file
    public static int LoadInt(string key, int @default = 0) {
            if (PlayerPrefs.HasKey(key)){
                return PlayerPrefs.GetInt(key);
            }
            return @default;
        }
    
    public static void refreshList(){
        scoreList.Clear();
        for(int i = 1; i <= 10; i++){
            var id = $"score{i}";
            var nameid = $"namescore{i}";
            var number = LoadInt(id);
            var player = PlayerPrefs.GetString(nameid, "Empty");
            scoreList.Add((number, player));
        }
    }

    private static void updatePrefs(){
        for(int i = 1; i <= 10; i++){
            var id = $"score{i}";
            var nameid = $"namescore{i}";
            var (score, name) = scoreList[i-1];
            PlayerPrefs.SetInt(id, score);
            PlayerPrefs.SetString(nameid, name);
        }
    }

    public static void updateScores(int newScore, string newName){
        scoreList.RemoveAt(9);
        scoreList.Add((newScore, newName));
        scoreList.Sort((x, y) => y.score.CompareTo(x.score));
        updatePrefs();
    }
}
