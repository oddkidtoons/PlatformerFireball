using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;
 
namespace OddKidAnimationEvents
{
    public class OddKidFeelBridge : MonoBehaviour
    {

     
        [SerializeField] MMFeedbacks[] feedbacks;

        [SerializeField] UnityEvent[] CustomEvents;
       

        [SerializeField] AudioClip[] PlayAudioClips;
        AudioSource aud;
        

        void Start()
        {
            aud = GetComponent<AudioSource>();
        }

     

        public void PlayFeedback(int index)
        {
            feedbacks[index]?.PlayFeedbacks();
            
           

        }
        public void PlayCustomEvent(int index)
        {
         
            
            CustomEvents[index].Invoke();

        }


        public void PlayClip(int index)
        {
            aud.PlayOneShot(PlayAudioClips[index]);
        }

    
    }


}
