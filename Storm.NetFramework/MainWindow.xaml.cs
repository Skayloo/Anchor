using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using Storm.NetFramework;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using System.Windows.Forms;
using System.IO;
using System.Windows.Input;

namespace Storm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

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

        #region Window
        public MainWindow()
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
        }

        private GridLength _rememberWidth = new GridLength(500);

        private void Grid_Collapsed(object sender, RoutedEventArgs e)
        {
            _rememberWidth = GridColumnInput.Width;
            GridColumnInput.Width = GridLength.Auto;
            GridColumnInput.MinWidth = 43;
            GridSplitter.IsEnabled = false;
            ExpanderHeader.Text = "  ОТКРЫТЬ ВВОД / РЕДАКТИРОВАНИЕ";
        }

        private void Grid_Expanded(object sender, RoutedEventArgs e)
        {
            GridColumnInput.Width = _rememberWidth;
            GridSplitter.IsEnabled = true;
            ExpanderHeader.Text = "  CКРЫТЬ ВВОД / РЕДАКТИРОВАНИЕ";
            GridColumnInput.MinWidth = 500;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridColumnInput.MaxWidth = this.ActualWidth / 2;
            scrollView.Height = this.ActualHeight - 115;
        }

        #endregion

        #region Search Insert Remove Open Buttons

        private void ClearText()
        {
            Name.Clear();
            GenderComboBox.SelectedValue = -1;
            DateOfBirth.SelectedDate = default;
            NationalComboBox.SelectedValue = -1;
            Blood.SelectedValue = -1;
            Weight.Clear();
            Health.Clear();
            MaritalStatus.Clear();
            Children.Clear();
            NationalDay.Clear();
            CriminalProsecution.Clear();
            MilitaryConflicts.Clear();
            Remarks.Clear();
            University.Clear();
            Faculty.Clear();
            EducationStatus.Clear();
            ArrivalDate.SelectedDate = default;
            HostCountryComboBox.SelectedValue = -1;
            HostPlace.Clear();
            PurposeOfStayComboBox.SelectedValue = -1;
            NameOfCompany.Clear();
            Department.Clear();
            Post.Clear();
            Rank.Clear();
            WorkingHours.Clear();
            Telephone.Clear();
            Email.Clear();
            Address.Clear();
            NameOfProject.Clear();
            ProjectDescription.Clear();
            ProjectDetails.Clear();
            ComboBoxHobby.SelectedValue = -1;
            ComboBoxLanguage.SelectedValue = -1;
            NameOfProject.Clear();
            ProjectDescription.Clear();
            ProjectDetails.Clear();
        }

        private void SearchButton(object sender, RoutedEventArgs e)
        {
            oc_Data.Clear();

            ContextDataContext db = new ContextDataContext();
            var findForeigners = (from F in db.foreigners
                                  join AF in db.arrival_foreigners on F.id_foreigner equals AF.id_foreigner
                                  join A in db.arrival on AF.id_arrival equals A.id_arrival
                                  join H in db.host_country on A.host_countryid_host_country equals H.id_host_country into AllCountry
                                  from allcntr in AllCountry.DefaultIfEmpty()
                                  join G in db.gender on F.genderid_gender equals G.id_gender into AllGender
                                  from allgen in AllGender.DefaultIfEmpty()
                                  select new
                                  {
                                      F.name_of_foreigner,
                                      F.id_foreigner,
                                      F.birth_date,
                                      allcntr.name_of_country,
                                      allgen.name_of_gender,
                                      A.place_of_stay
                                  }).ToList();

            var identity = (from ID in db.identity_document
                            join F in db.foreigners on ID.foreignersid_foreigner equals F.id_foreigner
                            select new { ID.serial_number_of_document, ID.foreignersid_foreigner }).ToList();

            if (SearchTextBox.Text != "")
            {
                SqlConnection conn = new SqlConnection(db.Connection.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = @"SELECT [dbo].[foreigners].id_foreigner, RowRank.[Rank] FROM [dbo].[foreigners] INNER JOIN 
                              FREETEXTTABLE ([dbo].[foreigners], [name_of_foreigner], @searchText) as RowRank 
                              ON [dbo].[foreigners].id_foreigner = RowRank.[KEY]"
                };

                SqlParameter varSearchText = new SqlParameter("@searchText", SearchTextBox.Text);
                cmd.Connection = conn;
                SqlDataAdapter daSearch = new SqlDataAdapter(cmd);
                cmd.Parameters.Add(varSearchText);
                DataTable dtSearch = new System.Data.DataTable();

                daSearch.Fill(dtSearch);

                int[] id = new int[dtSearch.Rows.Count];

                for (int i = 0; i < dtSearch.Rows.Count; i++)
                    id[i] = (int)dtSearch.Rows[i].ItemArray[0];

                findForeigners = (from F in db.foreigners
                                  join AF in db.arrival_foreigners on F.id_foreigner equals AF.id_foreigner
                                  join A in db.arrival on AF.id_arrival equals A.id_arrival
                                  join H in db.host_country on A.host_countryid_host_country equals H.id_host_country into AllCountry
                                  from allcntr in AllCountry.DefaultIfEmpty()
                                  join G in db.gender on F.genderid_gender equals G.id_gender into AllGender
                                  from allgen in AllGender.DefaultIfEmpty()
                                  where id.Contains(F.id_foreigner)
                                  select new
                                  {
                                      F.name_of_foreigner,
                                      F.id_foreigner,
                                      F.birth_date,
                                      allcntr.name_of_country,
                                      allgen.name_of_gender,
                                      A.place_of_stay
                                  }).ToList();

                identity = (from ID in db.identity_document
                            join F in db.foreigners on ID.foreignersid_foreigner equals F.id_foreigner
                            where id.Contains(F.id_foreigner)
                            select new { ID.serial_number_of_document, ID.foreignersid_foreigner }).ToList();

                //var gridData = db.foreigners.ToList();

                //foreach (var currGridData in gridData)
                //{
                //    var av = (from F in db.foreigners
                //              join AF in db.arrival_foreigners on F.id_foreigner equals AF.id_foreigner
                //              join A in db.arrival on AF.id_arrival equals A.id_arrival
                //              join H in db.host_country on A.host_countryid_host_country equals H.id_host_country
                //              where F.id_foreigner == currGridData.id_foreigner
                //              select new { H.name_of_country }).ToList();

                //    var pas = (from F in db.foreigners
                //               join ID in db.identity_document on F.id_foreigner equals ID.foreignersid_foreigner
                //               where F.id_foreigner == currGridData.id_foreigner
                //               select new { ID.serial_number_of_document }).ToList();

                //    var gen = (from G in db.gender
                //               join F in db.foreigners on G.id_gender equals F.genderid_gender
                //               where F.id_foreigner == currGridData.id_foreigner
                //               select new { G.name_of_gender }).FirstOrDefault();

                //    var place = (from F in db.foreigners
                //                 join AF in db.arrival_foreigners on F.id_foreigner equals AF.id_foreigner
                //                 join A in db.arrival on AF.id_arrival equals A.id_arrival
                //                 where F.id_foreigner == currGridData.id_foreigner
                //                 select new { A.place_of_stay }).FirstOrDefault();


                //    oc_Data.Add(new Data
                //    {
                //        Id_foreigner = currGridData.id_foreigner,
                //        Name_of_foreigner = currGridData.name_of_foreigner,
                //        Birth_date = currGridData.birth_date,
                //        Serial_number_of_document = string.Join(", ", pas.Select(s => s.serial_number_of_document)),
                //        Host_country = string.Join(", ", av.Select(s => s.name_of_country)),
                //        Gender = gen == null ? null : gen.name_of_gender.ToString(),
                //        Place_of_stay = place == null ? null : place.place_of_stay.ToString()
                //    });
                //}
                //if (oc_Data.Count == 0)
                //{
                //    System.Windows.MessageBox.Show("Записей нет", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                //}
                //else
                //{
                //    GridMain.ItemsSource = oc_Data;
                //}
            }

            oc_Data.Clear();

            foreach (var currFindForeigners in findForeigners)
            {
                oc_Data.Add(new Data    // как допишешь join'ы тут можно будет заполнить всю коллекцию
                {
                    Id_foreigner = currFindForeigners.id_foreigner,
                    Name_of_foreigner = currFindForeigners.name_of_foreigner,
                    Birth_date = currFindForeigners.birth_date,
                    Serial_number_of_document = string.Join(", ", identity.Where(x => x.foreignersid_foreigner == currFindForeigners.id_foreigner).Select(s => s.serial_number_of_document)),
                    Host_country = currFindForeigners.name_of_country,
                    Gender = currFindForeigners.name_of_gender,
                    Place_of_stay = currFindForeigners.place_of_stay

                });
            }
            if (oc_Data.Count == 0)
            {
                System.Windows.MessageBox.Show("Записей нет", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                GridMain.ItemsSource = oc_Data;
            }

        }

        private void InsertButton(object sender, RoutedEventArgs e)
        {
            ContextDataContext db = new ContextDataContext();

            foreigners newforeigners = new foreigners();
            try
            {
                newforeigners.name_of_foreigner = Name.Text;
                newforeigners.birth_date = DateOfBirth.SelectedDate;
                newforeigners.blood_tipe = Blood.Text;
                newforeigners.nationalityid_nationality = (int?)NationalComboBox.SelectedValue == -1 ? null : (int?)NationalComboBox.SelectedValue;
                newforeigners.genderid_gender = (int?)GenderComboBox.SelectedValue == -1 ? null : (int?)GenderComboBox.SelectedValue;
                newforeigners.health = Health.Text;
                if (Weight.Text != "")
                {
                    newforeigners.weight_kg_ = int.Parse(Weight.Text);
                }
                newforeigners.marital_status = MaritalStatus.Text;
                newforeigners.number_of_children = Children.Text;
                newforeigners.national_day = NationalDay.Text;
                newforeigners.criminal_prosecution_in_the_country_of_permanent_residence = CriminalProsecution.Text;
                newforeigners.participant_in_military_conflicts = MilitaryConflicts.Text;
                newforeigners.remarks = Remarks.Text;

                db.foreigners.InsertOnSubmit(newforeigners);
                db.SubmitChanges();

                foreach (var data in oc_DocumentAddOrEdit)
                {
                    identity_document newidentity_Document = new identity_document();
                    newidentity_Document.date_of_issue = data.DateFrom;
                    newidentity_Document.validity_period = data.DateTo;
                    newidentity_Document.foreignersid_foreigner = newforeigners.id_foreigner;
                    newidentity_Document.document_typeid_doctype = data.IdDocumentType;
                    newidentity_Document.serial_number_of_document = data.Serial;
                    db.identity_document.InsertOnSubmit(newidentity_Document);
                    db.SubmitChanges();
                }

                education neweducation = new education();
                neweducation.place_of_study = University.Text;
                neweducation.faculty = Faculty.Text;
                neweducation.training_level = EducationStatus.Text;

                db.education.InsertOnSubmit(neweducation);
                db.SubmitChanges();

                education_foreigners neweducationForeigners = new education_foreigners();
                neweducationForeigners.id_education = neweducation.id_education;
                neweducationForeigners.id_foreigner = newforeigners.id_foreigner;
                db.education_foreigners.InsertOnSubmit(neweducationForeigners);
                db.SubmitChanges();

                foreach (var datalanguage in oc_LanguageAddOrDelete)
                {
                    languages_foreigners newlanguagesforeigners = new languages_foreigners();
                    newlanguagesforeigners.id_language = datalanguage.IdLanguage;
                    newlanguagesforeigners.id_foreigner = newforeigners.id_foreigner;
                    db.languages_foreigners.InsertOnSubmit(newlanguagesforeigners);
                    db.SubmitChanges();
                }

                foreach (var datahobby in oc_HobbyAddOrDelete)
                {
                    hobby_foreigners newhobbyForeigners = new hobby_foreigners();
                    newhobbyForeigners.id_hobby = datahobby.IdHobby;
                    newhobbyForeigners.id_foreigner = newforeigners.id_foreigner;
                    db.hobby_foreigners.InsertOnSubmit(newhobbyForeigners);
                    db.SubmitChanges();
                }

                arrival newArrival = new arrival();
                newArrival.date_of_arrival = ArrivalDate.SelectedDate;
                newArrival.host_countryid_host_country = (int?)HostCountryComboBox.SelectedValue == -1 ? null : (int?)HostCountryComboBox.SelectedValue;
                newArrival.place_of_stay = HostPlace.Text;
                newArrival.purposeid_purpose = (int?)PurposeOfStayComboBox.SelectedValue == -1 ? null : (int?)PurposeOfStayComboBox.SelectedValue;
                db.arrival.InsertOnSubmit(newArrival);
                db.SubmitChanges();

                arrival_foreigners newarrivalForeigners = new arrival_foreigners();
                newarrivalForeigners.id_arrival = newArrival.id_arrival;
                newarrivalForeigners.id_foreigner = newforeigners.id_foreigner;
                db.arrival_foreigners.InsertOnSubmit(newarrivalForeigners);
                db.SubmitChanges();


                foreach (var datawork in oc_WorkplaceAddOrEdit)
                {
                    workplace newworkplace = new workplace();
                    newworkplace.name_of_company = datawork.NameOfCompany;
                    if (datawork.EmbassyIdEmbassy.ToString() != "-1")
                    {
                        newworkplace.embassyid_embassy = datawork.EmbassyIdEmbassy;
                    }
                    newworkplace.division_department_direction = datawork.Department;
                    newworkplace.position = datawork.Position;
                    newworkplace.military_rank = datawork.Rank;
                    newworkplace.office_hours = datawork.Hours;
                    newworkplace.number_fax = datawork.Telephone;
                    newworkplace.email = datawork.Email;
                    newworkplace.residential_address = datawork.Address;
                    newworkplace.id_arrival = newArrival.id_arrival;
                    db.workplace.InsertOnSubmit(newworkplace);
                    db.SubmitChanges();

                    workplace_foreigners newWorkplaceForeigners = new workplace_foreigners();
                    newWorkplaceForeigners.id_foreigner = newforeigners.id_foreigner;
                    newWorkplaceForeigners.id_workplace = newworkplace.id_workplace;
                    db.workplace_foreigners.InsertOnSubmit(newWorkplaceForeigners);
                    db.SubmitChanges();

                    foreach (var dataproject in oc_ProjectAddOrEdit)
                    {
                        if (dataproject.Uuid == datawork.ID)
                        {
                            project newproject = new project();
                            newproject.name_of_project = dataproject.NameOfProject;
                            newproject.nature_description_of_project = dataproject.Description;
                            newproject.project_details = dataproject.ProjectDetails;
                            newproject.id_workplace = newworkplace.id_workplace;
                            db.project.InsertOnSubmit(newproject);
                            db.SubmitChanges();

                            project_foreigners newProjectForeigners = new project_foreigners();
                            newProjectForeigners.id_foreigner = newforeigners.id_foreigner;
                            newProjectForeigners.id_project = newproject.id_project;
                            db.project_foreigners.InsertOnSubmit(newProjectForeigners);
                            db.SubmitChanges();
                        }
                    }
                }


                foreach (var dataproject in oc_ProjectAddOrEdit)
                {
                    if (dataproject.Uuid == "-1" || dataproject.Uuid == null)
                    {
                        project newproject = new project();
                        newproject.name_of_project = dataproject.NameOfProject;
                        newproject.nature_description_of_project = dataproject.Description;
                        newproject.project_details = dataproject.ProjectDetails;
                        newproject.id_workplace = null;
                        db.project.InsertOnSubmit(newproject);
                        db.SubmitChanges();

                        project_foreigners newProjectForeigners = new project_foreigners();
                        newProjectForeigners.id_foreigner = newforeigners.id_foreigner;
                        newProjectForeigners.id_project = newproject.id_project;
                        db.project_foreigners.InsertOnSubmit(newProjectForeigners);
                        db.SubmitChanges();
                    }
                }

                foreach (var datafile in oc_FileAddOrEdit)
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
                    newfiles_foreigners.id_foreigner = newforeigners.id_foreigner;
                    db.files_foreigners.InsertOnSubmit(newfiles_foreigners);
                    db.SubmitChanges();
                }
                System.Windows.MessageBox.Show("Новый объект успешно добавлен", "Добавление профиля объекта", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ClearText();
            oc_DocumentAddOrEdit.Clear();
            oc_HobbyAddOrDelete.Clear();
            oc_LanguageAddOrDelete.Clear();
            oc_WorkplaceAddOrEdit.Clear();
            oc_ProjectAddOrEdit.Clear();
            oc_FileAddOrEdit.Clear();
        }

        private void cmDelete_Click(object sender, RoutedEventArgs e)
        {
            var SummaryDelete = GridMain.SelectedItems;
            ContextDataContext db = new ContextDataContext();
            try
            {
                foreach (var ObjSelected in SummaryDelete)
                {
                    string selectedIdForeigner = ObjSelected.GetType().GetProperty("Id_foreigner").GetValue(ObjSelected, null).ToString();
                    int selectedIdToDelete = int.Parse(selectedIdForeigner);

                    List<int> filesToDelete = db.files_foreigners.Where(w => w.id_foreigner == selectedIdToDelete).Select(s => s.id_file).ToList();
                    foreach (var file in filesToDelete)
                    {
                        var delFileForeigner = db.files_foreigners.Where(w => w.id_file == file).FirstOrDefault();
                        var idDeleteFile = db.files.Where(w => w.id_file == file).FirstOrDefault();
                        db.files_foreigners.DeleteOnSubmit(delFileForeigner);
                        db.files.DeleteOnSubmit(idDeleteFile);
                    }

                    List<int> docId = db.identity_document.Where(w => w.foreignersid_foreigner == selectedIdToDelete).Select(s => s.id_document).ToList();
                    foreach (int currDocId in docId)
                    {
                        var delDocId = db.identity_document.Where(w => w.id_document == currDocId).FirstOrDefault();
                        db.identity_document.DeleteOnSubmit(delDocId);
                        db.SubmitChanges();
                    }

                    List<int> educationToForeigner = db.education_foreigners.Where(w => w.id_foreigner == selectedIdToDelete).Select(s => s.id_education).ToList();
                    foreach (var educ in educationToForeigner)
                    {
                        var delEducationForeigner = db.education_foreigners.Where(w => w.id_education == educ).FirstOrDefault();
                        var idDeleteEducation = db.education.Where(w => w.id_education == educ).FirstOrDefault();
                        db.education_foreigners.DeleteOnSubmit(delEducationForeigner);
                        db.education.DeleteOnSubmit(idDeleteEducation);
                        db.SubmitChanges();
                    }

                    List<int> langForeigner = db.languages_foreigners.Where(w => w.id_foreigner == selectedIdToDelete).Select(s => s.id_language).ToList();
                    foreach (int currLangForeigner in langForeigner)
                    {
                        var delLangFor = db.languages_foreigners.Where(w => w.id_language == currLangForeigner).FirstOrDefault();
                        db.languages_foreigners.DeleteOnSubmit(delLangFor);
                        db.SubmitChanges();
                    }

                    List<int> projectForeigner = db.project_foreigners.Where(w => w.id_foreigner == selectedIdToDelete).Select(s => s.id_project).ToList();
                    foreach (var proj in projectForeigner)
                    {
                        var delProjectForeigner = db.project_foreigners.Where(w => w.id_project == proj).FirstOrDefault();
                        var idDeleteProject = db.project.Where(w => w.id_project == proj).FirstOrDefault();
                        db.project_foreigners.DeleteOnSubmit(delProjectForeigner);
                        db.project.DeleteOnSubmit(idDeleteProject);
                        db.SubmitChanges();
                    }

                    List<int> workplaceForeigner = db.workplace_foreigners.Where(w => w.id_foreigner == selectedIdToDelete).Select(s => s.id_workplace).ToList();
                    foreach (var work in workplaceForeigner)
                    {
                        var delWorkplaceForeigner = db.workplace_foreigners.Where(w => w.id_workplace == work).FirstOrDefault();
                        var idDeleteWorkplace = db.workplace.Where(w => w.id_workplace == work).FirstOrDefault();
                        db.workplace_foreigners.DeleteOnSubmit(delWorkplaceForeigner);
                        db.workplace.DeleteOnSubmit(idDeleteWorkplace);
                        db.SubmitChanges();
                    }

                    int arrForeigner = db.arrival_foreigners.Where(w => w.id_foreigner == selectedIdToDelete).Select(s => s.id_arrival).FirstOrDefault();
                    arrival_foreigners arrivalForeigner = db.arrival_foreigners.FirstOrDefault(w => w.id_foreigner == selectedIdToDelete);
                    arrival arriva = db.arrival.FirstOrDefault(w => w.id_arrival == arrForeigner);
                    db.arrival_foreigners.DeleteOnSubmit(arrivalForeigner);
                    db.arrival.DeleteOnSubmit(arriva);

                    List<int> hobbyforeigner = db.hobby_foreigners.Where(w => w.id_foreigner == selectedIdToDelete).Select(s => s.id_hobby).ToList();
                    foreach (int currHobbyForeigner in hobbyforeigner)
                    {
                        var delHobbyFor = db.hobby_foreigners.Where(w => w.id_hobby == currHobbyForeigner).FirstOrDefault();
                        db.hobby_foreigners.DeleteOnSubmit(delHobbyFor);
                        db.SubmitChanges();
                    }

                    foreigners deleteForeigner = db.foreigners.FirstOrDefault(w => w.id_foreigner == selectedIdToDelete);
                    db.foreigners.DeleteOnSubmit(deleteForeigner);
                    db.SubmitChanges();
                }
                SearchButton(sender, e);
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ну умница, ты всё сломал(a), иди к разработчику сие чуда ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmOpenOutcome_Click(object sender, RoutedEventArgs e)
        {

            object rep = GridMain.SelectedValue;
            Report report = new Report(rep);
            report.ShowDialog();

        }

        private void cmUpdate_Click(object sender, RoutedEventArgs e)
        {
            object chan = GridMain.SelectedValue;
            ContextDataContext db = new ContextDataContext();
            var selectedFoiregner = db.foreigners.Where(w => w.id_foreigner == int.Parse(chan.GetType().GetProperty("Id_foreigner").GetValue(chan, null).ToString())).FirstOrDefault();
            if (selectedFoiregner != null)
            {
                Changing changes = new Changing(chan);
                changes.ShowDialog();
            }
        }

        #endregion

        #region Dictionaries

        private void ButtonTermAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContextDataContext db = new ContextDataContext();

                ComboBoxItem item = (ComboBoxItem)ComboBoxDict.SelectedItem;
                EditTerm editWindow = new EditTerm();
                editWindow.LabelTextBox1.Content = "Термин";
                editWindow.gridMain.RowDefinitions[1].Height = new GridLength(0);
                editWindow.gridMain.RowDefinitions[2].Height = new GridLength(0);
                editWindow.gridMain.RowDefinitions[3].Height = new GridLength(0);
                editWindow.gridMain.RowDefinitions[4].Height = new GridLength(0);
                editWindow.gridMain.RowDefinitions[5].Height = new GridLength(0);
                editWindow.Title = "Добавление нового термина в словарь - " + item.Content.ToString();
                editWindow.Height = 150;
                editWindow.MinHeight = 140;
                editWindow.ShowDialog();

                if (editWindow.DialogResult.Equals(true))
                {

                    switch (item.Content.ToString())
                    {
                        case "Национальность":
                            if (oc_Nationality.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                            {
                                nationality addTermNationality = new nationality
                                {
                                    name_of_nationality = editWindow.TextBox1.Text
                                };

                                db.nationality.InsertOnSubmit(addTermNationality);
                                db.SubmitChanges();
                                oc_Nationality.Add(new OneRecord { ID = addTermNationality.id_nationality, Term = addTermNationality.name_of_nationality });
                                BindingComboBoxes("Национальность");
                            }
                            else
                                System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить национальность, \nкоторая уже содержится в списке!", "Добавление новой национальности", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                        case "Посольство":
                            if (oc_Embassy.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                            {
                                embassy addTermEmbassy = new embassy
                                {
                                    name_of_embassy = editWindow.TextBox1.Text
                                };
                                db.embassy.InsertOnSubmit(addTermEmbassy);
                                db.SubmitChanges();
                                oc_Embassy.Add(new OneRecord { ID = addTermEmbassy.id_embassy, Term = addTermEmbassy.name_of_embassy });
                                BindingComboBoxes("Посольство");
                            }
                            else
                                System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить посольство, \nкоторое уже содержится в списке!", "Добавление нового посольства", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                        case "Цель прибытия":
                            if (oc_Purpose.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                            {
                                purpose addTermPurpose = new purpose
                                {
                                    name_of_purpose = editWindow.TextBox1.Text
                                };
                                db.purpose.InsertOnSubmit(addTermPurpose);
                                db.SubmitChanges();
                                oc_Purpose.Add(new OneRecord { ID = addTermPurpose.id_purpose, Term = addTermPurpose.name_of_purpose });
                                BindingComboBoxes("Цель прибытия");
                            }
                            else
                                System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить цель прибытия, \nкоторая уже содержится в списке!", "Добавление новой цели прибытия", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                        case "Владение языками":
                            if (oc_Language.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                            {
                                languages addTermLanguage = new languages
                                {
                                    name_of_language = editWindow.TextBox1.Text
                                };
                                db.languages.InsertOnSubmit(addTermLanguage);
                                db.SubmitChanges();
                                oc_Language.Add(new OneRecord { ID = addTermLanguage.id_language, Term = addTermLanguage.name_of_language });
                                BindingComboBoxes("Владение языками");
                            }
                            else
                                System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить язык, \nкоторый уже содержится в списке!", "Добавление нового языка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                        case "Пол":
                            if (oc_Gender.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                            {
                                gender addTermGender = new gender
                                {
                                    name_of_gender = editWindow.TextBox1.Text
                                };
                                db.gender.InsertOnSubmit(addTermGender);
                                db.SubmitChanges();
                                oc_Gender.Add(new OneRecord { ID = addTermGender.id_gender, Term = addTermGender.name_of_gender });
                                BindingComboBoxes("Пол");
                            }
                            else
                                System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить Пол, \nкоторый уже содержится в списке!", "Добавление нового пола", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                        case "Хобби":
                            if (oc_Hobby.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                            {
                                hobby addTermHobby = new hobby
                                {
                                    name_of_hobby = editWindow.TextBox1.Text
                                };
                                db.hobby.InsertOnSubmit(addTermHobby);
                                db.SubmitChanges();
                                oc_Hobby.Add(new OneRecord { ID = addTermHobby.id_hobby, Term = addTermHobby.name_of_hobby });
                                BindingComboBoxes("Хобби");
                            }
                            else
                                System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить хобби, \nкоторое уже содержится в списке!", "Добавление нового хобби", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                        case "Тип документа":
                            if (oc_DocumentType.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                            {
                                document_type addTermDocumentType = new document_type
                                {
                                    name_of_document_type = editWindow.TextBox1.Text
                                };
                                db.document_type.InsertOnSubmit(addTermDocumentType);
                                db.SubmitChanges();
                                oc_DocumentType.Add(new OneRecord { ID = addTermDocumentType.id_doctype, Term = addTermDocumentType.name_of_document_type });
                                BindingComboBoxes("Тип документа");
                            }
                            else
                                System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить тип удостоверения личность, \nкоторый уже содержится в списке!", "Добавление нового типа удостоверения личность", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                        case "Страна пребывания":
                            if (oc_HostCountry.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                            {
                                host_country addTermHostCountry = new host_country
                                {
                                    name_of_country = editWindow.TextBox1.Text
                                };
                                db.host_country.InsertOnSubmit(addTermHostCountry);
                                db.SubmitChanges();
                                oc_HostCountry.Add(new OneRecord { ID = addTermHostCountry.id_host_country, Term = addTermHostCountry.name_of_country });
                                BindingComboBoxes("Страна пребывания");
                            }
                            else
                                System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить страну пребывания, \nкоторая уже содержится в списке!", "Добавление новой страны пребывания", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                //Logging.RecordError(error);
            }
        }

        private void ComboBoxDict_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxItem item = (ComboBoxItem)ComboBoxDict.SelectedItem;
                DataGridDict.HeadersVisibility = DataGridHeadersVisibility.All;
                ButtonTermAdd.IsEnabled = true;
                switch (item.Content.ToString())
                {
                    case "Национальность":
                        DataGridDict.ItemsSource = oc_Nationality;
                        break;
                    case "Посольство":
                        DataGridDict.ItemsSource = oc_Embassy;
                        break;
                    case "Цель прибытия":
                        DataGridDict.ItemsSource = oc_Purpose;
                        break;
                    case "Владение языками":
                        DataGridDict.ItemsSource = oc_Language;
                        break;
                    case "Пол":
                        DataGridDict.ItemsSource = oc_Gender;
                        break;
                    case "Хобби":
                        DataGridDict.ItemsSource = oc_Hobby;
                        break;
                    case "Тип документа":
                        DataGridDict.ItemsSource = oc_DocumentType;
                        break;
                    case "Страна пребывания":
                        DataGridDict.ItemsSource = oc_HostCountry;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                //Logging.RecordError(error);
            }
        }

        private void EditRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContextDataContext db = new ContextDataContext();
                System.Windows.Controls.Button btEdit = sender as System.Windows.Controls.Button;
                int? id = 0;
                if (btEdit != null)
                    id = (int?)btEdit.Tag;
                else if (DataGridDict.SelectedItem != null)
                    id = (int)DataGridDict.SelectedItem.GetType().GetProperty("ID").GetValue(DataGridDict.SelectedItem);

                if (id != 0)
                {
                    EditTerm editWindow = new EditTerm();
                    editWindow.LabelTextBox1.Content = "Термин";

                    editWindow.gridMain.RowDefinitions[1].Height = new System.Windows.GridLength(0);
                    editWindow.gridMain.RowDefinitions[2].Height = new System.Windows.GridLength(0);
                    editWindow.gridMain.RowDefinitions[3].Height = new System.Windows.GridLength(0);
                    editWindow.gridMain.RowDefinitions[4].Height = new System.Windows.GridLength(0);
                    editWindow.gridMain.RowDefinitions[5].Height = new System.Windows.GridLength(0);
                    editWindow.Height = 150;
                    editWindow.MinHeight = 140;
                    ComboBoxItem item = (ComboBoxItem)ComboBoxDict.SelectedItem;

                    switch (item.Content.ToString())
                    {
                        case "Национальность":
                            editWindow.Title = "Редактирование термина в словаре - Национальность";
                            var nationality = db.nationality.Single(s => s.id_nationality == id);
                            editWindow.TextBox1.Text = nationality.name_of_nationality;
                            editWindow.TextBox1.SelectionStart = 0;
                            editWindow.TextBox1.SelectionLength = editWindow.TextBox1.Text.Count();
                            editWindow.TextBox1.Focus();
                            editWindow.ShowDialog();
                            if (editWindow.DialogResult.Equals(true))
                            {
                                if (oc_Nationality.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                                {
                                    oc_Nationality.First(f => f.ID == id).Term = editWindow.TextBox1.Text;
                                    nationality.name_of_nationality = editWindow.TextBox1.Text;
                                    db.SubmitChanges();
                                    BindingComboBoxes("Национальность");
                                }
                                else
                                    System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить национальность, \nкоторая уже содержится в списке!", "Добавление новой национальности", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            break;
                        case "Посольство":
                            editWindow.Title = "Редактирование термина в словаре - Посольство";
                            var embassy = db.embassy.Single(s => s.id_embassy == id);
                            editWindow.TextBox1.Text = embassy.name_of_embassy;
                            editWindow.TextBox1.SelectionStart = 0;
                            editWindow.TextBox1.SelectionLength = editWindow.TextBox1.Text.Count();
                            editWindow.TextBox1.Focus();
                            editWindow.ShowDialog();
                            if (editWindow.DialogResult.Equals(true))
                            {
                                if (oc_Embassy.Where(w => w.Term.ToLower() == editWindow.TextBox1.Text.ToLower().Trim()).Count() == 0)
                                {
                                    oc_Embassy.First(f => f.ID == id).Term = editWindow.TextBox1.Text;
                                    embassy.name_of_embassy = editWindow.TextBox1.Text;
                                    db.SubmitChanges();
                                    BindingComboBoxes("Посольство");
                                }
                                else
                                    System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить посольство, \nкоторое уже содержится в списке!", "Добавление нового посольства", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            break;
                        case "Цель прибытия":
                            editWindow.Title = "Редактирование термина в словаре - Цель прибытия";
                            var purpose = db.purpose.Single(s => s.id_purpose == id);
                            editWindow.TextBox1.Text = purpose.name_of_purpose;
                            editWindow.TextBox1.SelectionStart = 0;
                            editWindow.TextBox1.SelectionLength = editWindow.TextBox1.Text.Count();
                            editWindow.TextBox1.Focus();
                            editWindow.ShowDialog();
                            if (editWindow.DialogResult.Equals(true))
                            {
                                if (oc_Purpose.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                                {
                                    oc_Purpose.First(f => f.ID == id).Term = editWindow.TextBox1.Text;
                                    purpose.name_of_purpose = editWindow.TextBox1.Text;
                                    db.SubmitChanges();
                                    BindingComboBoxes("Цель прибытия");
                                }
                                else
                                    System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить wель прибытия, \nкоторая уже содержится в списке!", "Добавление новой цели прибытия", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            break;
                        case "Владение языками":
                            editWindow.Title = "Редактирование термина в словаре - Владение языками";
                            var language = db.languages.Single(s => s.id_language == id);
                            editWindow.TextBox1.Text = language.name_of_language;
                            editWindow.TextBox1.SelectionStart = 0;
                            editWindow.TextBox1.SelectionLength = editWindow.TextBox1.Text.Count();
                            editWindow.TextBox1.Focus();
                            editWindow.ShowDialog();
                            if (editWindow.DialogResult.Equals(true))
                            {
                                if (oc_Language.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                                {
                                    oc_Language.First(f => f.ID == id).Term = editWindow.TextBox1.Text;
                                    language.name_of_language = editWindow.TextBox1.Text;
                                    db.SubmitChanges();
                                    BindingComboBoxes("Владение языками");
                                }
                                else
                                    System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить язык, \nкоторый уже содержится в списке!", "Добавление нового языка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            break;
                        case "Пол":
                            editWindow.Title = "Редактирование термина в словаре - Пол";
                            var gender = db.gender.Single(s => s.id_gender == id);
                            editWindow.TextBox1.Text = gender.name_of_gender;
                            editWindow.TextBox1.SelectionStart = 0;
                            editWindow.TextBox1.SelectionLength = editWindow.TextBox1.Text.Count();
                            editWindow.TextBox1.Focus();
                            editWindow.ShowDialog();
                            if (editWindow.DialogResult.Equals(true))
                            {
                                if (oc_Gender.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                                {
                                    oc_Gender.First(f => f.ID == id).Term = editWindow.TextBox1.Text;
                                    gender.name_of_gender = editWindow.TextBox1.Text;
                                    db.SubmitChanges();
                                    BindingComboBoxes("Пол");
                                }
                                else
                                    System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить пол, \nкоторый уже содержится в списке!", "Добавление нового пола", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            break;
                        case "Хобби":
                            editWindow.Title = "Редактирование термина в словаре - Хобби";
                            var hobby = db.hobby.Single(s => s.id_hobby == id);
                            editWindow.TextBox1.Text = hobby.name_of_hobby;
                            editWindow.TextBox1.SelectionStart = 0;
                            editWindow.TextBox1.SelectionLength = editWindow.TextBox1.Text.Count();
                            editWindow.TextBox1.Focus();
                            editWindow.ShowDialog();
                            if (editWindow.DialogResult.Equals(true))
                            {
                                if (oc_Hobby.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                                {
                                    oc_Hobby.First(f => f.ID == id).Term = editWindow.TextBox1.Text;
                                    hobby.name_of_hobby = editWindow.TextBox1.Text;
                                    db.SubmitChanges();
                                    BindingComboBoxes("Хобби");
                                }
                                else
                                    System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить хобби, \nкоторое уже содержится в списке!", "Добавление нового хобби", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            break;
                        case "Тип документа":
                            editWindow.Title = "Редактирование термина в словаре - Тип документа";
                            var documentType = db.document_type.Single(s => s.id_doctype == id);
                            editWindow.TextBox1.Text = documentType.name_of_document_type;
                            editWindow.TextBox1.SelectionStart = 0;
                            editWindow.TextBox1.SelectionLength = editWindow.TextBox1.Text.Count();
                            editWindow.TextBox1.Focus();
                            editWindow.ShowDialog();
                            if (editWindow.DialogResult.Equals(true))
                            {
                                if (oc_DocumentType.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                                {
                                    oc_DocumentType.First(f => f.ID == id).Term = editWindow.TextBox1.Text;
                                    documentType.name_of_document_type = editWindow.TextBox1.Text;
                                    db.SubmitChanges();
                                    BindingComboBoxes("Тип документа");
                                }
                                else
                                    System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить тип удостоверения личности, \nкоторый уже содержится в списке!", "Добавление нового типа документа", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            break;
                        case "Страна пребывания":
                            editWindow.Title = "Редактирование термина в словаре - Страна пребывания";
                            var hostCountry = db.host_country.Single(s => s.id_host_country == id);
                            editWindow.TextBox1.Text = hostCountry.name_of_country;
                            editWindow.TextBox1.SelectionStart = 0;
                            editWindow.TextBox1.SelectionLength = editWindow.TextBox1.Text.Count();
                            editWindow.TextBox1.Focus();
                            editWindow.ShowDialog();
                            if (editWindow.DialogResult.Equals(true))
                            {
                                if (oc_HostCountry.Where(w => w.Term == editWindow.TextBox1.Text.Trim()).Count() == 0)
                                {
                                    oc_HostCountry.First(f => f.ID == id).Term = editWindow.TextBox1.Text;
                                    hostCountry.name_of_country = editWindow.TextBox1.Text;
                                    db.SubmitChanges();
                                    BindingComboBoxes("Страна пребывания");
                                }
                                else
                                    System.Windows.MessageBox.Show("Внимание, Вы пытаетесь добавить страну пребывания, \nкоторая уже содержится в списке!", "Добавление новой страны пребывания", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                //Logging.RecordError(error);
            }
        }

        private void DeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContextDataContext db = new ContextDataContext();

                System.Windows.Controls.Button btnDel = sender as System.Windows.Controls.Button;
                int? id = 0;
                if (btnDel != null)
                    id = (int?)btnDel.Tag;
                else if (DataGridDict.SelectedItem != null)
                    id = (int)DataGridDict.SelectedItem.GetType().GetProperty("ID").GetValue(DataGridDict.SelectedItem);

                if (id != 0)
                {
                    if (System.Windows.MessageBox.Show("Удалить термин?", "Редактирование словарей", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        ComboBoxItem item = (ComboBoxItem)ComboBoxDict.SelectedItem;
                        switch (item.Content.ToString())
                        {
                            case "Национальность":
                                if (db.foreigners.FirstOrDefault(f => f.nationalityid_nationality == id) == null)
                                {
                                    var nationality = db.nationality.Where(w => w.id_nationality == id).Single();
                                    db.nationality.DeleteOnSubmit(nationality);
                                    oc_Nationality.Remove(oc_Nationality.First(f => f.ID == id));
                                    db.SubmitChanges();
                                    BindingComboBoxes("Национальность");
                                }
                                else
                                    System.Windows.MessageBox.Show("Удаление термина невозможно. Имеются связанные записи!", "Редактирование словаря - Национальность", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                break;
                            case "Посольство":
                                if (db.workplace.FirstOrDefault(f => f.embassyid_embassy == id) == null)
                                {
                                    var embassy = db.embassy.Where(w => w.id_embassy == id).Single();
                                    db.embassy.DeleteOnSubmit(embassy);
                                    oc_Embassy.Remove(oc_Embassy.First(f => f.ID == id));
                                    db.SubmitChanges();
                                    BindingComboBoxes("Посольство");
                                }
                                else
                                    System.Windows.MessageBox.Show("Удаление термина невозможно. Имеются связанные записи!", "Редактирование словаря - Посольство", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                break;
                            case "Цель прибытия":
                                if (db.arrival.FirstOrDefault(f => f.purposeid_purpose == id) == null)
                                {
                                    var purpose = db.purpose.Where(w => w.id_purpose == id).Single();
                                    db.purpose.DeleteOnSubmit(purpose);
                                    oc_Purpose.Remove(oc_Purpose.First(f => f.ID == id));
                                    db.SubmitChanges();
                                    BindingComboBoxes("Цель прибытия");
                                }
                                else
                                    System.Windows.MessageBox.Show("Удаление термина невозможно. Имеются связанные записи!", "Редактирование словаря - Цель прибытия", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                break;
                            case "Владение языками":
                                if (db.languages_foreigners.FirstOrDefault(f => f.id_language == id) == null)
                                {
                                    var language = db.languages.Where(w => w.id_language == id).Single();
                                    db.languages.DeleteOnSubmit(language);
                                    oc_Language.Remove(oc_Language.First(f => f.ID == id));
                                    db.SubmitChanges();
                                    BindingComboBoxes("Владение языками");
                                }
                                else
                                    System.Windows.MessageBox.Show("Удаление термина невозможно. Имеются связанные записи!", "Редактирование словаря - Владение языками", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                break;
                            case "Пол":
                                if (db.foreigners.FirstOrDefault(f => f.genderid_gender == id) == null)
                                {
                                    var gender = db.gender.Where(w => w.id_gender == id).Single();
                                    db.gender.DeleteOnSubmit(gender);
                                    oc_Gender.Remove(oc_Gender.First(f => f.ID == id));
                                    db.SubmitChanges();
                                    BindingComboBoxes("Пол");
                                }
                                else
                                    System.Windows.MessageBox.Show("Удаление термина невозможно. Имеются связанные записи!", "Редактирование словаря - Пол", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                break;
                            case "Хобби":
                                if (db.hobby_foreigners.FirstOrDefault(f => f.id_hobby == id) == null)
                                {
                                    var hobby = db.hobby.Where(w => w.id_hobby == id).Single();
                                    db.hobby.DeleteOnSubmit(hobby);
                                    oc_Hobby.Remove(oc_Hobby.First(f => f.ID == id));
                                    db.SubmitChanges();
                                    BindingComboBoxes("Хобби");
                                }
                                else
                                    System.Windows.MessageBox.Show("Удаление термина невозможно. Имеются связанные записи!", "Редактирование словаря - Хобби", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                break;
                            case "Тип документа":
                                if (db.identity_document.FirstOrDefault(f => f.id_document == id) == null)
                                {
                                    var documentType = db.document_type.Where(w => w.id_doctype == id).Single();
                                    db.document_type.DeleteOnSubmit(documentType);
                                    oc_DocumentType.Remove(oc_DocumentType.First(f => f.ID == id));
                                    db.SubmitChanges();
                                    BindingComboBoxes("Тип документа");
                                }
                                else
                                    System.Windows.MessageBox.Show("Удаление термина невозможно. Имеются связанные записи!", "Редактирование словаря - Тип документа", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                break;
                            case "Страна пребывания":
                                if (db.arrival.FirstOrDefault(f => f.host_countryid_host_country == id) == null)
                                {
                                    var hostCountry = db.host_country.Where(w => w.id_host_country == id).Single();
                                    db.host_country.DeleteOnSubmit(hostCountry);
                                    oc_HostCountry.Remove(oc_HostCountry.First(f => f.ID == id));
                                    db.SubmitChanges();
                                    BindingComboBoxes("Страна пребывания");
                                }
                                else
                                    System.Windows.MessageBox.Show("Удаление термина невозможно. Имеются связанные записи!", "Редактирование словаря - Страна пребывания", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                // Logging.RecordError(error);
            }
        }

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

                    FilteredNationalityComboBox.DataContext = NationalityFilter;
                    FilteredNationalityComboBox.DisplayMemberPath = "Value";
                    FilteredNationalityComboBox.SelectedValuePath = "Key";
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
                    ComboBoxFilterPurpose.DataContext = PurposeFilter;
                    ComboBoxFilterPurpose.DisplayMemberPath = "Value";
                    ComboBoxFilterPurpose.SelectedValuePath = "Key";
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
                    FilteredHostCountryComboBox.DataContext = HostCountryFilter;
                    FilteredHostCountryComboBox.DisplayMemberPath = "Value";
                    FilteredHostCountryComboBox.SelectedValuePath = "Key";
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
                    Address = Address.Text,
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
                    //IdWorkplace = (string)(WorkplaceChosenComboBox.SelectedValue == null ? null : WorkplaceChosenComboBox.SelectedValue),
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
