public static class SaveManagerSwitcher
{
    private static GameData _testerData = null;
    private static bool _useTester = false;

    // Call this once when you load tester save
    public static void SetTesterSave(GameData testerData)
    {
        _testerData = testerData;
        _useTester = _testerData != null;
    }

    // Call this to disable tester save and go back to main
    public static void DisableTesterSave()
    {
        _testerData = null;
        _useTester = false;
    }

    // Returns the active GameData instance (main or tester)
    public static GameData CurrentData
    {
        get
        {
            return _useTester && _testerData != null ? _testerData : GameData.Instance;
        }
    }
}