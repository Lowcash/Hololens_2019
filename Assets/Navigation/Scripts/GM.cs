using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour {
    public GameObject player;

    [Header("Object to generate")]
    public GameObject findObject;
    public List<GameObject> navigations = new List<GameObject>();

    [Header("Generate object parents")]
    public GameObject findObjectParent;
    public GameObject navigationObjectParent;

    [Header("Distances and offsets")]
    public Vector3 findObjectGenerateDistanceFromPlayer;

    public Vector3 navigationObjectOffsetFromPlayer;

    private List<GameObject> _generatedFindObjects = new List<GameObject>();

    private List<Tracking> _trackings = new List<Tracking>();
    private List<Measurement> _measurements = new List<Measurement>();
    private List<Visualizing> _visualizings = new List<Visualizing>();

    private void Start()
    {
        GenerateObjectToFind();
        GenerateNavigationObject();

        if (_generatedFindObjects.Count > 0)
        {
            _measurements.ForEach(g => g.SetMeasurementTo(_generatedFindObjects[0]));
            _trackings.ForEach(g => g.SetTrackingTo(_generatedFindObjects[0]));

            _measurements.ForEach(g => g.StartMeasuring());
            _trackings.ForEach(g => g.StartTracking());
            _visualizings.ForEach(v => v.StartVisualizing());
        }
    }

    private void GenerateObjectToFind()
    {
        var playerPosition = player.transform.position;

        var generatedObject = Instantiate(findObject, GetRangeFromPlayer(playerPosition), Quaternion.identity, findObjectParent.transform);
        //var generatedObject = Instantiate(findObject, GetRandomRangeFromPlayer(playerPosition), Quaternion.identity, findObjectParent.transform);

        _generatedFindObjects.Add(generatedObject);
    }

    private void GenerateNavigationObject()
    {
        var playerPosition = player.transform.position;

        foreach (var navigation in navigations)
        {
            var generatedObject = Instantiate(navigation, GetDefaultNavigationPositionFromPlayer(playerPosition), Quaternion.identity, navigationObjectParent.transform);

            var tracking = generatedObject.GetComponent<Tracking>();
            var measurement = generatedObject.GetComponent<Measurement>();
            var visualizing = generatedObject.GetComponent<Visualizing>();

            if (tracking)
            {
                _trackings.Add(tracking);
            }
            if (measurement)
            {
                _measurements.Add(measurement);
            }
            if (visualizing)
            {
                _visualizings.Add(visualizing);
            }
        }
    }

    private Vector3 GetDefaultNavigationPositionFromPlayer(Vector3 playerPosition)
    {
        var navigationPosition = playerPosition + (player.transform.forward * navigationObjectOffsetFromPlayer.z);  // move forward
        navigationPosition.y += navigationObjectOffsetFromPlayer.y; // put it down

        return navigationPosition;
    }

    private Vector3 GetRandomRangeFromPlayer(Vector3 playerPosition)
    {
        var normalizedRandomRange = Random.onUnitSphere;

        return playerPosition + new Vector3(normalizedRandomRange.x * findObjectGenerateDistanceFromPlayer.x, normalizedRandomRange.y * findObjectGenerateDistanceFromPlayer.y, normalizedRandomRange.z * findObjectGenerateDistanceFromPlayer.z);
    }

    private Vector3 GetRangeFromPlayer(Vector3 playerPosition)
    {
        return playerPosition + findObjectGenerateDistanceFromPlayer;
    }
}