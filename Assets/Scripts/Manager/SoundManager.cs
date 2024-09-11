using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string soundName; //곡의 이름
    public AudioClip clip; //곡
}
public class SoundManager : Singleton<SoundManager>
{
    
    [Header("오디오클립")]
    [SerializeField] 
    public Sound[] bgmSounds;
    [SerializeField]
    public Sound[] sfxSounds;

    [Header("브금플레이어")]
    [SerializeField] 
    public AudioSource bgmPlayer;

    [Header("효과음플레이어")]
    [SerializeField] 
    public AudioSource[] sfxPlayer;
    public string[]playSoundName;

    void Start()
    {
        PlayRandomBGM();
    }

    public void PlaySE(string _soundName) //soundName 사운드를 재생한다.
    {
        for (int i = 0; i < sfxSounds.Length; i++) //sfxSounds 배열 길이만큼 반복
        {
            if (_soundName == sfxSounds[i].soundName) //만약 soundName과 일치하는 이름이 sfxsound 배열 안에 존재한다면,
            {

                for (int x = 0; x < sfxPlayer.Length; x++)
                {
                    if (!sfxPlayer[x].isPlaying) // 만약 sfxPlayer 오디오소스가 재생중이지 않다면
                    {
                        playSoundName[x] = sfxSounds[i].soundName; //재생중인 사운드를 playSoundName에 저장(재생중인 사운드 중지에 사용)
                        sfxPlayer[x].clip = sfxSounds[i].clip; //sfxSounds의 클립을 sfxPlayer에 넣고
                        sfxPlayer[x].Play(); //재생한다.
                        return;
                    }
                }
                Debug.Log("모든 sfxPlayer오디오소스가 사용중입니다."); // 재생중이라면 디버그
                return;
            }
        }
        Debug.Log(_soundName + "사운드가 SoundManager에 등록되지 않았습니다.");
}
public void StopAllSE() //모든 사운드 중지
{
    for(int i =0; i < sfxPlayer.Length; i++)
    sfxPlayer[i].Stop();
}

public void StopSE(string _soundName) //특정 사운드만 중지(playSoundName)
{
    for(int i =0; i < sfxPlayer.Length; i++)
    {
        if(playSoundName[i] == _soundName)
        {
          sfxPlayer[i].Stop();
          return;
        }
    }
    Debug.Log("재생 중인" + _soundName + "이 없습니다.");
}
    public void PlayRandomBGM() {
        int random = Random.Range(0, 2);
        bgmPlayer.clip = bgmSounds[random].clip;
        bgmPlayer.Play();
        //Database.Item.Get(0);
    }


}