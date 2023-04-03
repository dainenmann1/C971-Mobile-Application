using System;
using SQLite;
using Plugin.LocalNotifications;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGU_C971.Models;
using WGU_C971.Services;
using WGU_C971.Views;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGU_C971
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage MainHomePage;
        public List<SchoolTerm> TermList = new List<SchoolTerm>();
        public List<SchoolCourse> CoursesList = new List<SchoolCourse>();
        public List<Assessment> AssessmentList = new List<Assessment>();

        public MainPage()
        {
            InitializeComponent();
            DegreePlanListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ItemTapped);
            MainHomePage = this;
        }

        private async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            SchoolTerm term = (SchoolTerm)e.Item;
            await Navigation.PushAsync(new TermDetails(term, MainHomePage));
        }

        private void AddNewTermToolBarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new AddTermPage(MainHomePage));
        }

        protected override void OnAppearing()
        {
            PopulateData();
            DoNotify();
            base.OnAppearing();
        }

        private void DoNotify()
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
            {
                CoursesList = connection.Query<SchoolCourse>($"SELECT * FROM SchoolCourse WHERE Notify = True").ToList();
                AssessmentList = connection.Query<Assessment>($"SELECT * FROM Assessment WHERE Notify = True").ToList();
            }

            // Notifications for Course start and due dates
            foreach (SchoolCourse course in CoursesList)
            {
                if (course.StartDate == DateTime.Today)
                {
                    CrossLocalNotifications.Current.Show("REMINDER", $"{course.Name} starts today!");
                }
                if (course.EndDate == DateTime.Today)
                {
                    CrossLocalNotifications.Current.Show("REMINDER", $"{course.Name} ends today!");
                }
                if (course.EndDate == DateTime.Today.AddDays(1))
                {
                    CrossLocalNotifications.Current.Show("REMINDER", $"{course.Name} ends tomorrow!");
                }
            }

            // Notifications for Assessment due dates
            foreach (Assessment a in AssessmentList)
            {
                if (a.EndDate == DateTime.Today)
                {
                    CrossLocalNotifications.Current.Show("REMINDER", $"{a.Name} is due today!");
                }
                if (a.EndDate == DateTime.Today.AddDays(1))
                {
                    CrossLocalNotifications.Current.Show("REMINDER", $"{a.Name} is due tomorrow!");
                }
            }
        }

        private void PopulateData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
            {
                connection.CreateTable<SchoolTerm>();
                TermList = connection.Table<SchoolTerm>().ToList();
                connection.CreateTable<SchoolCourse>();
                CoursesList = connection.Table<SchoolCourse>().ToList();
                connection.CreateTable<Assessment>();
                AssessmentList = connection.Table<Assessment>().ToList();
            }

            if (TermList.Count < 1)
            {
                using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
                {
                    connection.DropTable<SchoolTerm>();
                    connection.DropTable<SchoolCourse>();
                    connection.DropTable<Assessment>();

                    connection.CreateTable<SchoolTerm>();
                    connection.CreateTable<SchoolCourse>();
                    connection.CreateTable<Assessment>();

                    DataStore.GenerateData(1);
                }
            }

            using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
            {
                TermList = connection.Table<SchoolTerm>().ToList();
                DegreePlanListView.ItemsSource = TermList;
            }
        }
    }
}