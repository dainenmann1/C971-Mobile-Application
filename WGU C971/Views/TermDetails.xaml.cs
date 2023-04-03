using System;
using SQLite;
using WGU_C971.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGU_C971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermDetails : ContentPage
    {
        public SchoolTerm Term;
        public MainPage MainPage;

        public TermDetails()
        {
            InitializeComponent();
        }
        public TermDetails(SchoolTerm term, MainPage mainPage)
        {
            InitializeComponent();
            MainPage = mainPage;
            Term = term;
            Title = term.Name;
            CourseListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(CourseItemTapped);
        }

        private async void CourseItemTapped(object sender, ItemTappedEventArgs e)
        {
            SchoolCourse selectedCourse = (SchoolCourse)e.Item;
            await Navigation.PushModalAsync(new CourseDetailPage(MainPage, Term, selectedCourse));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LblTermStartDate.Text = Term.StartDate.Date.ToString("MM/dd/yyyy");
            LblTermEndDate.Text = Term.EndDate.ToString("MM/dd/yyyy");

            using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
            {
                connection.CreateTable<SchoolCourse>();
                string coursesQueryString = $"SELECT * FROM SchoolCourse WHERE TermId = '{ Term.Id }'";
                List<SchoolCourse> courses = connection.Query<SchoolCourse>(coursesQueryString);
                CourseListView.ItemsSource = courses;
            }
        }

        private async void BtnAddNewCourse_Clicked(object sender, EventArgs e)
        {
            if (GetCourseCount() < 6)
            {
                await Navigation.PushModalAsync(new AddCoursePage(Term, MainPage));
            }
            else
            {
                string title = "Course Maximum Warning!";
                string message = "You cannot add more courses.\nA Maximum number of courses per Term reached.";
                await DisplayAlert(title, message, "OK");
            }
        }

        private async void BtnEditTerm_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditTermPage(Term, MainPage));
        }

        private async void BtnDeleteTerm_Clicked(object sender, EventArgs e)
        {
            // Cascade Delete associated courses and their associated assessments
            try
            {
                string message = $"Are you sure you want to delete { Term.Name }?";
                var response = await DisplayAlert("Warning!", message, "Yes", "No");
                if (response)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
                    {
                        List<SchoolCourse> courses = connection.Query<SchoolCourse>($"SELECT * FROM SchoolCourse WHERE TermId = '{ Term.Id }';");

                        foreach (SchoolCourse course in courses)
                        {
                            List<Assessment> assessments = connection.Query<Assessment>($"SELECT * FROM Assessment WHERE CourseId = '{ course.Id }';");
                            foreach (Assessment assessment in assessments)
                            {
                                connection.Delete(assessment);
                            }
                            connection.Delete(course);
                        }

                        connection.Delete(Term);
                        await Navigation.PopToRootAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private int GetCourseCount()
        {
            int count = 0;

            using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
            {
                string queryString = $"SELECT * FROM SchoolCourse WHERE TermId = '{ Term.Id }';";
                List<SchoolCourse> courses = connection.Query<SchoolCourse>(queryString);
                count = courses.Count();
            }
            return count;
        }
    }
}