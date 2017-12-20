//Q: 如何在现有的数据库项目中初始化代码优先工作流？
//A: 如果选导入存在的数据库项目同时想使用CodeFirstWorkFlow，EF会自动进行一系列逆向工程建立一个Model，这个Model包括三个部分，一个是connectionString，
  //第二个是用C#中的类表示数据库中表的列，即ColumnModel(属于ModelBuilder)，最后一个是数据库的域，即DbContext。做到这一步相当于完成了前提步骤1-3。
  //导入现有数据库的步骤与数据库优先导入数据库步骤很相似，为：
    //1.右键解决方案，添加一个ADO.NET Entity Data Model并命名；
    //2.在接下来的对话框中选择Code First from Database；
    //3.设置DataSource(Microsoft SQL Server)、服务器名、服务器访问权限以及已存在的数据库名；
    //4.选择需要导入的列表/视图(注意取消勾选_MigrationHistory表，这是EF自动生成用于记录迁移历史的表)，至此导入完成。
  
  //接下来完成代码优先前提步骤4、5即可完成初始化。

//Q: EF的逆向工程创建的Model与我们上一篇手动创建的Model有什么不同？
//A: 功能上是一样的，不过ColumnModel类和域类的内容上有些许差异，而且会更详尽。首先，上一篇我们手动建立的ColumnModel之一的Author类，代码如下：

public class Author   //发起者表成员的储存形式(或格式)，以类的形式表示
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IList<Course> Courses { get; set; }
}

  //看似没有什么问题，但是EF逆向工程生成的Author如下：

public class Author   //发起者表成员的储存形式(或格式)，以类的形式表示
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Course> Courses { get; set; }    //集合类为ICollection<T>而不是IList<T>
  
    public Author()   //多了一个构造器
    {
        Courses = new HashSet<Course>(); 
    }
}

  //首先ICollection<T>和IList<T>都可以，IList<T>继承于ICollection<T>，相比之下IList<T>多了添加/删除元素和停供了索引功能。两者都能使用，看个人喜好。
    //其次多的这个构造器确保了Author的正常初始化，应该是要加上的，在下一部分(OverridingCodeFirstConventions)会详细说明。
  
  //接下来我们来看域类模型(Domain Class Model)，即DbContext类。手写模型代码为：

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

  //而EF逆向工程的域类模型代码为：
  
public partial class PlutoContext : DbContext
{
    public PlutoContext()
        : base("name=PlutoContext")
    {
    }

    public virtual DbSet<Author> Authors { get; set; }
    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }

    //以上部分完全一样，多了以下的部分
    protected override void OnModelCreating(DbModelBuilder modelBuilder)    
    {
        modelBuilder.Entity<Author>()
            .HasMany(e => e.Courses)
            .WithOptional(e => e.Authors)
            .HasForeignKey(e => e.Author_Id);

        modelBuilder.Entity<Course>()
            .HasMany(e => e.Tags)
            .WithMany(e => e.Courses)
            .Map(m => m.ToTable("TagCourses").MapLeftKey("Course_Id").MapRightKey("Tag_Id"));
    }
}

    //这个多出的部分为一个复写方法，复写自定义于基类DbContext中的虚方法OnModelCreating。我们需要靠复写这些虚方法来复写CodeFirstModel中的约定，
      //即Coventions，详细会在下一章OverridingCodeFirstConventions中讨论。在这里多出的部分不会有任何影响。

//暂时想到这么多，最后更新2017/12/20
