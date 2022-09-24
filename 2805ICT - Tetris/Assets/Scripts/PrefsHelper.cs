using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrefsHelper
{
    //simple static function for loading ints from the PlayerPrefs file
    public static int LoadInt(string key, int @default = 0) {
            if (PlayerPrefs.HasKey(key)){
                return PlayerPrefs.GetInt(key);
            }
            return @default;
        }
}
