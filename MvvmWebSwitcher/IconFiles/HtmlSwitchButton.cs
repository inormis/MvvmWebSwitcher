using System.IO;
using MvvmWebSwitcher.Configurations;

namespace MvvmWebSwitcher.IconFiles {
    public class HtmlSwitchButton : SwitchButton {
        public HtmlSwitchButton(IDocumentContext context) : base(context, "Html.jpg") { }

        protected override string GetPathPattern(IConfiguration configuration) {
            var root = ConvertRootForPathPattern(configuration.HtmlRootPath);
            return $@"\\{root}((?<Module>\S*)\\)?(?<Controller>\S*)\\(?<Action>\S*)\.cshtml$";
        }

        protected override string GetFilePathFromPattern(FilePattern pattern, IConfiguration configuration) {
            return Path.Combine(pattern.RootDirectory,
                $@"{configuration.HtmlRootPath}{pattern.ModulePath}{pattern.Controller}\{pattern.Action}.cshtml");
        }
    }
}