using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public bool sound = true;
    private AudioClip[] sfx;
    private AudioSource audioSource;
    private AudioSource audioSourceHit;
    private bool playHitFlag = true;
    

    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        audioSourceHit = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey("sound"))
        {
            sound = Convert.ToBoolean(PlayerPrefs.GetInt("sound"));
            SetBtnSoundSprite();
        }
        
    }

    public void SoundTrigger()
    {
        sound = !sound;
        PlayerPrefs.SetInt("sound", Convert.ToInt32(sound));

        SetBtnSoundSprite();
    }

    public void SetBtnSoundSprite()
    {
        if (GetComponent<GameController>())
        {
            GetComponent<GameController>().btnSound.GetComponent<Image>().sprite =
                Resources.Load<Sprite>("Sprites/Sound" + Convert.ToInt32(sound));
        }
    }
    
    public void PlaySound(string soundName , float volume)
    {
        sfx = Resources.LoadAll<AudioClip>("Sfx").OfType<AudioClip>().ToArray();
        
        if (sound)
        {
            if (soundName != "Hit" )
            {
                foreach (var sound in sfx)
                {
                    if (sound.name == soundName)
                    {
                        audioSource.PlayOneShot(sound, volume);
                    }
                    
                }
            }
            else if (!audioSourceHit.isPlaying)
            {
                foreach (var sound in sfx)
                {
                    if (sound.name == soundName && playHitFlag)
                    {
                        audioSourceHit.PlayOneShot(sound, volume);
                        playHitFlag = false;
                        LeanTween.delayedCall(0.5f, () => { playHitFlag = true; });
                    }
                    
                }
            }
        }
        
    }
}
