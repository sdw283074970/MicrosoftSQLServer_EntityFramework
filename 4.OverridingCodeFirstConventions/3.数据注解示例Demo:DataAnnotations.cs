//Q: 能不能有一个完整的例子来演示数据注解的使用？
//A: 我们仍然以Pluto为例。以下为PlutoDataAnnotations的源代码：

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnnotations
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

  //其中，我们已经初始化了迁移并建立了数据库。我们可以看到，在Model转换成数据库的过程中EF使用了默认约定。如表Courses中Title、Decription的数据类型
    //在Model中为String，在数据库中就为nvarchar(MAX)，且都允许为空；表Courses的一个指向表Authors的外键也允许为空，且名字为Author_Id。

  //若项目需求为：
    //1.将Title的长度改为255；
    //2.将Description的长度改为2000；
    //3.不允许Title和Description出现空值；
    //4.将表Courses中指向表AUthors的外键命名为AuthorId且不能为空。
  //我们可以使用数据注解复写约定来满足项目需求。代码如下：

    public class Course
    {

        public int Id { get; set; }   

        [Required]    //复写约定，将Title在数据库中设为不可空
        [MaxLength(255)]    //复写约定，将Title在数据库中的数据类型设为MaxLength(255)
        public string Title { get; set; }   

        [Required]    //复写约定，将Description在数据库中设为不可空
        [MaxLength(2000)]   //复写约定，将Description在数据库中的数据类型设为MaxLength(2000)
        public string Description { get; set; }   
      
        public CourseLevel Level { get; set; } 
      
        public float FullPrice { get; set; }   

        [ForeignKey("Author")]    //将该字段设为指向Authors表中Author的外键，因为int类型本来就不能为空，所以数据库中也不能为空
        public int AuthorId { get; set; }   //建立一个名为AuthorId的字段

        public Author Author { get; set; }    

        public IList<Tag> Tags { get; set; }    
    }

//Q: 数据注解有没有什么限制或缺点？
//A: 有。一个缺点是数据注解无法自动随引用的名字的变更而变更，若一个引用名字改变，则该注解将失效。如

    public class Course
    {
        [ForeignKey("Author")]    //“Author”与字段Author连接，若字段Author更名，该注解将不会跟着更改。后果是抛出异常，复写失败。
        public int AuthorId { get; set; }

        public Author Author { get; set; } 
    }

    //另一个缺点是，数据注解无法复写有关于中间表的约定。如Courses表和Tags表为多对多关系，会生成一个中间表，默认约定该中间表的名字为TagCourses。
      //若我们想复写这个约定，那么只能用FluentAPI。

//暂时想到这么多，最后更新2018/01/05
