using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SFX Library")]
public class SFXLibrary : ScriptableObject
{
    public AudioClip move;
    public AudioClip attack;
    public AudioClip item;
    public AudioClip pickup;
    public AudioClip die;
    public AudioClip win;
    public AudioClip lose;
}