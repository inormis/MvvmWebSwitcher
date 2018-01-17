using System;

namespace MvvmWebSwitcher.Configurations {
    public interface IConfiguration {
        string CssRootPath { get; }

        string TSRootPath { get; }

        string CSharpRootPath { get; }

        string HtmlRootPath { get; }

        SwitchButtonsPosition Position { get; }

        event Action Changed;

        void Update(string csharpRootPath, string htmlRootPath, string cssRootPath, string ts, SwitchButtonsPosition position);
    }
}