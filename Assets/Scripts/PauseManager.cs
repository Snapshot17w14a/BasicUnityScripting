using System.Collections.Generic;
using UnityEngine;
using System;

public class PauseManager : MonoBehaviour
{
    private static readonly List<Rigidbody> rigidbodies = new();
    private static readonly List<MonoBehaviour> scripts = new();
    private static readonly List<AudioSource> audioSources = new();
    private static readonly Dictionary<Rigidbody, (Vector3, Vector3)> velocities = new();

    [SerializeField] private GameObject pauseMenu;
    private static GameObject staticPauseMenu;

    public static Action<bool> OnPauseStateChanged;

    public static bool allowStateChange = true;

    public static bool IsPaused => isPaused;
    private static bool isPaused = false;

    public static void ToggleGameState(bool togglePauseCanvas = false)
    {
        if (allowStateChange)
        {
            SetGameState(!isPaused);
            if (togglePauseCanvas) staticPauseMenu.SetActive(isPaused);
        }
    }

    private void Start()
    {
        ResetLists();
        staticPauseMenu = pauseMenu;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ToggleGameState(true);
    }

    public static void SetGameState(bool pause)
    {
        if (!isPaused && pause)
        {
            isPaused = true;

            rigidbodies.AddRange(FindObjectsOfType<Rigidbody>());
            foreach (var rb in rigidbodies)
            {
                velocities.Add(rb, (rb.velocity, rb.angularVelocity));
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                if (rb.TryGetComponent<IPausable>(out IPausable pausable))
                {
                    ((MonoBehaviour)pausable).enabled = false;
                    scripts.Add((MonoBehaviour)pausable);
                }
            }
            audioSources.AddRange(FindObjectsOfType<AudioSource>());
            foreach (var audioSource in audioSources)
            {
                if (audioSource.clip.name == "Space Cadet") continue;
                audioSource.Pause();
            }

            OnPauseStateChanged?.Invoke(true);
        }

        else if (isPaused && !pause)
        {
            isPaused = false;

            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = false;
                rb.velocity = velocities[rb].Item1;
                rb.angularVelocity = velocities[rb].Item2;
            }
            foreach (var audioSource in audioSources) audioSource.UnPause();

            foreach (var script in scripts) script.enabled = true;

            ResetLists();

            OnPauseStateChanged?.Invoke(false);
        }
    }

    private static void ResetLists()
    {
        audioSources.Clear();
        rigidbodies.Clear();
        velocities.Clear();
        scripts.Clear();
    }
}
