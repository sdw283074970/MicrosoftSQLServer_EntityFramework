//Q: 什么是FluentAPI？
//A: FluentAPI是指通过链式调用方法来复写约定的方法。在一个方法后继续调用另一个方法成为链式调用，如：

  modelBuilder.Entity<Course>().Property(t => t.Name).HasColumnName("sName");

  //这样调用非常流畅，故此得名FluentAPI。

//Q: 为什么要用FluentAPI而不用DataAnnotations?
//A: DataAnnotations虽然使用起来简单，但是有不少限制，具体在DataAnnotations章节有分析。FluentAPI则可以满足我们的所有需要。

//Q: 如何使用FluentAPI？
//A: 我们需要在Model0的Domain中(如PlutoContext.cs)的OnModelCreating方法中使用FluentAPI复写约定。OnModelCreating初始代码如下：

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      base.OnModelCreating(modelBuilder);
  }

  //我们可以看到这个方法传入一个类型为DbModelBuilder的参数。OnModelCreating方法顾名思义，是在创建Model的时候调用，意味着我们在这里复写约定就能影响
    //最终数据库的成型。类DbModelBuilder有很多方法，但是在复写约定中用到最多的还是Entity<T>()方法。这个方法是一个泛型方法，T代表了我们要复写的Entity
    //类型，如表Courses是一个Entity，那么Courses的类型就为Course。
  
  //通过调用类DbModelBuilder中的Entity<T>()方法，我们可以复写很多约定，这里以表Courses为例，列举Entity<T>()方法中的一些链式方法：
  //1.复写表名约定：

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 表名为类名的复数
      modelBuilder.Entity<Course>()
                  .ToTable("tbl_Course");   //复写：将基于Course类生成的表格命名为"tbl_Course"而不是"Courses"
      base.OnModelCreating(modelBuilder);
  }

  //2.复写主键约定：

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 主键为名为"Id"或名称中含有"Id"的列
      modelBuilder.Entity<Book>()
                  .HasKey(t => t.ISBN)   //复写：将ISBN列设为主键(这里的t指Func<Book, T>中的T，即返回类型)
      base.OnModelCreating(modelBuilder);
  }

  //3.复写组合键约定：
  
  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 无
      modelBuilder.Entity<OrderItem>()
                  .HasKey(t => new { t.OrderId, t.OrderItemId });   //复写：将OrderItem类中的OrderId列和OrderItemId列设为组合键
  }  

  //4.复写列名：

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 列名与类中的字段名相同
      modelBuilder.Entity<Course>()
                  .Property(t => t.Name)    //选定Course类中的Name属性作为对象
                  .HasColumnName("sName");    //复写：将该列名复写为"sName"
      base.OnModelCreating(modelBuilder);
  }

  //5.复写列的数据类型

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 一系列自适应对应转换关系，如string对应nvarchar(MAX)
      modelBuilder.Entity<Course>()
                  .Property(t => t.Name)    //选定Course类中的Name属性作为对象
                  .HasColumnType("varchar");    //复写：将该列的数据类型复写为varchar
      base.OnModelCreating(modelBuilder);
  }

  //6.复写列的顺序

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 列顺序与类中对应的字段顺序相同
      modelBuilder.Entity<Course>()
                  .Property(t => t.Name)    //选定Course类中的Name属性作为对象
                  .HasColumnOrder(2);    //复写：将该列在表中的顺序复写为第二列
      base.OnModelCreating(modelBuilder);
  }

  //7.复写数据库自动生成

  protected override void OnModelCreating(DbModelBuilder modelBuilder)
  {
      //默认约定： 
      modelBuilder.Entity<Course>()
                  .Property(t => t.Name)    //选定Course类中的Name属性作为对象
                  .HasColumnName("sName");    //复写：将列名复写为"sName"
      base.OnModelCreating(modelBuilder);
  }





