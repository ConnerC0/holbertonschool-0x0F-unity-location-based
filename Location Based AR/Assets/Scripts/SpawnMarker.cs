using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnMarker : MonoBehaviour
{
    public Button spawnButton;
    public TMP_InputField inputField;
    public LocationDisplay arObjectSpawner;

    private void Start()
    {
        spawnButton.onClick.AddListener(ShowInputField);
        inputField.onEndEdit.AddListener(SpawnObjectWithLabel);
        inputField.gameObject.SetActive(false);
    }

    private void ShowInputField()
    {
        inputField.gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
    }

    private void SpawnObjectWithLabel(string labelText)
    {
        if (!string.IsNullOrEmpty(labelText))
        {
            arObjectSpawner.SpawnGameObjectAtARDeviceLocation(labelText);
            inputField.text = "";
            inputField.gameObject.SetActive(false);
        }
    }
}
