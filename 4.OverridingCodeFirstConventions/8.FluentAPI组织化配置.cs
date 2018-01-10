//Q: 什么是组织化配置？
//A: 在实际生产中，我们可能需要写大量的覆写配置在Domain类中，这样体积过于庞大不利于维护。我们可以把这些覆写配置根据Entity<T>的引用进行收集，
  //然后新建一个类来储存这些配置，最后在Domain类中调用即可。我们称收集覆写配置为组织化。

//Q: 为什么要组织化？
//A: 最大的好处是易于维护，让代码调理更加清晰。

//Q: 如何组织化？
//A: 原理为将相同引用收集起来放在新建的类即可。这里以之前的FluentAPI为例，完整的FluentAPI中Domain类源代码如下：

    public class PlutoContext : DbContext
    {
        public DbSet<Course> Courses { get; set; } 
        public DbSet<Author> Authors { get; set; } 
        public DbSet<Tag> Tags { get; set; } 

        public PlutoContext() 
            : base("name=DefaultConnection") 
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Course>()
                .Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);

            modelBuilder.Entity<Course>()
                .HasRequired(c => c.Author)
                .WithMany(a => a.Courses)
                .HasForeignKey(c => c.AuthorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Tags)
                .WithMany(t => t.Courses)
                .Map(m =>
                {
                    m.ToTable("CourseTags");
                    m.MapLeftKey("CourseId");
                    m.MapRightKey("TagId");
                });

            modelBuilder.Entity<Course>()
                .HasRequired(c => c.Cover)
                .WithRequiredPrincipal(c => c.Course);
            
            base.OnModelCreating(modelBuilder);
        }
    }

  //可以看到以上都是以Course类为引用经行覆写。我们可以把这些涉及到Course类的覆写收集起来，放在一个叫CourseConfigurationl的类中，代码如下：
  
    class CourseConfigurationl : EntityTypeConfiguration<Course>
    {
        public CourseConfigurationl()
        {
            //由于这个类继承自EntityTypeConfiguration<Course>，就可以省略所有的ModelBuilder.Entity<Course>，直接调用方法
            //其他方法不变
            Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(255);

            Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(2000);

            HasRequired(c => c.Author)
            .WithMany(a => a.Courses)
            .HasForeignKey(c => c.AuthorId)
            .WillCascadeOnDelete(false);

            HasMany(c => c.Tags)
            .WithMany(t => t.Courses)
            .Map(m =>
            {
                m.ToTable("CourseTags");
                m.MapLeftKey("CourseId");
                m.MapRightKey("TagId");
            });

            HasRequired(c => c.Cover)
            .WithRequiredPrincipal(c => c.Course);
        }
    }

//Q: 这样看起来也很乱，关于配置的具体方法有没有顺序约定先写哪些后写哪些？
//A: 全凭个人喜好。比如可以遵循这样一套约定：
  //关于配置文件类的梳理，我们遵循“表名-主键-列属性-关系”的顺序来覆写配置，覆写后的代码如下：

    class CourseConfigurationl : EntityTypeConfiguration<Course>
    {
        public CourseConfigurationl()
        {
            Totable("tbl_Course");    //覆写表名
            
            HasKey(c => c.Id);    //覆写主键
            
            Property(c => c.Title)    //覆写列属性
            .IsRequired()
            .HasMaxLength(255);

            Property(c => c.Description)    //覆写列属性
            .IsRequired()
            .HasMaxLength(2000);

            HasRequired(c => c.Author)    //覆写一对多关系
            .WithMany(a => a.Courses)
            .HasForeignKey(c => c.AuthorId)
            .WillCascadeOnDelete(false);

            HasMany(c => c.Tags)    //覆写多对多关系
            .WithMany(t => t.Courses)
            .Map(m =>
            {
                m.ToTable("CourseTags");
                m.MapLeftKey("CourseId");
                m.MapRightKey("TagId");
            });

            HasRequired(c => c.Cover)   //覆写一对一关系
            .WithRequiredPrincipal(c => c.Course);
        }
    }

//Q: 如何在Domain类中调用配置类？
//A: 直接在Domain类中的OnModelCreating()方法中调用ModelBuilder类中的Configurations()方法中的Add()方法就行了，即：

            modelBuilder.Configurations.Add(EntityTypeConfiguration<TEntityType> entityTypeConfiguration);

  //Add签名中的EntityTypeConfiguration<TEntityType> entityTypeConfiguration就是配置文件类的实例，直接传入目标配置类的实例即可。如：

            modelBuilder.Configurations.Add(new CourseConfigurationl());

  //完整的OnModelCreating()方法代码为：

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CourseConfigurationl());
        }

//暂时想到这么多，最后更新2018/01/09
