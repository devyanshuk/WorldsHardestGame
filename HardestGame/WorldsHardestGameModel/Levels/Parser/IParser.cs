using WorldsHardestGameModel.Environment;

namespace WorldsHardestGameModel.Levels.Parser
{
    public interface IParser
    {
        void ParseLevel(int level, IGameEnvironment environment);

        void ClearAll();
    }
}
