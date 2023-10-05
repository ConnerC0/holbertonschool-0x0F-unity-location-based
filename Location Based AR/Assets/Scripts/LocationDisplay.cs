using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class LocationDisplay : MonoBehaviour
{
    public TextMeshProUGUI latitudeText;
    public TextMeshProUGUI longitudeText;
    public TextMeshProUGUI altitudeText;
    public TextMeshProUGUI distanceText;
    private LocationInfo storedLocation;
    public LocationInfo currentlocation;
    private Vector2 GPSPosition;
    internal Vector3 UnityLocation;
    public TextMeshProUGUI conversionText;
    public GameObject objectToSpawn;
    public float distanceFromCamera = 5f; // Set the distance from the camera

    public ARSessionOrigin arSessionOrigin;
    public Camera arCamera;
    

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
        if (Input.location.status == LocationServiceStatus.Running)
        {
            currentlocation = Input.location.lastData;
            latitudeText.text = $"{currentlocation.latitude}";
            longitudeText.text = $"{currentlocation.longitude}";
            altitudeText.text = $"{currentlocation.altitude}";
        }
    }

    private void OnDestroy()
    {
        Input.location.Stop();
    }

    public TextMeshProUGUI storedLocationText;
    public void StoreCurrentLocation()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            storedLocation = Input.location.lastData;
            storedLocationText.text = $"Longitude: {storedLocation.longitude}, Latitude:{storedLocation.latitude}, Altitude:{storedLocation.altitude}";
            Debug.Log(
                $"Stored location: Latitude = {storedLocation.latitude}, Longitude = {storedLocation.longitude}, Altitude = {storedLocation.altitude}");
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
            distanceText.text = $"{distance} meters";
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
    
    public void GPSConversion()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            GPSPosition = new Vector2(Input.location.lastData.longitude, Input.location.lastData.latitude);
            UnityLocation = GPSEncoder.GPSToUCS(GPSPosition);
            conversionText.text = $"Unity Local Position:{UnityLocation}";
        }
    }
    
    public void SpawnGameObjectAtARDeviceLocation(string labelText)
    {
        Vector3 spawnPosition = arCamera.transform.position + arCamera.transform.forward * distanceFromCamera;
        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        spawnedObject.transform.SetParent(null);
        
        TextMeshProUGUI label = spawnedObject.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = labelText;
        }
    }

}
