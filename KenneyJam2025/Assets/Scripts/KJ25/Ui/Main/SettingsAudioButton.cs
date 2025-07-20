using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsAudioButton : MonoBehaviour {
   private const float OFF_VOLUME = -80;
   private const float ON_VOLUME = 0;

   [SerializeField] private AudioMixerGroup _mixerGroup;
   [SerializeField] private string _volumeName = "Volume";
   [SerializeField] private Button _button;
   [SerializeField] private Image _iconImage;
   [SerializeField] private Sprite _onSprite;
   [SerializeField] private Sprite _offSprite;

   private bool IsOn => _mixerGroup.audioMixer.GetFloat(_volumeName, out var volume) && Mathf.Approximately(ON_VOLUME, volume);

   private void Start() {
      _button.onClick.AddListener(HandleButtonClicked);
   }

   private void HandleButtonClicked() {
      _mixerGroup.audioMixer.SetFloat(_volumeName, IsOn ? OFF_VOLUME : ON_VOLUME);
      RefreshIcon();
   }

   private void RefreshIcon() {
      _iconImage.sprite = IsOn ? _onSprite : _offSprite;
   }
}