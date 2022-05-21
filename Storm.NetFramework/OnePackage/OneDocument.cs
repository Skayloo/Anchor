using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Storm.NetFramework
{
    public class OneDocument : INotifyPropertyChanged
    {
        private string id;
        private string documentType;
        private int idDocumentType;
        private DateTime? dateFrom;
        private DateTime? dateTo;
        private string serial;

        public DateTime? DateFrom
        {
            get { return dateFrom; }
            set
            {
                dateFrom = value;
                OnPropertyChanged("DateFrom");
            }
        }

        public DateTime? DateTo
        {
            get { return dateTo; }
            set
            {
                dateTo = value;
                OnPropertyChanged("DateTo");
            }
        }

        public string DocumentType
        {
            get { return documentType; }
            set
            {
                documentType = value;
                OnPropertyChanged("DocumentType");
            }
        }

        public string Serial
        {
            get { return serial; }
            set
            {
                serial = value;
                OnPropertyChanged("Serial");
            }
        }

        public int IdDocumentType
        {
            get { return idDocumentType; }
            set
            {
                idDocumentType = value;
                OnPropertyChanged("IdDocumentType");
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
