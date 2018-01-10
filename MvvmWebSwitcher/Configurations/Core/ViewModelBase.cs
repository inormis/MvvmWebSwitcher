using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WebSwitcher.Configurations.Core {
    public abstract class ViewModelBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class DialogCloser {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached(
                "DialogResult",
                typeof(bool?),
                typeof(DialogCloser),
                new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e) {
            var window = d as Window;
            if (window != null)
                window.DialogResult = e.NewValue as bool?;
        }

        public static void SetDialogResult(Window target, bool? value) {
            target.SetValue(DialogResultProperty, value);
        }
    }
}