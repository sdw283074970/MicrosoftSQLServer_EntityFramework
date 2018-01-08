//Q: 什么是FluentAPI？
//A: FluentAPI是指通过链式调用方法来覆写约定的方法。在一个方法后继续调用另一个方法成为链式调用，如：

  modelBuilder.Entity<Course>().Property(t => t.Name).HasColumnName("sName");

  //这样调用非常流畅，故此得名FluentAPI。

//Q: 为什么要用FluentAPI而不用DataAnnotations?
//A: DataAnnotations虽然使用起来简单，但是有不少限制，具体在DataAnnotations章节有分析。FluentAPI则可以满足我们的所有需要。

//Q: 如何使用FluentAPI？
//A: 我们需要在Model0的Domain中(如PlutoContext.cs)的OnModelCreating方法中使用FluentAPI覆写约定。OnModelCreating初始代码如下：

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      base.OnModelCreating(modelBuilder);
  }

  //我们可以看到这个方法传入一个类型为DbModelBuilder的参数。OnModelCreating方法顾名思义，是在创建Model的时候调用，意味着我们在这里覆写约定就能影响
    //最终数据库的成型。类DbModelBuilder有很多方法，但是在覆写约定中用到最多的还是Entity<T>()方法。这个方法是一个泛型方法，T代表了我们要覆写的Entity
    //类型，如表Courses是一个Entity，那么Courses的类型就为Course。
  
  //通过调用类DbModelBuilder中的Entity<T>()方法，我们可以覆写很多约定，这里以表Courses为例，列举Entity<T>()方法中的一些链式方法：
  //1.覆写表名约定：

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 表名为类名的复数
      modelBuilder.Entity<Course>()
                  .ToTable("tbl_Course");   //覆写：将基于Course类生成的表格命名为"tbl_Course"而不是"Courses"
      base.OnModelCreating(modelBuilder);
  }

  //2.覆写主键约定：

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 主键为名为"Id"或名称中含有"Id"的列
      modelBuilder.Entity<Book>()
                  .HasKey(t => t.ISBN)   //覆写：将ISBN列设为主键(这里的t指Func<Book, T>中的T，即返回类型)
      base.OnModelCreating(modelBuilder);
  }

  //3.覆写组合键约定：
  
  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 无
      modelBuilder.Entity<OrderItem>()
                  .HasKey(t => new { t.OrderId, t.OrderItemId });   //覆写：将OrderItem类中的OrderId列和OrderItemId列设为组合键
  }  

  //4.覆写列名：

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 列名与类中的字段名相同
      modelBuilder.Entity<Course>()
                  .Property(t => t.Name)    //选定Course类中的Name属性作为对象
                  .HasColumnName("sName");    //覆写：将该列名覆写为"sName"
      base.OnModelCreating(modelBuilder);
  }

  //5.覆写列的数据类型

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 一系列自适应对应转换关系，如string对应nvarchar(MAX)
      modelBuilder.Entity<Course>()
                  .Property(t => t.Name)    //选定Course类中的Name属性作为对象
                  .HasColumnType("varchar");    //覆写：将该列的数据类型覆写为varchar
      base.OnModelCreating(modelBuilder);
  }

  //6.覆写列的顺序

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 列顺序与类中对应的字段顺序相同
      modelBuilder.Entity<Course>()
                  .Property(t => t.Name)    //选定Course类中的Name属性作为对象
                  .HasColumnOrder(2);    //覆写：将该列在表中的顺序覆写为第二列
      base.OnModelCreating(modelBuilder);
  }

  //7.覆写数据库自动生成

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定：若主键被定义为Identify，没新增一个条目，则数据库自动生成新主键(值为上一条目主键值+1)
      modelBuilder.Entity<Book>()
                  .Property(t => t.ISBN)    //选定Book类中的ISBN属性作为对象(ISBN为主键)
                  .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);    //覆写：关闭自动生成主键值
                //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);    //覆写：将数据库主键自动生成选项覆写为组合(来自其他列)
                //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identify);    //覆写：打开数据库主键值自动生成  (值为上一条目主键值+1)  
      base.OnModelCreating(modelBuilder);
  }

  //8.覆写可空类型

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 若C#中字段为可空，则数据库中也为可空
      modelBuilder.Entity<Course>()
                  .Property(t => t.Name)    //选定Course类中的Name属性作为对象
                  .IsRequired().    //覆写：将该列覆写为不可空
      base.OnModelCreating(modelBuilder);
  }

  //9.覆写字符串长度

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： String类对应nvarchar(MAX)
      modelBuilder.Entity<Course>()
                  .Property(t => t.Name)    //选定Course类中的Name属性作为对象
                  .HasMaxLength(255);    //覆写：将该列的长度最大值覆写为255
      
      modelBuilder.Entity<Course>()
                  .Property(t => t.Description)    //选定Course类中的Description属性作为对象
                  .IsMaxLength();    //覆写：将该列的长度覆写为最大值 
      base.OnModelCreating(modelBuilder);
  }

//暂时想到这么多，最后更新2018/1/8
