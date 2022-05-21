using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Storm.NetFramework
{
    public class Data : INotifyPropertyChanged
    {
        private int id_foreigner;
        private string name_of_foreigner;
        private DateTime? birth_date;
        private string serial_number_of_document;
        private string host_country;
        private string gender;
        private string place_of_stay;

        public string Place_of_stay
        {
            get { return place_of_stay; }
            set
            {
                place_of_stay = value;
                OnPropertyChanged("Place_of_stay");
            }
        }

        public int Id_foreigner
        {
            get { return id_foreigner; }
            set
            {
                id_foreigner = value;
                OnPropertyChanged("Id_foreigner");
            }
        }


        public string Gender
        {
            get { return gender; }
            set
            {
                gender = value;
                OnPropertyChanged("Gender");
            }
        }

        public string Host_country
        {
            get { return host_country; }
            set
            {
                host_country = value;
                OnPropertyChanged("Host_country");
            }
        }
        public string Name_of_foreigner
        {
            get { return name_of_foreigner; }
            set
            {
                name_of_foreigner = value;
                OnPropertyChanged("Name_of_foreigner");
            }
        }

        public DateTime? Birth_date
        {
            get { return birth_date; }
            set
            {
                birth_date = value;
            }
        }

        public string Serial_number_of_document
        {
            get { return serial_number_of_document; }
            set
            {
                serial_number_of_document = value;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    
}
