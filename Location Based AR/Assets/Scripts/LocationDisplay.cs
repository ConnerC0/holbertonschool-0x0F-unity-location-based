using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Android;

public class LocationDisplay : MonoBehaviour
{
    public TextMeshProUGUI latitudeText;
    public TextMeshProUGUI longitudeText;
    public TextMeshProUGUI altitudeText;

    private IEnumerator Start()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location services are not enabled. Status: " + Input.location.status);
            yield break;
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }

            // Wait for the user to authorize the permission
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return new WaitForSeconds(1);
            }
        }

        Input.location.Start(10, 10);

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
