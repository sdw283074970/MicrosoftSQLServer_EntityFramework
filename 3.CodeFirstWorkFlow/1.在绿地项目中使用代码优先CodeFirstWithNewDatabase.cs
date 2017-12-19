//Q: 如何使用代码优先工作流搭建应用程序与数据库的关系模型？
//A: 代码优先指数据库随代码变动，本身不对数据库做直接操作。实现这个机制的核心在PackageManager(包管理器)，在PM中使用命令对数据库更新。
  //对于绿地项目，即新项目，数据库同步机制如下：
  
  //前提1：安装EntityFramework。在PM中安装EntityFramework；
  //前提2：建立域。使用C#建立一个域(Domain)代表我们控制的数据库实体范围，并以集合以及类、字段的形式建立数据库中的表/列；
  //前提3：添加数据库连接。在App.Config中声明该解决方案连接的connectionStrings；
  //前提4：启用迁移。在PM中启用Migration(迁移)，在项目中建立Migration文件夹并初始化迁移；
  
  //同步步骤1：建立迁移类文件。在对程序代码做一个变动之后，在PM中使用命令建立迁移类；
  //同步步骤2：调整迁移类文件。按照实际情况对迁移类的Up和Down方法，即更新和还原方法进行编辑；
  //同步步骤3：同步。确认无误后在PM中使用更新命令将数据库与程序同步，此时更新完成。

//Q: 如何用PM安装EntityFramework？
//A: 首先Tools->NuGetPackageManager->PackageManager打开PM，输入命令install-Package EntityFrameWork即可安装最新版EF，也可以在该命令后加入参数
  //命令-Version:X.X.X获取对应版本的EF。

//Q: 如何建立域和表/列？
//A: 首先以类的形式建立表中的成员，一个类即声明了一种表的Model，然后让一个集合拥有这些类，即形成一个表，EF可以识别处并自动建立表关系。
  //本篇以一个课程数据为例，数据库有三个表和一个枚举类型：
  //1.表Course，储存课程信息；
  //2.表Author，储存本课发起人信息；
  //3.表Tag储存每一种课拥有的标签；
  //4.枚举CourseLevel，以文本形式表现了课的等级，如初学者、中级、高级。
  
  //以上四项可以由类表示：

public class Course   //课程表成员的储存形式(或格式)，以类的形式表示
{
    //以下为表中的列及列的数据类型，以字段的形式储存
    public int Id { get; set; }   //Id列，之后EF会自动将此列作为主键，也可修改，会详细讨论
    public string Title { get; set; }   //Title列，数据类型为string，这里指课程名
    public string Description { get; set; }   //Description列，数据类型为string，这里指课程的描述
    public CourseLevel Level { get; set; }    //Level列，数据类型为CourseLevel枚举类型
    public float FullPrice { get; set; }    //FullPrice列，数据类型为float
    public Author Author { get; set; }    //Author列，数据类型为Author，EF会自动将此列视为指向Author的外键(一对一或多对一)，之后会详细讨论
    public IList<Tag> Tags { get; set; }    //Tages列，数据类型为IList<Tag>，EF会自动将此列视为指向Tag的外键(多对多或一对多)，之后会详细讨论
}

public class Author   //发起者表成员的储存形式(或格式)，以类的形式表示
{
    public int Id { get; set; }   //Id列，之后EF会自动将此列作为主键
    public string Name { get; set; }    //Name列，储存类型为string，这里指发起者的名字
    public IList<Course> Courses { get; set; }    //Courses列，数据类型为IList<Course>，EF会将其视为指向Course的外键
    //据Course类可知，Author和Course是多对一关系，即一个Course只对应一个发起者，而一个发起者可能发起多个Course
}

public class Tag    //标签表成员的储存形式(或格式)，以类的形式表示
{
    public int Id { get; set; }   //Id列，之后EF会自动将此列作为主键
    public string Name { get; set; }    //Name列，储存类型为string，这里指标签名
    public IList<Course> Courses { get; set; }    //Course列，数据类型为IList<Course>，EF会将其视为指向Course的外键
    //据Course类可知，Tag和Course是多对多关系，即一个Course可能拥有多个Tag，而一个Tag可以往多个Course上贴
}

public enum CourseLevel   //枚举数据类型
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3
}
  
  //以上为表中的成员格式，以类的方式储存。接下来就是在这些类的基础上建立表。建立表之前需要建立一个域指代数据库，之前DatabaseFirst中XXDbContext.cs
  //就是其连接的数据库的域，在DFW中为EF自动生成，但在CFW中我们得手写。域是一个类，代码如下：

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

//Q: 如何手动设置App.Config中的connectionStrings？
//A: 既然标签叫connectionStrings，那么肯定可以同时为这个项目解决方案连接多个服务器中的数据库。这里我们只手动添加一个，代码如下：

<connectionStrings>
  <add name="DefaultConnection" connectionString="data source=[SERVER NAME]; initial catalog=[PROJECT NAME]; 
    integrated security=SSPI" providerName="System.Data.SqlClient" />
</connectionStrings>

  //与DatabaseFirstWorkflow自动生成的connectionString相比，手写的connectionString简介太多了。add属性主要也分为三个部分，也是简化后的三个部分：
    //1.name属性。如果name与域名相同，则可以省略，EF会根据connectionString中自动推断。如果与域名不同则需要声明，且在域中的构造器也要做相同声明；
    //2.connectionString属性。分为三个小部分：数据源(服务器名)、解决方案名(VS中的项目名)和服务器访问权限。SSPI指Windows访问权限；
    //3.providerName属性。属性声明了谁来做识别工作(EntityFramework)。
    
