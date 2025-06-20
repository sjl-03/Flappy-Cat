using UnityEngine;
using TMPro;

public class SaveScoreScript : MonoBehaviour
{
    [SerializeField] private LogicScript logic;
    [SerializeField] private GameObject nameInputContainer;
    [SerializeField] private TMP_InputField playerName;

    private

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    public void OnEnterName()
    {
        logic.SavePlayerScore(playerName.text);
    }
}
