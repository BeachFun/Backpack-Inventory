using Zenject;

namespace BackpackInventory
{
    /// <summary>
    /// Класс предназначенный для регистрации не Monobehaviour классов в DI контейнере
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