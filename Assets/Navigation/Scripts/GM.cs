using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;

public class GM : MonoBehaviour
{
    public GameObject player;
    public GameObject spatialMapping;
    public GameObject spatialProcessing;

    [Header("Settings")]
    public bool generateObjectToFind = true;
    public bool generateNavigations = true;
    public bool setObjectToFindRandomPosition = false;

    public Vector3 findObjectGenerateDistanceFromPlayer;
    public Vector3 navigationObjectOffsetFromPlayer;

    [Header("Sticker UI")]
    public List<GameObject> stickers = new List<GameObject>();

    [Header("Object to generate")]
    public GameObject findObject;

    [Header("Navigation")]
    public List<GameObject> navigations = new List<GameObject>();

    [Header("Generate object parents")]
    public GameObject findObjectParent;
    public GameObject navigationObjectParent;

    private Vector3 _playerPosition;

    private bool _isFindObjectGenerated = false;

    private List<GameObject> _generatedNavigations = new List<GameObject>();
    private List<GameObject> _generatedFindObjects = new List<GameObject>();

    private List<GameObject> _spatialMappingObjects = new List<GameObject>();

    private List<Tracking> _trackings = new List<Tracking>();
    private List<Measurement> _measurements = new List<Measurement>();
    private List<Visualizing> _visualizings = new List<Visualizing>();
    private List<Positioning> _positionings = new List<Positioning>();

    private List<SpatialMappingSource.SurfaceObject> _generatedSurfaces = new List<SpatialMappingSource.SurfaceObject>();

    private void Awake()
    {
        SpatialMappingObserver.OnSurfaceCreate += Surface_OnCreate;
    }

    private void Start()
    {
        UpdatePlayerPosition();

        if (generateObjectToFind)
        {
            if (setObjectToFindRandomPosition)
            {
                GenerateObjectToFind(GetRandomRangeFromPlayer(_playerPosition));
            }
            else
            {
                GenerateObjectToFind(GetRangeFromPlayer(_playerPosition));
            }
        }

        if (generateNavigations)
        {
            GenerateNavigations();
        }

        AssignManagingProperties(_generatedNavigations);
        AssignManagingProperties(stickers);

        if (_isFindObjectGenerated)
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
        if (findObject)
        {
            var generatedObject = Instantiate(findObject, positionToGenerate, Quaternion.identity, findObjectParent.transform);

            _generatedFindObjects.Add(generatedObject);

            _isFindObjectGenerated = true;
        }
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

                if (item.GetComponent<HologramBehaviour>())
                {
                    HologramBehaviour.OnClicked += Hologram_OnClick;
                }
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

    private void UpdateGeneratedSurfaces(SpatialMappingSource.SurfaceObject surface)
    {
        _generatedSurfaces.Add(surface);

#if UNITY_EDITOR
        Debug.LogFormat("{0} was added to GM surface referencies", surface.Object.name);
#endif
    }

    public void Surface_OnCreate(object sender, SurfaceEventArgs e)
    {
        UpdateGeneratedSurfaces(e.surfaceObject);
    }

    public void Hologram_OnClick(object sender, HologramClickEventArgs e)
    {
        SetSurfacesCollidersActive(e.value);

#if UNITY_EDITOR
        Debug.LogFormat("Hologram {0}", e.value ? "clicked" : "unclicked");
#endif
    }

    public void SetSurfacesCollidersActive(bool enabled)
    {
        _generatedSurfaces.ForEach(s => s.Collider.enabled = enabled);
    }
}