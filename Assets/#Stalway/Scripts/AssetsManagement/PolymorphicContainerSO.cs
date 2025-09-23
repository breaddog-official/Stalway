namespace Breaddog.AssetsManagement
{
    public class PolymorphicContainerSO<TValue, T> : ContainerSO<T> where TValue : T
    {
        public TValue value;
        public override T Value => value;
    }
}