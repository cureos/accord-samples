// Copyright (c) 2015 Anders Gustafsson, Cureos AB.
// All rights reserved.

namespace Clustering.GMM
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class Command : ICommand
    {
        #region FIELDS

        private readonly Action action;

        private readonly Func<bool> canExecute;

        #endregion

        #region CONSTRUCTORS

        public Command(Action action, Func<bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public Command(Action action)
            : this(action, () => true)
        {
        }

        #endregion

        #region EVENTS

        public event EventHandler CanExecuteChanged;

        #endregion

        #region METHODS

        public bool CanExecute(object parameter)
        {
            return this.canExecute();
        }

        public void Execute(object parameter)
        {
            this.action();
        }

        public void ChangeCanExecute(object sender)
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(sender, EventArgs.Empty);
            }
        }

        #endregion
    }
}
