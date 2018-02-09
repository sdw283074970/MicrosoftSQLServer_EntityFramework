//Q: 什么是库模式？
//A: 库模式 RepositoryPattern 即是部署库来实现其带来的好处的具体实施。

//Q: 如何实施库模式？
//A: 首先需要库的接口IRepository和实施这个接口的库类Repository，然后再建立一个为具体库准备的继承IRepository接口的接口(如ICourseRepository)，
  //最后建立一个继承库类Repository的具体数据库类(如CourseRepository)，并在这个类中实施刚刚建立的具体接口(如ICourseRepository)，至此库模式搭建完成。

  //以上一段话可能比较抽象，但是用图就能很好理解，如下图：
  
  //          Generic(泛型部分)                   Pluto(具体数据库部分)
  //                                  继承
  //        IRepository(泛型库接口)<——————————ICourseRepository(具体库接口)
  //                A                                A
  //                |实施接口                         |实施接口
  //                |             继承                |
  //        Repository(泛型库类)<—————————————CourseRepository(具体的库类)

  //我们要做的就是把上图的每个部分实现。
  //如上一节所说，库只应该有Add()、Remove()等方法，不能有直接影响数据库的方法，所以在泛型库接口中只用封装这些Add()、Remove()、Get(id)、Find(predict)
    //等方法，然后在实施这个接口的泛型库中填充这些方法的具体逻辑即可完成库模式泛型部分的设计。
