//本篇示例基于之前的CodeFirstExistingDatabase项目。原始文件可参考本章《2.在现有项目中使用代码优先CodeFirstWithExistingDatabase.cs》。

//Q: 删除类有什么讲究？
//A: 最大的问题在删除掉的类或许会影响到多个地方，我们需要找到这些地方按照顺序手动删除。如在上例中，我们要删除Category类。这个类首先存在与一个表中，
  //其次，这个表拥有一个指向Courses的外键，即Course类中也有一个Category类的字段。删除Category类意味着这个外键得消失，同时Categories表也得删除掉。
  //如果先删除Categories表将抛出异常，原因是Courses表依赖Categories表。所以我们要先删掉它们之间的一对一关系，即必须先将Course类中的Category字段删掉
  //后才能删掉Categories表。整个过程分为两步，即1.先删Course类中字段；2.删掉Category类。每一步都应该分别建立迁移文件。

  //首先，删除Course类中的Category属性。删除后，在PM中键入add-migration DeletedCatagoryColumnInCoursesTable建立如下迁移文件：

    public partial class DeletedCatagoryColumnInCoursesTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Courses", "Category_Id", "dbo.Categories");   //丢掉外键
            DropIndex("dbo.Courses", new[] { "Category_Id" });    //丢掉索引
            DropColumn("dbo.Courses", "Category_Id");   //丢掉列
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courses", "Category_Id", c => c.Int());
            CreateIndex("dbo.Courses", "Category_Id");
            AddForeignKey("dbo.Courses", "Category_Id", "dbo.Categories", "Id");
        }
    }
    
  //然后先update-database同步一次。第二步，删除Category类本身，然后键入add-migration DeletedCategriesTable建立迁移文件：

    public partial class DeletedCategriesTable : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Categories");    //直接丢掉Categories表
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            Sql("INSERT INTO Categories (Name) SELECT Name FROM _Categories");
            DropTable("dbo._Categories");
        }
    }
    
    //但是，删除掉一个表我们可能同时想将数据保存下来留以备用，可以在删除表之间将其备份：

    public partial class DeletedCategriesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(    //建立一个_Categories表，表示是一个历史表，用来储存将要删掉的Categories表中的数据
                "dbo._Categories",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.Id);
            Sql("INSERT INTO _Categories (Name) SELECT Name FROM Categories");    //将Categories表中的数据插入到新建的历史表中
            DropTable("dbo.Categories");    //再执行删除表的命令
        }
        
        public override void Down()   //记得操作反向
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            Sql("INSERT INTO Categories (Name) SELECT Name FROM _Categories");
            DropTable("dbo._Categories");
        }
    }
    
    //这样一来，再键入Update-database同步数据库即可完成删除类的操作，同时还保存了数据以防万一。

//暂时想到这么多，最后更新2017/12/22
