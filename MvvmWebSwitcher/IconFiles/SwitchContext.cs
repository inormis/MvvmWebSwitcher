namespace MvvmWebSwitcher.IconFiles {
    public class SwitchContext {
        public SwitchContext(string filePath, FilePattern pattern = null) {
            FilePath = filePath;
            Pattern = pattern;
        }

        public string FilePath { get; }

        public FilePattern Pattern { get; }
    }
}