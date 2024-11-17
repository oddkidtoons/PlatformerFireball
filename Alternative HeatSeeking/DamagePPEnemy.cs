using UnityEngine;
using PLAYERTWO.PlatformerProject;

namespace PLAYERTWO.PlatformerProject
{
    [RequireComponent(typeof(SphereCollider))]
    public class DamagePPEnemy : MonoBehaviour
    {
        [Header("Attack Settings")]
        public bool breakObjects;
        public int breakableDamage = 1;
        public int enemyDamageAmount = 1;
        protected Collider m_collider;

        void Start()
        {
            InitializeCollider();
        }

        protected virtual void InitializeCollider()
        {
            m_collider = GetComponent<Collider>();
            m_collider.isTrigger = true; // Make this a trigger to detect enemies without physical interaction
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
            // This will only trigger on the PlatformerProject enemy
            if (other.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.ApplyDamage(enemyDamageAmount, transform.position);
                HandleCustomCollision(other);
            }
            HandleCollision(other);
        }
    }
}
