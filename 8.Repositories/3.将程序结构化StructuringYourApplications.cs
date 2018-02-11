//Q: 什么是将程序结构化？
//A: 在写一个程序的过程中，肯定写大量的类，每一个类就是一个文件，放在一起显得杂乱无章。将程序结构化就是将这些类文件有条理的归纳收置，让整个程序显得
  //有条理、有结构，便于维护。

//Q: 按什么标准对这些文件经行分类整理？
//A: 通常以架构来归纳。以经典的三层架构为例，三层分别为：
  //展示层Presentation Layer：包括表单Form类、视觉模型ViewModel类、控制器Controller类等；
  //业务逻辑/核心层Business Logic/Core Layer：包括域Domain类、服务类、接口(如IUnitWOfWork、IRepository、ICourseRepository)等；
  //数据访问层DataAccess Layer：将数据持久化，执行逻辑层中的逻辑(接口)，如PlutoContext、UnitOfWork、Repository、CourseRepository等。

  //在业务逻辑/核心层中只有逻辑方法，没有这些方法的具体执行。具体执行在数据访问层中通过执行业务逻辑层中的接口来实现，这也是依赖倒置原则，即实现依赖
    //抽象(即接口)。

//Q: 如何让这些分类物理可见？
//A: 在VS中将这些文件放在正确的文件夹中即可。换句话说，为这些类安排合适的命名空间namespace。如Pluto项目中，业务逻辑/核心层我们专门建立Core或Logic的
  //文件夹用于储存关键服务接口(如IUnitOfWork接口)、Domai类和其他接口。在Core中建立Domain文件夹用于储存Domain类/实体类(如Author.cs、Course.cs等)、
  //Repository文件夹储存接口(如IRepository.cs、IAuthorRepository.cs甚至单元测试接口等)。而数据访问层我们就建立一个Presistence文件夹用来储存以上
  //接口的实例/服务/具体执行(如PlutoContext.cs、UnitOfWork.cs)、实体配置文件(EntityConfiguration文件夹，即之前覆写的约定如CourseConfiguration.cs)
  //等。一些具体对象的接口实施对象如CourseRepository.cs、AuthorRepository.cs、Repository.cs则放在Presistence/Repository文件夹中。

//暂时想到这么多，最后更新2018/02/11
