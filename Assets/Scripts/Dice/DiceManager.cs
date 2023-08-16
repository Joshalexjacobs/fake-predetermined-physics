using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{

    [SerializeField]
    private Transform dieSpawnPoint;

    [SerializeField]
    private Die diePrefab;

    private readonly List<Die> _dice = new();

    [SerializeField]
    private int diceToRoll = 1;

    [SerializeField]
    private int total;

    [SerializeField]
    private Text totalText;

    private Coroutine _playback;

    private void Start()
    {
        for (int i = 0; i < diceToRoll; i++)
        {
            var spawnRange = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

            _dice.Add(Instantiate(diePrefab, dieSpawnPoint.position + spawnRange, Quaternion.identity, transform));
        }

    }

    private const int Steps = 500;

    public void SimulateRoll()
    {
        total = 0;

        if (_playback != null)
        {
            StopCoroutine(_playback);

            _playback = null;
        }

        Physics.simulationMode = SimulationMode.Script;

        foreach (var die in _dice)
        {
            die.Reset();
            
            die.EnablePhysics();

            die.AddForceAndTorque();
        }

        for (var i = 0; i < Steps; i++)
        {
            foreach (var die in _dice) {
                var frame = new RecordedFrame(
                    die.transform.position,
                    die.transform.rotation);

                die.RecordFrame(frame);
            }

            Physics.Simulate(Time.fixedDeltaTime);
        }

        var faces = new List<int>();

        foreach (var die in _dice)
        {
            faces.Add(die.GetFaceValue());

            total += die.GetFaceValue();

            totalText.text = total.ToString();

            die.SpawnGhost();
        }

        Physics.simulationMode = SimulationMode.FixedUpdate;
    }

    private void DisablePhysics()
    {
        foreach (var die in _dice) {
            die.DisablePhysics();
        }
    }

    private void ResetToInitialState()
    {
        foreach (var die in _dice) {
            die.ResetToInitialState();
        }
    }

    public void PlayRecording()
    {
        if (_playback == null)
        {
            _playback = StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        DisablePhysics();

        ResetToInitialState();
        
        for (int i = 0; i < Steps; i += 2)
        {
            foreach (var die in _dice) {
                var position = die.recordingData.recordedAnimation[i].position;

                var rotation = die.recordingData.recordedAnimation[i].rotation;

                die.transform.position = position;

                die.transform.rotation = rotation;
            }

            yield return new WaitForFixedUpdate();
        }

        _playback = null;
    }
}

[System.Serializable]
public struct RecordedFrame
{
    public Vector3 position;
    public Quaternion rotation;

    public RecordedFrame(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}

[System.Serializable]
public class RecordingData
{
    public List<RecordedFrame> recordedAnimation;

    public RecordingData()
    {
        recordedAnimation = new List<RecordedFrame>();
    }

    public void Clear() {
        recordedAnimation.Clear();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DiceManager))]
public class DiceManagerEditor : Editor {

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    DiceManager diceManager = (DiceManager)target;

    if (GUILayout.Button("Simulate Roll")) {
        diceManager.SimulateRoll();
    }

    if (GUILayout.Button("Playback Roll")) {
        diceManager.PlayRecording();
    }

  }
}
#endif
