//Q: 什么是关系？
//A: 关系(Relationship)是表之间的关系。一共有三种关系，分别是一对一/零、一对多(多对一)和多对多关系。

//Q: 为什么要覆写表与表之间的关系？他们之间的关系不应该是声明类的时候就确定了吗？
//A: 关系是在类的声明就确定没错，但是还是有很多内容可以被覆写。如：
  //1.覆写中间表。多对多关系会生成中间表，如何按需生成中间表就涉及到覆写约定；
  //2.覆写外键。有时候会有指定外键的需求，外键的名称有时候也需要被覆写；
  //3.声明表与表的父子关系。如即使在类中声明一个一对一关系，EF也不会知道哪个是父表，哪个是子表，系统会抛出异常，这也是需要覆写声明的部分。

//Q: 如何通过FluentAPI覆写关系？
//A: 上面提到有三种关系，分别是一对一/零、一对多(多对一)和多对多关系。当处理两个类型之间(如Course和Tag)的关系时，需要选择一个具体的类型为出发点，即定义
  //类型1和类型2(方向并不重要)，如从类Course到类Tag，类Course就为类型1，类Tag为类型2。通常情况下，使用FluentAPI分为三步走：

  //1.确定类型1(起点类)和类型2，并通过Entity<T1>()方法来获得起点类的引用；
  //2.从类型1出发到类型2，根据关系类型确定调用对应方法。这里一共有三种方法：HasMany()方法(如果类型1有很多的类型2则调用)、HasRequired()方法(如果类型1
    //只有一个类型2则调用)、HasOptional()方法(如果类型1最多有1个类型2则调用)；
  //3.从类型2返回到类型1，根据关系类型确定调用对应方法。这里也有三种方法：WithMany()方法(如果类型2有很多类型1则调用)、WithRequired()方法(如果类型2只
    //有一个类型1则调用)、HasOptional()方法(如果类型2最多有一个类型1则调用)。

  //现在举例说明以上步骤。先从最简单的开始：
  //1.覆写一对多/多对一关系
    //如Author与Course就为一对多关系，即一个Author可以有很多Course，但一个Course只可能有一个Author。用FluentAPI覆写这个关系代码如下：

  modelBuilder.Entity<Author> //获得起点类Author的引用
              .HasMany(a => a.Courses)    //一个Author有多个Course，这里的Courses为AuThor类本身声明的类型为ICollection<Course>的字段
              .WithRequired(c => c.Author)    //一个Course只能有一个Author
              .HasForeignKey(c => c.AuthorId);   //可以可选择的调用覆写外键的方法，即覆写Course类中的AuthorId字段为外键(AuthorId必须存在Course类)
    
    //反过来如果将Course定为起点类，则有以下同效代码：
    
  ModelBilder.Entity<Course>    //获得起点类Course的引用
             .HasRequired(c => Author)    //一个Course只能有一个Author
             .WithMany(a => Courses)    //一个Author有多个Course
             .HasForeignKey(c => AuthorId);    //覆写Course类中的AuthorId字段为外键

  //2.覆写多对多关系
    //如Course和Tag就为多对多关系，即一个Course可以有很多Tag，且Tag可以给很多Course贴。用FluentAPI覆写这个关系代码如下：
    
  modelBuilder.Entity<Course>   //获得起点类Course的引用
              .HasMany(c => c.Tags)   //一个Course有很多Tags
              .WithMany(t => t.Courses)   //一个Tag有很多Courses
              .Map(m => m.ToTable("CourseTags"));   //多对多关系会生成中间表，调用Map()方法可以覆写中间表的属性，如通过ToTable()方法覆写中间表名
    
    //反过来一样一样的，不作赘述

  //3.覆写一对一/零关系
    //如Course和Caption就为一对一/零关系，即Course可能有一个Caption或可能没有，而Caption一定有一个Course。用FluentAPI覆写这个关系代码如下：

  modelBuilder.Entity<Course>   //获得起点类Course的引用
              .HasOptional(c => c.Caption)    //一个Course可能有一个Caption也可能没有
              .WithRequired(c => c.Course);   //一个Caption有且仅有一个Course
  
    //反之亦然。一对一/零关系有一个特殊的情况，即一对一关系。如Course和Cover就为一对一关系，他们都有且仅有一个对方。但是首先需要确定哪一个是父母，哪
      //一个是子女？有Course才会有Cover，所以Course是父母，在EF中表述为Principle，子女Cover则表述为Dependent。但是EF不知道哪一个是Principle，哪
      //一个是Dependent。我们通过在返回的时候调用WithRequiredPrinciple()方法或WithRequiredDependent()方法来告诉EntityFramework。

    //如果从Principle出发(即选父母为起点类)，则返回的时候应调用WithRequiredPrinciple()方法，反之调用WithRequiredDependent()方法。
    //用FluentAPI覆写特殊一对一关系代码如下：

  modelBuilder.Entity<Course>
              .HasRequired(c => c.Cover)    //一个Course只有一个Cover
              .WithRequiredPrinciple(c => c.Course);  //一个Cover只有一个Course，Course为父母且为起始类，所以调用WithRequiredPrinciple()
              
    //反过来以Dependent为起始类，则有以下代码：

  modelBuilder.Entity<Cover>
              .HasRequired(c => c.Course)   //一个Cover只有一个Course
              .WithRequiredDependent(c => c.Cover);  //一个Course只有一个Cover，Cover为子女且为起始类，所以调用WithRequiredDependent()方法
              
//暂时想到这么多，最后更新2018/01/08
