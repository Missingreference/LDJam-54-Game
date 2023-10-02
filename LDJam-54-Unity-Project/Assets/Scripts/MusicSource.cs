using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSource : MonoBehaviour
{
    AudioSource audioSource;
    AudioClip gameOverSong;
    AudioClip newAbilitySong;
    AudioClip pauseMenuSong;
    AudioClip gameplaySong1;
    AudioClip gameplaySong2;
    AudioClip gameplaySong3;
    AudioClip gameplaySong4;
    AudioClip gameplaySong5;
    AudioClip gameplaySong6;
    static public float volume = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        // get audio source
        audioSource = FindObjectOfType<AudioSource>();
        
        //get audio clips
        gameOverSong = Resources.Load<AudioClip>("Music/Game Over/Moonlit Forest Main Loop copy");
        newAbilitySong = Resources.Load<AudioClip>("Music/New Ability/Pretty Cave copy");
        pauseMenuSong = Resources.Load<AudioClip>("Music/Pause Menu/Pretty Dungeon LOOP copy");
        gameplaySong1 = Resources.Load<AudioClip>("Music/Gameplay/Assassins LOOP");
        gameplaySong2 = Resources.Load<AudioClip>("Music/Gameplay/Boss Battle 5 Loop");
        gameplaySong3 = Resources.Load<AudioClip>("Music/Gameplay/Dark Dungeon ACTION LOOP");
        gameplaySong4 = Resources.Load<AudioClip>("Music/Gameplay/Halloween copy");
        gameplaySong5 = Resources.Load<AudioClip>("Music/Gameplay/Hangar Escape LOOP copy");
        gameplaySong6 = Resources.Load<AudioClip>("Music/Gameplay/Space Station copy");

        availableSongs.Add(gameplaySong1);
        availableSongs.Add(gameplaySong2);
        availableSongs.Add(gameplaySong3);
        availableSongs.Add(gameplaySong4);
        availableSongs.Add(gameplaySong5);
        availableSongs.Add(gameplaySong6);
    }

    List<AudioClip> previouslyPlayedSongs = new List<AudioClip>();

    List<AudioClip> availableSongs = new List<AudioClip>();

    public void PickNextSong()
    {
        //audioSource.clip

        int randomNumber = Random.Range(0,availableSongs.Count);

        AudioClip nextClip = availableSongs[randomNumber];

        previouslyPlayedSongs.Add(nextClip);

        availableSongs.RemoveAt(randomNumber);

        audioSource.clip = nextClip;
        audioSource.Play();


        availableSongs.AddRange(previouslyPlayedSongs);
        previouslyPlayedSongs.Clear();




    }


    public void PlayGameOverMusic()
    {
        audioSource.Stop();
        audioSource.clip = gameOverSong;
        audioSource.Play();
        audioSource.loop = true;

    }

    // Update is called once per frame
    void Update()
    {
        
        if (!audioSource.isPlaying)
        {
            PickNextSong();
        }




    }
}
