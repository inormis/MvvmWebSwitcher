using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using MvvmWebSwitcher.Configurations;

namespace MvvmWebSwitcher {
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class MvvmWebSwitcherConfigureCommand {
        /// <summary>
        ///     Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        ///     Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("ff767a3d-4375-4315-8e7a-c566b070198f");

        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MvvmWebSwitcherConfigureCommand" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private MvvmWebSwitcherConfigureCommand(Package package) {
            if (package == null) throw new ArgumentNullException("package");

            this.package = package;

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null) {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static MvvmWebSwitcherConfigureCommand Instance { get; private set; }

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => package;

        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package) {
            Instance = new MvvmWebSwitcherConfigureCommand(package);
        }

        private void MenuItemCallback(object sender, EventArgs e) {
            new ConfigurationDialog().ShowDialog();
        }
    }
}