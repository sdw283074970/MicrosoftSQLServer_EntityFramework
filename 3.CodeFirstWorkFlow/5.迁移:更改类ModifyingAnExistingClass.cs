//本篇示例基于之前的CodeFirstExistingDatabase项目。原始文件可参考本章《2.在现有项目中使用代码优先CodeFirstWithExistingDatabase.cs》。

//Q: 更改现有的类对数据库有什么影响？
//A: 更改现有的类，有三种情况，即1.在类中添加新的属性(之前有提过)；2.更改类中的属性；3.删除类中的属性。我们分别来看这三种情况。
  //1.在类中添加新的属性。在一个类中添加新的属性，就意味着要向这个类的表中添加一个新列。如果是空表那问题不大，但如果是已经存在数据的表，我们需要将这个
    //新添加的列定义为可空类型，再上一篇有提到过它的重要性。如在Courses表中添加一个数据类型为DateTime类的新列DatePublished，即在Course类中添加一个
    //DateTime类的属性DatePublished，代码如下：

    public partial class Course
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Course()
        {
            Tags = new HashSet<Tag>();
        }

        public DateTime? DatePublished { get; set; }    //添加一个可空的DateTime类型属性DatePublished
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public float FullPrice { get; set; }
        public int? Author_Id { get; set; }
        public virtual Author Authors { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tag> Tags { get; set; }
    }

    //然后在PM中键入add-migration AddDatePublishedColumnToCourseTable建立迁移文件：

    public partial class AddDatePublishedColumnToCourseTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "DatePublished", c => c.DateTime());   //EF感知到需要在Courses表中添加一个新列
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "DatePublished");
        }
    }
    
    //最后在PM中键入update-database即可完成同步，这是类中添加新属性的情况。
    
  //2.类中更改属性。更改属性，如改名，选中要改名的属性，Ctrl+R+R即可改名(这种改法会将后面收到影响的地方同步改名)。如此例中我们要更改Course类中的
    //Title属性，将Title改为Name，代码如下：

    public partial class Course
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Course()
        {
            Tags = new HashSet<Tag>();
        }

        public DateTime? DatePublished { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }    //将Title改为Name
        public string Description { get; set; }
        public int Level { get; set; }
        public float FullPrice { get; set; }
        public int? Author_Id { get; set; }
        public virtual Author Authors { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tag> Tags { get; set; }
    }

    //然后在PM中键入add-migration RenameTitleInCourseToName建立迁移文件，代码如下：

    public partial class RenameTitleInCourseToName : DbMigration
    {
        public override void Up()   //EF将视改名操作为添加-删除，并按照这个顺序执行，达到与改名等同的效果
        {
            AddColumn("dbo.Courses", "Name", c => c.String());   //我们可以看到首先，EF新建了一个Name列
            DropColumn("dbo.Courses", "Title");   //EF将Title列删除，留下Name列，看起来等于改名
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courses", "Title", c => c.String());
            DropColumn("dbo.Courses", "Name");
        }
    }
    
    //但是，删除Title列会删除所有Title列中的数据，为了保存原数据，有两种方法可实现。第一种方法，手动将这些数据保留下来，通过Sql查询语句实现：

    public partial class RenameTitleInCourseToName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "Name", c => c.String(nullable: false));   //String为不可空类型，将此列设为不可空
            Sql("UPDATE Courses SET Name = Title");   //添加查询语句，在Title列被删除之前将其值赋予Name
            DropColumn("dbo.Courses", "Title");   //再删除Title列，这样原数据得以保留在Name列中
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courses", "Title", c => c.String(nullable: false));
            Sql("UPDATE Courses SET Title = Name");   //记得反向执行Up操作，否则降级版本的数据将丢失Title数据
            DropColumn("dbo.Courses", "Name");
        }
    }
    
    //第二种方法，试用RenameColumn方法，将迁移文件改为：

    public partial class RenameTitleInCourseToName : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Courses", "Title", "Name");   //直接将Title改为Name
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Courses", "Name", "Title");   //降级方法中别忘了将名字改回来 
        }
    }
  
    //然后通过update-database命令同步更新。以上为修改类中属性的名字。

  //3.删除类中的属性。这个就直接了当了，直接删除，然后迁移同步即可。如此例中，我们要删除在第一种情况中建立的DatePublished属性，首先删除该属性，
    //然后在PM中键入add-migration DeletedDatePublishedColumnInCoursesTable建立迁移文件，代码如下：

    public partial class DeletedDatePublishedColumnInCoursesTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Courses", "DatePublished");   //EF感知只有Courses表用到了Course类，并删除掉其中的DatePublished列
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courses", "DatePublished", c => c.DateTime());   //反向操作
        }
    }

    //最后update-database即可完成数据库与Model的同步更新。

//暂时想到这么多，最后更新2017/12/22
