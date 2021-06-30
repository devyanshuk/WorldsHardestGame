namespace WorldsHardestGameModel.ConfigTemplates
{
    public interface ILocalSettings
    {
        string LevelDir { get; }

        int MapWidth { get; }

        int MapHeight { get; }
    }
}
