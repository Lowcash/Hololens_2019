using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsingEffectManager : MonoBehaviour {
    public enum ScaleDirection { Forward, Backward }

    [Header("Pulse object")]
    public GameObject objectToScale;

    [Header("Scale settings")]
    public float fromScale = 0.0f;
    public float toScale = 1.0f;
    public float scaleDifferenceToFullTransparency = 0.2f;

    public ScaleDirection scaleDirection;

    [Header("Other properties")]
    public float speedOfTransition = 0.1f;

    public int countOfWaves = 5;

    private SpatialMappingSource spatialMappingSource;

    private Vector3 _defaultScale;
    private Vector3 _pulseToPosition;

    private float _cameraDistanceScale;
    private float _scaleBetweenWaves;

    private int _previousCountOfWaves = 5;

    private List<float> _actualScales = new List<float>();
    private List<float> _renderedScales = new List<float>();

    private List<GameObject> _scaleStencils = new List<GameObject>();
    private List<GameObject> _scaleObjects = new List<GameObject>();

    private List<ShaderExtensionEffectManager> _objectsShaderManagers = new List<ShaderExtensionEffectManager>();

    private List<Renderer> _roomRenderers = new List<Renderer>();

    private void Awake() {
        GM.OnChangeFindObjectTransform += ObjectToFind_OnChangeTransform;

        if ((spatialMappingSource = SpatialMappingManager.Instance.Source) != null) {
            spatialMappingSource.GetMeshRenderers().ForEach(r => _roomRenderers.Add(r));

            spatialMappingSource.SurfaceAdded += Surface_OnAdded;
        }
    }

    private void Start() {
        GenerateObjects();
    }

    private void Update() {
        // check if count of waves was changed
        if (_previousCountOfWaves != countOfWaves) {
            GenerateObjects();

            _previousCountOfWaves = countOfWaves;
        }

        RecalculateCameraDistance();

        UpdateScale(scaleDirection);
        SetWaveTransparency(scaleDirection);
    }

    private void RecalculateCameraDistance() {
        _cameraDistanceScale = Vector3.Distance(Camera.main.transform.position, transform.position);
    }

    private void UpdateScale( ScaleDirection direction ) {
        for (int i = 0; i < countOfWaves; i++) {
            _actualScales[i] += (Time.deltaTime * speedOfTransition) * (direction == ScaleDirection.Forward ? -1 : 1);

            // reset wave position
            switch (direction) {
                case ScaleDirection.Forward:
                    _actualScales[i] = _actualScales[i] < fromScale ? toScale : _actualScales[i];

                    break;

                case ScaleDirection.Backward:
                    _actualScales[i] = _actualScales[i] > toScale ? fromScale : _actualScales[i];

                    break;
            }

            _scaleObjects[i].transform.localScale = _defaultScale * _actualScales[i] * _cameraDistanceScale;

             _renderedScales[i] = _actualScales[i] * _cameraDistanceScale - 1;
        }

        for (int i = 0; i < _roomRenderers.Count; i++) {
            _roomRenderers[i].sharedMaterial.SetFloatArray("_Distances", _renderedScales);
        }
    }

    private void SetWaveTransparency( ScaleDirection direction ) {
        for (int i = 0; i < countOfWaves; i++) {
            if (_actualScales[i] < fromScale + scaleDifferenceToFullTransparency) {
                _objectsShaderManagers[i].SetTransparency(GetInterpolatedValueFromRange(fromScale, fromScale + scaleDifferenceToFullTransparency, _actualScales[i]));
            }
            if (_actualScales[i] > toScale - scaleDifferenceToFullTransparency) {
                _objectsShaderManagers[i].SetTransparency(GetInterpolatedValueFromRange(toScale, toScale - scaleDifferenceToFullTransparency, _actualScales[i]));
            }
        }
    }

    private float GetInterpolatedValueFromRange( float minValue, float maxValue, float actualValue ) {
        float hundredPercent = maxValue - minValue;
        float actualPercent = actualValue - minValue;

        return actualPercent / hundredPercent;
    }

    private void GenerateObjects() {
        DestroyGeneratedObjects();

        _defaultScale = objectToScale.transform.localScale;

        _scaleBetweenWaves = toScale / countOfWaves;

        for (int i = 0; i < countOfWaves; i++) {
            var scale = fromScale + (i * _scaleBetweenWaves);

            var generatedObject = Instantiate(objectToScale, transform);
            generatedObject.transform.localScale *= scale;

            _scaleObjects.Add(generatedObject);
            _actualScales.Add(scale);
            _renderedScales.Add(scale);
            _objectsShaderManagers.Add(generatedObject.GetComponent<ShaderExtensionEffectManager>());
        }
    }

    private void DestroyGeneratedObjects() {
        _scaleObjects.ForEach(o => GameObject.Destroy(o));
        _scaleStencils.ForEach(o => GameObject.Destroy(o));

        _scaleObjects.Clear();
        _scaleStencils.Clear();
        _actualScales.Clear();
        _renderedScales.Clear();
        _objectsShaderManagers.Clear();
    }

    private void Surface_OnAdded( object sender, DataEventArgs<SpatialMappingSource.SurfaceObject> e ) {
        UpdateGeneratedSurfacesRenderer(e.Data.Renderer);
    }

    private void RoomSurface_OnAdded( SpatialMappingSource.SurfaceObject e ) {
        UpdateGeneratedSurfacesRenderer(e.Renderer);
    }
    private void ObjectToFind_OnChangeTransform( object sender, ChangeFindObjectTransformEventArgs e ) {
        _pulseToPosition = e.createdObject.transform.position;

        _roomRenderers.ForEach(r => r.sharedMaterial.SetVector("_Position", _pulseToPosition));
    }

    private void UpdateGeneratedSurfacesRenderer( Renderer renderer ) {
        renderer.sharedMaterial.SetVector("_Position", _pulseToPosition);

        _roomRenderers.Add(renderer);

#if UNITY_EDITOR
        Debug.LogFormat("{0} renderer was added to Pulsing effect renderer referencies", renderer.name);
#endif
    }
}
