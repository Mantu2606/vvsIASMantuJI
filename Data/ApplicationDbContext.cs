using System;
using FossTech.Models.Menu;
using FossTech.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FossTech.Models.ProductModels;
using System.Threading.Tasks;
using System.Threading;
using FossTech.Models.StudentModels;
using FossTech.Models.StudyMaterialModels;
using FossTech.Models.TestPaperModels;
using System.Reflection.Emit;

namespace FossTech.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<AboutUs> AboutUs { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<SliderImage> SliderImages { get; set; }

        public DbSet<Banner> Banners { get; set; }

        public DbSet<RegistrationBanner> RegistrationBanners { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Franchise> Franchises { get; set; }
        public DbSet<ProdutFaq> ProdutFaqs { get; set; }
        public DbSet<BotAttempt> BotAttempts { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Update> Updates { get; set; }
        public DbSet<LatestNews> LatestNews { get; set; }
        public DbSet<Highlighter> Highlighters { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<WhatsappClickCount> WhatsappClickCounts { get; set; }
        public DbSet<CallClickCount> CallClickCounts { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Placement> Placements { get; set; }

        public DbSet<MultipleAddress> MultipleAddresses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Logo> Logos { get; set; }
        public DbSet<Seo> Seo { get; set; }
        public DbSet<KeyWord> KeyWords { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<FontChange> FontChanges { get; set; }
        public DbSet<ColorCombination> ColorCombinations { get; set; }

        public DbSet<FeedBackForm> FeedBackForms { get; set; }
        public DbSet<BusinessProfile> BusinessProfile { get; set; }
        public DbSet<Branch> Branches { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public DbSet<MenuProduct> MenuProducts { get; set; }
        public DbSet<MenuPage> MenuPages { get; set; }

        public DbSet<MenuCategory> MenuCategories { get; set; }

        public DbSet<MenuSubCategory> MenuSubCategories { get; set; }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<OurTopper> OurToppers { get; set; }
        public DbSet<OurDirector> OurDirectors { get; set; }
        public DbSet<OurHOD> OurHODs { get; set; }



        public DbSet<Review> Reviews { get; set; }
        public DbSet<NewsSubscriber> NewsSubscribers { get; set; }

        public DbSet<WishList> WishList { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductSubCategory> ProductSubCategories { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }

        public DbSet<SellerApplication> SellerApplications { get; set; }

        public DbSet<Collection> Collections { get; set; }

        public DbSet<CollectionProduct> CollectionProducts { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Video> Videos { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<HeaderCode> HeaderCodes { get; set; }

        public DbSet<FooterCode> FooterCodes { get; set; }

        public DbSet<WarrantyRegistration> WarrantyRegistrations { get; set; }

        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<DemoLecturesEnquiry> DemoLecturesEnquiries { get; set; }
        public DbSet<DemoLectureCoursesList> DemoLectureCoursesLists { get; set; }
        public DbSet<CoursesEnquiry> CoursesEnquiries { get; set; }
        public DbSet<ProductEnquiry> ProductEnquiries { get; set; }
        public DbSet<ServiceEnquiry> ServiceEnquiries { get; set; }
        public DbSet<FranchiseEnquiry> FranchiseEnquiries { get; set; }

        public DbSet<LoginPageImage> LoginPageImages { get; set; }

        public DbSet<BankAccount> BankAccount { get; set; }

        public DbSet<Theme> Themes { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<WebSetting> WebSettings { get; set; }
        public DbSet<StudyMaterial> StudyMaterials { get; set; }
        public DbSet<StudyMaterialEnquiry> studyMaterialEnquiries { get; set; }
        public DbSet<StudyMaterialPDF> StudyMaterialPDFs { get; set; }
        public DbSet<FlashCard> FlashCards { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Standard> Standards { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<TestPaper> TestPapers { get; set; }
        public DbSet<TestPaperPDF> TestPaperPDFs { get; set; }
        public DbSet<UserStudyMaterialAccess> UserStudyMaterialAccesses { get; set; }
        public DbSet<UserFlashCardAccess> UserFlashCardAccessess { get; set; }  

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>()
              .HasMany(p => p.SubCategories)
              .WithOne(t => t.Category)
              .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<StudentCourse>()
              .HasOne(p => p.User)
              .WithMany(t => t.Courses)
              .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentCourse>()
              .HasOne(p => p.Board)
              .WithMany()
              .HasForeignKey(p => p.BoardId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<StudentCourse>()
              .HasOne(p => p.Standard)
              .WithMany()
              .HasForeignKey(p => p.StandardId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<StudentCourse>()
              .HasOne(p => p.Product)
              .WithMany()
              .HasForeignKey(p => p.ProductId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Product>()
            .HasOne(p => p.Section)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Image>()
         .HasOne(i => i.Section)
         .WithMany(s => s.Images)
         .HasForeignKey(i => i.SectionId)
         .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudyMaterial>()
         .HasOne(sm => sm.Chapter)
         .WithMany()
         .HasForeignKey(sm => sm.ChapterId)
         .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Board>()
            .HasMany(b => b.Standards)
            .WithOne(s => s.Board)
            .HasForeignKey(s => s.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Standard>()
                .HasMany(s => s.Subjects)
                .WithOne(sub => sub.Standard)
                .HasForeignKey(sub => sub.StandardId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Subject>()
                .HasMany(s => s.Chapters)
                .WithOne(c => c.Subject)
                .HasForeignKey(c => c.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);

            //builder.Entity<Order>()
            //    .HasOne(a => a.User)
            //    .WithMany(a => a.Orders)
            //    .HasForeignKey(a => a.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<VariantOption>()
            //    .HasOne(e => e.Option)
            //    .WithMany()
            //    .HasForeignKey(x => x.OptionId)
            //    .OnDelete(DeleteBehavior.Restrict);


            //builder.Entity<VariantOption>()
            //   .HasOne(e => e.OptionValue)
            //   .WithMany()
            //   .HasForeignKey(x => x.OptionValueId)
            //   .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<VariantOption>()
            //   .HasOne(e => e.Product)
            //   .WithMany()
            //   .HasForeignKey(x => x.ProductId)
            //   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("getdate()");

            builder.Entity<Post>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("getdate()");


            //builder.Entity<Order>()
            //    .Property(o => o.Date)
            //    .HasDefaultValueSql("getdate()");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var newEntities = this.ChangeTracker.Entries()
                .Where(
                    x => x.State == EntityState.Added &&
                         x.Entity != null &&
                         x.Entity as ITimeStampedModel != null
                )
                .Select(x => x.Entity as ITimeStampedModel);

            var modifiedEntities = this.ChangeTracker.Entries()
                .Where(
                    x => x.State == EntityState.Modified &&
                         x.Entity != null &&
                         x.Entity as ITimeStampedModel != null
                )
                .Select(x => x.Entity as ITimeStampedModel);

            foreach (var newEntity in newEntities)
            {
                newEntity.CreatedAt = DateTime.UtcNow;
                newEntity.LastModified = DateTime.UtcNow;
            }

            foreach (var modifiedEntity in modifiedEntities)
            {
                modifiedEntity.LastModified = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
