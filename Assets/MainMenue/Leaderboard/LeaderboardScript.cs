using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private Transform entryContainer;
    [SerializeField] private Transform entryTemplate;
    private List<LeaderboardEntry> leaderboardEntryList;
    private List<Transform> leaderboardEntryTransformList;
    private void Awake()
    {
        if (entryTemplate == null || entryContainer == null)
        {
            Debug.LogError("Template or container not assigned.");
            return;
        }

        entryTemplate.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString("leaderboard");
        LeaderboardObject leaderboardObject = string.IsNullOrEmpty(jsonString)
            ? new LeaderboardObject { leaderboardEntryList = new List<LeaderboardEntry>() }
            : JsonUtility.FromJson<LeaderboardObject>(jsonString);

        if (leaderboardObject == null || leaderboardObject.leaderboardEntryList == null)
        {
            Debug.LogWarning("No leaderboard data found or data is malformed.");
            leaderboardEntryList = new List<LeaderboardEntry>(); // fallback empty list
        }
        else
        {
            leaderboardEntryList = leaderboardObject.leaderboardEntryList;
        }


        leaderboardEntryList.Sort((a, b) => b.score.CompareTo(a.score));

        leaderboardEntryTransformList = new List<Transform>();
        foreach (LeaderboardEntry leaderboardEntry in leaderboardEntryList)
        {
            CreateLeaderboardEntryTransform(leaderboardEntry, entryContainer, leaderboardEntryTransformList);
        }

        if (LeaderboardTransferScript.scoreToSubmit >= 0 && !string.IsNullOrEmpty(LeaderboardTransferScript.nameToSubmit))
        {
            AddLeaderboardEntry(LeaderboardTransferScript.scoreToSubmit, LeaderboardTransferScript.nameToSubmit);
            Debug.Log($"Added score from previous scene: {LeaderboardTransferScript.nameToSubmit} - {LeaderboardTransferScript.scoreToSubmit}");

            // Clear it so it's not reused on next load
            LeaderboardTransferScript.scoreToSubmit = -1;
            LeaderboardTransferScript.nameToSubmit = null;

            // OPTIONAL: refresh the leaderboard UI
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void CreateLeaderboardEntryTransform(LeaderboardEntry leaderboardEntry, Transform entryContainer, List<Transform> transformList)
    {
        if (entryTemplate == null || entryContainer == null)
        {
            Debug.LogError("Template or container not assigned.");
            return;
        }

        entryTemplate.gameObject.SetActive(false);

        float templateHeight = 50f;
        Transform entryTransform = Instantiate(entryTemplate, entryContainer);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString = rank.ToString();
        int score = leaderboardEntry.score;
        string name = leaderboardEntry.name;

        entryTransform.Find("PosText").GetComponent<Text>().text = rankString;
        entryTransform.Find("ScoreText").GetComponent<Text>().text = score.ToString();
        entryTransform.Find("NameText").GetComponent<Text>().text = name;
        entryTransform.Find("Background").gameObject.SetActive(rank % 2 == 1);

        transformList.Add(entryTransform);
    }

    private void AddLeaderboardEntry(int score, string name)
    {
        LeaderboardEntry leaderboardEntry = new LeaderboardEntry { score = score, name = name };
        string jsonString = PlayerPrefs.GetString("leaderboard");

        LeaderboardObject leaderboardObject;
        if (string.IsNullOrEmpty(jsonString))
        {
            leaderboardObject = new LeaderboardObject { leaderboardEntryList = new List<LeaderboardEntry>() };
        }
        else {
            leaderboardObject = JsonUtility.FromJson<LeaderboardObject>(jsonString);
        }

        leaderboardObject.leaderboardEntryList.Add(leaderboardEntry);

        string json = JsonUtility.ToJson(leaderboardObject);
        PlayerPrefs.SetString("leaderboard", json);
        PlayerPrefs.Save();
    }

    [ContextMenu("Clear Leaderboard")]
    public void ClearLeaderboard()
    {
        // Create and save a new empty leaderboard
        leaderboardEntryList = new List<LeaderboardEntry>();
        LeaderboardObject newLeaderboard = new LeaderboardObject
        {
            leaderboardEntryList = leaderboardEntryList
        };

        string json = JsonUtility.ToJson(newLeaderboard);
        PlayerPrefs.SetString("leaderboard", json);
        PlayerPrefs.Save();

        // Clear UI entries
        if (leaderboardEntryTransformList != null)
        {
            foreach (Transform entry in leaderboardEntryTransformList)
            {
                DestroyImmediate(entry.gameObject); // Editor-safe
            }
            leaderboardEntryTransformList.Clear();
        }

        Debug.Log("Leaderboard cleared and reset.");
    }


    private string GenerateRandomName(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] name = new char[length];
        for (int i = 0; i < length; i++)
        {
            name[i] = chars[Random.Range(0, chars.Length)];
        }
        return new string(name);
    }

    [ContextMenu("Add Random Leaderboard Entry")]
    public void AddRandomEntry()
    {
        string randomName = GenerateRandomName(6);
        int randomScore = Random.Range(1000, 100000); // or pick your desired range

        AddLeaderboardEntry(randomScore, randomName);
        Debug.Log($"Added entry: {randomName} - {randomScore}");
    }



    private class LeaderboardObject
    {
        public List<LeaderboardEntry> leaderboardEntryList;
    }

    [System.Serializable]
    private class LeaderboardEntry
    {
        public int score;
        public string name;
    }
}
