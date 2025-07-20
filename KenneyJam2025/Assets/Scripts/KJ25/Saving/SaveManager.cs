using UnityEngine;

namespace KJ25.Saving {
   public static class SaveManager {
      public static int LastUnlockedLevelIndex => PlayerPrefs.GetInt(nameof(LastUnlockedLevelIndex), 0);

      public static void SetUnlockedLevelIndex(int index) => PlayerPrefs.SetInt(nameof(LastUnlockedLevelIndex), Mathf.Max(index, LastUnlockedLevelIndex));
   }
}