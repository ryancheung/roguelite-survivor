using Arch.Core;
using Microsoft.Xna.Framework;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;

namespace RogueliteSurvivor.Systems
{
    public class AttackSpellCleanupSystem : ArchSystem, IUpdateSystem
    {
        public AttackSpellCleanupSystem(World world)
            : base(world, new QueryDescription()
                                .WithAny<Projectile, SingleTarget>())
        { }

        public void Update(GameTime gameTime, float totalElapsedTime)
        {

            world.Query(in query, (Entity entity, ref EntityStatus entityStatus) =>
            {
                if (entityStatus.State == State.Dead)
                {
                    world.Destroy(entity);
                }
            });
        }
    }
}
