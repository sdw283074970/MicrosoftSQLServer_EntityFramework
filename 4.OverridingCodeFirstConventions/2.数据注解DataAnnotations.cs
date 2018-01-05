//Q: 什么是数据注解？
//A: 数据注解(DataAnnotations)是指在Model中使用特性标签(Attributes)来复写约定的方法。

//Q: 如何使用数据注解？
//A: 在Model中对应的对象、字段、类之前打上特性标签即可完成复写。这些特性标签位于两个命名空间中，分别是：

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

  //以之前的Pluto数据库为例，在Course类中有一个字段为Title，代码如下：

    public class Course   //课程表成员的储存形式(或格式)，以类的形式表示
    {
        public string Title { get; set; }
    }

  //Title为string类型，在EF默认的约定下string类型的字段映射到数据库中的类型为nvarchar(MAX)，且默认为可空。如果我们想在数据库中限制Title的长度，并且
    //不想让其自动成可空类型，我们就可以使用数据注解来改写这一约定，代码如下：

    public class Course
    {
        [Required]    //复写约定为非可空类型
        [MaxLength(255)]    //复写约定为长度255的nvarchar类型
        public string Title { get; set; }
    }

  //以上即是数据注解的用法。

//Q: 关于数据注解的特性标签都有哪些？都怎么用？效果是什么？
//A: 1.表名(TableNames)类特性。默认约定:表名为类名的复数，如基于Course类的表将自动命名为Courses。特性标签：[Table("TableName")]。用法：

    [Table("tbl_Course")]    //复写效果：Model映射到数据库的表名为"tbl_Course"
    publlic class Course
      
  //2.列名(ColumnNames)类特性。默认约定：列名与与字段名相同，如Course类中的Title字段在表中被命名为Title。特性标签：[Column("ColumnName")]。用法：
      
    [Column("sTitle")]    //复写效果：Model映射到数据库的列名为"sTitle"
    publlic string Title { get; set; }
