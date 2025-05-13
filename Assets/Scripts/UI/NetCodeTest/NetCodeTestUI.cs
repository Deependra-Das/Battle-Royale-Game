using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetCodeTestUI : MonoBehaviour
{
    [SerializeField] private Button StartHostButton;
    [SerializeField] private Button StartClientButton;

    void Awake()
    {
        StartHostButton.onClick.AddListener(() =>
        {
            Debug.Log("Starting Host");
            NetworkManager.Singleton.StartHost();
            HideUI();
        });
        StartClientButton.onClick.AddListener(() =>
        {
            Debug.Log("Starting Client");
            NetworkManager.Singleton.StartClient();
            HideUI();
        });
    }

    void HideUI()
    {
        gameObject.SetActive(false);
    }
}
