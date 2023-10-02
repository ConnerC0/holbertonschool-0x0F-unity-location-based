using UnityEngine;
using TMPro;
using System.Collections;

public class LocationDisplay : MonoBehaviour
{
    public TextMeshProUGUI latitudeText;
    public TextMeshProUGUI longitudeText;
    public TextMeshProUGUI altitudeText;

    private IEnumerator Start()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location services are not enabled.");
            yield break;
        }

        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return new WaitForSeconds(1);
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Location services failed to initialize.");
            yield break;
        }
    }

    private void Update()
    {
        UpdateLocationText();
    }

    private void UpdateLocationText()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            LocationInfo location = Input.location.lastData;
            latitudeText.text = $"{location.latitude}";
            longitudeText.text = $"{location.longitude}";
            altitudeText.text = $"{location.altitude}";
        }
    }

    private void OnDestroy()
    {
        Input.location.Stop();
    }
}
