//本篇示例基于之前的CodeFirstExistingDatabase项目。原始文件可参考本章《2.在现有项目中使用代码优先CodeFirstWithExistingDatabase.cs》。

//Q: 如何在我们的Model中添加类？
//A: 正常添加类即可。如在示例中添加一个Category类，按ctrl+shift+A组合快捷键新建一个类文件如下：

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}

//Q: 如何同步这个类到数据库中？
//A: 按照迁移步骤建立迁移文件——按需编辑迁移文件——更新数据库即可完成同步。依照迁移黄金法则，每做一个小变动即要执行一次迁移，这里新建了一个类，就需要做一
  //迁移。建立迁移文件、完成同步都是在PM中完成，如在此例中，需要在PM中输入add-migration AddCategoriesTable即可建立一个叫做AddCategoryTable的迁移
  //文件。打开迁移文件我们可以发现EF生成了一个空的迁移文件，即Up方法和Down方法都为空。导致这样的原因是，类在数据库中代表的是表中的成员，单独是没有表现
  //的，所以EF没法识别。要想能在数据库中看到这个类，必须在数据库中有一个表能容纳这个类，如在域PlutoDbContext中新建一个叫做Category的表：

public partial class PlutoContext : DbContext
{
    public PlutoContext()
        : base("name=PlutoContext")
    {
    }

    public virtual DbSet<Author> Authors { get; set; }
    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<Category> Categories { get; set; }   //在数据库中新建一个表

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

    //此时再重新建立一次迁移文件PM会告诉我们有一个迁移文件(即之前生成的空迁移文件)正等待更新至数据库，更新前无法建立新迁移文件。这时我们就选哟在PM中
    //生成命令后面加上-Force强行覆盖，EF就能感知到Model对数据库的影响，生成如下覆盖后的迁移文件：
    
public partial class AddCategoriesTable : DbMigration   //AddCategoriesTable即我们在PM中的命名
{
    public override void Up()   //Up更新方法，这个方法包括一个新建表格的方法和两条Sql查询语句
    {
        CreateTable(    //建立表格方法
            "dbo.Categories",   //参数1，表名
            //参数2，一个Func<ColumnBuilder, 'a>委托，需能返回一个['a]匿名类参数的方法。这里这个类型为new {ColumnModel Id, ColumnModel Name}，
            //是根据DbSet<Category>中的Category而得来的，即如果Category类没有Name属性，那么此时的['a]就为 new {ColumnModel Id}
            c => new    //c即为ColumnBuilder，这个匿名方法分别调用了ColumnBuilder类中的Int方法和String方法来分别建立两个列
                {
                    Id = c.Int(nullable: false, identity: true),    //调用了ColumnBuilder类中的Int方法建立数据类型为Int的Id列
                    Name = c.String(),    //调用了ColumnBuilder类中的String方法建立数据类型为String的Name列
                })
            .PrimaryKey(t => t.Id);   //设置主键的方法。这里的t指PrimaryKey方法中Func<TColumns, object>中的TColumns

        //此例中我们需要为这个表插入一些表值，可以在这个Up方法中一并完成
        Sql("INSERT INTO Categories VALUES ('Web Development')");   //使用Sql方法即可在其中写任何Sql查询语句，这里为插入表值
        Sql("INSERT INTO Categories VALUES ('Xamarin Development')");
    }

    public override void Down()
    {
        DropTable("dbo.Categories");    //无脑与Up方向相反即可。这里不用先删除插入的表值，因为直接删除表必然会删除表值。当然先删除表值也没毛病。
    }
}

  //迁移文件生成完毕，按需修改也完毕，保存该文件后即可同步更新数据库了。在PM中输入update-database即可完成更新，这时候我们就能在数据库中看到多了一个
    //叫做Categotries的表，并且表中有两行数据，即Id为1的Web Development和Id为2的Xamarin Development。 

//Q: 迁移文件的命名有没有什么讲究？
//A: 有。建立迁移文件过程中，有一个为迁移文件命名共识，即大家公认的命名规则，之前将初始化的迁移文件命名为Initial也是共识的一部分。
  //这个规则有两种命名方式，即：
  //1.以模型为中心命名(ModelCentric)。格式为:[行为动词]+[对象]，如此例中按照这个方法迁移文件应命名为AddCategory；
  //2.以数据库为中心命名(DatabaseCentric)。格式为:[行为动词]+[对象表]，如此例中的AddCategoryTable。

//Q: 哪一种命名方法好？
//A: 既然采用CodeFirst，理论上应该用以模型为中心的命名方式，但是第二种适用性更广。在有些情况下，我们需要对数据库做出的变动在Model中并没有具体的表现。
  //如，当我们想建立一个触发器或更改一个储存过程、视图的时候，这些操作在Model中并没有具体体现，因此很难用ModelCentric命名。所以统一用DatabaseCentric
  //比较稳妥，但也需要知道ModelCentric，毕竟肯定有其他程序员这么写。

//Q: 如果需要向已经存在的表添加新的列该如何操作？
//A: 首先在Model中对应类中修改添加我们要的字段，然后执行迁移三步骤即可。但是需要注意的是，在已有表值中添加新的列，一定要确保该列数据类型为可空类型，
  //否则会抛出异常。即使我们不想让该列为可空类型，我们可以先设定可空，然后填充如数据，再改成不可空即可。或者一开始就填入一些没有意义的数据，再做更改。
  //如此例中，我们向Courses列表中添加一个Category列，需要首先在Course类中添加Category字段，如：

public partial class Course
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Course()
    {
        Tags = new HashSet<Tag>();
    }

    public Category Category { get; set; }      //添加一个新字段Category
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Level { get; set; }
    public float FullPrice { get; set; }
    public int? Author_Id { get; set; }
    public virtual Author Authors { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public virtual ICollection<Tag> Tags { get; set; }
}
  
  //然后在PM中键入新建迁移文件命令: add-migration AddCategoryColumnToCoursesTable，生成以下迁移文件：

public partial class AddCategoryColumnToCoursesTable : DbMigration
{
    public override void Up()
    {
        AddColumn("dbo.Courses", "Category_Id", c => c.Int(nullable: true));//此处Category_Id因手动改为可空，如需要可在填充后的Up方法改回
        CreateIndex("dbo.Courses", "Category_Id");    //将这个带Id的列设为索引
        AddForeignKey("dbo.Courses", "Category_Id", "dbo.Categories", "Id");//建立指向Catrgories表Id的外键，两者为一对一关系
        Sql("UPDATE Courses SET Category_Id = 1");  //执行Sql语句，将所有Category_Id列设为1，如此列非可空，则这一步为必须
    }

    public override void Down()
    {
        DropForeignKey("dbo.Courses", "Category_Id", "dbo.Categories");
        DropIndex("dbo.Courses", new[] { "Category_Id" });
        DropColumn("dbo.Courses", "Category_Id");
    }
}

  //最后使用PM中的updat-database命令即可完成数据库与Model的同步更新。
  
//暂时想到这么多，最后更新2017/12/22
