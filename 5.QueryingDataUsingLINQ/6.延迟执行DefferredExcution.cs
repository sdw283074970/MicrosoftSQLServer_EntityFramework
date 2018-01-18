//Q: 什么是延迟执行？
//A: 延迟执行(DefferredExcution)是EF到SQLSever的运行规则，即查询并不在创建的时候就执行，查询仅在当有以下情况发生时才执行：
  //1.迭代查询变量；
  //2.调用ToList()/ToArray()/ToDictionary()三个方法中的一个；
  //3.调用First()/Last()/Single()/Count()/Max()/Min()/Average()等返回一个对象值的方法；

//Q: 与我何干？
//A: 延迟执行让查询变量更具扩展性。如：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            //以下三条命令编译完成吼都不执行
            var query = context.Courses;  //查询已经建立完毕，但是并不会执行
            var filtered = query.Where(c => c.Level == 1);  //即使查询建立完毕，但仍然能更改/扩展
            var sorted = filtered.OrderBy(c => c.Name);   //可以在扩展的基础上再扩展，但这种写法并不推荐

            foreach (var q in query)    //直到迭代输出结果，查询才会被执行
                Console.WriteLine(q.Name);
        }

  //之所以能这样，是因为在编译时C#语句并没有同步翻译成SQL语句。
  //另一个好处是能让语句执行一些自定义字段。如某些字段的getter()方法包含了自定义逻辑，当编译到这步时，EF并不知道如何将这些自定义逻辑转化为SQL语言。
    //但是如果在此之前用以上三种方法中的一种触发了查询，内存中有了数据，再执行这些自定义字段逻辑就可行。如，在Course类中新加一个自定义字段：

    class Program
    {
        public bool IsBiginnerLevel   //自定义字段，自定义逻辑为判定课程等级是否为1
        {
            get { return Level == 1; }
        }
    }

  //如果直接执行以下程序则抛出异常：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var query = context.Courses.Where(c => c.IsBigginerLevel == true);    //EF并不知道如何将c.IsBigginerLevel == true转换为SQL语言

            foreach (var q in query)
                Console.WriteLine(q.Name);
        }
  
  //但是如果在此之前查询成功执行过，如调用了ToList()方法，则执行会成功。代码如下：

        static void Main(string[] args)
        {
            var context = new PlutoContext();

            var query = context.Courses
              .ToList()   //成功执行查询
              .Where(c => c.IsBigginerLevel == true);   //执行过查询，内存中有Course的实例，即c.IsBigginerLevel == true可以判定

            foreach (var q in query)
                Console.WriteLine(q.Name);
        }

  //如果洁身自好，拒绝在EF中使用自定义字段，那么以上第二个好处请无视。

//暂时想到这么多，最后更新2018/01/17
