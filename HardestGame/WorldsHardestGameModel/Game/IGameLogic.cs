using WorldsHardestGameModel.Environment;

namespace WorldsHardestGameModel.Game
{
    public interface IGameLogic
    {
        IGameEnvironment gameEnvironment { get; }

        int level { get; }

        int coinsCollected { get; }

        int fails { get; }

        void InitializeGameEnvironment();

        void UpdateEntityStates();

        void OnFail();
    }
}
