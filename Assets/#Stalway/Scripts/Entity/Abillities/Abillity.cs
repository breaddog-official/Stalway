using Mirror;

namespace Breaddog.Gameplay
{
    public abstract class Abillity : NetworkBehaviour
    {
        public Entity Entity { get; private set; }

        public bool IsInit { get; protected set; }

        public virtual void Init(Entity entity)
        {
            Entity = entity;
            Init();
            IsInit = true;
        }

        public virtual void Init() { }
    }
}
