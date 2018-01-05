//Q: 能不能有一个完整的例子来演示数据注解的使用？
//A: 我们仍然以Pluto为例。以下为PlutoDataAnnotations的源代码：

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnnotations
{
    public class Author   //发起者表成员的储存形式(或格式)，以类的形式表示
    {
        public int Id { get; set; }   //Id列，之后EF会自动将此列作为主键
        public string Name { get; set; }    //Name列，储存类型为string，这里指发起者的名字
        public IList<Course> Courses { get; set; }    //Courses列，数据类型为IList<Course>，EF会将其视为指向Course的外键
                                                      //据Course类可知，Author和Course是多对一关系，即一个Course只对应一个发起者，而一个发起者可能发起多个Course
    }
  
    public class Course   //课程表成员的储存形式(或格式)，以类的形式表示
    {
        //以下为表中的列及列的数据类型，以字段的形式储存
        public int Id { get; set; }   //Id列，之后EF会自动将此列作为主键，也可修改，会详细讨论

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }   //Title列，数据类型为string，这里指课程名

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }   //Description列，数据类型为string，这里指课程的描述

        public CourseLevel Level { get; set; }    //Level列，数据类型为CourseLevel枚举类型

        public float FullPrice { get; set; }    //FullPrice列，数据类型为float

        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        public Author Author { get; set; }    //Author列，数据类型为Author，EF会自动将此列视为指向Author的外键(一对一或多对一)，之后会详细讨论

        public IList<Tag> Tags { get; set; }    //Tages列，数据类型为IList<Tag>，EF会自动将此列视为指向Tag的外键(多对多或一对多)，之后会详细讨论
    }
  
    public enum CourseLevel   //枚举数据类型
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }
  
    public class PlutoContext : DbContext   //PlutoContext即域名，继承自DbContext
    {
        //DbSet为一种集合泛型类，往集合中填充表成员类即可声明一个成员类对应的表
        public DbSet<Course> Courses { get; set; }    //Courses表，成员(格式)为Course
        public DbSet<Author> Authors { get; set; }    //Authors表，成员(格式)为Author
        public DbSet<Tag> Tags { get; set; }    //Tags表，成员(格式)为Tag

        public PlutoContext()   //构造器，继承DbContext构造器，参数为在App.Config中声明的所连接数据库的连接名，即connectionString的名字
            : base("name=DefaultConnection")    //如果connectionString的名字域域名想同，那么此参数可以省略
        {

        }
    }
  
    public class Tag    //标签表成员的储存形式(或格式)，以类的形式表示
    {
        public int Id { get; set; }   //Id列，之后EF会自动将此列作为主键
        public string Name { get; set; }    //Name列，储存类型为string，这里指标签名
        public IList<Course> Courses { get; set; }    //Course列，数据类型为IList<Course>，EF会将其视为指向Course的外键
                                                      //据Course类可知，Tag和Course是多对多关系，即一个Course可能拥有多个Tag，而一个Tag可以往多个Course上贴
    }
  
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
