using System.IO;
using MvvmWebSwitcher.Configurations;

namespace MvvmWebSwitcher.IconFiles {
    public class CssSwitchButton : SwitchButton {
        public CssSwitchButton(IDocumentContext context) : base(context, "CSS.jpg") { }

        protected override string GetPathPattern(IConfiguration configuration) {
            var root = ConvertRootForPathPattern(configuration.CssRootPath);
            return $@"\\{root}((?<Module>\S*)\\)?(?<Controller>\S*)\\(?<Action>\S*)\.css$";
        }

        protected override string GetFilePathFromPattern(FilePattern pattern, IConfiguration configuration) {
            return Path.Combine(pattern.RootDirectory,
                $@"{configuration.CssRootPath}{pattern.ModulePath}{pattern.Controller}\{pattern.Action}.css");
        }
    }
}