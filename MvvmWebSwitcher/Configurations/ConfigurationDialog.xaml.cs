namespace MvvmWebSwitcher.Configurations {
    public partial class ConfigurationDialog {
        public ConfigurationDialog() {
            InitializeComponent();
            DataContext = new ConfigurationViewModel();
        }
    }
}