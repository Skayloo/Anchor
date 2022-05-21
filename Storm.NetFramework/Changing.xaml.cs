using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace Storm.NetFramework
{
    /// <summary>
    /// Логика взаимодействия для Changing.xaml
    /// </summary>

    public partial class Changing : Window
    {
        int idForeigner = 0;
        ObservableCollection<Data> oc_Data = new ObservableCollection<Data>();
        ObservableCollection<OneRecord> oc_Nationality = new ObservableCollection<OneRecord>();
        ObservableCollection<OneRecord> oc_Embassy = new ObservableCollection<OneRecord>();
        ObservableCollection<OneRecord> oc_Purpose = new ObservableCollection<OneRecord>();
        ObservableCollection<OneRecord> oc_Language = new ObservableCollection<OneRecord>();
        ObservableCollection<OneRecord> oc_Gender = new ObservableCollection<OneRecord>();
        ObservableCollection<OneRecord> oc_Hobby = new ObservableCollection<OneRecord>();
        ObservableCollection<OneRecord> oc_DocumentType = new ObservableCollection<OneRecord>();
        ObservableCollection<OneRecord> oc_HostCountry = new ObservableCollection<OneRecord>();

        ObservableCollection<OneDocument> oc_DocumentAddOrEdit = new ObservableCollection<OneDocument>();
        ObservableCollection<OneLanguage> oc_LanguageAddOrDelete = new ObservableCollection<OneLanguage>();
        ObservableCollection<OneHobby> oc_HobbyAddOrDelete = new ObservableCollection<OneHobby>();
        ObservableCollection<OneWorkplace> oc_WorkplaceAddOrEdit = new ObservableCollection<OneWorkplace>();
        ObservableCollection<OneProject> oc_ProjectAddOrEdit = new ObservableCollection<OneProject>();
        ObservableCollection<OneFile> oc_FileAddOrEdit = new ObservableCollection<OneFile>();

        public Changing(object chan)
        {
            InitializeComponent();
            BindingDictionaries();
            BindingComboBoxes("Все");
            DataGridAddOrEditDocument.ItemsSource = oc_DocumentAddOrEdit;
            DataGridAddOrDeleteLanguage.ItemsSource = oc_LanguageAddOrDelete;
            DataGridAddOrDeleteHobby.ItemsSource = oc_HobbyAddOrDelete;
            DataGridAddOrDeleteWorkplace.ItemsSource = oc_WorkplaceAddOrEdit;
            DataGridAddOrDeleteProject.ItemsSource = oc_ProjectAddOrEdit;
            DataGridAddOrDeleteFile.ItemsSource = oc_FileAddOrEdit;
            GetIno(chan);
        }
        private void GetIno(object ino)
        {
            try
            {
                string ss = ino.GetType().GetProperty("Id_foreigner").GetValue(ino, null).ToString();
                int idshnik = int.Parse(ss);
                GetFor(idshnik);
                idForeigner = idshnik;
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void GetFor(int idshnik)
        {
            ContextDataContext db = new ContextDataContext();
            var selectedForeigner = db.foreigners.Where(w => w.id_foreigner == idshnik).FirstOrDefault();
            Name.Text = selectedForeigner.name_of_foreigner;

            if (selectedForeigner.genderid_gender != null)
                GenderComboBox.SelectedValue = selectedForeigner.genderid_gender;
            else
                GenderComboBox.SelectedValue = -1;

            if (selectedForeigner.birth_date != null)
                DateOfBirth.SelectedDate = selectedForeigner.birth_date;

            if (selectedForeigner.nationalityid_nationality != null)
                NationalComboBox.SelectedValue = selectedForeigner.nationalityid_nationality;
            else
                NationalComboBox.SelectedValue = -1;


            var DocumentType = (from F in db.foreigners
                                join ID in db.identity_document on F.id_foreigner equals ID.foreignersid_foreigner
                                join DT in db.document_type on ID.document_typeid_doctype equals DT.id_doctype
                                where F.id_foreigner == idshnik
                                select new { ID.date_of_issue, ID.document_typeid_doctype, ID.serial_number_of_document, ID.validity_period, DT.name_of_document_type }).ToList();

            for (int i = 0; i < DocumentType.Count; i++)
            {

                oc_DocumentAddOrEdit.Add(new OneDocument
                {
                    ID = Guid.NewGuid().ToString(),
                    DateFrom = DocumentType[i].date_of_issue,
                    DateTo = DocumentType[i].validity_period,
                    Serial = DocumentType[i].serial_number_of_document.ToString(),
                    IdDocumentType = DocumentType[i].document_typeid_doctype,
                    DocumentType = DocumentType[i].name_of_document_type.ToString()
                });
            }

            Blood.Text = selectedForeigner.blood_tipe;

            Weight.Text = selectedForeigner.weight_kg_.ToString();

            Health.Text = selectedForeigner.health;

            MaritalStatus.Text = selectedForeigner.marital_status;

            Children.Text = selectedForeigner.number_of_children;

            NationalDay.Text = selectedForeigner.national_day;

            CriminalProsecution.Text = selectedForeigner.criminal_prosecution_in_the_country_of_permanent_residence;

            MilitaryConflicts.Text = selectedForeigner.participant_in_military_conflicts;

            Remarks.Text = selectedForeigner.remarks;

            var educationForeigner = (from F in db.foreigners
                                      join EF in db.education_foreigners on F.id_foreigner equals EF.id_foreigner
                                      join E in db.education on EF.id_education equals E.id_education
                                      where F.id_foreigner == idshnik
                                      select new { E.place_of_study, E.faculty, E.training_level }).FirstOrDefault();

            if (educationForeigner != null)
            {
                University.Text = educationForeigner.place_of_study;
                Faculty.Text = educationForeigner.faculty;
                EducationStatus.Text = educationForeigner.training_level;
            }

            var LanguagesForeigner = (from F in db.foreigners
                                      join LF in db.languages_foreigners on F.id_foreigner equals LF.id_foreigner
                                      join L in db.languages on LF.id_language equals L.id_language
                                      where F.id_foreigner == idshnik
                                      select new { L.name_of_language, L.id_language }).ToList();

            for (int i = 0; i < LanguagesForeigner.Count; i++)
            {
                oc_LanguageAddOrDelete.Add(new OneLanguage
                {
                    Language = LanguagesForeigner[i].name_of_language,
                    IdLanguage = LanguagesForeigner[i].id_language
                });
            }


            var ArrivalForeigner = (from F in db.foreigners
                                    join AF in db.arrival_foreigners on F.id_foreigner equals AF.id_foreigner
                                    join A in db.arrival on AF.id_arrival equals A.id_arrival
                                    where F.id_foreigner == idshnik
                                    select new { A.date_of_arrival, A.host_countryid_host_country, A.place_of_stay, A.purposeid_purpose }).FirstOrDefault();

            if (ArrivalForeigner != null)
            {
                ArrivalDate.SelectedDate = ArrivalForeigner.date_of_arrival;
                HostCountryComboBox.SelectedValue = ArrivalForeigner.host_countryid_host_country;
                if (ArrivalForeigner.purposeid_purpose != null)
                {
                    PurposeOfStayComboBox.SelectedValue = ArrivalForeigner.purposeid_purpose;
                }
                HostPlace.Text = ArrivalForeigner.place_of_stay;
            }


            var HobbyForeigner = (from F in db.foreigners
                                  join HF in db.hobby_foreigners on F.id_foreigner equals HF.id_foreigner
                                  join H in db.hobby on HF.id_hobby equals H.id_hobby
                                  where F.id_foreigner == idshnik
                                  select new { H.id_hobby, H.name_of_hobby }).ToList();

            for (int i = 0; i < HobbyForeigner.Count; i++)
            {
                oc_HobbyAddOrDelete.Add(new OneHobby
                {
                    NameOfHobby = HobbyForeigner[i].name_of_hobby,
                    IdHobby = HobbyForeigner[i].id_hobby
                });
            }

            var WorkplaceForeigner = (from F in db.foreigners
                                      join WF in db.workplace_foreigners on F.id_foreigner equals WF.id_foreigner
                                      join W in db.workplace on WF.id_workplace equals W.id_workplace
                                      where F.id_foreigner == idshnik
                                      select new
                                      { W.name_of_company, W.embassyid_embassy, W.division_department_direction, W.position, W.military_rank, W.office_hours, W.number_fax, W.email, W.residential_address, W.id_workplace }).ToList();

            for (int i = 0; i < WorkplaceForeigner.Count; i++)
            {
                string embassyComboBoxItemToFillOC = null;
                if (WorkplaceForeigner[i].embassyid_embassy == null)
                {
                    EmbassyComboBox.SelectedValue = -1;
                    embassyComboBoxItemToFillOC = EmbassyComboBox.Text;
                }
                else
                {
                    int IdEmbassy = (int)WorkplaceForeigner[i].embassyid_embassy;
                    var DataEmbassy = db.embassy.ToList();
                    string EmbassyIdForeigner = (from E in db.embassy
                                                 where E.id_embassy == IdEmbassy
                                                 select new { E.name_of_embassy }).FirstOrDefault().name_of_embassy;
                    embassyComboBoxItemToFillOC = EmbassyIdForeigner;
                }
                oc_WorkplaceAddOrEdit.Add(new OneWorkplace
                {
                    ID = Guid.NewGuid().ToString(),
                    IdWorkplace = WorkplaceForeigner[i].id_workplace,
                    NameOfCompany = WorkplaceForeigner[i].name_of_company,
                    EmbassyIdEmbassy = WorkplaceForeigner[i].embassyid_embassy,
                    Embassy = embassyComboBoxItemToFillOC,
                    Department = WorkplaceForeigner[i].division_department_direction,
                    Position = WorkplaceForeigner[i].position,
                    Rank = WorkplaceForeigner[i].military_rank,
                    Hours = WorkplaceForeigner[i].office_hours,
                    Telephone = WorkplaceForeigner[i].number_fax,
                    Email = WorkplaceForeigner[i].email,
                    Address = WorkplaceForeigner[i].residential_address,
                });

                FindWorkplaces();
            }

            var NameOfProjectForeigner = (from F in db.foreigners
                                          join PF in db.project_foreigners on F.id_foreigner equals PF.id_foreigner
                                          join P in db.project on PF.id_project equals P.id_project
                                          join W in db.workplace on P.id_workplace equals W.id_workplace into AllProjects
                                          where F.id_foreigner == idshnik
                                          from prgs in AllProjects.DefaultIfEmpty()
                                          select new { prgs, P.name_of_project, P.nature_description_of_project, P.project_details, P.id_workplace }).ToList();


            foreach (var currNameOfProjectForeigner in NameOfProjectForeigner)
            {
                oc_ProjectAddOrEdit.Add(new OneProject
                {
                    ID = Guid.NewGuid().ToString(),
                    NameOfProject = currNameOfProjectForeigner.name_of_project,
                    Workplace = currNameOfProjectForeigner.prgs == null ? null : currNameOfProjectForeigner.prgs.name_of_company,
                    Description = currNameOfProjectForeigner.nature_description_of_project,
                    ProjectDetails = currNameOfProjectForeigner.project_details,
                    IdWorkplace = currNameOfProjectForeigner.id_workplace
                });
            }


            var FilesForeigner = (from F in db.foreigners
                                  join FF in db.files_foreigners on F.id_foreigner equals FF.id_foreigner
                                  join FI in db.files on FF.id_file equals FI.id_file
                                  where F.id_foreigner == idshnik
                                  select new { FI.address }).ToList();

            for (int i = 0; i < FilesForeigner.Count; i++)
            {
                oc_FileAddOrEdit.Add(new OneFile
                {
                    FileName = FilesForeigner[i].address,
                    IdFile = Guid.NewGuid().ToString()
                });
            }
            
        }

        private void ButtonUpdateForeigner_Click(object sender, RoutedEventArgs e)
        {
            ContextDataContext db = new ContextDataContext();
            try 
            {
                foreigners updateforeigner = db.foreigners.FirstOrDefault(p => p.id_foreigner == idForeigner);
                updateforeigner.name_of_foreigner = Name.Text;
                updateforeigner.birth_date = DateOfBirth.SelectedDate;
                updateforeigner.blood_tipe = Blood.Text;
                if (NationalComboBox.SelectedValue.ToString() != null && NationalComboBox.SelectedValue.ToString() != "-1")
                {
                    updateforeigner.nationalityid_nationality = (int?)NationalComboBox.SelectedValue;
                }
                if (GenderComboBox.SelectedValue.ToString() != "-1")
                {
                    updateforeigner.genderid_gender = (int?)GenderComboBox.SelectedValue;
                }
                updateforeigner.health = Health.Text;
                if (Weight.Text != "")
                {
                    updateforeigner.weight_kg_ = int.Parse(Weight.Text);
                }
                updateforeigner.marital_status = MaritalStatus.Text;
                updateforeigner.number_of_children = Children.Text;
                updateforeigner.national_day = NationalDay.Text;
                updateforeigner.criminal_prosecution_in_the_country_of_permanent_residence = CriminalProsecution.Text;
                updateforeigner.participant_in_military_conflicts = MilitaryConflicts.Text;
                updateforeigner.remarks = Remarks.Text;
                db.SubmitChanges();

                List<int> docId = db.identity_document.Where(w => w.foreignersid_foreigner == idForeigner).Select(s => s.id_document).ToList();
                foreach (int currDocId in docId)
                {
                    var delDocId = db.identity_document.Where(w => w.id_document == currDocId).FirstOrDefault();
                    db.identity_document.DeleteOnSubmit(delDocId);
                    db.SubmitChanges();
                }

                foreach (var data in oc_DocumentAddOrEdit)
                {
                    identity_document updateDocForeigner = new identity_document();
                    updateDocForeigner.date_of_issue = data.DateFrom;
                    updateDocForeigner.validity_period = data.DateTo;
                    updateDocForeigner.foreignersid_foreigner = updateforeigner.id_foreigner;
                    updateDocForeigner.document_typeid_doctype = data.IdDocumentType;
                    updateDocForeigner.serial_number_of_document = data.Serial;
                    db.identity_document.InsertOnSubmit(updateDocForeigner);
                    db.SubmitChanges();
                }

                List<int> langForeigner = db.languages_foreigners.Where(w => w.id_foreigner == idForeigner).Select(s => s.id_language).ToList();
                foreach (int currLangForeigner in langForeigner)
                {
                    var delLangFor = db.languages_foreigners.Where(w => w.id_language == currLangForeigner).FirstOrDefault();
                    db.languages_foreigners.DeleteOnSubmit(delLangFor);
                    db.SubmitChanges();
                }

                foreach (var data in oc_LanguageAddOrDelete)
                {
                    languages_foreigners updatelanguagesforeigners = new languages_foreigners();
                    updatelanguagesforeigners.id_language = data.IdLanguage;
                    updatelanguagesforeigners.id_foreigner = updateforeigner.id_foreigner;
                    db.languages_foreigners.InsertOnSubmit(updatelanguagesforeigners);
                    db.SubmitChanges();
                }

                int arrivalForeigner = db.arrival_foreigners.Where(w => w.id_foreigner == idForeigner).Select(s => s.id_arrival).FirstOrDefault();
                arrival updatearrival = db.arrival.FirstOrDefault(w => w.id_arrival == arrivalForeigner);
                updatearrival.date_of_arrival = ArrivalDate.SelectedDate;
                updatearrival.host_countryid_host_country = (int?)HostCountryComboBox.SelectedValue == -1 ? null : (int?)HostCountryComboBox.SelectedValue;
                updatearrival.place_of_stay = HostPlace.Text;
                updatearrival.purposeid_purpose = (int?)PurposeOfStayComboBox.SelectedValue == -1 ? null : (int?)PurposeOfStayComboBox.SelectedValue;
                db.SubmitChanges();


                List<int> hobbyforeigner = db.hobby_foreigners.Where(w => w.id_foreigner == idForeigner).Select(s => s.id_hobby).ToList();
                foreach (int currHobbyForeigner in hobbyforeigner)
                {
                    var delHobbyFor = db.hobby_foreigners.Where(w => w.id_hobby == currHobbyForeigner).FirstOrDefault();
                    db.hobby_foreigners.DeleteOnSubmit(delHobbyFor);
                    db.SubmitChanges();
                }

                foreach (var data in oc_HobbyAddOrDelete)
                {
                    hobby_foreigners updatehobbyForeigners = new hobby_foreigners();
                    updatehobbyForeigners.id_hobby = data.IdHobby;
                    updatehobbyForeigners.id_foreigner = updateforeigner.id_foreigner;
                    db.hobby_foreigners.InsertOnSubmit(updatehobbyForeigners);
                    db.SubmitChanges();
                }

                List<int> educationToForeigner = db.education_foreigners.Where(w => w.id_foreigner == idForeigner).Select(s => s.id_education).ToList();
                foreach (var educ in educationToForeigner)
                {
                    var delEducationForeigner = db.education_foreigners.Where(w => w.id_education == educ).FirstOrDefault();
                    var idDeleteEducation = db.education.Where(w => w.id_education == educ).FirstOrDefault();
                    db.education_foreigners.DeleteOnSubmit(delEducationForeigner);
                    db.education.DeleteOnSubmit(idDeleteEducation);
                    db.SubmitChanges();
                }


                education updateeducation = new education();
                updateeducation.place_of_study = University.Text;
                updateeducation.faculty = Faculty.Text;
                updateeducation.training_level = EducationStatus.Text;

                db.education.InsertOnSubmit(updateeducation);
                db.SubmitChanges();

                education_foreigners updateeducationForeigners = new education_foreigners();
                updateeducationForeigners.id_education = updateeducation.id_education;
                updateeducationForeigners.id_foreigner = updateforeigner.id_foreigner;
                db.education_foreigners.InsertOnSubmit(updateeducationForeigners);
                db.SubmitChanges();


                List<int> projectForeigner = db.project_foreigners.Where(w => w.id_foreigner == idForeigner).Select(s => s.id_project).ToList();
                foreach (var proj in projectForeigner)
                {
                    var delProjectForeigner = db.project_foreigners.Where(w => w.id_project == proj).FirstOrDefault();
                    var idDeleteProject = db.project.Where(w => w.id_project == proj).FirstOrDefault();
                    db.project_foreigners.DeleteOnSubmit(delProjectForeigner);
                    db.project.DeleteOnSubmit(idDeleteProject);
                    db.SubmitChanges();
                }

                List<int> workplaceForeigner = db.workplace_foreigners.Where(w => w.id_foreigner == idForeigner).Select(s => s.id_workplace).ToList();
                foreach (var work in workplaceForeigner)
                {
                    var delWorkplaceForeigner = db.workplace_foreigners.Where(w => w.id_workplace == work).FirstOrDefault();
                    var idDeleteWorkplace = db.workplace.Where(w => w.id_workplace == work).FirstOrDefault();
                    db.workplace_foreigners.DeleteOnSubmit(delWorkplaceForeigner);
                    db.workplace.DeleteOnSubmit(idDeleteWorkplace);
                    db.SubmitChanges();
                }

                foreach (var datawork in oc_WorkplaceAddOrEdit)
                {
                    workplace updateworkplace = new workplace();
                    updateworkplace.name_of_company = datawork.NameOfCompany;
                    if (datawork.EmbassyIdEmbassy.ToString() != "-1")
                    {
                        updateworkplace.embassyid_embassy = datawork.EmbassyIdEmbassy;
                    }
                    updateworkplace.division_department_direction = datawork.Department;
                    updateworkplace.position = datawork.Position;
                    updateworkplace.military_rank = datawork.Rank;
                    updateworkplace.office_hours = datawork.Hours;
                    updateworkplace.number_fax = datawork.Telephone;
                    updateworkplace.email = datawork.Email;
                    updateworkplace.residential_address = datawork.Address;
                    updateworkplace.id_arrival = updatearrival.id_arrival;
                    db.workplace.InsertOnSubmit(updateworkplace);
                    db.SubmitChanges();

                    workplace_foreigners newWorkplaceForeigners = new workplace_foreigners();
                    newWorkplaceForeigners.id_foreigner = updateforeigner.id_foreigner;
                    newWorkplaceForeigners.id_workplace = updateworkplace.id_workplace;
                    db.workplace_foreigners.InsertOnSubmit(newWorkplaceForeigners);
                    db.SubmitChanges();

                    foreach (var dataproject in oc_ProjectAddOrEdit)
                    {
                        if (dataproject.IdWorkplace == datawork.IdWorkplace || dataproject.Uuid == datawork.ID)
                        {
                            project newproject = new project();
                            newproject.name_of_project = dataproject.NameOfProject;
                            newproject.nature_description_of_project = dataproject.Description;
                            newproject.project_details = dataproject.ProjectDetails;
                            newproject.id_workplace = updateworkplace.id_workplace;
                            db.project.InsertOnSubmit(newproject);
                            db.SubmitChanges();

                            project_foreigners newProjectForeigners = new project_foreigners();
                            newProjectForeigners.id_foreigner = updateforeigner.id_foreigner;
                            newProjectForeigners.id_project = newproject.id_project;
                            db.project_foreigners.InsertOnSubmit(newProjectForeigners);
                            db.SubmitChanges();
                        }
                    }
                }

                foreach (var dataproject in oc_ProjectAddOrEdit)
                {
                    if (dataproject.IdWorkplace == null && dataproject.Uuid == null || dataproject.Uuid == "-1")
                    {
                        project newproject = new project();
                        newproject.name_of_project = dataproject.NameOfProject;
                        newproject.nature_description_of_project = dataproject.Description;
                        newproject.project_details = dataproject.ProjectDetails;
                        newproject.id_workplace = null;
                        db.project.InsertOnSubmit(newproject);
                        db.SubmitChanges();

                        project_foreigners newProjectForeigners = new project_foreigners();
                        newProjectForeigners.id_foreigner = updateforeigner.id_foreigner;
                        newProjectForeigners.id_project = newproject.id_project;
                        db.project_foreigners.InsertOnSubmit(newProjectForeigners);
                        db.SubmitChanges();
                    }
                }

                var FilesForeigner = (from F in db.foreigners
                                      join FF in db.files_foreigners on F.id_foreigner equals FF.id_foreigner
                                      join FI in db.files on FF.id_file equals FI.id_file
                                      where F.id_foreigner == idForeigner
                                      select new { FI.address, FI.id_file }).ToList();

                foreach(var file in FilesForeigner)
                {
                    if (!oc_FileAddOrEdit.Contains(oc_FileAddOrEdit.Where(w => w.FileName == file.address).FirstOrDefault()))
                    {
                        var delFilesForeigner = db.files_foreigners.Where(w => w.id_file == file.id_file).FirstOrDefault();
                        var idDeleteFile = db.files.Where(w => w.id_file == file.id_file).FirstOrDefault();
                        db.files_foreigners.DeleteOnSubmit(delFilesForeigner);
                        db.files.DeleteOnSubmit(idDeleteFile);
                        db.SubmitChanges();
                    }
                }
                
                foreach(var datafile in oc_FileAddOrEdit)
                {
                    if(!FilesForeigner.Contains(FilesForeigner.Where(w => w.address == datafile.FileName).FirstOrDefault()))
                    {
                        FileStream InoFiles = new FileStream(datafile.FileName, FileMode.Open);
                        BinaryReader binRead = new BinaryReader(InoFiles);
                        byte[] fileContent;
                        fileContent = new byte[InoFiles.Length];
                        for (int i = 0; i < InoFiles.Length; i++)
                        {
                            fileContent[i] = binRead.ReadByte();
                        }
                        binRead.Close();
                        InoFiles.Close();

                        files newfiles = new files();
                        newfiles.documents = fileContent;
                        newfiles.address = Path.GetFileName(datafile.FileName);
                        db.files.InsertOnSubmit(newfiles);
                        db.SubmitChanges();

                        files_foreigners newfiles_foreigners = new files_foreigners();
                        newfiles_foreigners.id_file = newfiles.id_file;
                        newfiles_foreigners.id_foreigner = updateforeigner.id_foreigner;
                        db.files_foreigners.InsertOnSubmit(newfiles_foreigners);
                        db.SubmitChanges();
                    }
                }


                System.Windows.MessageBox.Show("Изменения внесены", "Редактирование профиля объекта", MessageBoxButton.OK, MessageBoxImage.Asterisk);

            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Dictionaries

        private void BindingComboBoxes(string how)
        {
            try
            {

                if (how == "Национальность" || how == "Все")
                {
                    Dictionary<int, string> NationalityFilter = new Dictionary<int, string>
                            {
                                { -1, "--- Все ---" }
                            };
                    foreach (var currNationality in oc_Nationality)
                        NationalityFilter.Add(currNationality.ID, currNationality.Term);

                    NationalComboBox.DataContext = NationalityFilter;
                    NationalComboBox.DisplayMemberPath = "Value";
                    NationalComboBox.SelectedValuePath = "Key";
                }

                if (how == "Посольство" || how == "Все")
                {
                    Dictionary<int, string> Embassy = new Dictionary<int, string>
                    {
                        { -1, "--- Не выбран ---" }
                    };
                    foreach (var currEmbassy in oc_Embassy)
                        Embassy.Add(currEmbassy.ID, currEmbassy.Term);
                    EmbassyComboBox.DataContext = Embassy;
                    EmbassyComboBox.DisplayMemberPath = "Value";
                    EmbassyComboBox.SelectedValuePath = "Key";
                }

                if (how == "Цель прибытия" || how == "Все")
                {
                    Dictionary<int, string> PurposeFilter = new Dictionary<int, string>
                            {
                                { -1, "--- Все ---" }
                            };
                    foreach (var currPurpose in oc_Purpose)
                        PurposeFilter.Add(currPurpose.ID, currPurpose.Term);

                    PurposeOfStayComboBox.DataContext = PurposeFilter;
                    PurposeOfStayComboBox.DisplayMemberPath = "Value";
                    PurposeOfStayComboBox.SelectedValuePath = "Key";
                }

                if (how == "Владение языками" || how == "Все")
                {
                    Dictionary<int, string> Language = new Dictionary<int, string>
                            {
                                { -1, "--- Не выбран ---" }
                            };
                    foreach (var currLanguage in oc_Language)
                        Language.Add(currLanguage.ID, currLanguage.Term);

                    ComboBoxLanguage.DataContext = Language;
                    ComboBoxLanguage.DisplayMemberPath = "Value";
                    ComboBoxLanguage.SelectedValuePath = "Key";
                }

                if (how == "Пол" || how == "Все")
                {
                    Dictionary<int, string> Gender = new Dictionary<int, string>
                            {
                                { -1, "--- Не выбран ---" }
                            };
                    foreach (var currGender in oc_Gender)
                        Gender.Add(currGender.ID, currGender.Term);

                    GenderComboBox.DataContext = Gender;
                    GenderComboBox.DisplayMemberPath = "Value";
                    GenderComboBox.SelectedValuePath = "Key";
                }

                if (how == "Хобби" || how == "Все")
                {
                    Dictionary<int, string> Hobby = new Dictionary<int, string>
                            {
                                { -1, "--- Не выбран ---" }
                            };
                    foreach (var currHobby in oc_Hobby)
                        Hobby.Add(currHobby.ID, currHobby.Term);
                    ComboBoxHobby.DataContext = Hobby;
                    ComboBoxHobby.DisplayMemberPath = "Value";
                    ComboBoxHobby.SelectedValuePath = "Key";
                }

                if (how == "Тип документа" || how == "Все")
                {
                    Dictionary<int, string> DocumentType = new Dictionary<int, string>
                            {
                                { -1, "--- Не выбран ---" }
                            };
                    foreach (var currDocumentType in oc_DocumentType)
                        DocumentType.Add(currDocumentType.ID, currDocumentType.Term);

                    ComboBoxDocumentType.DataContext = DocumentType;
                    ComboBoxDocumentType.DisplayMemberPath = "Value";
                    ComboBoxDocumentType.SelectedValuePath = "Key";

                }

                if (how == "Страна пребывания" || how == "Все")
                {

                    Dictionary<int, string> HostCountryFilter = new Dictionary<int, string>
                            {
                                { -1, "--- Все ---" }
                            };
                    foreach (var currHostCountry in oc_HostCountry)
                        HostCountryFilter.Add(currHostCountry.ID, currHostCountry.Term);

                    HostCountryComboBox.DataContext = HostCountryFilter;
                    HostCountryComboBox.DisplayMemberPath = "Value";
                    HostCountryComboBox.SelectedValuePath = "Key";
                }

            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BindingDictionaries()
        {
            ContextDataContext db = new ContextDataContext();

            var Nationality = from N in db.nationality orderby N.name_of_nationality select new { Term = N.name_of_nationality, ID = N.id_nationality };
            foreach (var currNationality in Nationality)
                oc_Nationality.Add(new OneRecord { ID = currNationality.ID, Term = currNationality.Term });

            var Embassy = from E in db.embassy orderby E.name_of_embassy select new { Term = E.name_of_embassy, ID = E.id_embassy };
            foreach (var currEmbassy in Embassy)
                oc_Embassy.Add(new OneRecord { ID = currEmbassy.ID, Term = currEmbassy.Term });

            var Purpose = from P in db.purpose orderby P.name_of_purpose select new { Term = P.name_of_purpose, ID = P.id_purpose };
            foreach (var currPurpose in Purpose)
                oc_Purpose.Add(new OneRecord { ID = currPurpose.ID, Term = currPurpose.Term });

            var Language = from L in db.languages orderby L.name_of_language select new { Term = L.name_of_language, ID = L.id_language };
            foreach (var currLanguage in Language)
                oc_Language.Add(new OneRecord { ID = currLanguage.ID, Term = currLanguage.Term });

            var Gender = from G in db.gender orderby G.name_of_gender select new { Term = G.name_of_gender, ID = G.id_gender };
            foreach (var currGender in Gender)
                oc_Gender.Add(new OneRecord { ID = currGender.ID, Term = currGender.Term });

            var Hobby = from H in db.hobby orderby H.name_of_hobby select new { Term = H.name_of_hobby, ID = H.id_hobby };
            foreach (var currHobby in Hobby)
                oc_Hobby.Add(new OneRecord { ID = currHobby.ID, Term = currHobby.Term });

            var DocumentType = from DT in db.document_type orderby DT.name_of_document_type select new { Term = DT.name_of_document_type, ID = DT.id_doctype };
            foreach (var currDocumentType in DocumentType)
                oc_DocumentType.Add(new OneRecord { ID = currDocumentType.ID, Term = currDocumentType.Term });

            var HostCountry = from HC in db.host_country orderby HC.name_of_country select new { Term = HC.name_of_country, ID = HC.id_host_country };
            foreach (var currHostCountry in HostCountry)
                oc_HostCountry.Add(new OneRecord { ID = currHostCountry.ID, Term = currHostCountry.Term });
        }

        #endregion

        #region Identity

        //private void DatePickerDocumentFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (ComboBoxDocumentType.SelectedValue != null && (int)ComboBoxDocumentType.SelectedValue != -1 && !string.IsNullOrWhiteSpace(TextBoxSerialNumberOfDocumentAddOrEdit.Text) && DatePickerDocumentTo.SelectedDate != null && DatePickerDocumentFrom.SelectedDate != null)
        //            ButtonDocumentAdd.IsEnabled = true;
        //        else
        //            ButtonDocumentAdd.IsEnabled = false;
        //    }
        //    catch (Exception error)
        //    {
        //        System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private void ComboBoxDocumentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboBoxDocumentType.SelectedIndex != -1 && ComboBoxDocumentType.SelectedIndex != 0)
                    ButtonDocumentAdd.IsEnabled = true;
                else
                    ButtonDocumentAdd.IsEnabled = false;
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //private void TextBoxSerialNumberOfDocumentAddOrEdit_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (ComboBoxDocumentType.SelectedValue != null && (int)ComboBoxDocumentType.SelectedValue != -1 && !string.IsNullOrWhiteSpace(TextBoxSerialNumberOfDocumentAddOrEdit.Text) && DatePickerDocumentTo.SelectedDate != null && DatePickerDocumentFrom.SelectedDate != null)
        //            ButtonDocumentAdd.IsEnabled = true;
        //        else
        //            ButtonDocumentAdd.IsEnabled = false;
        //    }
        //    catch (Exception error)
        //    {
        //        System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void DatePickerDocumentTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (ComboBoxDocumentType.SelectedValue != null && (int)ComboBoxDocumentType.SelectedValue != -1 && !string.IsNullOrWhiteSpace(TextBoxSerialNumberOfDocumentAddOrEdit.Text) && DatePickerDocumentTo.SelectedDate != null && DatePickerDocumentFrom.SelectedDate != null)
        //            ButtonDocumentAdd.IsEnabled = true;
        //        else
        //            ButtonDocumentAdd.IsEnabled = false;
        //    }
        //    catch (Exception error)
        //    {
        //        System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private void DeleteRecordInDocumentAddOrEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is System.Windows.Controls.Button btnDel)
                {
                    if (System.Windows.MessageBox.Show("Вы подтверждаете удаление выбранного удостоверения личности?", "Редактирование удостоверения личности", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string id = btnDel.Tag.ToString();
                        if (id != null)
                            oc_DocumentAddOrEdit.Remove(oc_DocumentAddOrEdit.First(s => s.ID == id));
                    }
                }
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonDocumentAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (oc_DocumentAddOrEdit.Where(s => s.IdDocumentType == (int)ComboBoxDocumentType.SelectedValue && s.DateFrom == DatePickerDocumentFrom.SelectedDate && s.DateTo == DatePickerDocumentTo.SelectedDate && s.Serial == TextBoxSerialNumberOfDocumentAddOrEdit.Text.Trim()).Count() == 0)
                {

                    oc_DocumentAddOrEdit.Add(new OneDocument
                    {
                        ID = Guid.NewGuid().ToString(),
                        DateFrom = DatePickerDocumentFrom.SelectedDate,
                        DateTo = DatePickerDocumentTo.SelectedDate,
                        Serial = TextBoxSerialNumberOfDocumentAddOrEdit.Text.Trim(),
                        DocumentType = ComboBoxDocumentType.Text,
                        IdDocumentType = (int)ComboBoxDocumentType.SelectedValue
                    });
                    ButtonDocumentAdd.IsEnabled = false;
                    ComboBoxDocumentType.SelectedIndex = 0;
                    DatePickerDocumentFrom.SelectedDate = null;
                    DatePickerDocumentTo.SelectedDate = null;
                    TextBoxSerialNumberOfDocumentAddOrEdit.Text = null;
                }
                else
                    System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить автора, который \nуже содержится в списке авторов этого документа!", "Добавление нового автора", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Hobby

        private void ComboBoxHobby_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboBoxHobby.SelectedValue != null && (int)ComboBoxHobby.SelectedValue != -1)
                    ButtonHobbyAdd.IsEnabled = true;
                else
                    ButtonHobbyAdd.IsEnabled = false;
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteRecordInHobbyAddOrDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is System.Windows.Controls.Button btnDel)
                {
                    if (System.Windows.MessageBox.Show("Вы подтверждаете удаление выбранного хобби?", "Редактирование списка хобби", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string id = btnDel.Tag.ToString();
                        if (id != null)
                        {
                            oc_HobbyAddOrDelete.Remove(oc_HobbyAddOrDelete.First(s => s.IdHobby.ToString() == id));
                        }
                    }
                }

            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonHobbyAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (oc_HobbyAddOrDelete.Where(s => s.IdHobby == (int)ComboBoxHobby.SelectedValue).Count() == 0)
                {
                    oc_HobbyAddOrDelete.Add(new OneHobby { NameOfHobby = ComboBoxHobby.Text, IdHobby = (int)ComboBoxHobby.SelectedValue });
                    ButtonHobbyAdd.IsEnabled = false;
                    ComboBoxHobby.SelectedIndex = 0;
                }
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Language

        private void ComboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboBoxLanguage.SelectedValue != null && (int)ComboBoxLanguage.SelectedValue != -1)
                    ButtonLanguageAdd.IsEnabled = true;
                else
                    ButtonLanguageAdd.IsEnabled = false;
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteRecordInLanguagesAddOrDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is System.Windows.Controls.Button btnDel)
                {
                    if (System.Windows.MessageBox.Show("Вы подтверждаете удаление выбранного иностранного языка?", "Редактирование иностранных языков", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string id = btnDel.Tag.ToString();
                        if (id != null)
                        {
                            oc_LanguageAddOrDelete.Remove(oc_LanguageAddOrDelete.First(s => s.IdLanguage.ToString() == id));
                        }
                    }
                }

            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonLanguageAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (oc_LanguageAddOrDelete.Where(s => s.IdLanguage == (int)ComboBoxLanguage.SelectedValue).Count() == 0)
                {
                    oc_LanguageAddOrDelete.Add(new OneLanguage { Language = ComboBoxLanguage.Text, IdLanguage = (int)ComboBoxLanguage.SelectedValue });
                    ButtonLanguageAdd.IsEnabled = false;
                    ComboBoxLanguage.SelectedIndex = 0;
                }
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Workplaces

        private void ButtonWorkplaceAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                oc_WorkplaceAddOrEdit.Add(new OneWorkplace
                {
                    ID = Guid.NewGuid().ToString(),
                    NameOfCompany = NameOfCompany.Text,
                    EmbassyIdEmbassy = (int?)EmbassyComboBox.SelectedValue,
                    Embassy = EmbassyComboBox.Text,
                    Department = Department.Text,
                    Position = Post.Text,
                    Rank = Rank.Text,
                    Hours = WorkingHours.Text,
                    Telephone = Telephone.Text,
                    Email = Email.Text,
                    Address = Address.Text
                });
                EmbassyComboBox.SelectedIndex = 0;
                NameOfCompany.Clear();
                Department.Clear();
                Post.Clear();
                Rank.Clear();
                WorkingHours.Clear();
                Telephone.Clear();
                Email.Clear();
                Address.Clear();
                FindWorkplaces();
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteRecordInWorkplacesAddOrDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is System.Windows.Controls.Button btnDel)
                {
                    if (System.Windows.MessageBox.Show("Вы подтверждаете удаление выбранного места работы?", "Редактирование мест работы", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string id = btnDel.Tag.ToString();
                        if (id != null)
                            oc_WorkplaceAddOrEdit.Remove(oc_WorkplaceAddOrEdit.First(s => s.ID == id));
                        FindWorkplaces();
                    }
                }
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Projects

        private void DeleteRecordInProjectAddOrDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btnDel)
            {
                if (System.Windows.MessageBox.Show("Вы подтверждаете удаление выбранного проекта?", "Редактирование списка проектов", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    string id = btnDel.Tag.ToString();
                    if (id != null)
                        oc_ProjectAddOrEdit.Remove(oc_ProjectAddOrEdit.First(s => s.ID == id));
                }
            }
        }
        private void ButtonProjectAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                oc_ProjectAddOrEdit.Add(new OneProject
                {
                    ID = Guid.NewGuid().ToString(),
                    Workplace = WorkplaceChosenComboBox.SelectedValue == null ? null : WorkplaceChosenComboBox.Text,
                    Uuid = (string)(WorkplaceChosenComboBox.SelectedValue == null ? null : WorkplaceChosenComboBox.SelectedValue),
                    NameOfProject = NameOfProject.Text,
                    Description = ProjectDescription.Text,
                    ProjectDetails = ProjectDetails.Text
                });

                WorkplaceChosenComboBox.SelectedIndex = 0;
                NameOfProject.Clear();
                ProjectDescription.Clear();
                ProjectDetails.Clear();
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FindWorkplaces()
        {
            Dictionary<string, string> WorkplaceFilter = new Dictionary<string, string>
                            {
                                { "-1", "--- Не выбрано ---" }
                            };
            foreach (var currWorkplace in oc_WorkplaceAddOrEdit)
                WorkplaceFilter.Add(currWorkplace.ID, currWorkplace.NameOfCompany);

            WorkplaceChosenComboBox.DataContext = WorkplaceFilter;
            WorkplaceChosenComboBox.DisplayMemberPath = "Value";
            WorkplaceChosenComboBox.SelectedValuePath = "Key";

        }

        #endregion

        #region Files
        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Multiselect = true;
            DialogResult result = fd.ShowDialog();

            if (result.ToString().Equals("OK"))
            {
                String[] OutcomFiles = fd.FileNames;
                foreach (var ss in OutcomFiles)
                {
                    oc_FileAddOrEdit.Add(new OneFile
                    {
                        FileName = ss,
                        IdFile = Guid.NewGuid().ToString()
                    });
                }
            }
        }

        private void DeleteRecordInFilesAddOrDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is System.Windows.Controls.Button btnDel)
                {
                    if (System.Windows.MessageBox.Show("Вы подтверждаете удаление выбранного файла?", "Редактирование файлов иностранца", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string id = btnDel.Tag.ToString();
                        if (id != null)
                        {
                            oc_FileAddOrEdit.Remove(oc_FileAddOrEdit.First(s => s.IdFile.ToString() == id));
                        }
                    }
                }

            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region SomeTrash

        private void WeightTextInput(object sender, TextCompositionEventArgs e)
        {
            int val;
            if (!Int32.TryParse(e.Text, out val))
            {
                e.Handled = true;
            }
        }

        private void Weight_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        #endregion
    }
}
