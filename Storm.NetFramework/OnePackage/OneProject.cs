using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Storm.NetFramework
{
    public class OneProject : INotifyPropertyChanged
    {
        private string id;
        private string nameOfProject;
        private string description;
        private string projectDetails;
        private string workplace;
        private int? idWorkplace;
        private string uuid;

        public string NameOfProject
        {
            get { return nameOfProject; }
            set
            {
                nameOfProject = value;
                OnPropertyChanged("NameOfProject");
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        public string ProjectDetails
        {
            get { return projectDetails; }
            set
            {
                projectDetails = value;
                OnPropertyChanged("ProjectDetails");
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

        public string Workplace
        {
            get { return workplace; }
            set
            {
                workplace = value;
                OnPropertyChanged("Workplace");
            }
        }

        public int? IdWorkplace
        {
            get { return idWorkplace; }
            set
            {
                idWorkplace = value;
                OnPropertyChanged("IdWorkplace");
            }
        }

        public string Uuid
        {
            get { return uuid; }
            set
            {
                uuid = value;
                OnPropertyChanged("Uuid");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
