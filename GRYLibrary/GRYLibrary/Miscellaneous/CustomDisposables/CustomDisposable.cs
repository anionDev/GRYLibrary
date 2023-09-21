using System;

namespace GRYLibrary.Core.Miscellaneous.CustomDisposables
{
    public class CustomDisposable : IDisposable
    {
        public Action DisposeAction { get; set; }
        public CustomDisposable() : this(() => { })
        {
        }
        public CustomDisposable(Action disposeAction)
        {
            this.DisposeAction = disposeAction;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.DisposeAction();
        }
    }
}