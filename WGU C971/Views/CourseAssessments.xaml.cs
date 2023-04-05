using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGU_C971.Models;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGU_C971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseAsssessments : ContentPage
    {
        public MainPage MainPage;
        public SchoolCourse Course;
        public CourseDetailPage CourseDetailPage;

        public CourseAsssessments(MainPage mainPage, SchoolCourse course, CourseDetailPage courseDetailPage)
        {
            InitializeComponent();
            CourseDetailPage = courseDetailPage;
            MainPage = mainPage;
            Course = course;
            Title = course.Name;

            ListViewAssessments.ItemTapped += new EventHandler<ItemTappedEventArgs>(CourseItemTapped);
        }

        private async void CourseItemTapped(object sender, ItemTappedEventArgs e)
        {
            Assessment assessment = (Assessment)e.Item;
            await Navigation.PushModalAsync(new EditAssessmentPage(MainPage, Course, assessment));
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
            {
                connection.CreateTable<Assessment>();
                List<Assessment> assessments = connection.Query<Assessment>($"SELECT * FROM Assessment WHERE CourseId = { Course.Id };");
                ListViewAssessments.ItemsSource = assessments;
            }
        }

        private async void BtnAddNewAssessment_Clicked(object sender, EventArgs e)
        {
            List<Assessment> assessments;
            using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
            {
                assessments = connection.Query<Assessment>($"SELECT * FROM Assessment WHERE CourseId = {Course.Id};");
            }

            if (assessments.Count >= 2)
            {
                await DisplayAlert("Warning!", "A course can only have 1 Performance Assessment and 1 Objective Assessment", "OK");
            }
            else
            {
                await Navigation.PushModalAsync(new AddCourseAssessment(Course, MainPage));
            }
        }

        private async void BtnBack_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}