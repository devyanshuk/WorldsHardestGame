using WorldsHardestGameModel.Environment;

namespace WorldsHardestGameModel.Game
{
    public interface IGameLogic
    {
        IGameEnvironment gameEnvironment { get; set; }

        int level { get; set; }

        int fails { get; }

        void InitializeGameEnvironment();

        void UpdateEntityStates();
    }
}
