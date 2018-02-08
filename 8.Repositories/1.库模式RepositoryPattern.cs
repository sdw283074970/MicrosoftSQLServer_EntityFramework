//Q: 什么是库？
//A: 库(RepositoryPattern)调和了数据连接层和主域，在执行上类似于存在内存中的主域对象集合(PoEAA by Martin Fowler)，因此可以轻松访问。

//Q: 为什么要用库？
//A: 有很多好处。第一个好处就是能最少化重复的查询逻辑。如，假设在一个应用中的很多地方都要查询一个前五最畅销的课程，在没有库的情况下就得在每个需要查询
  //的地方经行相同的查询，代码如下：

      var topSellingCourses = context.Courses
          .Where(c => c.IsPublic && c.IsApproved)
          .OrderByDescending(c => c.Sales)
          .Take(10);

  //有了库，我们可以将这个查询封装进库方法，代码如下：

      var courses = repository.GetTopSellingCourses(category, count);

  //另一个好处就是库能将应用程序从持续性框架(如EntityFramework)中剥离，形成松耦合关系。未来如果想换一个框架，则能够将需要改变的地方降到最少。
  //最后一个就是能够提高程序的可测试性。库的本质是建立一堆接口让各类之间形成松耦合关系，这也让各类能够轻易的进行单元测试。

//Q: 库具体应该是什么样子？
//A: 库应该像集合一样，有Add(ojb)、Remove(obj)、Get(id)、GetAll()、Find(predicate)等方法。这里不应该有类似于Update()的方法，要更新数据其实
  //直接从内存读取再更改就好了，一个库不应该直接影响数据库。如，代码如等下：

      var course = collection.Find(1);
      course.Name = "New Name";

//Q: 那如何通过库来更新和储存改变并同步至数据库？
//A: 需要引入工作单元UnitOfWork的概念。工作单元负责维护和管理一系列被业务逻辑转换所影响的对象，并协调写出这些影响和变化(PoEAA by Martin Fowler)。
  //一些程序员认为EntiryFramework已经整合了Repository，他们认为DbSet<T>就是一种repository，因为DbSet<T>也有Add()、Remove()等方法，也没有update()
  //或者Save()的方法。甚至，这些程序员认为DbContext就是工作单元，因为正好DbContext有SaveChanges()方法。

  //表面上看起来是这样的，DbSet和DbContext部分符合我们对库和工作单元的定义。但是，DbSet直接就不符合第一条库的作用，虽然可以用LINQ扩展方法来将这重复
    //的查询封装进一个方法，但是这治标不治本，因为不管怎么用扩展方法封装，它总是会返回IQueryable类型的数据，这意味着这东西可能被别的程序员拿去做进一步
    //的其他查询。按照OOP设计原则，执行的内部细节不应该暴露在外边，所以不应该用LINQ扩展方法封装重复的查询。

  //至于库的好处的第二条就更明显了。DbContext和DbSet完全不能降低程序与EntityFrameWork的耦合性。
  
  //所以，即使DbSet和DbContext看起来很像库和工作单元，但是它们并不能提供库模式架构上的好处。如果要设计一个干净的架构应该遵循：架构应该独立于框架。

//Q: 什么时候应该用库模式？
//A: 只有当使用库模式让整个程序变得更简洁的时候才使用。换句话说，有些情况使用库模式反而会让程序变得臃肿复杂，如一些小项目就没必要用库模式了。

//暂时想到这么多，最后更新2018/02/08
