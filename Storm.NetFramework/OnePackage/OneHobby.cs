using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Storm.NetFramework
{
    class OneHobby : INotifyPropertyChanged
    {
        private string nameOfHobby;
        private int idHobby;

        public string NameOfHobby
        {
            get { return nameOfHobby; }
            set
            {
                nameOfHobby = value;
                OnPropertyChanged("NameOfHobby");
            }
        }

        public int IdHobby
        {
            get { return idHobby; }
            set
            {
                idHobby = value;
                OnPropertyChanged("IdHobby");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
