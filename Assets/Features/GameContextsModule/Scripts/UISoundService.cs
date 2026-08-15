using UnityEngine;

namespace Features.GameContextsModule.Scripts {
    // The source lives in the global scene, so a click still finishes playing after the view
    // that triggered it is destroyed.
    public class UISoundService : IUISoundService {
        private readonly AudioSource _audioSource;
        private readonly AudioClip _clickClip;

        public UISoundService(AudioSource audioSource, AudioClip clickClip) {
            _audioSource = audioSource;
            _clickClip = clickClip;
        }

        public void PlayClick() {
            if (_audioSource == null || _clickClip == null)
                return;

            _audioSource.PlayOneShot(_clickClip);
        }
    }
}
