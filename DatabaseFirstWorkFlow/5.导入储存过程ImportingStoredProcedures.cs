//Q: 什么是储存过程？
//A: 储存过程(Stored Procedure)可以看作是高级的可以接受参数的查询，集成很多SQL语句为一体，与函数/方法类似，其中有输入输出参数、可以声明变量、可以有逻
  //辑，可以直接调用。提到储存过程就要提到触发器，储存过程与触发器的关系就像是C#中方法与事件的关系，储存过程可以直接调用，触发器由满足条件后触发作用。

//Q: 储存过程是在哪写的？
//A: 在数据库中储存过程放在数据库中的可编程序文件夹中的Store Procedure中，为一个.sql查询文件。

//Q: 如何通过EF导入储存过程？导入后储存过程又放在哪里？
//A: 同样使用数据库更新/同步程序(Updat Wizard)导入。右键点击edmx视图中空白处，选择Update from Database打开更新程序，在“添加”选项卡中勾选“储存过程
  //和函数(Stored Procedure and Functions)”即可选中数据库中的储存过程和函数，可以按需导入，注意一定要勾选“将储存过程和函数导入到EF中”，否则储存过程
  //和函数将不可调用(我也不知为何有如此多余选项)，最后点完成即可。

  //之前的章节有说过，StorageModel是数据库在程序中的具体表现形式，任何数据库的内容都会在SotrageModel中有一个映射，所以导入的储存过程也会在
  //StorageModel中有呈现，具体在Model视图中的XXX.Store就能查看导入的储存过程和函数(右键edmx空白处选择Model Browser)。

  //同样，在ConceptualModel中也可以看到这些储存过程/函数的呈现(如果没有勾选“将储存过程和函数导入到EF中”就在ConceptualModel中看不到)，并且，主程序就是
  //调用的这里的储存过程/函数。

//Q: 导入的储存过程/函数既然存在ConceptualModel中，是否也有C#版本的代码？
//A: 有的。在我们保存edmx后，EF会将这些储存过程翻译为C#代码储存在DB Context类中。DB Context是什么？在哪？DB Context就是之前说的自动工具模板生成的
  //数据库的主类，存在于.edmx文件下的XX.Context.TT中的XX.Context.cs中。这个类名自动生成为XXDbContext(XX为数据库名)，继承DbContext类，导入的储存
  //过程和函数以C#方法的形式储存在XXDbContext类中。如：

public partial class PlutoDbContext : DbContext
{
    public PlutoDbContext()
        : base("name=PlutoDbContext")
    {
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)  //这应该是一个触发器
    {
        throw new UnintentionalCodeFirstException();
    }

    //以下为本来就生成的表的C#代码
    public virtual DbSet<Author> Authors { get; set; }
    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<CourseSection> CourseSections { get; set; }
    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<tblUser> tblUsers { get; set; }

    //以下为自动生成的储存方法/函数的C#版本代码，以方法的形式储存
    
    public virtual int DeleteCourse(Nullable<int> courseID)   //比如这是一个删除Courses表中一个对象的储存过程
    {
        var courseIDParameter = courseID.HasValue ?
            new ObjectParameter("CourseID", courseID) :
            new ObjectParameter("CourseID", typeof(int));

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteCourse", courseIDParameter);
    }

    [DbFunction("PlutoDbContext", "funcGetAuthorCourses")]
    public virtual IQueryable<funcGetAuthorCourses_Result> funcGetAuthorCourses(Nullable<int> authorID)//查询课程作者的名字函数
    {
        var authorIDParameter = authorID.HasValue ?
            new ObjectParameter("AuthorID", authorID) :
            new ObjectParameter("AuthorID", typeof(int));

        return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<funcGetAuthorCourses_Result>("[PlutoDbContext].
          [funcGetAuthorCourses](@AuthorID)", authorIDParameter);
    }

    public virtual ObjectResult<GetCoursesResult> GetCourses()  //查询课程的储存过程
    {
        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetCoursesResult>("GetCourses");
    }

    public virtual int InsertCourse(Nullable<int> authorID, string title, string description, Nullable<short> price, string levelString, 
      Nullable<byte> level)//添加课程的储存过程
    {
        var authorIDParameter = authorID.HasValue ?
            new ObjectParameter("AuthorID", authorID) :
            new ObjectParameter("AuthorID", typeof(int));

        var titleParameter = title != null ?
            new ObjectParameter("Title", title) :
            new ObjectParameter("Title", typeof(string));

        var descriptionParameter = description != null ?
            new ObjectParameter("Description", description) :
            new ObjectParameter("Description", typeof(string));

        var priceParameter = price.HasValue ?
            new ObjectParameter("Price", price) :
            new ObjectParameter("Price", typeof(short));

        var levelStringParameter = levelString != null ?
            new ObjectParameter("LevelString", levelString) :
            new ObjectParameter("LevelString", typeof(string));

        var levelParameter = level.HasValue ?
            new ObjectParameter("Level", level) :
            new ObjectParameter("Level", typeof(byte));

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertCourse", authorIDParameter, titleParameter, descriptionParameter, priceParameter, levelStringParameter, levelParameter);
    }

    public virtual int UpdateCourse(Nullable<int> courseID, string title, string description, string levelString, Nullable<byte> level)
    //更新课程的储存过程
    {
        var courseIDParameter = courseID.HasValue ?
            new ObjectParameter("CourseID", courseID) :
            new ObjectParameter("CourseID", typeof(int));

        var titleParameter = title != null ?
            new ObjectParameter("Title", title) :
            new ObjectParameter("Title", typeof(string));

        var descriptionParameter = description != null ?
            new ObjectParameter("Description", description) :
            new ObjectParameter("Description", typeof(string));

        var levelStringParameter = levelString != null ?
            new ObjectParameter("LevelString", levelString) :
            new ObjectParameter("LevelString", typeof(string));

        var levelParameter = level.HasValue ?
            new ObjectParameter("Level", level) :
            new ObjectParameter("Level", typeof(byte));

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateCourse", courseIDParameter, titleParameter, 
          descriptionParameter, levelStringParameter, levelParameter);
    }
}

//Q: 如何在程序中调用这些储存过程？
//A: 首先实例化XXDbContext类，通过实例调用这些方法就行了。

//暂时想到这么多，最后更新2017/12/18
