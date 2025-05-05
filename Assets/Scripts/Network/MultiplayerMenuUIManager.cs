using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerMenuUIManager : MonoBehaviour
{
    public TMP_InputField playerNameInputText;
    public Button SaveJoinButton;

    private void Awake()
    {
        SaveJoinButton.onClick.AddListener(OnJoinGameClicked);
    }

    private void OnDestroy()
    {
        SaveJoinButton.onClick.RemoveListener(OnJoinGameClicked);
    }

    void Start()
    {
        if(PlayerPrefs.HasKey("PlayerName"))
        {
            playerNameInputText.text = PlayerPrefs.GetString("PlayerName");
        }
    }

    public void OnJoinGameClicked()
    {
        PlayerPrefs.SetString("PlayerName", playerNameInputText.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MultiplayerTestScene");
    }
}
