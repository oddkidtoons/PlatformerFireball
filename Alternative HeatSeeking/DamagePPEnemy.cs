using UnityEngine;
using PLAYERTWO.PlatformerProject;

namespace PLAYERTWO.PlatformerProject
{
    [RequireComponent(typeof(AudioSource))]
    public class DamagePPEnemy : MonoBehaviour
    {
        [Header("Attack Settings")]
        public bool breakObjects;
        public int breakableDamage = 1;
        public int enemyDamageAmount = 1;

        private Collider m_collider;

        void Start()
        {
            InitializeCollider();
        }

        protected virtual void InitializeCollider()
        {
            m_collider = GetComponent<Collider>();
            if (m_collider != null)
            {
                m_collider.isTrigger = true;  // Ensure this is a trigger collider
            }
        }

        protected virtual void HandleCollision(Collider other)
        {
            if (other.TryGetComponent(out Breakable breakable))
            {
                HandleBreakableObject(breakable);
            }
        }

        protected virtual void HandleBreakableObject(Breakable breakable)
        {
            if (!breakObjects) return;
            breakable.ApplyDamage(breakableDamage);
        }

        protected virtual void HandleCustomCollision(Collider other) { }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                // Damage enemy logic
                if (other.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.ApplyDamage(enemyDamageAmount, transform.position);
                    HandleCustomCollision(other);
                }
            }

            HandleCollision(other);
        }
    }
}
