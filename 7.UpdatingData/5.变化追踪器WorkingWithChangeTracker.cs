//Q: 变化追踪器很重要么？
//A: 绝大多数的时候我们并不直接操作变化追踪器(CT)，但是通过了解CT我们可以从状态(Status)的角度解释我们的程序在哪里出了问题。

//Q: 如何通过CT查看对象的状态？
//A: 我们可以通过DbContext类下的ChangeTracker字段来查看，ChangeTracker的类型为DbChangeTracker，我们再通过这个类下的Entry()方法和Entries<T>()
  //方法来获取条目的状态。Entries()方法为返回所有条目的状态信息，Entries<T>()方法为返回所有T类型条目状态信息的方法。我们可以通过迭代来获取这些条目
  //的具体状态，在Debug模式下可以更清楚的查看。

//Q: Entries()方法返回的对象是什么？都有哪些状态信息？
//A: Entries()和Entries<T>()方法分别返回类型为DbEntiryEntry和IEnumerable<DbEntiryEntry>的对象。一个DbEntiryEntry类型对象有四个字段，分别是
  //CurrentValues(当前值)、OriginalValues(原始值)、Entity(实体对象)和Status(状态)。在Debug模式中可以很清楚的看到这些字段属性的变化。
  //值得介绍的是DbEntityEntry类有一个Reload()方法，这个方法是重新将这个对象的原始值加载到当前值中。

//Q: 遇到“已有打开的与此 Command 相关联的 DataReader，必须首先将它关闭。”的错误应该怎么解决？
//A: 两种办法。一种是設定ConnectionString加上MultipleActiveResultSets=true,但只適用於SQL 2005以後之版本。
  //另一种是先讀出放置在List中。如：var speciesInDb = context.SpeciesInventories.Where(c => c.Id > 0).ToList();

//暂时想到这么多，最后更新2018/02/07
