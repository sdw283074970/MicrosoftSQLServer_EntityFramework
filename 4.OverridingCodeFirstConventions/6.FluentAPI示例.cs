//Q: 能不能有一个完整的例子来演示FluentAPI的应用？
//A: 我们还是以Pluto数据库为例，建立一个FluentAPI演示项目，结合实际需求改进原始代码。其原始代码FluentAPI如下：

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentAPI
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Course> Courses { get; set; } 
    }
  
    public class Course
    {
        public int Id { get; set; }
        
        public string Title { get; set; }   

        public string Description { get; set; }   

        public CourseLevel Level { get; set; }    

        public float FullPrice { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; }    

        public IList<Tag> Tags { get; set; } 
    }
  
    public enum CourseLevel   
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }
  
    public class PlutoContext : DbContext 
    {
        public DbSet<Course> Courses { get; set; }    
        public DbSet<Author> Authors { get; set; }    
        public DbSet<Tag> Tags { get; set; }    

        public PlutoContext()
            : base("name=DefaultConnection") 
        {

        }
    }
  
    public class Tag    
    {
        public int Id { get; set; }   
        public string Name { get; set; }    
        public IList<Course> Courses { get; set; }    
    }
  
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}

  //其中，我们已经初始化了迁移并建立了数据库，并且在Model转换成数据库的过程中EF使用了默认约定。以下为默认约定下的迁移文件源代码：

    public partial class InitialModel : DbMigration   //初始化的迁移文件，沿用默认约定，只标注需要注意的部分
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),   //默认约定下Title属性将为可空，数据类型为nvarchar(MAX)
                        Description = c.String(),
                        Level = c.Int(nullable: false),
                        FullPrice = c.Single(nullable: false),
                        Author_Id = c.Int(nullable: false),   //默认约定下自动生成一个Author_Id字段而不是AuthorId
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Authors", t => t.AuthorId, cascadeDelete: true)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TagCourses",   //默认约定下自动生成中间表名为TagCourses
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Course_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Course_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)   //连带删除为开
                .ForeignKey("dbo.Courses", t => t.Course_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Course_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagCourses", "Course_Id", "dbo.Courses");
            DropForeignKey("dbo.TagCourses", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.Courses", "AuthorId", "dbo.Authors");
            DropIndex("dbo.TagCourses", new[] { "Course_Id" });
            DropIndex("dbo.TagCourses", new[] { "Tag_Id" });
            DropIndex("dbo.Courses", new[] { "AuthorId" });
            DropTable("dbo.TagCourses");
            DropTable("dbo.Tags");
            DropTable("dbo.Courses");
            DropTable("dbo.Authors");
        }
    }

  //若项目需求为：
    //1.将Title属性设为非空且长度限制为255；
    //2.将Description属性设为非空且长度限制为2000；
    //3.重新配置Author与Courses的关系，将外键命名为AuthorId而不是默认的Author_Id，并且关闭连带删除；
    //4.将Courses与Tags的中间表命名为“CourseTags”而不是默认的“TagCourses”；
    //5.建立一个新类Cover，并赋予Cover与Couse一个一对一关系；

  //我们只能使用FluentAPI来满足以上需求。由于本次项目是CodeFirseWorkflow，所以要注意“一次小改变就做一次迁移”的黄金原则，即每完成一条需求，就做
    //一次迁移。第一条和第二条需求都是在覆写Course类字段，我们可以从Domain类(PlutoContext)中的OnModelCreating()方法同时实现二者，其代码如下：

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
            modelBuilder.Entity<Course>()   //取得Course类的引用
                .Property(c => c.Title)   //选取Course类中的Title字段作为对象
                .IsRequired()   //将对象的是否可空约定覆写为否，即不能为空
                .HasMaxLength(255);   //将对象的长度约定覆写为255
                
            modelBuilder.Entity<Course>()
                .Property(c => c.Description)   //选取Course类中的Description字段作为对象
                .IsRequired()   //将对象的是否可空约定覆写为否，即不能为空
                .HasMaxLength(2000);    //将对象的长度约定覆写为2000
            base.OnModelCreating(modelBuilder);
        }
    }
    
  //完成第一、二条项目需求后我们需要进行一次数据库迁移和更新。在PM中键入add-migration AlterCoursesColumns建立迁移文件，生成代码如下：

    public partial class AlterCoursesColumns : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Courses", "Title", c => c.String(nullable: false, maxLength: 255));    //可以看到覆写Title字段成功
            AlterColumn("dbo.Courses", "Description", c => c.String(nullable: false, maxLength: 2000));   //覆写Description字段成功
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Courses", "Description", c => c.String());
            AlterColumn("dbo.Courses", "Title", c => c.String());
        }
    }
    
  //同步数据库后开始第三条需求。为外键命名的实质是指定一个已命名好的字段作为外键。Author与Course为一对多关系，首先建立起关系才能覆写外键。代码如下：

            //续写在OnModelCreating方法中，此处省略
            modelBuilder.Entity<Course>()   //以Course为起始类
                .HasRequired(c => c.Author)   //一个Course只有一个Author，所以先调用HasRequired()方法
                .WithMany(a => a.Courses)   //一个Author可以有多个Course，所以返回至起始类时调用WithMany()方法
                .HasForeignKey(c => c.AuthorId)   //将AuthorId指定为这个外键
                .WillCascadeOnDelete(false);    //默认约定下连带删除为开，调用WillCascadeOnDelete()方法将其覆写为关闭

  //其中连带删除为：删除两两关系中的一个，另一个也跟着被删除。在PM中键入 daa-migation AlterCoursesColumnsDisableCasecadeDelete 建立新的迁移文件，
    //其生成代码如下：

    public partial class AlterCoursesColumnsDisableCasecadeDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Courses", "Author_Id", "dbo.Authors");    //删除默认外键，cascadeDelete在方法中默认为关(但约定为开)
            AddForeignKey("dbo.Courses", "AuthorId", "dbo.Authors", "Id");    //添加外键，指认AuthorId为外键
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "AuthorId", "dbo.Authors");
            AddForeignKey("dbo.Courses", "AuthorId", "dbo.Authors", "Id", cascadeDelete: true);   //如打开连带删除需要声明
        }
    }
    
  //同步至数据库后可以开始第四条需求。


