//Q: 什么是Migration？如何启用和初始化Migration？
//A: Migration即是迁移，指将VS中的C#代码翻译成SQL代码并在数据库中执行。打开PM，输入enable-migrations即可启用迁移，在整个项目开发过程中只用启用
  //一次即可。启用成功后，我们可以在解决方案目录中看到新建的Migrations文件夹，这个文件夹将保存今后每一次的迁移记录，通过正确维护的迁移记录我们可以随时
  //降级数据库版本或者回到之前某一时间点的数据库版本。
  //初始化迁移指将4个同步前提做完后进行的第一次迁移，即在项目中第一次将C#代码转换为SQL生成数据库。打开PM，输入add-migration [MigrationName]即可生成
  //一次迁移文件。第一次[MigrationName]统一保存为InitialModel，即完整输入add-migration InitialModel即生成一个时间戳+InitialModel的迁移文件，即
  //一个迁移类，该文件结构如下：
  
public partial class [MigrationName] : DbMigration
{
    public override void Up()
    {
        //Up方法，即更新方法。EF将检测解决方案模型与数据的不同，然后基于模型与数据库的差异生成一个Up方法，执行后数据库就会根据项目代码的变化而变化。
        //我们也可以在这里植入一些Sql查询语句来对数据库进行操作。
    }

    public override void Down()
    {
        //Down方法，即还原/降级方法。此方法与Up方法无脑相反，即通过调用这个方法来将数据库还原到调用Up方法之前的状态，EF也会自动生成此方法代码。
        //但是请切记，任何在Up方法做过变动后都要检查Down方法，千万要保持Down方法与所有Up方法无脑相反，否则数据库GG。
    }
}
  
  //再了解了迁移类文件结构后我们来看以上课程-发起者-标签例子中的初始化迁移类代码：

public partial class InitialModel : DbMigration   //这是我们在PM中自己设立的迁移类名称
{
    public override void Up()   //Up更新方法
    {
        //这一次初始化迁移建立了三张表，这是主要变化。EF在建立这三张表的同时发现了Courses表和Tags表中的多对多关系，于是EF还会新建一张关系表
        //以下是建立Authors表的代码（EF自动生成）
        CreateTable(    //CreatTable即新建表的方法，可以分析以下参数
            "dbo.Authors",    //参数1即表名
            c => new    //参数2即一个委托，此处传入一个符合委托格式的方法即可(用匿名方法)
                {
                    Id = c.Int(nullable: false, identity: true),  //声明Id列及其数据类型、是否可空以及是否为自动Id
                    Name = c.String(),    //声明Name列及其数据类型
                })
            .PrimaryKey(t => t.Id);   //将Id列设为主键，这里的一切我们都可以按需更改
      
        //以下是建立Courses表的代码（EF自动生成）
        CreateTable(    //重复性的代码就不做赘述
            "dbo.Courses",
            c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Title = c.String(),
                    Description = c.String(),
                    Level = c.Int(nullable: false),
                    FullPrice = c.Single(nullable: false),
                    Author_Id = c.Int(),
                })
            .PrimaryKey(t => t.Id)
            .ForeignKey("dbo.Authors", t => t.Author_Id)    //将Author_Id列设为一个指向表Authors主键的外键
            .Index(t => t.Author_Id);   //为此外键设立索引

        //以下是建立Tags表的代码（EF自动生成）
        CreateTable(
            "dbo.Tags",
            c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
            .PrimaryKey(t => t.Id);

        //由于以上表中存在多对多关系，所以应存在一个中间表。以下为建立多对多关系中间表的代码（自动生成）
        CreateTable(
            "dbo.TagCourses",   //中间表的表名为两个表名的合体
            c => new
                {
                    Tag_Id = c.Int(nullable: false),    //创建Tag_Id列
                    Course_Id = c.Int(nullable: false),   //创建Course_Id列
                })
            .PrimaryKey(t => new { t.Tag_Id, t.Course_Id })   //创建一个联合主键
            .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)   //将列Tag_Id设为指向表Tags主键的外键
            .ForeignKey("dbo.Courses", t => t.Course_Id, cascadeDelete: true)   //将列Course_Id设为指向表Courses主键的外键
            .Index(t => t.Tag_Id)   //将列Tag_Id设为索引类型
            .Index(t => t.Course_Id);   //将列Course_Id设为索引类型

    }

    public override void Down()   //降级方法（自动生成）
    {
        //以下为Up方法的无脑反向执行，请确保这一点
        DropForeignKey("dbo.TagCourses", "Course_Id", "dbo.Courses");
        DropForeignKey("dbo.TagCourses", "Tag_Id", "dbo.Tags");
        DropForeignKey("dbo.Courses", "Author_Id", "dbo.Authors");
        DropIndex("dbo.TagCourses", new[] { "Course_Id" });
        DropIndex("dbo.TagCourses", new[] { "Tag_Id" });
        DropIndex("dbo.Courses", new[] { "Author_Id" });
        DropTable("dbo.TagCourses");
        DropTable("dbo.Tags");
        DropTable("dbo.Courses");
        DropTable("dbo.Authors");
    }
}















