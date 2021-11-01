using UnityEngine;

public class SongManager : MonoBehaviour
{
    public AudioClip[] Songs;

    private AudioSource audioSource;
    private const float TIME_BETWEEN_SONGS = 15f;
    private int songIndex;
    private float nextSongTime;

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        songIndex = Random.Range(0, Songs.Length);
        NextSong();
    }

    void Update()
    {
        if (Time.time > nextSongTime)
        {
            NextSong();
        }
    }

    private void NextSong()
    {
        songIndex += 1;
        songIndex = songIndex % Songs.Length;
        audioSource.clip = Songs[songIndex];
        audioSource.Play();
        nextSongTime = Time.time + Songs[songIndex].length + TIME_BETWEEN_SONGS;
    }
}