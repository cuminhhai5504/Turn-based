using UnityEngine;
using System.Collections;

public class GameLoader : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null;

        var mgr = SaveLoadManager.Instance;

        // 🔥 FIX QUAN TRỌNG
        if (mgr == null || mgr.cachedData == null)
        {
            Debug.Log("New Game → skip load");
            yield break;
        }

        Debug.Log("Continue → load game");

        yield return mgr.LoadRoutine(mgr.cachedData);

        mgr.cachedData = null;
    }
}