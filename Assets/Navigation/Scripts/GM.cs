using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;

public class ChangeFindObjectTransformEventArgs : EventArgs {
    public GameObject createdObject;
}

public class RestartEventArgs : EventArgs { }

public class GM : MonoBehaviour {
    public static EventHandler<RestartEventArgs> OnRestart;
    public static EventHandler<ChangeFindObjectTransformEventArgs> OnChangeFindObjectTransform;

    public GameObject player;
    public GameObject initScreen;
    public GameObject spatialMapping;
    public GameObject spatialProcessing;

    [Header("Settings")]
    public bool generateObjectToFind = true;
    public bool generateNavigations = true;
    public bool generateStickers = true;
    public bool setObjectToFindRandomPosition = false;

    [Header("Distance settings")]
    public Vector3 findObjectGenerateDistanceFromPlayer;

    [Header("Object to generate")]
    public List<GameObject> objectsToFind = new List<GameObject>();
    public List<GameObject> stickers = new List<GameObject>();
    public List<GameObject> navigations = new List<GameObject>();

    [Header("Generate object parents")]
    public Transform objectToFindParent;
    public Transform navigationObjectParent;
    public Transform UIParent;

    private bool _isSceneInitializing;

    private List<GameObject> _generatedFindObjects = new List<GameObject>();
    private List<GameObject> _generatedNavigations = new List<GameObject>();
    private List<GameObject> _generatedStickers = new List<GameObject>();

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
        SetActiveSpatialObjects(false);
        SetSceneInit(true);

        if (generateNavigations)
            GenerateObjects(navigations, ref _generatedNavigations, navigationObjectParent);

        if (generateStickers)
            GenerateObjects(stickers, ref _generatedStickers, UIParent);

        if (generateObjectToFind) {
            if (setObjectToFindRandomPosition) {
                GenerateObjectToFind(objectsToFind[0], GetRandomRangeFromPlayer(player.transform.position), objectToFindParent);
            } else {
                GenerateObjectToFind(objectsToFind[0], GetRangeFromPlayer(player.transform.position), objectToFindParent);
            }

            setObjectToFindRandomPosition = true;
        }

        SetActiveSpatialObjects(true);

        AssignManagingProperties(_generatedNavigations);
        AssignManagingProperties(_generatedStickers);

        _measurements.ForEach(g => g.SetMeasurementTo(_generatedFindObjects[0]));
        _trackings.ForEach(g => g.SetTrackingTo(_generatedFindObjects[0]));

        _measurements.ForEach(g => g.StartMeasuring());
        _trackings.ForEach(g => g.StartTracking());
        _visualizings.ForEach(v => v.StartVisualizing());
        _positionings.ForEach(p => p.StartPositioning());
    }

    private void UpdateFindObjectPositon( ref GameObject objectToFind, Vector3 newPosition ) {
        objectToFind.transform.position = newPosition;

        OnChangeFindObjectTransform(this, new ChangeFindObjectTransformEventArgs() { createdObject = objectToFind });
    }

    private void GenerateObjectToFind( GameObject objectToFind, Vector3 positionToGenerate, Transform generationParent ) {
        var generatedObject = Instantiate(objectToFind, generationParent);

        UpdateFindObjectPositon(ref generatedObject, positionToGenerate);

        generatedObject.SetActive(!_isSceneInitializing);

        _generatedFindObjects.Add(generatedObject);
    }

    private void AssignManagingProperties( List<GameObject> objects ) {
        foreach (var item in objects) {
            if (item) {
                var tracking = item.GetComponent<Tracking>();
                var measurement = item.GetComponent<Measurement>();
                var visualizing = item.GetComponent<Visualizing>();
                var positioning = item.GetComponent<Positioning>();

                if (tracking)
                    _trackings.Add(tracking);
                if (measurement)
                    _measurements.Add(measurement);
                if (visualizing)
                    _visualizings.Add(visualizing);
                if (positioning)
                    _positionings.Add(positioning);
            }
        }
    }

    private void GenerateObjects( List<GameObject> gameObjectPrefabs, ref List<GameObject> gameObjectInstances, Transform generateObjectParent ) {
        foreach (var prefab in gameObjectPrefabs) {
            var generatedPrefab = Instantiate(prefab, generateObjectParent);

            generatedPrefab.SetActive(!_isSceneInitializing);

            gameObjectInstances.Add(generatedPrefab);
        }
    }

    private Vector3 GetRandomRangeFromPlayer( Vector3 playerPosition ) {
        var normalizedRandomRange = UnityEngine.Random.onUnitSphere;

        return playerPosition + new Vector3(normalizedRandomRange.x * findObjectGenerateDistanceFromPlayer.x, normalizedRandomRange.y * findObjectGenerateDistanceFromPlayer.y, normalizedRandomRange.z * findObjectGenerateDistanceFromPlayer.z);
    }

    private Vector3 GetRangeFromPlayer( Vector3 playerPosition ) {
        return playerPosition + findObjectGenerateDistanceFromPlayer;
    }

    private void UpdateGeneratedSurfacesCollider( Collider surfaceCollider ) {
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

    private void SetActiveSpatialObjects( bool isActive ) {
        spatialMapping.SetActive(isActive);
        spatialProcessing.SetActive(isActive);
    }

    #region UI
    private void SetSceneInit( bool isInitializing ) {
        _isSceneInitializing = isInitializing;

        initScreen.SetActive(isInitializing);

        ObjectHelper.SetGameObjectsActive(ref _generatedStickers, !isInitializing);
        ObjectHelper.SetGameObjectsActive(ref _generatedNavigations, !isInitializing);
        ObjectHelper.SetGameObjectsActive(ref _generatedFindObjects, !isInitializing);
    }
    #endregion UI

    #region Handlers
    private void Surface_OnCreate( object sender, SurfaceEventArgs e ) {
        UpdateGeneratedSurfacesCollider(e.surfaceObject.Collider);
    }

    private void Plane_OnCreate( object sender, PlanesEventArgs e ) {
        UpdateGeneratedPlanesCollider(e.planeObject);
    }

    private void Hologram_OnClick( object sender, HologramClickEventArgs e ) {
        ObjectHelper.SetCollidersActive(ref _generatedPlanesCollider, e.isClicked);

#if UNITY_EDITOR
        Debug.LogFormat("Hologram {0}", e.isClicked ? "clicked" : "unclicked");
#endif
    }

    private void Plane_OnChangeProcessingState( object sender, SpatialProcessingEventArgs e ) {
        if (!e.isProcessing) { SetSceneInit(false); }

        Debug.LogFormat("Scene initializion {0}", e.isProcessing ? "started" : "ended");
    }

    private void Restart_OnTrigger( object sender, RestartEventArgs e ) {
        if (generateObjectToFind) {
            var generatedObject = _generatedFindObjects[0];

            if (setObjectToFindRandomPosition) {
                UpdateFindObjectPositon(ref generatedObject, GetRandomRangeFromPlayer(player.transform.position));
            } else {
                UpdateFindObjectPositon(ref generatedObject, GetRangeFromPlayer(player.transform.position));
            }
        }

        Debug.Log("Restart");
    }
    #endregion Handlers
}