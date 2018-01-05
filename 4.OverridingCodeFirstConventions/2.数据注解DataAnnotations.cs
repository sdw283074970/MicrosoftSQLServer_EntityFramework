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
//A: 1.表名(TableNames)特性。默认约定:表名为类名的复数，如基于Course类的表将自动命名为Courses。特性标签：[Table("Name")]。用法：

    [Table("tbl_Course")]    //复写效果：Model映射到数据库的表名为"tbl_Course"
    publlic class Course
      
  //2.列名(ColumnNames)特性。默认约定：列名与字段名相同，如Course类中的Title字段在表中被命名为Title。特性标签：[Column("Name", TypeName)]。用法：
      
    [Column("sTitle", TypeName = "varchar")]    //复写效果：Model映射到数据库的列名为"sTitle"，类型改为varchar。其中参数都为可选
    publlic string Title { get; set; }

  //3.主键(PrimaryKey)特性。默认约定：将名字为Id或含有Id的字段设为主键。特性标签：[Key][DatabaseGenerated(DatabaseGeneratedOption)]。用法：

    [key]   //复写效果：将该字段设为主键
    [DatabaseGenerated(DatabaseGeneratedOption.None)]   //复写效果：设定是否数据库自动生成值，有三个枚举，分别为None, Identity, Computed
      //None为关闭自动生成值；Identity将自动生成Int类型的唯一序号；Computed将生成组合值
    public string ISBN { get; set; }

  //4.组合键(CompositeKeys)特性。默认约定：无。特性标签：[Key][Column(Order = ?)]。用法：

    [Key]   //复写效果：将该字段设为组合键之一
    [Column(Order = 1)]   //复写效果：将该键的序号设为1
    public int OrderId { get; set; }

    [Key]   //复写效果：将该字段设为组合键之一
    [Column(Order = 2)]   //复写效果：将该键的序号设为2
    public int OrderItemId { get; set; }

  //5.空(Nulls)特性。默认约定：若字段为可空类型则映射列也为可空类型。特性标签：[Required]。用法：

    [Required]    //复写效果：将该字段设为不可空
    public string Title { get; set; }

  //6.字符串长度(LengthOfStrings)特性。默认约定：最大值。特性标签[MaxLength(?)]。用法：

    [MaxLength(255)]    //复写效果：将该字段长度的最大值设为255
    public string Name { get; set; }

  //7.索引[Index]。默认约定：无。特性标签：[Index(IsUnique = ?)]。用法：

    [Index(IsUnique = true)]   //复写效果：将该字段设为唯一索引列
    public int AuthorId { get; set; }
    
    //如果索引包含多个列，那么应该在多个字段前加上索引特性即序号。特性标签：[Index("IX_IndexName", ?)]。用法：

    [Index("IX_AuthorStudentsCount", 1)]    //复写效果：将该字段设为索引"IX_AuthorStudentsCount"的第一个
    public int AuthorId { get; set; }

    [Index("IX_AuthorStudentsCount", 2)]    ////复写效果：将该字段设为索引"IX_AuthorStudentsCount"的第二个
    public int StudentsCount { get; set; }

//8.外键(ForeignKeys)特性。默认：在表中添加名为“外表列名_Id”的列并设其为外键。特性标签：[ForeignKey("外键Id")]或[ForeignKey("指定列名")]。用法：

  //我们通过特性标签来为指定列赋予外键的职能，即自己为外键列命名
  public class Course
  {
      public int AuthorId { get; set; }   //自己建立一个字段并将其命名为理想的名字
    
      [ForeignKey("AuthorId")]    //复写效果：将指定的AuthorId列设为外键，并通过Author列连接Authors表
      public Author Author { get; set; }
  }

  //或者
  public class Course
  {
      [ForeignKey("Author")]    //复写效果：将该字段设为外键，并通过Author列连接Authors表
      public int AuthorId { get; set; }   //自己建立一个字段并将其命名为理想的名字
    
      public Author Author { get; set; }
  }

//暂时想到这么多，最后更新2018/01/04
