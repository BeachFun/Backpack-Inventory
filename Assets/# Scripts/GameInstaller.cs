using Zenject;

namespace BackpackInventory
{
    /// <summary>
    /// ����� ��������������� ��� ����������� �� Monobehaviour ������� � DI ����������
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Inventory>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<WebRequestDispatcher>().AsSingle().NonLazy();
        }
    }
}