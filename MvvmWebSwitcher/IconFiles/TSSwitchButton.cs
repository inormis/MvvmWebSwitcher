using System.IO;
using WebSwitcher.Configurations;

namespace WebSwitcher.IconFiles {
    public class TSSwitchButton : SwitchButton {
        public TSSwitchButton(IDocumentContext context) : base(context, "TS.jpg") { }

        protected override string GetPathPattern(IConfiguration configuration) {
            var root = ConvertRootForPathPattern(configuration.TSRootPath);
            return $@"\\{root}((?<Module>\S*)\\)?(?<Controller>\S*)\\(?<Action>\S*)\.ts$";
        }

        protected override string GetFilePathFromPattern(FilePattern pattern, IConfiguration configuration) {
            return Path.Combine(pattern.RootDirectory,
                $@"{configuration.TSRootPath}{pattern.ModulePath}{pattern.Controller}\{pattern.Action}.ts");
        }
    }
}