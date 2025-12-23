using System;
using System.Windows.Input;

namespace RustControlPanel.ViewModels
{
    public class RelayCommand(Action execute) : ICommand
    {
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => execute();
        public event EventHandler? CanExecuteChanged;
    }
}