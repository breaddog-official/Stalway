namespace Breaddog.AssetsManagement
{
    public class BasicContainerSO<T> : ContainerSO<T>
    {
        public T value;
        public override T Value => value;
    }
}