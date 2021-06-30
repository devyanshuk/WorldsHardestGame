using Castle.MicroKernel.Registration;

using WorldsHardestGameModel.Game;
using WorldsHardestGameModel.Environment;
using WorldsHardestGameModel.Levels.Parser;
using WorldsHardestGameModel.ConfigTemplates;

namespace WorldsHardestGameModel.DependencyRegistry
{
    public class DependencyRegistry
    {
        public static IRegistration[] GetRegistrations()
        {
            return new IRegistration[]
            {
                //LifestyleSingleton
                Component.For<IParser>().ImplementedBy<Parser>().LifestyleSingleton(),
                Component.For<IGameLogic>().ImplementedBy<GameLogic>().LifestyleSingleton(),
                Component.For<ILocalSettings>().ImplementedBy<LocalSettings>().LifestyleSingleton(),
                Component.For<IGameEnvironment>().ImplementedBy<GameEnvironment>().LifestyleSingleton()
            };
        }
    }
}
