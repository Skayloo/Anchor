using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Storm.NetFramework
{
    public class OneWorkplace : INotifyPropertyChanged
    {
        private string id;
        private string nameOfCompany;
        private string department;
        private string position;
        private string rank;
        private string hours;
        private string telephone;
        private string email;
        private string address;
        private int? embassyIdEmbassy;
        private string embassy;
        private int idWorkplace;

        public string NameOfCompany
        {
            get { return nameOfCompany; }
            set
            {
                nameOfCompany = value;
                OnPropertyChanged("NameOfCompany");
            }
        }

        public string Embassy
        {
            get { return embassy; }
            set
            {
                embassy = value;
                OnPropertyChanged("Embassy");
            }
        }

        public string Department
        {
            get { return department; }
            set
            {
                department = value;
                OnPropertyChanged("Department");
            }
        }

        public string Position
        {
            get { return position; }
            set
            {
                position = value;
                OnPropertyChanged("Position");
            }
        }
        public string Rank
        {
            get { return rank; }
            set
            {
                rank = value;
                OnPropertyChanged("Rank");
            }
        }
        public string Hours
        {
            get { return hours; }
            set
            {
                hours = value;
                OnPropertyChanged("Hours");
            }
        }
        public string Telephone
        {
            get { return telephone; }
            set
            {
                telephone = value;
                OnPropertyChanged("Telephone");
            }
        }
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }
        public int? EmbassyIdEmbassy
        {
            get { return embassyIdEmbassy; }
            set
            {
                embassyIdEmbassy = value;
                OnPropertyChanged("EmbassyIdEmbassy");
            }
        }

        public string ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        public int IdWorkplace
        {
            get { return idWorkplace; }
            set
            {
                idWorkplace = value;
                OnPropertyChanged("IdWorkplace");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
