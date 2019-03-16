using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour {
    public GameObject player;

    [Header("Sticker UI")]
    public List<GameObject> stickers = new List<GameObject>();

    [Header("Object to generate")]
    public GameObject findObject;

    [Header("Navigation")]
    public List<GameObject> navigations = new List<GameObject>();

    [Header("Generate object parents")]
    public GameObject findObjectParent;
    public GameObject navigationObjectParent;

    [Header("Distances and offsets")]
    public Vector3 findObjectGenerateDistanceFromPlayer;

    public Vector3 navigationObjectOffsetFromPlayer;

    private List<GameObject> _generatedNavigations = new List<GameObject>();
    private List<GameObject> _generatedFindObjects = new List<GameObject>();

    private List<Tracking> _trackings = new List<Tracking>();
    private List<Measurement> _measurements = new List<Measurement>();
    private List<Visualizing> _visualizings = new List<Visualizing>();
    private List<Positioning> _positionings = new List<Positioning>();

    private Vector3 _playerPosition;

    private void Start()
    {
        UpdatePlayerPosition();

        GenerateObjectToFind(GetRangeFromPlayer(_playerPosition));
        //GenerateObjectToFind(GetRandomRangeFromPlayer(_playerPosition));
        GenerateNavigations();

        AssignManagingProperties(_generatedNavigations);
        AssignManagingProperties(stickers);

        if (_generatedFindObjects.Count > 0)
        {
            _measurements.ForEach(g => g.SetMeasurementTo(_generatedFindObjects[0]));
            _trackings.ForEach(g => g.SetTrackingTo(_generatedFindObjects[0]));
        }

        _measurements.ForEach(g => g.StartMeasuring());
        _trackings.ForEach(g => g.StartTracking());
        _visualizings.ForEach(v => v.StartVisualizing());
        _positionings.ForEach(p => p.StartPositioning());
    }

    private void UpdatePlayerPosition()
    {
        _playerPosition = player.transform.position;
    }

    private void GenerateObjectToFind(Vector3 positionToGenerate)
    {
        var generatedObject = Instantiate(findObject, positionToGenerate, Quaternion.identity, findObjectParent.transform);

        _generatedFindObjects.Add(generatedObject);
    }

    private void AssignManagingProperties(List<GameObject> objects)
    {
        foreach (var item in objects)
        {
            var tracking = item.GetComponent<Tracking>();
            var measurement = item.GetComponent<Measurement>();
            var visualizing = item.GetComponent<Visualizing>();
            var positioning = item.GetComponent<Positioning>();

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
            if (positioning)
            {
                _positionings.Add(positioning);
            }
        }
    }

    private void GenerateNavigations()
    {
        var playerPosition = player.transform.position;

        foreach (var navigation in navigations)
        {
            var generatedNavigation = Instantiate(navigation, GetDefaultNavigationPositionFromPlayer(playerPosition), Quaternion.identity, navigationObjectParent.transform);

            _generatedNavigations.Add(generatedNavigation);
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