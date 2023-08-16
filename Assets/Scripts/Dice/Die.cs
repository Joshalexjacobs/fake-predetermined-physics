using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Die : MonoBehaviour
{

    [SerializeField]
    private List<DieFace> faces;

    [SerializeField]
    private float force = 5f;

    [SerializeField]
    private GameObject ghostPrefab;

    private GameObject _spawnedGhost;

    private Rigidbody _rigidbody;

    private Quaternion _rotation;

    private Vector3 _direction;

    private Vector3 _origin;

    [HideInInspector]
    public RecordingData recordingData = new();

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.maxAngularVelocity = 10;

        DisablePhysics();

        _rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

        _direction = new Vector3(Random.Range(-1f, -1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        _origin = transform.position;
    }

    public void Reset() {
        transform.position = _origin;
        
        recordingData.Clear();
    }

    public void EnablePhysics()
    {
        _rigidbody.useGravity = true;

        _rigidbody.isKinematic = false;
    }

    public void DisablePhysics()
    {
        _rigidbody.useGravity = false;

        _rigidbody.isKinematic = true;
    }

    public void ResetToInitialState()
    {
        transform.position = _origin;

        transform.rotation = _rotation;
    }

    public void AddForceAndTorque()
    {
        _rigidbody.isKinematic = false;

        _rigidbody.AddForce(_direction * Random.Range(-force, force));

        _rigidbody.AddTorque(_direction * Random.Range(-force, force));
    }

    public void RecordFrame(RecordedFrame frame)
    {
        recordingData.recordedAnimation.Add(frame);
    }

    public void SpawnGhost() 
    {
        DestroyGhost();
        
        _spawnedGhost = Instantiate(ghostPrefab, transform.position, transform.rotation);
    }

    private void DestroyGhost() 
    {
        if (_spawnedGhost) 
        {
            Destroy(_spawnedGhost);
        }        
    }

    public int GetFaceValue()
    {
        var dieFace = faces.OrderByDescending((face) => face.transform.position.y).ToArray()[0];

        return dieFace.value;
    }

}
