﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGU_C971.Models;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGU_C971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseDetailPage : ContentPage
    {
        public MainPage MainPage;
        public SchoolCourse Course;
        public SchoolTerm Term;
        public CourseDetailPage CourseHomeDetailPage;

        public CourseDetailPage(MainPage mainPage, SchoolTerm term, SchoolCourse course)
        {
            InitializeComponent();
            MainPage = mainPage;
            Term = term;
            Course = course;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Course.Notify == true)
            {
                notifBox.IsChecked = true;
            }
            else
            {
                notifBox.IsChecked = false;
            }
            TxtCourseName.Text = Course.Name;
            PickerCourseStatus.SelectedItem = Course.Status;
            DatePickerStartDate.Date = Course.StartDate;
            DatePickerEndDate.Date = Course.EndDate;
            TxtInstructorName.Text = Course.InstructorName;
            TxtInstructorEmail.Text = Course.InstructorEmail;
            TxtInstructorPhone.Text = Course.InstructorPhone;
            TxtNotes.Text = Course.Note;
        }
        private async void BtnShareNote_Clicked(object sender, EventArgs e)
        {
            await ShareNotes();
        }

        private async void BtnSaveCourse_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (RequiredCourseFieldsPopulated())
                {
                    ValidateStartAndEndDates();

                    Course.Name = TxtCourseName.Text;
                    Course.Status = PickerCourseStatus.SelectedItem.ToString();
                    Course.StartDate = DatePickerStartDate.Date;
                    Course.EndDate = DatePickerEndDate.Date;
                    Course.InstructorName = TxtInstructorName.Text;
                    Course.InstructorEmail = TxtInstructorEmail.Text;
                    Course.InstructorPhone = TxtInstructorPhone.Text;
                    Course.Note = TxtNotes.Text;

                    if (notifBox.IsChecked == true)
                    {
                        Course.Notify = true;
                    }
                    else
                    {
                        Course.Notify = false;
                    }

                    using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
                    {
                        connection.Update(Course);
                    }
                    await Navigation.PopModalAsync();
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
                        throw new ApplicationException("Instructor Phone number is REQUIRED");
                    }
                }
            }
            catch (ApplicationException ex)
            {
                await DisplayAlert("Warning", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await DisplayAlert("Error", "An Error Occured.", "OK");
            }
        }

        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void BtnAssessments_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new CourseAsssessments(MainPage, Course, CourseHomeDetailPage));
        }

        private async void BtnDelete_Clicked(object sender, EventArgs e)
        {
            try
            {
                string message = $"Are you sure you want to delete { Course.Name }";
                bool response = await DisplayAlert("Warning", message, "Yes", "No");
                if (response)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
                    {
                        connection.Delete(Course);

                        await DisplayAlert("Success", $"{Course.Name} Was Successfully deleted.", "OK");
                    }

                    await Navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await DisplayAlert("Error", "An Exception Error Occured", "OK");
            }
        }

        private bool RequiredCourseFieldsPopulated()
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

        private async Task ShareNotes()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Subject = $"{Course.Name} Notes",
                Title = "Sharing Notes",
                Text = Course.Note
            });
        }
    }
}