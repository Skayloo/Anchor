using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace Storm.NetFramework
{
    /// <summary>
    /// Логика взаимодействия для Report.xaml
    /// </summary>
    public partial class Report : System.Windows.Window
    {
        public Report(object rep)
        {
            InitializeComponent();
            GetIno(rep);
        }

        private void GetFor(int idshnik)
        {
            ContextDataContext db = new ContextDataContext();
            Section foreignerOutTable = new Section();
            var DataForeigners = db.foreigners.ToList();
            var selectedForeigner = db.foreigners.Where(w => w.id_foreigner == idshnik).FirstOrDefault();

            string foreignerName = selectedForeigner.name_of_foreigner;
            //Paragraph foreignersName = new Paragraph(new Run("ФИО - " + foreignerName));
            //foreignerOutTable.Blocks.Add(foreignersName);


            var DataGender = db.gender.ToList();
            string foreignerGender = null;
            try
            {
                string GenderForeigner = (from G in db.gender
                                          join F in db.foreigners on G.id_gender equals F.genderid_gender
                                          where F.id_foreigner == idshnik
                                          select new { G.name_of_gender }).FirstOrDefault().name_of_gender;
                foreignerGender = GenderForeigner;
            }
            catch
            {
                foreignerGender = "Не указан";
            }
            Paragraph foreignersCombinedFIOGender = new Paragraph(new Run("ФИО - " + foreignerName + " |  Пол - " + foreignerGender));
            foreignerOutTable.Blocks.Add(foreignersCombinedFIOGender);


            var DataNationality = db.nationality.ToList();
            string foreignerNationality = null;
            try
            {
                string NationalityForeigner = (from N in db.nationality
                                               join F in db.foreigners on N.id_nationality equals F.nationalityid_nationality
                                               where F.id_foreigner == idshnik
                                               select new { N.name_of_nationality }).FirstOrDefault().name_of_nationality;
                foreignerNationality = NationalityForeigner;
            }
            catch
            {
                foreignerNationality = "Не указана";
            }
            string foreignerBirthDate = selectedForeigner.birth_date.ToString();
            if(foreignerBirthDate == "")
            {
                foreignerBirthDate = "Не указана";
            }
            Paragraph foreignersCombinedNationalityBirthDate = new Paragraph(new Run("Дата рождения  - " + foreignerBirthDate + " |  Национальность - " + foreignerNationality ));
            foreignerOutTable.Blocks.Add(foreignersCombinedNationalityBirthDate);


            string foreignerWeight = selectedForeigner.weight_kg_.ToString();
            if (foreignerWeight == "")
            {
                foreignerWeight = "Не указан";
            }
            string foreignerHealth = selectedForeigner.health;
            if (foreignerHealth == "")
            {
                foreignerHealth = "Нет данных";
            }
            Paragraph foreignersCombinedHealthWeight = new Paragraph(new Run("Здоровье - " + foreignerHealth + " |  Вес - " + foreignerWeight));
            foreignerOutTable.Blocks.Add(foreignersCombinedHealthWeight);


            string foreignerMarital = selectedForeigner.marital_status;
            if (foreignerMarital == "")
            {
                foreignerMarital = "Нет данных";
            }
            string foreignerChildren = selectedForeigner.number_of_children;
            if (foreignerChildren == "")
            {
                foreignerChildren = "Нет";
            }
            Paragraph foreignersCombinedChildrenMarital = new Paragraph(new Run("Дети - " + foreignerChildren + " |  Семейное положение - " + foreignerMarital));
            foreignerOutTable.Blocks.Add(foreignersCombinedChildrenMarital);


            string foreignerBlood = selectedForeigner.blood_tipe;
            if(foreignerBlood == "")
            {
                foreignerBlood = "Не указана";
            }
            string foreignerNationalDay = selectedForeigner.national_day;
            if(foreignerNationalDay == "")
            {
                foreignerNationalDay = "Не указан";
            }
            Paragraph foreignersCombinedBloodNationalDay = new Paragraph(new Run("Группа крови - " + foreignerBlood + " |  Национальный день - " + foreignerNationalDay));
            foreignerOutTable.Blocks.Add(foreignersCombinedBloodNationalDay);


            string foreignerCriminal = selectedForeigner.criminal_prosecution_in_the_country_of_permanent_residence;
            if(foreignerCriminal == "")
            {
                foreignerCriminal = "Нет данных";
            }
            Paragraph foreignersCriminal = new Paragraph(new Run("Уголовные преследования в стране пребывания - " + foreignerCriminal));
            foreignerOutTable.Blocks.Add(foreignersCriminal);


            string foreignerMilitary = selectedForeigner.participant_in_military_conflicts;
            if(foreignerMilitary == "")
            {
                foreignerMilitary = "Нет данных";
            }
            Paragraph foreignersMilitary = new Paragraph(new Run("Участие в вооруженных конфликтах - " + foreignerMilitary));
            foreignerOutTable.Blocks.Add(foreignersMilitary);


            string foreignerRemarks = selectedForeigner.remarks;
            if(foreignerRemarks == "")
            {
                foreignerRemarks = "Заметок нет";
            }
            Paragraph foreignersRemarks = new Paragraph(new Run("Заметки - " + foreignerRemarks));
            foreignerOutTable.Blocks.Add(foreignersRemarks);

            var DocumentType = (from F in db.foreigners
                                join ID in db.identity_document on F.id_foreigner equals ID.foreignersid_foreigner
                                join DT in db.document_type on ID.document_typeid_doctype equals DT.id_doctype
                                where F.id_foreigner == idshnik
                                select new { ID.date_of_issue, ID.document_typeid_doctype, ID.serial_number_of_document, ID.validity_period, DT.name_of_document_type }).ToList();

            List docList = new List();
            ListItem docListItem = new ListItem();
            if (DocumentType.Count == 0)
            {
                Paragraph nullDocs = new Paragraph(new Run("Данных нет"));
                docListItem.Blocks.Add(nullDocs);
                docList.ListItems.Add(docListItem);
            }
            else
            {
                for (int i = 0; i < DocumentType.Count; i++)
                {
                    string dateOfIssue;
                    string validityPeriod;
                    if(DocumentType[i].date_of_issue.ToString() == "")
                        dateOfIssue = "нет данных";
                    else
                        dateOfIssue = DocumentType[i].date_of_issue.ToString();

                    if (DocumentType[i].validity_period.ToString() == "")
                        validityPeriod = "нет данных";
                    else
                        validityPeriod = DocumentType[i].validity_period.ToString();

                    Paragraph documentSets = new Paragraph(new Run(DocumentType[i].name_of_document_type + " - " + DocumentType[i].serial_number_of_document.ToString()));
                    Paragraph documentPeriod = new Paragraph(new Run("Действителен с - (" + dateOfIssue + ") по - (" + validityPeriod + ")\n"));
                    docListItem.Blocks.Add(documentSets);
                    docListItem.Blocks.Add(documentPeriod);
                    docList.ListItems.Add(docListItem);
                }
            }

            var educationForeigner = (from F in db.foreigners
                                      join EF in db.education_foreigners on F.id_foreigner equals EF.id_foreigner
                                      join E in db.education on EF.id_education equals E.id_education
                                      where F.id_foreigner == idshnik
                                      select new { E.place_of_study, E.faculty, E.training_level }).FirstOrDefault();
            if (educationForeigner.place_of_study.ToString() != "")
            {
                Paragraph foreignersPlaceOfStudy = new Paragraph(new Run("Университет - " + educationForeigner.place_of_study));
                foreignerOutTable.Blocks.Add(foreignersPlaceOfStudy);
            }
            else
            {
                Paragraph foreignersPlaceOfStudy = new Paragraph(new Run("Университет - Данных нет"));
                foreignerOutTable.Blocks.Add(foreignersPlaceOfStudy);
            }
            if(educationForeigner.faculty.ToString() != "")
            {
                Paragraph foreignersFaculty = new Paragraph(new Run("Факультет - " + educationForeigner.faculty));
                foreignerOutTable.Blocks.Add(foreignersFaculty);
            }
            else
            {
                Paragraph foreignersFaculty = new Paragraph(new Run("Факультет - Данных нет"));
                foreignerOutTable.Blocks.Add(foreignersFaculty);
            }
            if(educationForeigner.training_level.ToString() != "")
            {
                Paragraph foreignersTrainingLevel = new Paragraph(new Run("Ученая степень - " + educationForeigner.training_level));
                foreignerOutTable.Blocks.Add(foreignersTrainingLevel);
            }
            else
            {
                Paragraph foreignersTrainingLevel = new Paragraph(new Run("Ученая степень - Данных нет"));
                foreignerOutTable.Blocks.Add(foreignersTrainingLevel);
            }

            Section languageOutTable = new Section();
            Paragraph languageInTable = new Paragraph(new Run("Владение иностранными языками: "));
            languageOutTable.Blocks.Add(languageInTable);
            var LanguagesForeigner = (from F in db.foreigners
                                      join LF in db.languages_foreigners on F.id_foreigner equals LF.id_foreigner
                                      join L in db.languages on LF.id_language equals L.id_language
                                      where F.id_foreigner == idshnik
                                      select new { L.name_of_language }).ToList();

            List languageList = new List();
            ListItem languageListItem = new ListItem();
            if (LanguagesForeigner.Count == 0)
            {
                Paragraph nullDocs = new Paragraph(new Run("Данных нет"));
                languageListItem.Blocks.Add(nullDocs);
                languageList.ListItems.Add(languageListItem);
            }
            else
            {
                for (int i = 0; i < LanguagesForeigner.Count; i++)
                {
                    Paragraph languageSets = new Paragraph(new Run(LanguagesForeigner[i].name_of_language.ToString()));
                    languageListItem.Blocks.Add(languageSets);
                    languageList.ListItems.Add(languageListItem);
                }
            }


            var ArrivalForeigner = (from F in db.foreigners
                                    join AF in db.arrival_foreigners on F.id_foreigner equals AF.id_foreigner
                                    join A in db.arrival on AF.id_arrival equals A.id_arrival
                                    join HC in db.host_country on A.host_countryid_host_country equals HC.id_host_country into AllCountry
                                    from allcntr in AllCountry.DefaultIfEmpty()
                                    join P in db.purpose on A.purposeid_purpose equals P.id_purpose into AllPurpose
                                    from allpur in AllPurpose.DefaultIfEmpty()
                                    where F.id_foreigner == idshnik
                                    select new { A.date_of_arrival, A.place_of_stay, allcntr.name_of_country, allpur.name_of_purpose}).FirstOrDefault();

            Section arrivalTableOut = new Section();
            if (ArrivalForeigner.date_of_arrival.ToString() != "")
            {
                Paragraph foreignersDateOfArrival = new Paragraph(new Run("Дата прибытия - " + ArrivalForeigner.date_of_arrival.ToString()));
                arrivalTableOut.Blocks.Add(foreignersDateOfArrival);
            }
            else
            {
                Paragraph foreignersDateOfArrival = new Paragraph(new Run("Дата прибытия -  Данных нет"));
                arrivalTableOut.Blocks.Add(foreignersDateOfArrival);
            }
            if (ArrivalForeigner.name_of_country != null)
            {
                Paragraph foreignersNameOfCountry = new Paragraph(new Run("Страна - " + ArrivalForeigner.name_of_country.ToString()));
                arrivalTableOut.Blocks.Add(foreignersNameOfCountry);
            }
            else
            {
                Paragraph foreignersNameOfCountry = new Paragraph(new Run("Страна -  Данных нет"));
                arrivalTableOut.Blocks.Add(foreignersNameOfCountry);
            }
            if (ArrivalForeigner.place_of_stay.ToString() != "")
            {
                Paragraph foreignersPlaceOfStay = new Paragraph(new Run("Адрес проживания - " + ArrivalForeigner.place_of_stay.ToString()));
                arrivalTableOut.Blocks.Add(foreignersPlaceOfStay);
            }
            else
            {
                Paragraph foreignersPlaceOfStay = new Paragraph(new Run("Адрес проживания -  Данных нет"));
                arrivalTableOut.Blocks.Add(foreignersPlaceOfStay);
            }
            if (ArrivalForeigner.name_of_purpose != null)
            {
                Paragraph foreignersPurpose = new Paragraph(new Run("Цель прибытия - " + ArrivalForeigner.name_of_purpose.ToString()));
                arrivalTableOut.Blocks.Add(foreignersPurpose);
            }
            else
            {
                Paragraph foreignersPurpose = new Paragraph(new Run("Цель прибытия -  Данных нет"));
                arrivalTableOut.Blocks.Add(foreignersPurpose);
            }

            Section hobbyOutTable = new Section();
            Paragraph hobbyInTable = new Paragraph(new Run("Хобби: "));
            hobbyOutTable.Blocks.Add(hobbyInTable);
            var HobbyForeigner = (from F in db.foreigners
                                  join HF in db.hobby_foreigners on F.id_foreigner equals HF.id_foreigner
                                  join H in db.hobby on HF.id_hobby equals H.id_hobby
                                  where F.id_foreigner == idshnik
                                  select new { H.name_of_hobby }).ToList();
            List hobbyList = new List();
            ListItem hobbyListItem = new ListItem();
            if (HobbyForeigner.Count == 0)
            {
                Paragraph nullDocs = new Paragraph(new Run("Данных нет"));
                hobbyListItem.Blocks.Add(nullDocs);
                hobbyList.ListItems.Add(hobbyListItem);
            }
            else
            {
                for (int i = 0; i < HobbyForeigner.Count; i++)
                {
                    Paragraph hobbySets = new Paragraph(new Run(HobbyForeigner[i].name_of_hobby.ToString()));
                    hobbyListItem.Blocks.Add(hobbySets);
                    hobbyList.ListItems.Add(hobbyListItem);
                }
            }

            Paragraph DocsIntro = new Paragraph(new Run("Известные документы человека: "));
            foreignerOutTable.Blocks.Add(DocsIntro);
            FlowDocument foreignerReport = new FlowDocument();
            foreignerReport.ColumnWidth = 700;
            foreignerReport.TextAlignment = TextAlignment.Left;
            //foreignerReport.FontStyle = FontStyles.Italic;
            foreignerReport.Blocks.Add(foreignerOutTable);
            foreignerReport.Blocks.Add(docList);
            foreignerReport.Blocks.Add(languageOutTable);
            foreignerReport.Blocks.Add(languageList);
            foreignerReport.Blocks.Add(arrivalTableOut);
            foreignerReport.Blocks.Add(hobbyOutTable);
            foreignerReport.Blocks.Add(hobbyList);
            this.Content = foreignerReport;

        }

        private void GetIno(object ino)
        {
            try
            {
                string ss = ino.GetType().GetProperty("Id_foreigner").GetValue(ino, null).ToString();
                int idshnik = int.Parse(ss);
                GetFor(idshnik);
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
