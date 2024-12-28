using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerNameHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField textField;
    [SerializeField] private Button connectButton;

    public const string playerNameKey = "PlayerName";

    private void Start()
    {
        if(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            LoadNextScene();
            return;
        }

        textField.text = PlayerPrefs.GetString(playerNameKey,string.Empty);

        CheackPlayerNameField();
        textField.onValueChanged.AddListener((x) =>
        {
            CheackPlayerNameField();
        });
        
        connectButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString(playerNameKey, textField.text);
            LoadNextScene();
        });
    }

    private void CheackPlayerNameField()
    {
        if (textField.text.Length <= 3 | textField.text.Length >= 13)
        {
            connectButton.interactable = false;
        }
        else
        {
            connectButton.interactable = true;
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
