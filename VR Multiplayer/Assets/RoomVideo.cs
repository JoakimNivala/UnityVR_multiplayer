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
        //[SerializeField] bool m_AutoPlay;
        [SerializeField] VideoPlayer m_VideoPlayer;  // VideoPlayer component
        [SerializeField] VideoClip[] m_VideoClips;
        [SerializeField] AudioSource m_AudioSource;
        [SerializeField] Slider m_TimelineSlider;
        
      
        //[SerializeField] Toggle m_PlayPauseToggle;
        //[SerializeField] Toggle m_ShuffleToggle;
        [SerializeField] Button m_NextButton;
        [SerializeField] Button m_PreviousButton;
        //[SerializeField] Image m_PlayPauseImage;
        [SerializeField] Sprite m_PauseSprite;
        [SerializeField] Sprite m_PlaySprite;
        [SerializeField] Slider  m_VolumeSlider;
        [SerializeField] readonly NetworkVariable<float> m_VolumeSliderVal = new NetworkVariable<float>(0.50f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        [SerializeField] readonly NetworkVariable<int> m_CurrentVideoIdNetworked = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        [SerializeField] readonly NetworkVariable<bool> m_IsPlayingNetworked = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        [SerializeField] readonly NetworkVariable<float> m_CurrentVideoTime = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        bool m_Shuffle;
        

        void Start()
        {
            // Ensure VideoPlayer is assigned
            if (m_VideoPlayer == null)
            {
                Debug.LogError("VideoPlayer component is missing!");
                enabled = false;
                return;
            }

            //m_Dropdown.ClearOptions();

            //if (m_VideoClips == null || m_VideoClips.Length == 0)
            //{
            //    Debug.LogWarning("No video clips found. Please add some video clips.");
            //    enabled = false;
            //    return;
            //}

            //foreach (var clip in m_VideoClips)
            //{
            //    m_Dropdown.options.Add(new TMP_Dropdown.OptionData(clip.name));
            //}

            m_VideoPlayer.loopPointReached += OnVideoEnd; // Handle video ending

            //if (m_AutoPlay)
            //{
            //    PickRandomVideo();
            //}
          
            SetupUIListeners();
            //UpdateVideoTitleText();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_VolumeSlider.value = m_VolumeSliderVal.Value;
            m_AudioSource.volume = m_VolumeSliderVal.Value;
            m_CurrentVideoIdNetworked.OnValueChanged += CurrentVideoUpdated;
            m_IsPlayingNetworked.OnValueChanged += OnIsPlayingChanged;
            m_CurrentVideoTime.OnValueChanged += (oldValue, newValue) => m_VideoPlayer.time = newValue; m_IsPlayingNetworked.OnValueChanged += OnIsPlayingChanged;

            if (IsServer)
            {
                
                m_CurrentVideoTime.Value = (float)m_VideoPlayer.time;
                //m_IsPlayingNetworked.Value = m_AutoPlay;
                //if (!m_AutoPlay)
                //{
                //    m_CurrentVideoIdNetworked.Value = 0;
                //    SetClipTime(0.0f);
                //    m_TimelineSlider.SetValueWithoutNotify(0.0f);
                //}
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
            //m_PlayPauseToggle.onValueChanged.AddListener(TogglePlay);
            //m_ShuffleToggle.onValueChanged.AddListener(ToggleShuffle);
            m_NextButton.onClick.AddListener(() => PickNewVideo(1));
            m_PreviousButton.onClick.AddListener(() => PickNewVideo(-1));
        }

        void RemoveUIListeners()
        {
            m_TimelineSlider.onValueChanged.RemoveListener(SetClipTime);
            //m_PlayPauseToggle.onValueChanged.RemoveListener(TogglePlay);
            //m_ShuffleToggle.onValueChanged.RemoveListener(ToggleShuffle);
            m_NextButton.onClick.RemoveListener(() => PickNewVideo(1));
            m_PreviousButton.onClick.RemoveListener(() => PickNewVideo(-1));
        }

        void ToggleShuffle(bool toggle) => m_Shuffle = toggle;

        void TogglePlay(bool toggle)
        {
            if (IsServer)
            {
                m_IsPlayingNetworked.Value = toggle;
            }
        }

        void PickNewVideo(int dir = 1)
        {
            if (!IsServer)
                return;

            if (m_Shuffle)
            {
                PickRandomVideo();
            }
            else
            {
                int nextVideoId = Utils.RealMod(m_CurrentVideoIdNetworked.Value + dir, m_VideoClips.Length);
                Debug.Log(nextVideoId);
                PickVideo(nextVideoId);
            }
        }

        void PickRandomVideo()
        {
            int randomVideoId = m_CurrentVideoIdNetworked.Value;
            for (int i = 0; i < 10; i++)
            {
                int newId = Random.Range(0, m_VideoClips.Length);
                if (newId != randomVideoId)
                {
                    randomVideoId = newId;
                    break;
                }
            }
            PickVideo(randomVideoId);
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
                    //m_PlayPauseImage.sprite = m_PauseSprite;
                    //m_PlayPauseToggle.SetIsOnWithoutNotify(true);
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

        void OnIsPlayingChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                m_VideoPlayer.Play();
                //m_PlayPauseImage.sprite = m_PauseSprite;
            }
            else
            {
                m_VideoPlayer.Pause();
                //m_PlayPauseImage.sprite = m_PlaySprite;
            }

            //m_PlayPauseToggle.SetIsOnWithoutNotify(newValue);
        }

        void SetClipTime(float value) => m_VideoPlayer.time = Mathf.Clamp(value, 0.01f, 0.99f) * m_VideoPlayer.length;
        void SetVolumeVal(float value)
        {
            m_VolumeSliderVal.Value = value;
            UpdateVolume(m_VolumeSliderVal.Value);

        }
        void OnVideoEnd(VideoPlayer vp) => PickNewVideo();

       

       

       
        

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

       

    }
}
