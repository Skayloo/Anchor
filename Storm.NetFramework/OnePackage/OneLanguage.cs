using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Storm.NetFramework
{
    class OneLanguage : INotifyPropertyChanged
    {
        private string language;
        private int idLanguage;

        public string Language
        {
            get { return language; }
            set
            {
                language = value;
                OnPropertyChanged("Language");
            }
        }

        public int IdLanguage
        {
            get { return idLanguage; }
            set
            {
                idLanguage = value;
                OnPropertyChanged("IdLanguage");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
