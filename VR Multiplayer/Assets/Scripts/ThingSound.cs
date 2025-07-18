using UnityEngine;

public class ThingSound : MonoBehaviour
{
    private AudioClip glorp;
    [SerializeField] private AudioSource AudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        glorp = GetComponent<AudioClip>();
        AudioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Puck")
        {
            AudioSource.PlayOneShot(glorp);
        }
        else
        {
            Application.Quit();
        }
    }
}
