using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NotepadRs4.ViewModels
{
    public class NotificationBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // SetField (Name, value); // where there is a data member
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string property = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            RaisePropertyChanged(property);
            return true;
        }

        // SetField(()=> somewhere.Name = value; somewhere.Name, value) // Advanced case where you rely on another property
        protected void SetProperty<T>(T currentValue, T newValue, Action doSet, [CallerMemberName] string property = null)
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue)) return;
            doSet.Invoke();
            RaisePropertyChanged(property);
        }

        // Update the content of the property by giving a signal to the property that it has changed
        protected void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public class NotificationBase<T> : NotificationBase where T : class, new()
    {
        protected readonly T This;
        public static implicit operator T(NotificationBase<T> thing) { return thing.This; }
        protected NotificationBase(T thing = null)
        {
            This = thing ?? new T();
        }
    }
}
