using System;
using System.ComponentModel;

namespace FossTech.Models
{
    public class WebSetting : ITimeStampedModel
    {
        public int Id { get; set; }
        [DisplayName("Banner Slider")]
        public bool Slider { get; set; } = true;

        [DisplayName("Show/Hide Top Navbar")]
        public bool TopNav { get; set; } = true;


        [DisplayName("About Us")]
        public bool AboutUs { get; set; } = true;
        [DisplayName("About Title")]
        public string AboutTitle { get; set; } = "About Us Title";
        [DisplayName("About SubTitle")]
        public string AboutSubTitle { get; set; } = "About Us SubTitle";



        [DisplayName("Categories")]
        public bool Categories { get; set; } = true;
        [DisplayName("Category Title")]
        public string CategoryTitle { get; set; } = "Our Categories Title";
        [DisplayName("Category SubTitle")]
        public string CategorySubTitle { get; set; } = "Our Categories SubTitle";
        [DisplayName("Category Count")]
        public int CategoryCount { get; set; } = 8;



        [DisplayName("Courses")]
        public bool Courses { get; set; } = true;
        [DisplayName("Courses Title")]
        public string CourseTitle { get; set; } = "Our Courses Title";
        [DisplayName("Courses Sub Title")]
        public string CourseSubTitle { get; set; } = "Our Courses SubTitle";
        [DisplayName("Courses Count")]
        public int CoursesCount { get; set; } = 8;



        [DisplayName("Register Now")]
        public bool Register { get; set; } = true;
        [DisplayName("Register Now Title")]
        public string RegisterTitle { get; set; } = "Register Now";
        [DisplayName("Register Now SubTitle")]
        public string RegisterSubTitle { get; set; } = "Register Now";

        [DisplayName("Sections")]
        public bool Section { get; set; } = true;
        [DisplayName("Section Title")]
        public string SectionTitle { get; set; } = "Our Section";
        [DisplayName("Section Sub-Title")]
        public string SectionSubTitle { get; set; } = "Our Section";



        [DisplayName("Our Teacher")]
        public bool Teacher { get; set; } = true;
        [DisplayName("Our Teacher Title")]
        public string TeacherTitle { get; set; } = "Our Teacher";
        [DisplayName("Our Teacher SubTitle")]
        public string TeacherSubTitle { get; set; } = "Our Teacher";
        [DisplayName("Teachers Count")]
        public int TeachersCount { get; set; } = 8;



        [DisplayName("Our Director")]
        public bool Director { get; set; } = true;
        [DisplayName("Our Director Title")]
        public string DirectorTitle { get; set; } = "Our Director";
        [DisplayName("Our Director SubTitle")]
        public string DirectorSubTitle { get; set; } = "Our Director";
        [DisplayName("Directors Count")]
        public int DirectorsCount { get; set; } = 8;




        [DisplayName("Our HOD")]
        public bool HOD { get; set; } = true;
        [DisplayName("Our HOD Title")]
        public string HODTitle { get; set; } = "Our HOD";
        [DisplayName("Our HOD SubTitle")]
        public string HODSubTitle { get; set; } = "Our HOD";
        [DisplayName("HOD Count")]
        public int HODCount { get; set; } = 8;



        [DisplayName("Our Topper")]
        public bool Topper { get; set; } = true;
        [DisplayName("Our Topper Title")]
        public string TopperTitle { get; set; } = "Our Topper";
        [DisplayName("Our Topper SubTitle")]
        public string TopperSubTitle { get; set; } = "Our Topper";
        [DisplayName("Topper Count")]
        public int TopperCount { get; set; } = 8;


        [DisplayName("Placement")]
        public bool Placement { get; set; } = true;
        [DisplayName("Placement Title")]
        public string PlacementTitle { get; set; } = "Placement Title";
        [DisplayName("Placement SubTitle")]
        public string PlacementSubTitle { get; set; } = "Placement SubTitle";
        [DisplayName("Placement Count")]
        public int PlacementCount { get; set; } = 8;



        [DisplayName("FAQ's")]
        public bool FAQs { get; set; } = true;
        [DisplayName("FAQ's Title")]
        public string FAQsTitle { get; set; } = "FAQ's Title";
        [DisplayName("FAQ's SubTitle")]
        public string FAQsSubTitle { get; set; } = "FAQ's SubTitle";
        [DisplayName("FAQs Count")]
        public int FAQsCount { get; set; } = 8;




        [DisplayName("Latest Blog")]
        public bool Blog { get; set; } = true;
        [DisplayName("Latest Blog Title")]
        public string BlogTitle { get; set; } = "Latest Blog Title";
        [DisplayName("Latest Blog SubTitle")]
        public string BlogSubTitle { get; set; } = "Latest Blog SubTitle";
        [DisplayName("Blog Count")]
        public int BlogCount { get; set; } = 8;



        [DisplayName("Latest News")]
        public bool LatestNews { get; set; } = true;
        [DisplayName("Latest News Title")]
        public string LatestNewsTitle { get; set; } = "Latest News Title";
        [DisplayName("Latest News SubTitle")]
        public string LatestNewsSubTitle { get; set; } = "Latest News SubTitle";
        [DisplayName("LatestNews Count")]
        public int LatestNewsCount { get; set; } = 8;



        [DisplayName("Updates")]
        public bool Updates { get; set; } = true;
        [DisplayName("Updates Title")]
        public string UpdatesTitle { get; set; } = "Updates Title";
        [DisplayName("Updates SubTitle")]
        public string UpdatesSubTitle { get; set; } = "Updates SubTitle";
        [DisplayName("Updates Count")]
        public int UpdatesCount { get; set; } = 8;




        [DisplayName("Our Testimonials")]
        public bool Testimonial { get; set; } = true;
        [DisplayName("Our Testimonials Title")]
        public string TestimonialTitle { get; set; } = "Our Testimonials Title";
        [DisplayName("Our Testimonials SubTitle")]
        public string TestimonialSubTitle { get; set; } = "Our Testimonials SubTitle";
        [DisplayName("Testimonials Count")]
        public int TestimonialsCount { get; set; } = 8;



        [DisplayName("Button 1")]
        public bool Button1 { get; set; } = true;
        [DisplayName("Button 1 Title")]
        public string Button1Title { get; set; } = "Button 1 Title";
        [DisplayName("Button 1 URL")]
        public string Button1URL { get; set; } = "Button 1 URL";


        [DisplayName("Button 2")]
        public bool Button2 { get; set; } = true;
        [DisplayName("Button 2 Title")]
        public string Button2Title { get; set; } = "Button 2 Title";
        [DisplayName("Button 2 URL")]
        public string Button2URL { get; set; } = "Button 2 URL";



        [DisplayName("Footer Title 1")]
        public string FooterTitle1 { get; set; } = "Footer Title 1";
        [DisplayName("Footer Title 2")]
        public string FooterTitle2 { get; set; } = "Footer Title 2";
        [DisplayName("Footer Title 3")]
        public string FooterTitle3 { get; set; } = "Footer Title 3";
        [DisplayName("Footer Title 4")]
        public string FooterTitle4 { get; set; } = "Footer Title 4";


        [DisplayName("Enable Under Construction Mode")]
        public bool IsUnderConstruction { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
