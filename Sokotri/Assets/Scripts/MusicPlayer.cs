using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip music;

    [Range(0, 1)]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private float audioVolume;

    [SerializeField]
    private bool isMuted = false;

    [SerializeField]
    private TheGrid grid_ref;

    [SerializeField]
    private Player player;

    // Start is called before the first frame update
    private void Start()
    {
        if (grid_ref == null)
            grid_ref = GameObject.FindGameObjectWithTag("Grid").GetComponent<TheGrid>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        audioSource.volume = audioVolume;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void ToggleMuteSfx()
    {
        List<TriBox> triBoxes = UtilityTools.FindComponentsWithTag<TriBox>("TriBox");

        triBoxes.ForEach(tri => tri.ToggleMute());

        player.ToggleMute();
        grid_ref.ToggleMute();
    }

    public void ToggleMute()
    {
        MusicPlayer.ToggleMute(audioSource, ref isMuted, 1f, audioVolume);
    }

    public void ToggleMuteMusic()
    {
        ToggleMute();
    }

    public static void ToggleMute(AudioSource source, ref bool isMuted, float fadeTime, float normalVolume = 0.9f)
    {
        isMuted = !isMuted;

        if (!source) return;

        float newVolume = isMuted ? 0 : normalVolume;

        source.DOFade(newVolume, 1f);
    }
}