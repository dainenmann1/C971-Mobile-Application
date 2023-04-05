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
    public partial class AddCoursePage : ContentPage
    {
        public SchoolTerm Term;
        public MainPage MainPage;

        public AddCoursePage(SchoolTerm term, MainPage mainPage)
        {
            InitializeComponent();
            Term = term;
            MainPage = mainPage;
        }

        public AddCoursePage()
        {
            InitializeComponent();
        }

        private async void BtnSaveCourse_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (RequiredCourseEntryPopulated())
                {
                    ValidateStartAndEndDates();

                    SchoolCourse course = new SchoolCourse()
                    {
                        Name = TxtCourseName.Text,
                        Status = PickerCourseStatus.SelectedItem.ToString().Trim(),
                        StartDate = DatePickerStartDate.Date,
                        EndDate = DatePickerEndDate.Date,
                        InstructorName = TxtInstructorName.Text,
                        InstructorEmail = TxtInstructorEmail.Text,
                        InstructorPhone = TxtInstructorPhone.Text,
                        Note = TxtNotes.Text,
                        TermId = Term.Id,
                    };
                    if (notifBox.IsChecked == true)
                    {
                        course.Notify = true;
                    }
                    else
                    {
                        course.Notify = false;
                    }
                    using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
                    {
                        connection.Insert(course);
                        MainPage.CoursesList.Add(course);
                        await Navigation.PopModalAsync();
                    }
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(TxtCourseName.Text))
                    {
                        throw new ApplicationException("Course Name is REQUIRED");
                    }

                    if (PickerCourseStatus.SelectedItem == null)
                    {
                        throw new ApplicationException("Course Status is REQUIRED");
                    }

                    if (String.IsNullOrWhiteSpace(DatePickerStartDate.Date.ToString()))
                    {
                        throw new ApplicationException("Start Date is REQUIRED");
                    }

                    if (String.IsNullOrWhiteSpace(DatePickerEndDate.Date.ToString()))
                    {
                        throw new ApplicationException("End Date is REQUIRED");
                    }

                    if (String.IsNullOrWhiteSpace(TxtInstructorName.Text))
                    {
                        throw new ApplicationException("Instructor Name is REQUIRED");
                    }

                    if (String.IsNullOrWhiteSpace(TxtInstructorEmail.Text))
                    {
                        throw new ApplicationException("Instructor Email is REQUIRED");
                    }

                    if (String.IsNullOrWhiteSpace(TxtInstructorPhone.Text))
                    {
                        throw new ApplicationException("Instructor Phone is REQUIRED");
                    }
                }
            }
            catch (ApplicationException ex)
            {
                await DisplayAlert("Warning", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await DisplayAlert("Warning", Title, ex.Message, "OK");
            }
        }

        private bool RequiredCourseEntryPopulated()
        {
            return !String.IsNullOrWhiteSpace(TxtCourseName.Text) &&
                PickerCourseStatus.SelectedItem != null &&
                !String.IsNullOrWhiteSpace(DatePickerStartDate.Date.ToString()) &&
                !String.IsNullOrWhiteSpace(DatePickerEndDate.Date.ToString()) &&
                !String.IsNullOrWhiteSpace(TxtInstructorName.Text) &&
                !String.IsNullOrWhiteSpace(TxtInstructorEmail.Text) &&
                !String.IsNullOrWhiteSpace(TxtInstructorPhone.Text);
        }

        private void ValidateStartAndEndDates()
        {
            if (DatePickerStartDate.Date > DatePickerEndDate.Date)
            {
                string message = "Please check the start and end date!\nSelected start date cannot come after the end date";
                throw new ApplicationException(message);
            }

            if (DatePickerStartDate.Date < DateTime.Today)
            {
                string message = "Start date cannot Be a past date.\nPlease select a future date";
                throw new ApplicationException(message);
            }

            if (DatePickerStartDate.Date < Term.StartDate ||
                DatePickerStartDate.Date > Term.EndDate ||
                DatePickerEndDate.Date > Term.EndDate)
            {
                string message = "Please check the start and end date!\n" +
                    "The course must start end with the Term's dates";
                throw new ApplicationException(message);
            }
        }

        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}