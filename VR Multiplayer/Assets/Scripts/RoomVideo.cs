using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Video;

namespace XRMultiplayer
{
    /// <summary>
    /// Controls the video playback in a room.
    /// </summary>
    public class RoomVideo : NetworkBehaviour
    {
        [SerializeField] VideoPlayer m_VideoPlayer;  // VideoPlayer component
        [SerializeField] VideoClip[] m_VideoClips;
        [SerializeField] AudioSource m_AudioSource;
        [SerializeField] Slider m_TimelineSlider;
        [SerializeField] Button m_PlayPauseToggle;
        [SerializeField] Button m_NextButton;   
        [SerializeField] Slider  m_VolumeSlider;
        [SerializeField] public readonly NetworkVariable<float> m_VolumeSliderVal = new NetworkVariable<float>(0.50f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        [SerializeField] public readonly NetworkVariable<int> m_CurrentVideoIdNetworked = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        [SerializeField] readonly NetworkVariable<bool> m_IsPlayingNetworked = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        [SerializeField] public readonly NetworkVariable<float> m_CurrentVideoTime = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        

        void Start()
        {
            // Ensure VideoPlayer is assigned
            if (m_VideoPlayer == null)
            {
                Debug.LogError("VideoPlayer component is missing!");
                enabled = false;
                return;
            }

        

            // Handle video ending

       
          
            SetupUIListeners();
           
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_VideoPlayer.loopPointReached += OnVideoEnd;
            m_VolumeSlider.value = m_VolumeSliderVal.Value;
            m_AudioSource.volume = m_VolumeSliderVal.Value;
            m_CurrentVideoIdNetworked.OnValueChanged += CurrentVideoUpdated;
            m_IsPlayingNetworked.OnValueChanged += OnIsPlayingChanged;
            m_CurrentVideoTime.OnValueChanged += (oldValue, newValue) => m_VideoPlayer.time = newValue; m_IsPlayingNetworked.OnValueChanged += OnIsPlayingChanged;

            if (IsServer)
            {
                
                m_CurrentVideoTime.Value = (float)m_VideoPlayer.time;
                
            }
            else
            {
                if (m_IsPlayingNetworked.Value)
                {
                    RequestCurrentVideoTimeServerRpc();
                   
                    SetVideo(m_CurrentVideoIdNetworked.Value);
                    
                }
                OnIsPlayingChanged(false, m_IsPlayingNetworked.Value);
            }

            CurrentVideoUpdated(0, m_CurrentVideoIdNetworked.Value);
        }

        private void Update()
        {
           
            if (!m_IsPlayingNetworked.Value || !NetworkManager.Singleton.IsConnectedClient)
                return;

            float perc = (float)(m_VideoPlayer.time / m_VideoPlayer.length);
            m_TimelineSlider.SetValueWithoutNotify(perc);

            if (perc >= 0.999f)
            {
                PickNewVideo();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemoveUIListeners();
        }

        void SetupUIListeners()
        {
            m_VolumeSlider.onValueChanged.AddListener(SetVolumeVal);
            m_TimelineSlider.onValueChanged.AddListener(SetClipTime);
            m_NextButton.onClick.AddListener(() => PickNewVideo(1));
          
        }

        void RemoveUIListeners()
        {
            m_TimelineSlider.onValueChanged.RemoveListener(SetClipTime);
            m_NextButton.onClick.RemoveListener(() => PickNewVideo(1));
           
        }

     

       
       

        void PickNewVideo(int dir = 1)
        {
            if (!IsServer)
                return;

      
            else
            {
                int nextVideoId = Utils.RealMod(m_CurrentVideoIdNetworked.Value + dir, m_VideoClips.Length);
                Debug.Log(nextVideoId);
                PickVideo(nextVideoId);
            }
        }

  
        void UpdateVolume(float volume)
        {
            m_AudioSource.volume = volume;
        }
        void PickVideo(int videoId)
        {
            SetClipTime(0.0f);
            if (IsServer)
            {
                m_CurrentVideoIdNetworked.Value = videoId;
                m_IsPlayingNetworked.Value = true;
                UpdateVolume(m_VolumeSliderVal.Value);
            }
        }

        void CurrentVideoUpdated(int oldVideoId, int videoId)
        {
            if (videoId >= 0 && videoId < m_VideoClips.Length)
            {
                SetVideo(videoId);

                if (m_IsPlayingNetworked.Value)
                {
                    SetClipTime(0.0f);
                    m_VideoPlayer.Play();
                    
                }
            }
        }

        void SetVideo(int videoId)
        {
            m_VideoPlayer.clip = m_VideoClips[videoId];
            m_VideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            m_VideoPlayer.SetTargetAudioSource(0, m_AudioSource);
            m_VideoPlayer.EnableAudioTrack(0, true);
            m_VideoPlayer.Prepare();

        }

        public void OnIsPlayingChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                m_VideoPlayer.Play();
               
            }
            else
            {
                m_VideoPlayer.Pause();
             
            }

           
        }

        void SetClipTime(float value) => m_VideoPlayer.time = Mathf.Clamp(value, 0.01f, 0.99f) * m_VideoPlayer.length;
        void SetVolumeVal(float value)
        {
            m_VolumeSliderVal.Value = value;
            UpdateVolume(m_VolumeSliderVal.Value);

        }
        void OnVideoEnd(VideoPlayer vp) => PickNewVideo();





        public void ToggleState()
        {
            m_IsPlayingNetworked.Value = !m_IsPlayingNetworked.Value;
            SetCurrentStateRpc(m_IsPlayingNetworked.Value, !m_IsPlayingNetworked.Value);
        }


        [Rpc(SendTo.SpecifiedInParams)]
        void SendCurrentVideoTimeToClientRpc(float time, RpcParams rpcParams = default)
        {
            m_VideoPlayer.time = time;
        }

        [Rpc(SendTo.Server)]
        void RequestCurrentVideoTimeServerRpc(RpcParams rpcParams = default)
        {
            SendCurrentVideoTimeToClientRpc((float)m_VideoPlayer.time, RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));
        }

       
        [Rpc(SendTo.Server)]
        void SetCurrentStateRpc(bool oldValue, bool newValue)
        {
            oldValue = newValue;
            newValue = !newValue;
            OnIsPlayingChanged(oldValue, newValue);
        }

        

    }
}
