using Mirror;

namespace Breaddog.Gameplay
{
    public class Controller : NetworkBehaviour
    {
        public Entity Entity { get; private set; }

        public virtual void Init(Entity entity)
        {
            Entity = entity;
            Init();
        }

        public virtual void Init() { }
    }
}
