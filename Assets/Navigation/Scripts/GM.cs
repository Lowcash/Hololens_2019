using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;

public class RestartEventArgs : EventArgs { }

public class GM : MonoBehaviour {
    public static EventHandler<RestartEventArgs> OnRestart;

    public GameObject player;
    public GameObject initScreen;

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

    private bool _isFindObjectGenerated;
    private bool _isSceneInitializing;

    private List<GameObject> _generatedNavigations = new List<GameObject>();
    private List<GameObject> _generatedFindObjects = new List<GameObject>();

    private List<Tracking> _trackings = new List<Tracking>();
    private List<Measurement> _measurements = new List<Measurement>();
    private List<Visualizing> _visualizings = new List<Visualizing>();
    private List<Positioning> _positionings = new List<Positioning>();

    private List<Collider> _generatedSurfacesCollider = new List<Collider>();
    private List<Collider> _generatedPlanesCollider = new List<Collider>();

    private void Awake() {
        SurfaceMeshesToPlanes.OnChangePlaneProcessingState += Plane_OnChangeProcessingState;
        SurfaceMeshesToPlanes.OnPlaneCreate += Plane_OnCreate;
        SpatialMappingObserver.OnSurfaceCreate += Surface_OnCreate;
        HologramBehaviour.OnClicked += Hologram_OnClick;

        OnRestart += Restart_OnTrigger;
    }

    private void Start() {
        SetSceneInit(true);

        UpdatePlayerPosition();

        if (generateObjectToFind) {
            if (setObjectToFindRandomPosition) {
                GenerateObjectToFind(GetRandomRangeFromPlayer(_playerPosition));
            } else {
                GenerateObjectToFind(GetRangeFromPlayer(_playerPosition));
            }
        }

        if (generateNavigations) {
            GenerateNavigations();
        }

        AssignManagingProperties(_generatedNavigations);
        AssignManagingProperties(stickers);

        if (_isFindObjectGenerated) {
            _measurements.ForEach(g => g.SetMeasurementTo(_generatedFindObjects[0]));
            _trackings.ForEach(g => g.SetTrackingTo(_generatedFindObjects[0]));
        }

        _measurements.ForEach(g => g.StartMeasuring());
        _trackings.ForEach(g => g.StartTracking());
        _visualizings.ForEach(v => v.StartVisualizing());
        _positionings.ForEach(p => p.StartPositioning());
    }

    private void UpdatePlayerPosition() {
        _playerPosition = player.transform.position;
    }

    private void GenerateObjectToFind( Vector3 positionToGenerate ) {
        if (findObject) {
            var generatedObject = Instantiate(findObject, positionToGenerate, Quaternion.identity, findObjectParent.transform);

            generatedObject.SetActive(!_isSceneInitializing);

            _generatedFindObjects.Add(generatedObject);

            _isFindObjectGenerated = true;
        }
    }

    private void AssignManagingProperties( List<GameObject> objects ) {
        foreach (var item in objects) {
            if (item) {
                var tracking = item.GetComponent<Tracking>();
                var measurement = item.GetComponent<Measurement>();
                var visualizing = item.GetComponent<Visualizing>();
                var positioning = item.GetComponent<Positioning>();

                if (tracking) {
                    _trackings.Add(tracking);
                }
                if (measurement) {
                    _measurements.Add(measurement);
                }
                if (visualizing) {
                    _visualizings.Add(visualizing);
                }
                if (positioning) {
                    _positionings.Add(positioning);
                }
            }
        }
    }

    private void GenerateNavigations() {
        var playerPosition = player.transform.position;

        foreach (var navigation in navigations) {
            var generatedNavigation = Instantiate(navigation, GetDefaultNavigationPositionFromPlayer(playerPosition), Quaternion.identity, navigationObjectParent.transform);

            generatedNavigation.SetActive(!_isSceneInitializing);

            _generatedNavigations.Add(generatedNavigation);
        }
    }

    private Vector3 GetDefaultNavigationPositionFromPlayer( Vector3 playerPosition ) {
        var navigationPosition = playerPosition + (player.transform.forward * navigationObjectOffsetFromPlayer.z);  // move forward
        navigationPosition.y += navigationObjectOffsetFromPlayer.y; // put it down

        return navigationPosition;
    }

    private Vector3 GetRandomRangeFromPlayer( Vector3 playerPosition ) {
        var normalizedRandomRange = UnityEngine.Random.onUnitSphere;

        return playerPosition + new Vector3(normalizedRandomRange.x * findObjectGenerateDistanceFromPlayer.x, normalizedRandomRange.y * findObjectGenerateDistanceFromPlayer.y, normalizedRandomRange.z * findObjectGenerateDistanceFromPlayer.z);
    }

    private Vector3 GetRangeFromPlayer( Vector3 playerPosition ) {
        return playerPosition + findObjectGenerateDistanceFromPlayer;
    }

    private void UpdateGeneratedSurfacesCollider( SpatialMappingSource.SurfaceObject surface ) {
        var surfaceCollider = surface.Collider;

        surfaceCollider.enabled = false;

        _generatedSurfacesCollider.Add(surfaceCollider);

#if UNITY_EDITOR
        Debug.LogFormat("{0} collider was added to GM surface collider referencies", surfaceCollider.name);
#endif
    }

    private void UpdateGeneratedPlanesCollider( GameObject plane ) {
        var planeCollider = plane.GetComponent<Collider>();
        var planeRenderer = plane.GetComponent<Renderer>();

        planeCollider.enabled = false;
        planeRenderer.enabled = false;

        planeCollider.name = string.Format("{0} - {1}", planeCollider.name, _generatedPlanesCollider.Count);

        _generatedPlanesCollider.Add(planeCollider);

#if UNITY_EDITOR
        Debug.LogFormat("{0} collider was added to GM planes collider referencies", planeCollider.name);
#endif
    }

    #region UI
    private void SetSceneInit( bool isInitializing ) {
        _isSceneInitializing = isInitializing;

        initScreen.SetActive(isInitializing);

        SceneHelper.SetGameObjectsActive(ref stickers, !isInitializing);
        SceneHelper.SetGameObjectsActive(ref _generatedNavigations, !isInitializing);
        SceneHelper.SetGameObjectsActive(ref _generatedFindObjects, !isInitializing);
    }
    #endregion UI

    #region Handlers
    public void Surface_OnCreate( object sender, SurfaceEventArgs e ) {
        UpdateGeneratedSurfacesCollider(e.surfaceObject);
    }

    public void Plane_OnCreate( object sender, PlanesEventArgs e ) {
        UpdateGeneratedPlanesCollider(e.planeObject);
    }

    public void Hologram_OnClick( object sender, HologramClickEventArgs e ) {
        SceneHelper.SetCollidersActive(ref _generatedPlanesCollider, e.isClicked);

#if UNITY_EDITOR
        Debug.LogFormat("Hologram {0}", e.isClicked ? "clicked" : "unclicked");
#endif
    }

    public void Plane_OnChangeProcessingState(object sender, SpatialProcessingEventArgs e) {
        if (!e.isProcessing) {
            SetSceneInit(false);
        }

        Debug.LogFormat("Scene initializion {0}", e.isProcessing ? "started" : "ended");
    }

    public void Restart_OnTrigger( object sender, RestartEventArgs e ) {
        Debug.Log("Restart");
    }
    #endregion Handlers
}