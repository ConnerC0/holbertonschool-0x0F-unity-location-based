using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Android;

public class LocationDisplay : MonoBehaviour
{
    public TextMeshProUGUI latitudeText;
    public TextMeshProUGUI longitudeText;
    public TextMeshProUGUI altitudeText;
    public TextMeshProUGUI distanceText;
    private LocationInfo storedLocation;

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
    
    public void StoreCurrentLocation()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            storedLocation = Input.location.lastData;
            Debug.Log($"Stored location: Latitude = {storedLocation.latitude}, Longitude = {storedLocation.longitude}, Altitude = {storedLocation.altitude}");
        }
        else
        {
            Debug.LogError("Location services are not running. Cannot store the current location.");
        }
    }
    
    public void CalculateDistance()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            LocationInfo currentLocation = Input.location.lastData;
            float distance = CalculateDistanceBetweenCoordinates(storedLocation, currentLocation);
            distanceText.text = $"Distance: {distance} meters";
        }
        else
        {
            Debug.LogError("Location services are not running. Cannot calculate the distance.");
        }
    }

    private float CalculateDistanceBetweenCoordinates(LocationInfo location1, LocationInfo location2)
    {
        float earthRadius = 6371e3f; // Earth's radius in meters
        float lat1 = Mathf.Deg2Rad * location1.latitude;
        float lat2 = Mathf.Deg2Rad * location2.latitude;
        float deltaLat = Mathf.Deg2Rad * (location2.latitude - location1.latitude);
        float deltaLon = Mathf.Deg2Rad * (location2.longitude - location1.longitude);

        float a = Mathf.Sin(deltaLat / 2) * Mathf.Sin(deltaLat / 2) +
                  Mathf.Cos(lat1) * Mathf.Cos(lat2) * Mathf.Sin(deltaLon / 2) * Mathf.Sin(deltaLon / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        return earthRadius * c;
    }
}
