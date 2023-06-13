using UnityEngine;
using EmeraldAI;
using PLAYERTWO.PlatformerProject;
public class EntityHitboxEmeraldAI : EntityHitbox
{
    protected EmeraldAISystem m_emerald;


protected override void HandleCustomCollision(Collider other)
    {
        if (other.TryGetComponent(out m_emerald))
        {
            HandleRebound();
            HandlePushBack();
            m_emerald.Damage(damage, EmeraldAISystem.TargetType.AI);
        }
    }
}
