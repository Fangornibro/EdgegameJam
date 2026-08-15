using System;
using UnityEngine;

namespace Features.LevelDesign.Scripts {
    public class CollisionTriggerReactor : MonoBehaviour {
        public event Action<Collider2D> OnTriggerEnterEvent;
        public event Action<Collider2D> OnTriggerExitEvent;

        private void OnTriggerEnter2D(Collider2D other) =>
            OnTriggerEnterEvent?.Invoke(other);

        private void OnTriggerExit2D(Collider2D other) =>
            OnTriggerExitEvent?.Invoke(other);
    }
}
